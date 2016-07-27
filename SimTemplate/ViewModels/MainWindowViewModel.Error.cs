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
            public Error(MainWindowViewModel outer) : base(outer)
            { }

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Indicate we have errored
                Outer.m_TemplatingViewModel.StatusImage = new Uri("pack://application:,,,/Resources/StatusImages/Error.png");
                Outer.m_TemplatingViewModel.PromptText = "Fault";

                // Clear UI
                Outer.m_TemplatingViewModel.QuitTemplating();
            }

            public override void DataController_InitialisationComplete(InitialisationCompleteEventArgs e)
            {
                throw IntegrityCheck.Fail("Not expected to have InitialisationComplete event when in error.");
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
        }
    }
}
