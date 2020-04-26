using Ninject.Modules;
using TolltechCore;

namespace Tolltech.TestsNetCore
{
    public class ConfigurationModule : NinjectModule
    {
        public override void Load()
        {
            IoCResolver.Resolve((@interface, implementation) => this.Bind(@interface).To(implementation), null, "Tolltech");
        }
    }
}