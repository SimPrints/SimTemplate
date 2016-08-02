using System;
using System.Windows;
using SimTemplate.Utilities;
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel
    {
        public class Error : MainWindowState
        {
            public Error(MainWindowViewModel outer) : base(outer, Activity.Fault)
            { }

            #region Overriden Methods

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Indicate we have errored
                Outer.PromptText = "Fault";

                // Clear UI
                Outer.m_TemplatingViewModel.QuitTemplating();
            }

            public override void DataController_InitialisationComplete(InitialisationCompleteEventArgs e)
            {
                throw IntegrityCheck.Fail(
                    "Not expected to have DataController.InitialisationComplete event when in error.");
            }

            public override void EscapeAction()
            {
                // Ignore.
            }

            public override void LoadFile()
            {
                // Ignore.
            }

            public override void SaveTemplate()
            {
                // Ignore.
            }

            public override void SetScannerType(ScannerType type)
            {
                // Ignore.
            }

            public override void Reinitialise()
            {
                TransitionTo(typeof(Initialising));
            }

            #endregion
        }
    }
}
