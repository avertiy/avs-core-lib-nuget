using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;
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

        public string JsonText { get; set; }
        public string Error { get; set; }
        public string Source { get; set; }

        public bool IsEmpty => (string.IsNullOrEmpty(JsonText) || JsonText == "{}" || JsonText == "[]") && Error == null;

        protected Projection(string jsonText, string source, string error = null)
        {
            JsonText = jsonText;
            Source = source;
            Error = error;

            //to-do JsonText could be tested on error strait here in c-tor thus it will simplify the logic about error detection
            //will do that later..
        }

        protected bool ContainsError(out string error)
        {
            if (!string.IsNullOrEmpty(Error))
            {
                if (string.IsNullOrEmpty(JsonText))
                {
                    error = Error;
                }
                else
                {
                    error = Error + " " + JsonText;
                }
                return true;
            }

            error = null;

            if (IsEmpty)
            {
                return false;
            }

            var match = ErrorRegex.Match(JsonText);
            if (match.Success)
            {
                error = match.Groups["error"].Value;
                if (string.IsNullOrEmpty(error))
                    error = JsonText;
            }

            if (JsonText.Length < 200 && JsonText.Contains("code") && JsonText.Contains("message"))
            {
                error = JsonText;
            }

            return error != null;
        }

        protected Response<T> CreateResponse<T>() where T : new()
        {
            var response = Response.Create<T>();
            if (ContainsError(out var err))
            {
                response.Error = err;
            }else if (IsEmpty)
            {
                response.Data = new T();
            }
            return response;
        }

        protected Response<T> CreateResponse<T, TProjection>() where TProjection : T, new()
        {
            var response = Response.Create<T>();
            response.Source = Source;
            if (ContainsError(out var err))
            {
                response.Error = err;
            }else if (IsEmpty)
            {
                response.Data = new TProjection();
            }
            return response;
        }

        protected MapResult CreateMapResult<TProjection>() where TProjection : new()
        {
            var result = new MapResult() { Source = Source };
            if (ContainsError(out var err))
            {
                result.Error = err;
            }else if (IsEmpty)
            {
                result.Data = new TProjection();
            }
            return result;
        }

        protected string _selectTokenPath;

        protected Task<Response<T>> MapAsyncInternal<T>(Func<Response<T>> map)
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
                throw new MapException($"{this.GetType().ToStringNotation()}::Map json failed.", ex) { JsonText = JsonText, Source = Source };
            }
        }

    }
}