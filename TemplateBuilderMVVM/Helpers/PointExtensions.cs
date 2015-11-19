using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TemplateBuilderMVVM.Helpers
{
    public static class PointExtensions
    {
        /// <summary>
        /// Scales the point by the specified scaling factor.
        /// </summary>
        /// <param name="p">The point</param>
        /// <param name="factor">The scaling factor.</param>
        /// <returns></returns>
        public static Point Scale(this Point p, double factor)
        {
            return new Point(
                p.X * factor,
                p.Y * factor);
        }
    }
}
