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
using SimTemplate.ViewModels;
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel
    {
        public class Idle : MainWindowState
        {
            public Idle(MainWindowViewModel outer) : base(outer, Activity.Idle)
            { }

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Clear status indicators
                Outer.PromptText = "Ready";
            }

            public override void LoadFile()
            {
                TransitionTo(typeof(Loading));
            }

            public override void SetScannerType(ScannerType type)
            {
                // Ignore.
            }

            public override void DataController_GetCaptureComplete(GetCaptureCompleteEventArgs e)
            {
                // Ignore.
                // We may have a race condition on Aborting a get capture request.
            }
        }
    }
}
