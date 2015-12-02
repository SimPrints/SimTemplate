using log4net;
using log4net.Config;
using System;
using System.Windows;
using System.Windows.Input;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model.Database;
using TemplateBuilder.View.Progress;
using TemplateBuilder.ViewModel.MainWindow;
using TemplateBuilder.ViewModel.Progress;

namespace TemplateBuilder.View.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        private static readonly ILog m_Log = LogManager.GetLogger(typeof(MainWindowView));

        private readonly TemplateBuilderViewModel m_ViewModel;
        private ProgressView m_ProgressDialog;

        #region Constructor

        public MainWindowView()
        {
            XmlConfigurator.Configure();
            m_Log.Debug("Logging initialised.");

            m_ViewModel = new TemplateBuilderViewModel(new DataController());
            InitializeComponent();
            DataContext = m_ViewModel;

            m_ViewModel.ProgressChanged += ViewModel_ProgressChanged;

            Dispatcher.BeginInvoke((Action)(() => m_ViewModel.Start()));
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

        private void Image_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        #region Event Handler

        private void ViewModel_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            m_Log.DebugFormat(
                "ViewModel_ProgressChanged(Progress={0}, IsShowDialog={1}) called.",
                e.Progress,
                e.Action);

            switch (e.Action)
            {
                case TemplateBuilder.ViewModel.DialogAction.DoNothing:
                    IntegrityCheck.IsNotNull(m_ProgressDialog);
                    m_ProgressDialog.ViewModel.Progress = e.Progress;
                    break;

                case TemplateBuilder.ViewModel.DialogAction.Show:
                    IntegrityCheck.IsNull(m_ProgressDialog);
                    // Create new modal dialog to display
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        ProgressViewModel viewModel = new ProgressViewModel();
                        m_ProgressDialog = new ProgressView(viewModel);
                        m_ProgressDialog.Owner = this;
                        m_ProgressDialog.ShowDialog();
                    }));
                    break;

                case TemplateBuilder.ViewModel.DialogAction.Hide:
                    IntegrityCheck.IsNotNull(m_ProgressDialog);
                    m_ProgressDialog.Close();
                    m_ProgressDialog = null;
                    break;

                default:
                    throw IntegrityCheck.FailUnexpectedDefault(e.Action);
            }
        }

        #endregion
    }
}
