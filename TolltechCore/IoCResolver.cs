using System;
using System.Linq;

namespace TolltechCore
{
    public class IoCResolver
    {
        public static void Resolve(Action<Type, Type> resolve, params string[] assemblyNames)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => assemblyNames.Any(y => x.FullName.StartsWith(y))).ToArray();
            var interfaces = assemblies.SelectMany(x => x.GetTypes().Where(y => y.IsInterface)).ToArray();
            var types = assemblies.SelectMany(x => x.GetTypes().Where(y => !y.IsInterface && y.IsClass && !y.IsAbstract)).ToArray();
            foreach (var @interface in interfaces)
            {
                var realizations = types.Where(x => @interface.IsAssignableFrom(x)).ToArray();
                foreach (var realization in realizations)
                {
                    resolve(@interface, realization);
                }
            }
        }
    }
}