using log4net;
using log4net.Config;
using System.Windows;
using SimTemplate.ViewModels;
using SimTemplate.Utilities;
#if DEBUG
using SimTemplate.Model.DataControllers.Local;
#else
using SimTemplate.Model.DataControllers.TempApi;
#endif

namespace SimTemplate.Views.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        private static readonly ILog m_Log = LogManager.GetLogger(typeof(MainWindowView));

        private readonly MainWindowViewModel m_ViewModel;

        #region Constructor

        public MainWindowView()
        {
            XmlConfigurator.Configure();
            m_Log.Debug("Logging initialised.");

            ISettingsManager settingsMgr = new SettingsManager();
            m_ViewModel = new MainWindowViewModel(
#if DEBUG
                new LocalDataController(),
#else
                new TempApiDataController(),
#endif
                new TemplatingViewModel(),
                new SettingsViewModel(settingsMgr),
                settingsMgr,
                new WindowService());

            InitializeComponent();
            DataContext = m_ViewModel;

            Loaded += MainWindowView_Loaded;
        }

        #endregion

        private void MainWindowView_Loaded(object sender, RoutedEventArgs e)
        {
            m_ViewModel.BeginInitialise();
        }

    }
}
