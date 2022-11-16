using System;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Abstractions.Rest;

namespace AVS.CoreLib.REST.Extensions
{
    public static class PropsContainerExtensions
    {
        public static T GetProps<T>(this IPropsContainer container)
        {
            var props = (T)container.Props;
            if (Equals(props, default(T)))
                throw new ArgumentException($"Props {typeof(T).Name} are required");
            return props;
        }
    }
}