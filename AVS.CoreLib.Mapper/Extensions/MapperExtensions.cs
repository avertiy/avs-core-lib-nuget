using System.Linq;

namespace AVS.CoreLib.Mapper.Extensions
{
    public static class MapperExtensions
    {
        public static string[] GetMapperKeys(this IMapper mapper)
        {
            return mapper.Keys.Where(x => x.Contains("->")).ToArray();
        }

        public static string[] GetUpdateMapperKeys(this IMapper mapper)
        {
            return mapper.Keys.Where(x => x.Contains("->") == false).ToArray();
        }
    }
}
