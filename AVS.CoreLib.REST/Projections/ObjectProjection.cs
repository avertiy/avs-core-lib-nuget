using System;
using System.Diagnostics;
using AVS.CoreLib.REST.Json.Newtonsoft;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
    /// <summary>
    /// create object projection to map json to type <see cref="T"/>
    /// expected json that represent an object e.g. { "prop1" = 123, "prop2" = [], ... }
    /// 
    /// <code>
    ///    //use it with concrete type projection:
    ///    //direct mapping 
    ///    var projection = jsonResult.AsObject`MyObject`()
    ///    Response`MyObject` response = projection.Map();
    /// 
    ///    //indirect mapping:
    ///    var projection = jsonResult.AsObject{MyObject}().UseProxy`MyProxy{MyObject}`();
    ///    Response`MyObject` response = projection.Map();
    /// </code>
    ///  An interface projection is also possible: 
    /// <code>
    ///    //interface projection (direct mapping):
    ///    var projection = jsonResult.AsObject{IMyObject}()
    ///    Response`IMyObject` response = projection.Map{MyObject}();
    /// </code>
    ///  An interface projection (indirect mapping)
    ///  <seealso cref="ObjectProjection{T, TProjection}"/> could be more convenient for this
    /// <code>
    ///    var projection = jsonResult.AsObject{IMyObject}().UseProxy`MyProxy`();
    ///    Response`IMyObject` response = projection.Map`MyProjection`(); //here MyProxy create IMyObject from MyProjection
    /// </code>
    /// </summary>
    public class ObjectProjection<T> : Projection
    {
        protected Action<T> _postProcessAction;
        protected Action<T> _preProcessAction;

        /// <summary>
        /// in case target object can't be deserialized directly use proxy object `Proxy`
        /// the json will be deserialized into Proxy object, then the builder will Create the target object
        /// <see cref="IObjectProxy{T,TData}"/>
        /// </summary>
        public IObjectProxy<T> Proxy { get; private set; }

        [DebuggerStepThrough]
        public ObjectProjection(RestResponse response) : base(response)
        {
        }

        public ObjectProjection<T> PreProcess(Action<T> action)
        {
            _preProcessAction = action;
            return this;
        }

        public ObjectProjection<T> PostProcess(Action<T> action)
        {
            _postProcessAction = action;
            return this;
        }

        public ObjectProjection<T> PostProcess<TProjection>(Action<TProjection> action) 
            where TProjection : T
        {
            _postProcessAction = x => action((TProjection)x);
            return this;
        }

        /// <summary>
        /// setup deserialization proxy object <see cref="IObjectProxy{T,TData}"/>
        /// </summary>
        public ObjectProjection<T> UseProxy<TProxy>(Action<TProxy> initialize = null)
            where TProxy : class, IObjectProxy<T>, new()
        {
            var proxy = new TProxy();
            initialize?.Invoke(proxy);
            Proxy = proxy;
            return this;
        }

        private void EnsureProxyInitialized()
        {
            if (Proxy == null)
                throw new AppException("Proxy is not initialized", $"You might need to use UseProxy<{typeof(T).Name}>() method first");
        }

        public object InspectDeserialization(Action<JObject, IObjectProxy<T>> inspect, out Exception err)
        {
            EnsureProxyInitialized();
            try
            {
                LoadToken<JObject, T>(jObject =>
                {
                    inspect(jObject, Proxy);
                    JsonHelper.Populate(Proxy, jObject);
                });
                var data = Proxy.Create();
                err = null;
                return data;
            }
            catch (Exception ex)
            {
                err = ex;
                return null;
            }
        }

        /// <summary>
        /// Map json into Response`T using <see cref="Proxy"/>
        /// </summary>
        /// <remarks>
        /// if the Map method fails, try inspect deserialization <see cref="InspectDeserialization"/> 
        /// </remarks>
        public Response<T> Map()
        {
            EnsureProxyInitialized();

            var response = MapInternal<T>(response =>
            {
                if (IsEmpty)
                {
                    response.Data = Proxy.Create();
                }
                else
                {
                    LoadToken<JObject, T>(jObject =>
                    {
                        JsonHelper.Populate(Proxy, jObject);
                    });

                    var data = Proxy.Create();
                    _preProcessAction?.Invoke(data);
                    _postProcessAction?.Invoke(data);
                    response.Data = data;
                }
            });
            
            return response;
        }

        public virtual Response<T> Map<TProjection>(Action<TProjection> action = null)
            where TProjection : T, new()
        {
            var response = CreateResponse<T, TProjection>();
            if (response.Success)
            {
                var data = new TProjection();
                _preProcessAction?.Invoke(data);
                LoadToken<JObject, TProjection>(jObject =>
                {
                    JsonHelper.Populate(data, jObject);
                });
                action?.Invoke(data);
                _postProcessAction?.Invoke(data);
                response.Data = data;
            }
            return response;
        }

        public virtual Response<T> Map<TProjection>(Func<TProjection> createFunc) where TProjection : T, new()
        {
            var response = new Response<T>() { Source = Source, Data = createFunc(), Error = Error };
            if (IsEmpty || HasError)
                return response;

            var data = response.Data;
            _preProcessAction?.Invoke(data);
            LoadToken<JObject, TProjection>(jObject =>
            {
                JsonHelper.Populate(data, jObject);
            });
            _postProcessAction?.Invoke(data);
            response.Data = data;
            return response;
        }

        /// <summary>
        /// Maps json into TProjection object, returns response with T data
        /// </summary>
        /// <typeparam name="TImplementation">implementation of interface T</typeparam>
        /// <typeparam name="TProjection">the type container where json should be deserialized</typeparam>
        public virtual Response<T> Map<TImplementation, TProjection>(Action<TImplementation, TProjection> action)
            where TProjection : new()
            where TImplementation : T, new()
        {
            var response = CreateResponse<T, TImplementation>();
            if (response.Success)
            {
                var data = new TImplementation();
                _preProcessAction?.Invoke(data);
                var projection = new TProjection();
                LoadToken<JObject, TProjection>(jObject =>
                {
                    JsonHelper.Populate(projection, jObject);
                });
                action?.Invoke(data, projection);
                _postProcessAction?.Invoke(data);
                response.Data = data;
            }
            return response;
        }
    }

    /// <summary>
    /// Create object projection to map a json object to type <see cref="TProjection"/>
    /// but return result as a <see cref="Response{T}"/>
    /// 
    /// It is case of indirect mapping, when <see cref="TProjection"/> neither implement, neither inherit type <see cref="T"/>
    /// The <see cref="UseProxy{TProxy}"/> is required, the proxy creates <see cref="T"/> from <see cref="TProjection"/>:
    /// <code>
    ///    var projection = jsonResult.AsObject{IMyObject, ProjectionType}().UseProxy`MyProxy{IMyObject}`();
    ///    Response`IMyObject` response = projection.Map();
    /// </code>
    /// </summary>
    public class ObjectProjection<T, TProjection> : Projection
        where TProjection : new()
    {
        protected Action<TProjection> _preProcess;
        protected Action<T> _postProcess;
        protected Action<TProjection> _postProcessProjection;
        protected IObjectProxy<T, TProjection> _proxy;

        [DebuggerStepThrough]
        public ObjectProjection(RestResponse response) : base(response)
        {
        }

        public ObjectProjection<T, TProjection> PreProcess(Action<TProjection> action)
        {
            _preProcess = action;
            return this;
        }

        public ObjectProjection<T, TProjection> PostProcess(Action<T> action)
        {
            _postProcess = action;
            return this;
        }

        public ObjectProjection<T, TProjection> PostProcess(Action<TProjection> action)
        {
            _postProcessProjection = action;
            return this;
        }

        /// <summary>
        /// setup deserialization proxy object <see cref="IObjectProxy{T,TData}"/>
        /// </summary>
        public ObjectProjection<T, TProjection> UseProxy<TProxy>(Action<TProxy> initialize = null)
            where TProxy : class, IObjectProxy<T, TProjection>, new()
        {
            var proxy = new TProxy();
            initialize?.Invoke(proxy);
            _proxy = proxy;
            return this;
        }

        public object InspectDeserialization(Action<JObject, IObjectProxy<T, TProjection>> inspect, out Exception err)
        {
            EnsureProxyInitialized();
            try
            {
                LoadToken<JObject, T>(jObject =>
                {
                    inspect(jObject, _proxy);
                    JsonHelper.Populate(_proxy, jObject);
                });
                var data = _proxy.Create();
                err = null;
                return data;
            }
            catch (Exception ex)
            {
                err = ex;
                return null;
            }
        }

        private void EnsureProxyInitialized()
        {
            if (_proxy == null)
                throw new AppException("Proxy is not initialized", $"You might need to use UseProxy<{typeof(T).Name}>() method first");
        }

        public Response<T> Map()
        {
            EnsureProxyInitialized();

            var response = MapInternal<T>(response =>
            {
                if (IsEmpty)
                {
                    response.Data = _proxy.Create();
                }

                else
                {
                    LoadToken<JObject, TProjection>(jObject =>
                    {
                        var projection = new TProjection();
                        _preProcess?.Invoke(projection);
                        JsonHelper.Populate(projection, jObject);
                        _postProcessProjection?.Invoke(projection);
                        _proxy.Add(projection);
                    });

                    var data = _proxy.Create();
                    _postProcess?.Invoke(data);
                    response.Data = data;
                }
            });
            
            return response;
        }

        public Response<T> Map(Func<TProjection, T> map)
        {
            var response = Response.Create<T>(source: Source, error: Error);

            if (HasError)
                return response;

            if (IsEmpty)
            {
                response.Data = map(new TProjection());
                return response;
            }

            LoadToken<JObject, TProjection>(jObject =>
            {
                var projection = new TProjection();
                _preProcess?.Invoke(projection);
                JsonHelper.Populate(projection, jObject);
                var data = map(projection);
                _postProcessProjection?.Invoke(projection);
                _postProcess?.Invoke(data);
                response.Data = data;
            });

            return response;
        }
    }
}