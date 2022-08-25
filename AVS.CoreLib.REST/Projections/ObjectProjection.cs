using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AVS.CoreLib.Json;
using AVS.CoreLib.REST.Json;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
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
        public ObjectProjection(string jsonText, string source = null) : base(jsonText, source)
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

        [Obsolete("use UseProxy method instead")]
        public ObjectProjection<T> UseBuilder<TBuilder>(Action<TBuilder> initialize = null)
            where TBuilder : class, IObjectProxy<T>, new()
        {
            var builder = new TBuilder();
            initialize?.Invoke(builder);
            Proxy = builder;
            return this;
        }

        public Task<Response<T>> MapAsync()
        {
            return MapAsync(Map);
        }

        private void EnsureProxyInitialized()
        {
            if (Proxy == null)
                throw new AppException("Proxy is not initialized", "You might need to use UseProxy<TProxy>() method first");
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
        public virtual Response<T> Map()
        {
            EnsureProxyInitialized();

            var response = Response.Create<T>();
            response.Source = Source;

            if (IsEmpty)
            {
                response.Data = Proxy.Create();
            }
            else if (ContainsError(out string err))
            {
                response.Error = err;
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
            var response = new Response<T>() { Source = Source, Data = createFunc() };
            if (IsEmpty)
                return response;

            if (ContainsError(out string err))
            {
                response.Error = err;
            }
            else
            {
                var data = response.Data;
                _preProcessAction?.Invoke(data);
                LoadToken<JObject, TProjection>(jObject =>
                {
                    JsonHelper.Populate(data, jObject);
                });
                _postProcessAction?.Invoke(data);
                response.Data = data;
            }

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

    public class ObjectProjection<T, TProjection> : Projection
        where TProjection : new()
    {
        protected Action<TProjection> _preProcessAction;
        protected Action<T> _postProcessAction;
        protected IObjectProxy<T, TProjection> _proxy;

        [DebuggerStepThrough]
        public ObjectProjection(string jsonText, string source = null) : base(jsonText, source)
        {
        }

        public ObjectProjection<T, TProjection> PreProcess(Action<TProjection> action)
        {
            _preProcessAction = action;
            return this;
        }

        public ObjectProjection<T, TProjection> PostProcess(Action<T> action)
        {
            _postProcessAction = action;
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

        [Obsolete("use UseProxy instead")]
        public ObjectProjection<T, TProjection> UseBuilder<TBuilder>(Action<TBuilder> initialize = null) where TBuilder : class, IObjectProxy<T, TProjection>, new()
        {
            var builder = new TBuilder();
            initialize?.Invoke(builder);
            _proxy = builder;
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
                throw new AppException("Proxy is not initialized", "You might need to use UseProxy<TProxy>() method first");
        }

        public Task<Response<T>> MapAsync()
        {
            return MapAsync(Map);
        }

        public virtual Response<T> Map()
        {
            EnsureProxyInitialized();

            var response = Response.Create<T>();
            response.Source = Source;

            if (IsEmpty)
            {
                response.Data = _proxy.Create();
            }
            else if (ContainsError(out string err))
            {
                response.Error = err;
            }
            else
            {
                LoadToken<JObject, TProjection>(jObject =>
                {
                    var projection = new TProjection();
                    _preProcessAction?.Invoke(projection);
                    JsonHelper.Populate(projection, jObject);
                    _proxy.Add(projection);
                });

                var data = _proxy.Create();
                _postProcessAction?.Invoke(data);
                response.Data = data;
            }
            return response;
        }

        public virtual Response<T> Map(Func<TProjection, T> map)
        {
            var response = Response.Create<T>();
            response.Source = Source;

            if (IsEmpty)
            {
                response.Data = map(new TProjection());
            }
            else if (ContainsError(out string err))
            {
                response.Error = err;
            }
            else
            {
                LoadToken<JObject, TProjection>(jObject =>
                {
                    var projection = new TProjection();
                    _preProcessAction?.Invoke(projection);
                    JsonHelper.Populate(projection, jObject);
                    var data = map(projection);
                    _postProcessAction?.Invoke(data);
                    response.Data = data;
                });
            }
            return response;
        }
    }
}