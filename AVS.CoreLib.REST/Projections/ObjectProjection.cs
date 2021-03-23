using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AVS.CoreLib.Json;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
    public class ObjectProjection<T> : Projection
    {
        protected Action<T> _postProcessAction;
        protected Action<T> _preProcessAction;
        public IObjectBuilder<T> Builder { get; private set; }
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

        public ObjectProjection<T> UseBuilder<TBuilder>(Action<TBuilder> initialize = null)
            where TBuilder : class, IObjectBuilder<T>, new()
        {
            var builder = new TBuilder();
            initialize?.Invoke(builder);
            Builder = builder;
            return this;
        }

        public Task<Response<T>> MapAsync()
        {
            return MapAsync(Map);
        }

        public Response<T> Map()
        {
            if (Builder == null)
                throw new AppException("Builder is not initialized", "You might need to use UseBuilder<TBuilder>() method first");

            var response = Response.Create<T>();
            response.Source = Source;

            if (IsEmpty)
            {
                response.Data = Builder.Create();
            }
            else if (ContainsError(out string err))
            {
                response.Error = err;
            }
            else
            {
                LoadToken<JObject, T>(jObject =>
                {
                    JsonHelper.Populate(Builder, jObject);
                });

                var data = Builder.Create();
                _preProcessAction?.Invoke(data);
                _postProcessAction?.Invoke(data);
                response.Data = data;
            }
            return response;
        }

        public Response<T> Map<TProjection>(Action<TProjection> action = null)
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

        public Response<T> Map<TProjection>(Func<TProjection> createFunc) where TProjection : T, new()
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
        public Response<T> Map<TImplementation, TProjection>(Action<TImplementation, TProjection> action)
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
        protected IObjectBuilder<T, TProjection> _builder;

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

        public ObjectProjection<T, TProjection> UseBuilder<TBuilder>(Action<TBuilder> initialize = null) where TBuilder : class, IObjectBuilder<T, TProjection>, new()
        {
            var builder = new TBuilder();
            initialize?.Invoke(builder);
            _builder = builder;
            return this;
        }

        public Task<Response<T>> MapAsync()
        {
            return MapAsync(Map);
        }

        public Response<T> Map()
        {
            if (_builder == null)
                throw new AppException("Builder is not initialized", "You might need to use UseBuilder<TBuilder>() method first");

            var response = Response.Create<T>();
            response.Source = Source;

            if (IsEmpty)
            {
                response.Data = _builder.Create();
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
                    _builder.Add(projection);
                });

                var data = _builder.Create();
                _postProcessAction?.Invoke(data);
                response.Data = data;
            }
            return response;
        }

        public Response<T> Map(Func<TProjection, T> map)
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