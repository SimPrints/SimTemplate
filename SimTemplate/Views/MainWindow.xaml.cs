using log4net;
using log4net.Config;
using System.Windows;
using SimTemplate.Model.DataControllers.TempApi;
using SimTemplate.ViewModels;
using SimTemplate.Model.DataControllers.Local;
using SimTemplate.Utilities;

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
            m_ViewModel = new MainWindowViewModel(
#if DEBUG
                new LocalDataController(),
#else
                new TempApiDataController(),
#endif
                new TemplatingViewModel(),
                new SettingsViewModel(),
                new WindowService());


            InitializeComponent();
            DataContext = m_ViewModel;

            m_ViewModel.BeginInitialise();
        }

#endregion
    }
}
