// Copyright 2016 Sam Briggs
//
// This file is part of SimTemplate.
//
// SimTemplate is free software: you can redistribute it and/or modify it under the
// terms of the GNU General Public License as published by the Free Software 
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// SimTemplate is distributed in the hope that it will be useful, but WITHOUT ANY 
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
// A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// SimTemplate. If not, see http://www.gnu.org/licenses/.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimTemplate.Utilities
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
