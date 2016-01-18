using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TemplateBuilderMVVM.States;

namespace TemplateBuilderMVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private State m_State;

        public MainWindow()
        {
            InitializeComponent();
            m_State = new Idle(this);
        }

        /// <summary>
        /// Gets the canvas box.
        /// </summary>
        /// <value>
        /// The canvas.
        /// </value>
        public Canvas Canvas { get { return canvas; } }
        /// <summary>
        /// Gets or sets the filename of the image currently being templated.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        public string Filename { get; set; }

        public void TransitionState(State state)
        {
            Console.WriteLine("State transition: {0}->{1}", m_State.Name, state.Name);
            m_State.OnLeavingState();
            state.OnEnteringState();
            m_State = state;
        }

        #region Event Handlers

        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("openFile_Click() called");
            m_State.openFile_Click(sender, e);
        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("canvas_MouseLeftButtonUp()");
            m_State.canvas_MouseLeftButtonUp(sender, e);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("canvas_MouseMove()");
            m_State.canvas_MouseMove(sender, e);
        }

        private void saveTemplate_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("saveTemplate_Click()");
            m_State.saveTemplate_Click(sender, e);
        }

        #endregion
    }
}
