// Copyright 2016 Sam Briggs
//
// This file is part of SimTemplate.
//
// SimTemplate is free software: you can redistribute it and/or modify it under the
// terms of the GNU General Public License as published by the Free Software 
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// SimTemplate is distributed in the hope that it will be useful, but WITHOUT ANY 
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
// A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// SimTemplate. If not, see http://www.gnu.org/licenses/.
//
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
