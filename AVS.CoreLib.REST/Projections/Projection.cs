using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AVS.CoreLib.REST.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
    public abstract class Projection
    {
        public static Regex ErrorRegex =
            new Regex("(error|err-msg|error-message)\\\\?[\"']:[\\s\\\\\"']+(?<error>[=():/\\-\\.\\?\\w\\s&]+)[\"']?",
                RegexOptions.IgnoreCase);

        protected string JsonText { get; set; }

        protected string Source { get; set; }

        protected Projection(string jsonText, string source = null)
        {
            JsonText = jsonText;
            Source = source;
        }

        public virtual bool IsEmpty => string.IsNullOrEmpty(JsonText) || JsonText == "{}" || JsonText == "[]";

        protected bool ContainsError(out string error)
        {
            error = null;
            if (IsEmpty)
                return false;

            var match = ErrorRegex.Match(JsonText);
            if (match.Success)
            {
                error = match.Groups["error"].Value;
                if (string.IsNullOrEmpty(error))
                    error = JsonText;
            }

            return error != null;
        }

        protected Response<T> CreateResponse<T>() where T : new()
        {
            var response = Response.Create<T>();
            if (IsEmpty)
            {
                response.Data = new T();
            }
            else if (ContainsError(out string err))
            {
                response.Error = err;
            }
            return response;
        }

        protected Response<T> CreateResponse<T, TProjection>() where TProjection : T, new()
        {
            var response = Response.Create<T>();
            response.Source = Source;
            if (IsEmpty)
            {
                response.Data = new TProjection();
            }
            else if (ContainsError(out string err))
            {
                response.Error = err;
            }
            return response;
        }

        protected MapResult CreateMapResult<TProjection>() where TProjection : new()
        {
            var result = new MapResult() { Source = Source };
            if (IsEmpty)
            {
                result.Data = new TProjection();
            }
            else if (ContainsError(out string err))
            {
                result.Error = err;
            }
            return result;
        }

        protected string _selectTokenPath;

        protected Task<Response<T>> MapAsync<T>(Func<Response<T>> map)
        {
            return IsEmpty || JsonText.Length < 2000 ? Task.FromResult(map()) : Task.Factory.StartNew(map);
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

            throw new JsonReaderException($"Unexpected JToken type {token.Type} [expect {typeof(TToken).Name}");
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
                throw new MapException("LoadToken failed", ex) {JsonText = JsonText };
            }
        }

        protected void LoadToken<TToken, TProjection, TItem>(Action<TToken> action) where TToken : JContainer
        {
            try
            {
                var token = LoadToken<TToken>();
                action(token);
            }
            catch (Exception ex)
            {
                throw new MapJsonException<TProjection, TItem>(ex);
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
                throw new MapJsonException<TProjection>(ex);
            }
        }

        protected Response<T> MapInternal<T>(Action<Response<T>> map)
        {
            var response = Response.Create<T>();
            response.Source = Source;
            try
            {
                if (ContainsError(out var err))
                    response.Error = err;
                else
                    map(response);

                return response;
            }
            catch (MapException) { throw; }
            catch (Exception ex)
            {
                throw new MapException("Projection map failed", ex) { JsonText = JsonText };
            }
        }

    }
}