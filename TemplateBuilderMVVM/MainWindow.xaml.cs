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
        private Canvas m_Canvas;

        public MainWindow()
        {
            m_ViewModel = new TemplateBuilderViewModel();
            InitializeComponent();
            DataContext = m_ViewModel;

            itemsControl.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            m_Canvas = UIHelper.FindChild<Canvas>(Application.Current.MainWindow, "itemsControlCanvas");
        }

        public TemplateBuilderViewModel ViewModel { get {return m_ViewModel;} }

        private void itemsControl_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(image);

            m_ViewModel.PositionMove(pos);
        }

        private void itemsControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(image);

            m_ViewModel.PositionInput(pos);
        }

        private void Ellipse_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            object item = (sender as FrameworkElement).DataContext;
            int index = itemsControl.Items.IndexOf(item);

            m_ViewModel.RemoveItem(index);
        }

        private void image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Get scaling in each dimension.
            double scaleX = e.NewSize.Width / m_ViewModel.Image.Width;
            double scaleY = e.NewSize.Height / m_ViewModel.Image.Height;
            // Check that scaling factor is equal for each dimension.
            IntegrityCheck.AreEqual(scaleX, scaleY);
            double scale = (scaleX + scaleY) / 2;
            // Update ViewModel scale
            m_ViewModel.Scale = scale;
        }
    }
}
