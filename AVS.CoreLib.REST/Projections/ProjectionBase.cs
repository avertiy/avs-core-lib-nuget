#nullable enable
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.REST.Helpers;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
    public abstract class ProjectionBase
    {
        private string? _selectTokenPath;

        public string JsonText { get; set; }
        public string Source { get; set; }
        public IRequest? Request { get; set; }
        public string? Error { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsEmpty => (string.IsNullOrEmpty(JsonText) || JsonText == "{}" || JsonText == "[]") && Error == null;
        public bool HasError => Error != null;

        protected ProjectionBase(RestResponse response)
        {
            Source = response.Source;
            Request = response.Request;
            StatusCode = response.StatusCode;
            JsonText = response.Content ?? string.Empty;
            Error = GetErrorText(response);
        }

        private string? GetErrorText(RestResponse response)
        {
            var error = response.Error;
            if (!string.IsNullOrEmpty(error))
                return error;

            if (ResponseHelper.ContainsError(response.Content, out error))
                return error;

            if (!response.IsSuccessStatusCode)
                return response.StatusCode.ToString();

            return null;
        }

        protected Response<T> CreateResponse<T>() where T : new()
        {
            var response = Response.Create<T>(Source, Error, Request);
            if (IsEmpty)
            {
                response.Data = new T();
            }
            return response;
        }

        protected Response<T> CreateResponse<T, TProjection>() where TProjection : T, new()
        {
            var response = Response.Create<T>(Source, Error, Request);
            if (IsEmpty)
            {
                response.Data = new TProjection();
            }
            return response;
        }

        /// <summary>
        /// Selects a <see cref="T:Newtonsoft.Json.Linq.JToken" /> using a JSONPath expression. Selects the token that matches the object path.
        /// </summary>
        /// <param name="path">
        /// A <see cref="T:System.String" /> that contains a JSONPath expression.
        /// </param>
        public void SelectToken(string path)
        {
            _selectTokenPath = path;
        }

        //method is public to allow caller code to debug token deserialization in case map fails without error
        public TToken LoadToken<TToken>() where TToken : JToken
        {
            using var stringReader = new StringReader(JsonText);
            using var reader = new JsonTextReader(stringReader);
            var token = JToken.Load(reader);

            if (_selectTokenPath != null)
            {
                token = token.SelectToken(_selectTokenPath);
                if (token == null)
                    throw new JsonReaderException($"Invalid token path {_selectTokenPath}");
            }

            if (token is TToken tToken)
            {
                return tToken;
            }

            throw new JsonReaderException($"Unexpected JTokenType {token.Type}, expected {typeof(TToken).Name} [json: {JsonText.Truncate(100)}]");
        }

        protected void LoadToken(Action<JToken> action)
        {
            try
            {
                var token = LoadToken<JToken>();
                action(token);
            }
            catch (Exception ex)
            {
                throw;
                //throw new MapException("LoadToken failed", ex) {JsonText = JsonText, Source = Source };
            }
        }

        protected void LoadToken<TToken, T, TItem>(Action<TToken> action) where TToken : JContainer
        {
            try
            {
                var token = LoadToken<TToken>();
                action(token);
            }
            catch (Exception ex)
            {
                throw;
                //throw new MapJsonException<T, TItem>(ex) { JsonText = JsonText, Source = Source };
            }
        }

        protected void LoadToken<TToken, TProjection>(Action<TToken> action) where TToken : JContainer
        {
            try
            {
                var token = LoadToken<TToken>();
                action(token);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected Response<T> MapInternal<T>(Action<Response<T>> map)
        {
            var response = Response.Create<T>(Source, Error, Request);
            if (HasError)
                return response;
            try
            {
                map(response);
                return response;
            }
            catch (MapException) { throw; }
            catch (Exception ex)
            {
                throw new MapException($"{this.GetType().GetReadableName()}::Map json failed.", ex) { JsonText = JsonText, Source = Source };
            }
        }

        protected Task<Response<T>> MapAsyncInternal<T>(Func<Response<T>> map)
        {
            return IsEmpty || JsonText.Length < 2000 ? Task.FromResult(map()) : Task.Factory.StartNew(map);
        }

    }
}