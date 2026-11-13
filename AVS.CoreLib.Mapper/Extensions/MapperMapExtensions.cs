using System;

namespace AVS.CoreLib.Mapper.Extensions
{
    /// <summary>
    /// Map extensions
    /// </summary>
    public static class MapperMapExtensions
    {
        /// <summary>
        /// Execute mapping to PRODUCE NEW <typeparamref name="TDestination"/> object, while <paramref name="modify"/> allows to modify it
        /// </summary>
        public static TDestination Map<TSource, TDestination>(this IMapper mapper, TSource source, Action<TDestination> modify, string delegateRef)
        {
            var model = mapper.Map<TSource, TDestination>(source, delegateRef);
            modify.Invoke(model);
            return model;
        }
    }
}
