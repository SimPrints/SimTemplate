using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TemplateBuilderMVVM.States
{
    abstract public class State
    {
        protected MainWindow m_Outer;

        public State(MainWindow outer)
        {
            m_Outer = outer;
        }

        public void openFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            //od.Filter = "XLS files|*.xls";
            //od.Multiselect = true;
            if (od.ShowDialog() == true)
            {
                // TODO: only pass valid file types.

                // Display selected image
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri(od.FileName, UriKind.Absolute));
                m_Outer.Canvas.Background = ib;

                // Record image filename
                m_Outer.Filename = od.FileName;

                // Transition state to 'Templating'
                m_Outer.TransitionState(new Templating(m_Outer));
            }
        }

        virtual public void OnEnteringState() { }

        virtual public void OnLeavingState() { }

        abstract public void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e);

        abstract public void canvas_MouseMove(object sender, MouseEventArgs e);

        abstract public void saveTemplate_Click(object sender, RoutedEventArgs e);

        public string Name { get { return GetType().Name; } }
    }
}
