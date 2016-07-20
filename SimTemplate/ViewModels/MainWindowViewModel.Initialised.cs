using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SimTemplate.Helpers;
using SimTemplate.Model.DataControllers.EventArguments;

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel
    {
        public abstract class Initialised : MainWindowState
        {
            #region Constants

            private const int MAX_INVALID_FILES = 10;

            #endregion

            public Initialised(MainWindowViewModel outer) : base(outer)
            { }

            #region Overriden Methods

            public override void DataController_InitialisationComplete(InitialisationCompleteEventArgs e)
            {
                throw IntegrityCheck.Fail("Not expected to have InitialisationComplete event when initialised.");
            }

            #endregion

        }
    }
}
