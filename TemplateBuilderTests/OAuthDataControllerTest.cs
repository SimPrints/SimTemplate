using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateBuilder.Model.Database;

namespace TemplateBuilderTests
{
    [TestClass]
    [System.Runtime.InteropServices.Guid("141707AC-B412-49FD-9362-D7BFD191776E")]
    public class OAuthDataControllerTest
    {
        [TestMethod]
        public void TestRequest()
        {
            OAuthDataController controller = new OAuthDataController();

            Assert.IsNotNull(controller);
        }
    }
}
