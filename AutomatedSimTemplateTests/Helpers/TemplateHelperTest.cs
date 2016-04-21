using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SimTemplate.Helpers;
using SimTemplate.Model;
using SimTemplate.ViewModel.MainWindow;

namespace AutomatedSimTemplateTests.Helpers
{
    [TestClass]
    public class TemplateHelperTest
    {
        private const string TEMPLATE_1_HEX = "464D5200203230000000003C0000012C019000C500C5010000105B054087000B660080B5003B6700407100176C00407600346D0080A0004BE2000000";
        private static readonly IEnumerable<MinutiaRecord> TEMPLATE_1_MINUTAE = new List<MinutiaRecord>()
        {
            { new MinutiaRecord(new Point(135.918, 11.429), 143.746, MinutiaType.Termination) },
            { new MinutiaRecord(new Point(181.633, 59.592), 145.008, MinutiaType.Bifurication) },
            { new MinutiaRecord(new Point(113.061, 23.265), 152.103, MinutiaType.Termination) },
            { new MinutiaRecord(new Point(118.776, 52.653), 154.654, MinutiaType.Termination) },
            { new MinutiaRecord(new Point(160, 75.918), 319.086, MinutiaType.Bifurication) },
        };

        private const string TEMPLATE_2_HEX = "464D5200203230000000003C0000012C019000C500C5010000105B0580DE01C857004039017A7400807B0035F90040E7FFFBC00080507652E2000000";
        private static readonly IEnumerable<MinutiaRecord> TEMPLATE_2_MINUTAE = new List<MinutiaRecord>()
        {
            { new MinutiaRecord(new Point(222.1, 456.7), 123.41234, MinutiaType.Bifurication) },
            { new MinutiaRecord(new Point(57.2, 378.999), 164.5, MinutiaType.Termination) },
            { new MinutiaRecord(new Point(123.667, 53.1234), 350.2346, MinutiaType.Bifurication) },
            { new MinutiaRecord(new Point(231.223, 65531.12345), 270.4235, MinutiaType.Termination) },
            { new MinutiaRecord(new Point(80, 30290.1235), 319.086, MinutiaType.Bifurication) },
        };

        [TestMethod]
        public void TestTemplate1()
        {
            TestToIsoTemplate(TEMPLATE_1_MINUTAE, TEMPLATE_1_HEX);
        }

        [TestMethod]
        public void TestTemplate2()
        {
            TestToIsoTemplate(TEMPLATE_2_MINUTAE, TEMPLATE_2_HEX);
        }

        private void TestToIsoTemplate(IEnumerable<MinutiaRecord> minutae, string isoTemplateHex)
        {
            // Get the IsoTemplate
            byte[] template = TemplateHelper.ToIsoTemplate(minutae);
            // Convert the IsoTemplate back to a list of minutia (loss of data in casting)
            IEnumerable<MinutiaRecord> convert_minutae = TemplateHelper.ToMinutae(template);

            // Convert it to Hex for comparison
            string templateHex = BitConverter.ToString(template);
            templateHex = templateHex.Replace("-", String.Empty);

            // Assertions
            CollectionAssert.AreEqual(TemplateHelper.ToByteArray(isoTemplateHex), template);
            Assert.AreEqual(minutae.Count(), convert_minutae.Count());
            for (int i = 0; i < convert_minutae.Count(); i++)
            {
                MinutiaRecord real_minutia = minutae.ElementAt(i);
                MinutiaRecord converted_minutia = convert_minutae.ElementAt(i);
                Assert.AreEqual((int)real_minutia.Position.X, converted_minutia.Position.X);
                Assert.AreEqual((int)real_minutia.Position.Y, converted_minutia.Position.Y);
                // y(x,a) = ax - floor(ax)
                // max(y(x,a)) = 1, min(y(x,a)) = 0
                // e(x,a) = x - x_hat =  1/a * floor(ax) = 1/a * y(x,a)
                // Thus max(e(x,a)) = 1/a, min(e(x,a)) = 0
                Assert.IsTrue(real_minutia.Angle - converted_minutia.Angle < 1.0 / (256 / 360));
            }
        }
    }
}
