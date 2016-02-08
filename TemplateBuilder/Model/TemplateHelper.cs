using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateBuilder.Helpers;
using TemplateBuilder.ViewModel.MainWindow;

namespace TemplateBuilder.Model
{
    public static class TemplateHelper
    {
        #region Constants

        private const string HEADER_TOP = "464D520020323000000000";
        private const string HEADER_BOTTOM = "0000012C019000C500C5010000105B";

        private const byte BIFURICATION = 0x80;
        private const byte TERMINATION = 0x40;
        private const byte OTHER = 0x00;
        private const byte QUALITY = 0x00;

        private const Int16 END = 0x0000;

        #endregion

        public static byte[] ToIsoTemplate(IList<MinutiaRecord> minutae)
        {
            List<byte> data = new List<byte>() { };

            data.AddRange(ToByteArray(HEADER_TOP));
            data.Add((byte)(27 + 3 + 6 * minutae.Count()));
            data.AddRange(ToByteArray(HEADER_BOTTOM));
            data.Add((byte)(minutae.Count()));

            foreach (MinutiaRecord minutia in minutae)
            {
                byte type;
                switch (minutia.Type)
                {
                    case MinutiaType.Termination:
                        type = TERMINATION;
                        break;

                    case MinutiaType.Bifurication:
                        type = BIFURICATION;
                        break;

                    default:
                        throw IntegrityCheck.FailUnexpectedDefault(minutia.Type);
                }

                data.Add(type);
                data.Add((byte)minutia.Location.X);
                byte[] angle = BitConverter.GetBytes((Int16)minutia.Location.Y);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(angle);
                }
                data.AddRange(angle);
                data.Add((byte)(minutia.Direction * 256 / 360));
                data.Add(QUALITY);
            }

            data.AddRange(BitConverter.GetBytes(END));

            return data.ToArray();
        }

        public static byte[] ToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ToHex(byte[] data)
        {
            return BitConverter.ToString(data);
        }
    }
}
