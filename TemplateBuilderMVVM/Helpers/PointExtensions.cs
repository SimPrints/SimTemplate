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

        /// <summary>
        /// Scales the point by the specified eigenvalues. Where the eigenvectors are the x and y
        /// unit vectors.
        /// </summary>
        /// <param name="p">The point.</param>
        /// <param name="eigenvalues">The eigenvalues.</param>
        /// <returns></returns>
        public static Point Scale(this Point p, Vector eigenvalues)
        {
            return new Point(
                p.X * eigenvalues.X,
                p.Y * eigenvalues.Y);
        }

        /// <summary>
        /// Scales the point by the inverse of the specified factor.
        /// </summary>
        /// <param name="p">The point.</param>
        /// <param name="factor">The factor.</param>
        /// <returns></returns>
        public static Point InvScale(this Point p, double factor)
        {
            return new Point(
                p.X * 1 / factor,
                p.Y * 1 / factor);
        }

        /// <summary>
        /// Scales the point by the inverses of the specified eigenvalues, where the eigenvectors
        /// are the x and y unit vectors.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="eigenvalues">The eigenvalues.</param>
        /// <returns></returns>
        public static Point InvScale(this Point p, Vector eigenvalues)
        {
            return new Point(
                p.X * 1 / eigenvalues.X,
                p.Y * 1 / eigenvalues.Y);
        }
    }
}
