using log4net;
using log4net.Config;
using System;
using System.Windows;
using System.Windows.Input;
using TemplateBuilder.Model.Database;
using TemplateBuilder.ViewModel.MainWindow;

namespace TemplateBuilder.View.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        private static readonly ILog m_Log = LogManager.GetLogger(typeof(MainWindowView));
        private readonly TemplateBuilderViewModel m_ViewModel;

        #region Constructor

        public MainWindowView()
        {
            XmlConfigurator.Configure();
            m_Log.Debug("Logging initialised.");

            m_ViewModel = new TemplateBuilderViewModel(new DataController());
            InitializeComponent();
            DataContext = m_ViewModel;

            Dispatcher.BeginInvoke((Action)(() => m_ViewModel.Start()));
        }

        #endregion

        public TemplateBuilderViewModel ViewModel { get {return m_ViewModel;} }

        #region Event Handlers

        private void itemsControl_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(image);

            m_ViewModel.MouseMove(pos);
        }

        private void itemsControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            m_Log.Debug("itemsControl_MouseUp(...) called.");

            Point pos = e.GetPosition(image);

            m_ViewModel.itemsControl_MouseUp(pos, e.ChangedButton);
        }

        private void image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            m_Log.Debug("image_SizeChanged(...) called.");

            m_ViewModel.image_SizeChanged(e.NewSize);
        }

        #endregion

        private void Minutia_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m_Log.Debug("Minutia_MouseDown(...) called.");

            if (e.ChangedButton == MouseButton.Left)
            {
                object item = (sender as FrameworkElement).DataContext;
                int index = itemsControl.Items.IndexOf(item);
                m_ViewModel.StartMove(index);
            }
        }

        private void Minutia_MouseUp(object sender, MouseButtonEventArgs e)
        {
            m_Log.Debug("Minutia_MouseUp(...) called.");

            if (e.ChangedButton == MouseButton.Right)
            {
                object item = (sender as FrameworkElement).DataContext;
                int index = itemsControl.Items.IndexOf(item);

                m_ViewModel.Minutia_MouseUp(index);

                // Mark event as handled so that we don't create a new minutia as soon as we have
                // deleted one.
                e.Handled = true;
            }
        }

        private void Minutia_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(image);
            m_ViewModel.MoveMinutia(pos);
        }
    }
}
