using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using TemplateBuilderMVVM.Helpers;
using TemplateBuilderMVVM.ViewModel;

namespace TemplateBuilderMVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TemplateBuilderViewModel m_ViewModel;

        #region Constructor

        public MainWindow()
        {
            m_ViewModel = new TemplateBuilderViewModel();
            InitializeComponent();
            DataContext = m_ViewModel;
        }

        #endregion

        public TemplateBuilderViewModel ViewModel { get {return m_ViewModel;} }

        #region Event Handlers

        private void itemsControl_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(image);

            m_ViewModel.itemsControl_MouseMove(pos);
        }

        private void itemsControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(image);

            m_ViewModel.itemsControl_MouseUp(pos);
        }

        private void Ellipse_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            object item = (sender as FrameworkElement).DataContext;
            int index = itemsControl.Items.IndexOf(item);

            m_ViewModel.Ellipse_MouseRightButtonUp(index);
        }

        private void image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            m_ViewModel.image_SizeChanged(e.NewSize);
        }

        #endregion
    }
}
