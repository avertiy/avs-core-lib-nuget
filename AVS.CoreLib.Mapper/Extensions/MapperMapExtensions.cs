using System;

namespace AVS.CoreLib.Mapper.Extensions
{
    public static class MapperMapExtensions
    {
        

        /// <summary>
        /// Execute mapping to PRODUCE NEW <see cref="TDestination"/> object
        /// </summary>
        public static TDestination Map<TSource, TDestination>(this IMapper mapper, TSource source, Action<TDestination> modify, string delegateRef)
        {
            var model = mapper.Map<TSource, TDestination>(source, delegateRef);
            modify.Invoke(model);
            return model;
        }
    }
}
