using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Utils
{
    public class DtoMapper<TItem, TDto>
    {
        private readonly Func<TItem, TDto> _map;
        public DtoMapper(Func<TItem, TDto> map)
        {
            _map = map;
        }

        public IList<TDto> Map(IEnumerable<TItem> data)
        {
            var list = new List<TDto>();
            foreach (var model in data)
            {
                list.Add(_map(model));
            }
            return list;
        }

        public TDto Map(TItem data)
        {
            return _map(data);
        }
    }
}