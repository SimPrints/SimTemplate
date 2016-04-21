using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomatedTests
{
    [TestClass]
    public static class AssemblyInitializer
    {
        [AssemblyInitialize]
        public static void Configure(TestContext tc)
        {
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}