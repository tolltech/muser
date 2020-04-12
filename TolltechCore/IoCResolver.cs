using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TolltechCore
{
    public static class IoCResolver
    {
        public static void Resolve(Action<Type, Type> resolve, params string[] assemblyNames)
        {

            //System.Reflection.Assembly.GetExecutingAssembly()
            //    .GetTypes()
            //    .Where(item => item.GetInterfaces()
            //        .Where(i => i.IsGenericType).Any(i => i.GetGenericTypeDefinition() == typeof(IDatabaseService<>)) && !item.IsAbstract && !item.IsInterface)
            //    .ToList()
            //    .ForEach(assignedTypes =>
            //    {
            //        var serviceType = assignedTypes.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(IDatabaseService<>));
            //        services.AddScoped(serviceType, assignedTypes);
            //    });

            var assemblyNameHashSet = new HashSet<string>(assemblyNames);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => assemblyNameHashSet.Any(y => x.FullName.StartsWith(y)))
                .ToArray();

            assemblies = assemblies
                .Concat(assemblies
                    .SelectMany(x => x.GetReferencedAssemblies())
                    .Where(x => assemblyNameHashSet.Any(y => x.FullName.StartsWith(y)))
                    .Select(Assembly.Load))
                .GroupBy(x => x.FullName)
                .Select(x => x.First())
                .ToArray();

            var interfaces = assemblies.SelectMany(x => x.GetTypes().Where(y => y.IsInterface)).ToArray();
            var types = assemblies
                .SelectMany(x => x.GetTypes().Where(y => !y.IsInterface && y.IsClass && !y.IsAbstract)).ToArray();
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