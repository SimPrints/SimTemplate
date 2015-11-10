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
using TemplateBuilderMVVM.ViewModel;

namespace TemplateBuilderMVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TemplateBuilderViewModel m_ViewModel;

        public MainWindow()
        {
            m_ViewModel = new TemplateBuilderViewModel();
            InitializeComponent();
            DataContext = m_ViewModel;
        }

        public TemplateBuilderViewModel ViewModel { get {return m_ViewModel;} }

        private void itemsControl_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(itemsControl);

            m_ViewModel.PositionMove(pos);
        }

        private void itemsControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(itemsControl);

            m_ViewModel.PositionInput(pos);
        }

        private void Ellipse_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            object item = (sender as FrameworkElement).DataContext;
            int index = itemsControl.Items.IndexOf(item);

            m_ViewModel.RemoveItem(index);
        }
    }
}
