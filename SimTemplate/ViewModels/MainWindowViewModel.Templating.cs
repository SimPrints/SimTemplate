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
using SimTemplate.Utilities;
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel
    {
        public class Templating : MainWindowState
        {
            #region Constructor

            public Templating(MainWindowViewModel outer) : base(outer, Activity.Templating)
            { }

            #endregion

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Initialise the TemplatingViewModel so that we can process images.
                Outer.m_TemplatingViewModel.BeginInitialise();
            }

            public override void OnLeavingState()
            {
                base.OnLeavingState();

                // Clear the TemplatingViewModel of any leftover information
                Outer.TemplatingViewModel.QuitTemplating();
            }

            public override void LoadFile()
            {
                TransitionTo(typeof(Loading));
            }

            public override void SaveTemplate()
            {
                IntegrityCheck.IsFalse(Outer.m_TemplatingViewModel.IsSaveTemplatePermitted);
                TransitionTo(typeof(Saving));
            }

            public override void SetScannerType(ScannerType type)
            {
                // TODO: Prompt if user wants to save their work?
                // if (Outer.m_Minutia)
                // {
                // }
                // TransitionTo(typeof(Loading));
            }

            public override void EscapeAction()
            {
                // Pass escape handling to TemplatingViewModel
                Outer.m_TemplatingViewModel.EscapeAction();
            }
        }
    }
}
