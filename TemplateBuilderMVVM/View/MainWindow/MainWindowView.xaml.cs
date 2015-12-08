using log4net;
using log4net.Config;
using System;
using System.Windows;
using System.Windows.Input;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model.Database;
using TemplateBuilder.ViewModel.MainWindow;

namespace TemplateBuilder.View.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        private static readonly ILog m_Log = LogManager.GetLogger(typeof(MainWindowView));

        private readonly TemplateBuilderViewModel m_ViewModel;

        private int? m_SelectedMinutia;
        private object m_SelectedMinutiaLock = new object();

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

            lock (m_SelectedMinutiaLock)
            {
                if (m_SelectedMinutia.HasValue)
                {
                    m_ViewModel.MoveMinutia(m_SelectedMinutia.Value, pos);
                }
            }
        }

        private void itemsControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(image);

            m_ViewModel.itemsControl_MouseUp(pos);

            if (m_SelectedMinutia != null &&
                e.ChangedButton == MouseButton.Left)
            {
                lock (m_SelectedMinutiaLock)
                {
                    m_SelectedMinutia = null;
                }
                m_ViewModel.StopMove();
            }
        }

        private void image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            m_ViewModel.image_SizeChanged(e.NewSize);
        }

        #endregion

        private void Minutia_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                object item = (sender as FrameworkElement).DataContext;
                int index = itemsControl.Items.IndexOf(item);

                lock (m_SelectedMinutiaLock)
                {
                    m_SelectedMinutia = index;
                }
                m_ViewModel.StartMove();
            }
        }

        private void Minutia_MouseUp(object sender, MouseButtonEventArgs e)
        {
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

            lock (m_SelectedMinutiaLock)
            {
                if (m_SelectedMinutia.HasValue)
                {
                    m_ViewModel.MoveMinutia(m_SelectedMinutia.Value, pos);
                }
            }
        }
    }
}
