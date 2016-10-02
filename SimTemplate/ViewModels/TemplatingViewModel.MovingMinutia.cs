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
using System.Windows.Input;
using SimTemplate.Utilities;
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.ViewModels
{
    public partial class TemplatingViewModel
    {
        private object m_SelectedMinutiaLock = new object();

        public class MovingMinutia : Initialised
        {
            public MovingMinutia(TemplatingViewModel outer) : base(outer)
            { }

            public override void EscapeAction()
            {
                // Ignore.
            }

            public override void PositionInput(Point position)
            {
                if (Outer.m_SelectedMinutia != null)
                {
                    StopMove();
                }
            }

            public override void PositionUpdate(Point position)
            {
                IntegrityCheck.IsNotNull(position);

                lock (Outer.m_SelectedMinutiaLock)
                {
                    IntegrityCheck.IsNotNull(Outer.m_SelectedMinutia.HasValue);
                    // Set position
                    Outer.Minutae[Outer.m_SelectedMinutia.Value].Position = position;
                }
            }

            public override void RemoveMinutia(int index)
            {
                // Ignore.
            }

            public override void SetMinutiaType(MinutiaType type)
            {
                // Ignore.
            }

            public override void StartMove(int index)
            {
                StopMove();
            }

            private void StopMove()
            {
                lock (Outer.m_SelectedMinutiaLock)
                {
                    Outer.m_SelectedMinutia = null;
                }
                TransitionTo(typeof(WaitLocation));
            }
        }
    }
}
