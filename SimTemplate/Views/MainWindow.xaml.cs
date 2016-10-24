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
using SimTemplate.ViewModels.Interfaces;
using SimTemplate.DataTypes.Enums;
using System.Windows.Media;
using System;
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

        private readonly IMainWindowViewModel m_ViewModel;

        #region Constructor

        public MainWindowView()
        {
            XmlConfigurator.Configure();
            m_Log.Debug("Logging initialised.");

            ISettingsManager settingsMgr = new SettingsManager();
            IDispatcherHelper dispatcherHelper = new DispatcherHelper();
            m_ViewModel = new MainWindowViewModel(
#if DEBUG
                new LocalDataController(),
#else
                new TempApiDataController(),
#endif
                new TemplatingViewModel(dispatcherHelper),
                new SettingsViewModel(settingsMgr),
                settingsMgr,
                new WindowService(),
                dispatcherHelper);

            InitializeComponent();
            DataContext = m_ViewModel;

            Loaded += MainWindowView_Loaded;
            m_ViewModel.ActivityChanged += ViewModel_ActivityChanged;
        }

        #endregion

        #region Event Handlers

        private void MainWindowView_Loaded(object sender, RoutedEventArgs e)
        {
            m_ViewModel.BeginInitialise();
        }

        private void ViewModel_ActivityChanged(object sender, ActivityChangedEventArgs e)
        {
            // Set the Status Image
            ImageSource statusImageSource;
            switch (e.NewActivity)
            {
                case Activity.Fault:
                    statusImageSource = (ImageSource)mainWindow.Resources["errorStatus"];
                    break;

                case Activity.Loading:
                case Activity.Transitioning:
                    statusImageSource = (ImageSource)mainWindow.Resources["loadingStatus"];
                    break;

                case Activity.Templating:
                case Activity.Idle:
                // TODO: uninitialised image?
                case Activity.Uninitialised:
                    statusImageSource = null;
                    break;

                default:
                    throw IntegrityCheck.FailUnexpectedDefault(e.NewActivity);
            }

            // Load File icon
            ImageSource loadFileIcon;
            if (e.NewActivity == Activity.Loading)
            {
                loadFileIcon = (ImageSource)mainWindow.Resources["cancelIcon"];
            }
            else
            {
                loadFileIcon = (ImageSource)mainWindow.Resources["loadIcon"];
            }

            // Update UI element on application thread
            m_ViewModel.DispatcherHelper.Invoke(new Action(() =>
            {
                statusImage.Source = statusImageSource;
                ((System.Windows.Controls.Image)loadFile.Content).Source = loadFileIcon;
            }));
        }

        #endregion

    }
}
