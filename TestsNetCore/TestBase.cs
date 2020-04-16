using Ninject;
using NUnit.Framework;

namespace Tolltech.TestsNetCore
{
    [TestFixture]
    public abstract class TestBase
    {
        protected StandardKernel container;

        [SetUp]
        protected virtual void SetUp()
        {
            container = new StandardKernel(new ConfigurationModule());
        }
    }
}