using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TemplateBuilderMVVM.States
{
    public class Idle : State
    {
        public Idle(MainWindow outer) : base(outer) { }



        public override void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Do nothing when no image has been loaded.
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            // Do nothing when no image has been loaded.
        }

        public override void OnEnteringState()
        {
            // TODO: grey out 'save template button'?
        }

        public override void saveTemplate_Click(object sender, RoutedEventArgs e)
        {
            // Do nothing when no image has been loaded
        }
    }
}
