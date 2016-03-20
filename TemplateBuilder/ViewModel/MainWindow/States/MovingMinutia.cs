﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model;

namespace TemplateBuilder.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        private object m_SelectedMinutiaLock = new object();

        public class MovingMinutia : Templating
        {
            public MovingMinutia(TemplateBuilderViewModel outer) : base(outer)
            { }

            public override void EscapeAction()
            {
                // Ignore.
            }

            public override void PositionInput(Point point, MouseButton changedButton)
            {
                if (Outer.m_SelectedMinutia != null &&
                    changedButton == MouseButton.Left)
                {
                    StopMove();
                }
            }

            public override void PositionMove(Point point)
            {
                IntegrityCheck.IsNotNull(point);

                lock (Outer.m_SelectedMinutiaLock)
                {
                    IntegrityCheck.IsNotNull(Outer.m_SelectedMinutia.HasValue);
                    // Set position TO SCALE
                    Outer.Minutae[Outer.m_SelectedMinutia.Value].Position = point;
                }
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
