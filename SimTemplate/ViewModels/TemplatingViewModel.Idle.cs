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
using SimTemplate.DataTypes;
using SimTemplate.Utilities;

namespace SimTemplate.ViewModels
{
    public partial class TemplatingViewModel
    {
        public class Idle : Initialised
        {
            #region Constructor

            public Idle(TemplatingViewModel outer) : base(outer)
            { }

            #endregion

            #region Virtual Overrides

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Hide old image from UI, and remove other things.
                Outer.Capture = null;
                Outer.Minutae.Clear();
            }

            #region View Methods

            public override void PositionUpdate(Point position)
            {
                // Ignore a position change when Idle.
            }

            #endregion

            #endregion
        }
    }
}
