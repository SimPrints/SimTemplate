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
                // Prepare to start templating.
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Outer.Minutae.Clear();
                }));

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

            #endregion
        }
    }
}
