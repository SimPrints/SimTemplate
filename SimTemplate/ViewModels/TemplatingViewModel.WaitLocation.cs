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
using SimTemplate.ViewModels;
using SimTemplate.DataTypes;
using SimTemplate.DataTypes.Enums;
using SimTemplate.ViewModels.Interfaces;

namespace SimTemplate.ViewModels
{
    public partial class TemplatingViewModel
    {
        public class WaitLocation : Initialised
        {
            // TODO: put string resources in a resource manager
            private const string PLACE_MINUTIA_PROMPT = "Please place minutia";

            #region Constructor

            public WaitLocation(TemplatingViewModel outer) : base(outer)
            { }

            #endregion

            #region Virtual Overrides

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                Outer.OnUserActionRequired(new UserActionRequiredEventArgs(PLACE_MINUTIA_PROMPT));
            }

            #region View Methods

            public override void PositionInput(Point position)
            {
                // The user is starting to record a new minutia

                // Start a new minutia data record.
                MinutiaRecord record = new MinutiaRecord();

                // Save the position
                record.Position = position;
                // Save the current type.
                record.Type = Outer.InputMinutiaType;
                // Record minutia information.
                Outer.Minutae.Add(record);

                // Indicate next input defines the direction.
                TransitionTo(typeof(WaitDirection));
            }

            public override void PositionUpdate(Point e)
            {
                // Ignore.
            }

            public override void RemoveMinutia(int index)
            {
                // Remove the item at the specified index.
                Outer.Minutae.RemoveAt(index);
            }

            #endregion

            #region ITemplatingViewModel

            public override void EscapeAction()
            {
                // Nothing to escape.
            }

            public override void SetMinutiaType(MinutiaType type)
            {
                // Ignore. No current record to update.
            }

            public override void StartMove(int index)
            {
                Outer.m_SelectedMinutia = index;
                TransitionTo(typeof(MovingMinutia));
            }

            public override byte[] GetTemplate()
            {
                // Return template in ISO template format.
                return IsoTemplateHelper.ToIsoTemplate(Outer.Minutae);
            }

            #endregion

            #endregion

            #region Helper Methods

            private static string ToRecord(MinutiaRecord labels)
            {
                return String.Format("{0}, {1}, {2}, {3}",
                    labels.Position.X, labels.Position.Y, labels.Angle, labels.Type);
            }

            #endregion
        }
    }
}
