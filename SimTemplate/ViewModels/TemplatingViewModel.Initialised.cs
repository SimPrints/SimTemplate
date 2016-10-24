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
using SimTemplate.DataTypes;
using SimTemplate.Utilities;

namespace SimTemplate.ViewModels
{
    public partial class TemplatingViewModel
    {
        public class Initialised : TemplatingState
        {
            #region Constructor

            public Initialised(TemplatingViewModel outer) : base(outer)
            { }

            #endregion

            #region Overriden Methods

            public override void BeginInitialise()
            {
                // If we are already initialised then ignore this call
                // If the ApiKey is changed we don't want to lose any work we've been doing
                // Ignore.
            }

            public override void BeginTemplating(CaptureInfo capture)
            {
                // Prepare to start templating a new capture
                // Clear the UI of any previous work
                Outer.m_DispatcherHelper.Invoke(new Action(() =>
                {
                    Outer.Minutae.Clear();
                }));

                // Record the new capture
                Outer.Capture = capture;
                if (Outer.Capture.TemplateData != null)
                {
                    // If there is a template in the capture info, load it.
                    IEnumerable<MinutiaRecord> template = IsoTemplateHelper
                        .ToMinutae(Outer.Capture.TemplateData);
                    foreach (MinutiaRecord rec in template)
                    {
                        // Ensure we use the UI thread to add to the ObservableCollection.
                        App.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Outer.Minutae.Add(rec);
                        }));
                    }
                }
                TransitionTo(typeof(WaitLocation));
            }

            public override void QuitTemplating()
            {
                // NOTE: Clearing minutae must happen before clearing the capture
                // Minutae position is bound to capture image size!
                Outer.m_DispatcherHelper.Invoke(new Action(() =>
                {
                    Outer.Minutae.Clear();
                }));
                Outer.Capture = null;

                TransitionTo(typeof(Idle));
            }

            #endregion
        }
    }
}
