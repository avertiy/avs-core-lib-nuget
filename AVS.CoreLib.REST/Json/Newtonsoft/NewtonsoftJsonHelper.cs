#nullable enable
using System;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Json.Newtonsoft
{
    internal static class NewtonsoftJsonHelper
    {
        internal static T? Deserialize<T>(JToken? jToken, Type itemType)
        {
            if (jToken == null)
                return default;

            return (T?)NewtonsoftJsonSerializer.Serializer.Deserialize(jToken.CreateReader(), itemType);
        }

        /// <summary>
        /// populate the JSON values onto the target object
        /// by utilizing JsonSerializer.Populate(jToken.CreateReader(), target); 
        /// </summary>        
        internal static void Populate<T>(JToken jToken, T target)
        {
            NewtonsoftJsonSerializer.Serializer.Populate(jToken.CreateReader(), target!);
        }        
    }
}