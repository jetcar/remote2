using System.Collections.Generic;

namespace IoC
{
    public class IocKernel
    {
        public static Dictionary<object,object> registeredServices = new Dictionary<object, object>();


        public static T GetInstance<T>()
        {
            return (T)registeredServices[typeof(T)];
        }
    }
}
