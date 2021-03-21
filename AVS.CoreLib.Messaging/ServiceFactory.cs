using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Messaging
{
    /// <summary>
    /// Represents delegate which is bound to <see cref="T:System.IServiceProvider" /> GetService method 
    /// Get a service of type <paramref name="serviceType" /> from the <see cref="T:System.IServiceProvider" />  
    /// </summary>
    public delegate object ServiceFactory(Type serviceType);

    /// <summary>
    /// Represents delegate which is bound to <see cref="T:System.IServiceProvider" /> GetServices extension method 
    /// Get an enumeration of services of type <paramref name="serviceType" /> from the <see cref="T:System.IServiceProvider" /> 
    /// </summary>
    public delegate IEnumerable<object> ServiceFactoryResolveAll(Type serviceType);
}