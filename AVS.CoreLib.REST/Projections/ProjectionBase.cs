#nullable enable
using System.IO;
using System.Net;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.REST.Helpers;
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
        public TToken LoadToken<TToken>(string json) where TToken : JToken
        {
            using var stringReader = new StringReader(json);
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
    }
}