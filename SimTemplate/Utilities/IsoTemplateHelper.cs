using System;
using System.Collections.Generic;
using System.Linq;
using SimTemplate.DataTypes;
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.Utilities
{
    /// <summary>
    /// A static helper class for converting between TemplateBuilder's MinutiaRecord class and the
    /// ISO 19794-2 standard fingerprint template.
    /// </summary>
    public static class IsoTemplateHelper
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

        /// <summary>
        /// Convert a collection of MinutiaRecords to the ISO 19794-2 standard template.
        /// NOTE: ISO template stores data as Int8 (X and angle) and Int16 values (Y) so there may
        /// be loss of data when casting.
        /// </summary>
        /// <param name="minutae">The minutae.</param>
        /// <returns></returns>
        public static byte[] ToIsoTemplate(IEnumerable<MinutiaRecord> minutae)
        {
            // TODO: Better understand what bytes in IsoTemplate are (Header).
            // ISO 19794-2?

            List<byte> data = new List<byte>() { };

            data.AddRange(ToByteArray(HEADER_TOP));
            data.Add((byte)ToHeaderMinutiaCountByte(minutae.Count()));
            data.AddRange(ToByteArray(HEADER_BOTTOM));
            data.Add((byte)(minutae.Count()));

            foreach (MinutiaRecord minutia in minutae)
            {
                data.Add(TypeToByte(minutia.Type));
                data.Add((byte)minutia.Position.X);
                data.AddRange(YToByte(minutia.Position.Y));
                data.Add(AngleToByte(minutia.Angle));
                data.Add(QUALITY);
            }

            data.AddRange(BitConverter.GetBytes(END));

            return data.ToArray();
        }

        public static IEnumerable<MinutiaRecord> ToMinutae(byte[] isoTemplate)
        {
            IntegrityCheck.IsNotNull(isoTemplate);
            // TODO: IntegrityCheck start is HEADER_TOP
            // TODO: IntegrityCheck HEADER_BOTTOM

            int headerTopCount = ToByteArray(HEADER_TOP).Count(); // TODO: Make constant
            int minutiaCount_1 = ToMinutiaCount(isoTemplate[headerTopCount]);
            int headerBottomCount = ToByteArray(HEADER_BOTTOM).Count(); // TODO: Make constant
            int minutiaCount_2 = (UInt16)isoTemplate[headerTopCount + 1 + headerBottomCount];

            // TODO: IntegrityCheck minutiaCount_1 == minutiaCount_2
            int minutiaCount = minutiaCount_1;

            int startByte = headerTopCount + 1 + headerBottomCount + 1; // TODO: Make constant
            int bytesPerMinutia = 6; // TODO: Make constant
            List<MinutiaRecord> minutae = new List<MinutiaRecord>(minutiaCount);
            for (int i = 0; i < minutiaCount; i++)
            {
                MinutiaType type = TypeToMinutiaType(isoTemplate[startByte]);
                // TODO: IntegrityCheck type != None

                // Get Minutia X-Position
                int x = (UInt16)isoTemplate[startByte + 1];

                // Get Minutia Y-Position
                byte[] yData = new byte[2];
                Array.Copy(isoTemplate, startByte + 2, yData, 0, 2);
                int y = YToInt(yData);

                // Get Minutia angle from the two angle bytes.
                double angle = AngleToDouble(isoTemplate[startByte + 4]);

                // TODO: IntegrityCheck quality

                // Create minutia from data and add to list.
                MinutiaRecord minutia = new MinutiaRecord(
                    new System.Windows.Point(x, y),
                    angle,
                    type);
                minutae.Add(minutia);

                startByte += bytesPerMinutia;
            }
            // TODO: More assertions
            return minutae;
        }

        public static byte[] ToByteArray(string hex)
        {
            byte[] template;
            try
            {
                template = Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
            }
            catch (FormatException ex)
            {
                throw new SimTemplateException(
                    String.Format("Badly formatted hex string {0}", hex), ex);
            }
            return template;
        }

        //public static string ToHex(byte[] data)
        //{
        //    return BitConverter.ToString(data);
        //}

        public static double RadianToDegree(double rads)
        {
            double degs = rads * (180.0 / Math.PI);
            while (degs < 0)
            {
                degs += 360;
            }
            return degs;
        }

        #region Minutia Count

        private static byte ToHeaderMinutiaCountByte(int minutiaCount)
        {
            return (byte)(30 + 6 * minutiaCount);
        }

        private static int ToMinutiaCount(byte headerMinutiaCountByte)
        {
            return ((int)headerMinutiaCountByte - 30) / 6;
        }

        #endregion

        #region Minutia Type

        private static MinutiaType TypeToMinutiaType(byte typeData)
        {
            MinutiaType type = MinutiaType.None;
            if (typeData == TERMINATION)
            {
                type = MinutiaType.Termination;
            }
            else if (typeData == BIFURICATION)
            {
                type = MinutiaType.Bifurication;
            }
            return type;
        }

        private static byte TypeToByte(MinutiaType type)
        {
            byte typeData;
            switch (type)
            {
                case MinutiaType.Termination:
                    typeData = TERMINATION;
                    break;

                case MinutiaType.Bifurication:
                    typeData = BIFURICATION;
                    break;

                default:
                    throw IntegrityCheck.FailUnexpectedDefault(type);
            }
            return typeData;
        }

        #endregion

        #region Minutia Y

        private static byte[] YToByte(double y)
        {
            byte[] yData = BitConverter.GetBytes((Int16)y);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(yData);
            }
            return yData;
        }

        private static int YToInt(byte[] angleData)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(angleData);
            }
            return BitConverter.ToUInt16(angleData, 0);
        }

        #endregion

        #region Minutia Direction

        private static byte AngleToByte(double angle)
        {
            return (byte)((uint)(angle * 256.0 / 360.0));
        }

        private static double AngleToDouble(byte angleData)
        {
            return (double)(angleData / (256.0 / 360.0));
        }

        #endregion
    }
}
