using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Utilities
{
    /// <summary>
    /// Helper class for mathematical operations
    /// </summary>
    public static class MathsHelper
    {
        /// <summary>
        /// Converts an angle from radians to degrees.
        /// </summary>
        /// <param name="angle">The angle radians.</param>
        /// <returns>the angle in degrees (between 0-360)</returns>
        public static double RadianToDegree(double angle)
        {
            double angleDegrees = angle * (180.0 / Math.PI);
            while (angleDegrees < 0)
            {
                angleDegrees += 360;
            }
            return angleDegrees;
        }
    }
}
