﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SimTemplate.Helpers;
using SimTemplate.ViewModel;
using SimTemplate.ViewModel.Database;
using SimTemplate.ViewModel.DataControllers.EventArguments;

namespace SimTemplate.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        public class Error : TemplateBuilderBaseState
        {
            public Error(TemplateBuilderViewModel outer) : base(outer)
            { }

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Indicate we have errored
                Outer.StatusImage = new Uri("pack://application:,,,/Resources/StatusImages/Error.png");
                Outer.PromptText = "Fault";

                // Clear UI.
                Outer.Capture = null;
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Outer.Minutae.Clear();
                }));
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

            public override void PositionInput(Point position)
            {
                // Ignore.
            }

            public override void PositionUpdate(Point point)
            {
                // Ignore.
            }

            public override void RemoveMinutia(int index)
            {
                // Ignore.
            }

            public override void SaveTemplate()
            {
                // Ignore.
            }

            public override void SetMinutiaType(MinutiaType type)
            {
                // Ignore.
            }

            public override void MoveMinutia(Point point)
            {
                // Ignore.
            }

            public override void StartMove(int index)
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