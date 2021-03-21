using System;
using AVS.CoreLib.Abstractions;

namespace AVS.CoreLib.Extensions
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