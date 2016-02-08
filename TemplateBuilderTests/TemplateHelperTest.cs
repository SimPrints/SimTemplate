using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TemplateBuilder.Model;
using TemplateBuilder.ViewModel.MainWindow;

namespace TemplateBuilderTests
{
    [TestClass]
    public class TemplateHelperTest
    {
        [TestMethod]
        public void TestToIsoTemplate()
        {
            string output = "464D5200203230000000003C0000012C019000C500C5010000105B054087000B660080B5003B6700407100176C00407600346D0080A0004BE2000000";

            IList<MinutiaRecord> minutae = new List<MinutiaRecord>()
            {
                { new MinutiaRecord(new Point(135.918, 11.429), 143.746, MinutiaType.Termination) },
                { new MinutiaRecord(new Point(181.633, 59.592), 145.008, MinutiaType.Bifurication) },
                { new MinutiaRecord(new Point(113.061, 23.265), 152.103, MinutiaType.Termination) },
                { new MinutiaRecord(new Point(118.776, 52.653), 154.654, MinutiaType.Termination) },
                { new MinutiaRecord(new Point(160, 75.918), 319.086, MinutiaType.Bifurication) },
            };

            byte[] template = TemplateHelper.ToIsoTemplate(minutae);

            string templateHex = BitConverter.ToString(template);
            templateHex = templateHex.Replace("-", String.Empty);

            CollectionAssert.AreEqual(TemplateHelper.ToByteArray(output), template);
        }
    }
}
