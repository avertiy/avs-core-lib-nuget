using System;
using System.Collections.Generic;
using System.Diagnostics;
using AVS.CoreLib.Json;
using AVS.CoreLib.REST.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Projections
{
    /// <summary>
    /// basically this type is just a synonym of <see cref="ArrayProjection{T,TItem}"/>
    /// </summary>
    public class ListProjection<T, TItem> : ArrayProjection<T, TItem> where T : class
    {
        [DebuggerStepThrough]
        public ListProjection(string jsonText, string source = null) : base(jsonText, source)
        {
        }

        [Obsolete("Seems MapResult could be avoided, so it will be removed")]
        public virtual MapResult MapResult<TProjection>() where TProjection : TItem, new()
        {
            var result = CreateMapResult<List<TItem>>();
            if (result.Success)
            {
                LoadToken<JArray, TProjection, TItem>(jArray =>
                {
                    _preProcessAction?.Invoke(result.Data as T);
                    result.Data = JsonHelper.ParseList<TItem>(jArray, typeof(TProjection), _itemAction, _where);
                    _postProcessAction?.Invoke(result.Data as T);
                });
            }

            return result;
        }
    }
}