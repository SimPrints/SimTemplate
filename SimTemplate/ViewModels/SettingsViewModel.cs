using System;
using System.Windows.Input;
using SimTemplate.ViewModels.Commands;
using System.Text.RegularExpressions;
using SimTemplate.ViewModels.CustomEventArgs;

namespace SimTemplate.ViewModels
{
    public partial class SettingsViewModel : ViewModel, ISettingsViewModel
    {
        // Commands
        private ICommand m_UpdateSettingsCommand;

        // View-Driven Properties
        private string m_ApiKey;

        // ViewModel-Driven Properties
        private bool m_IsApiKeyValid;

        // Events
        private event EventHandler<SettingsUpdatedEventArgs> m_SettingsUpdated;

        #region Constructor

        public SettingsViewModel()
        {
            ((ISettingsViewModel)this).Refresh();
            InitialiseCommands();
        }

        #endregion

        #region Directly Bound Properties

        public string ApiKey
        {
            get { return m_ApiKey; }
            set
            {
                if (m_ApiKey != value)
                {
                    m_ApiKey = value;
                    NotifyPropertyChanged();
                    // Validate value
                    IsApiKeyValid = ValidateApiKey(m_ApiKey);
                }
            }
        }

        public bool IsApiKeyValid
        {
            get { return m_IsApiKeyValid; }
            set
            {
                if (m_IsApiKeyValid != value)
                {
                    m_IsApiKeyValid = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsUpdateSettingsPermitted
        {
            get
            {
                return (m_ApiKey != Properties.Settings.Default.ApiKey) && m_IsApiKeyValid;
            }
        }

        #endregion

        #region CommandCallbacks

        private void UpdateSettings()
        {
            Log.Debug("UpdateSettings() called");

            if (IsApiKeyValid)
            {
                OnSettingsUpdated(new SettingsUpdatedEventArgs(m_ApiKey));
            }
            else
            {
                Log.DebugFormat("ApiKey invalid: {0}", m_ApiKey);
            }
        }

        #endregion

        #region Commands

        public ICommand UpdateSettingsCommand { get { return m_UpdateSettingsCommand; } }

        #endregion

        #region ISettingsViewModel

        void ISettingsViewModel.Refresh()
        {
            ApiKey = Properties.Settings.Default.ApiKey;
        }

        event EventHandler<SettingsUpdatedEventArgs> ISettingsViewModel.SettingsUpdated
        {
            add { m_SettingsUpdated += value; }
            remove { m_SettingsUpdated -= value; }
        }

        #endregion

        #region Private Methods

        private void InitialiseCommands()
        {
            m_UpdateSettingsCommand = new RelayCommand(
                x => UpdateSettings(),
                x => IsUpdateSettingsPermitted);
        }

        private bool ValidateApiKey(string apiKey)
        {
            Regex rg = new Regex(@"^[a-z\-0-9]*$");
            return rg.IsMatch(apiKey);
        }

        private void OnSettingsUpdated(SettingsUpdatedEventArgs e)
        {
            EventHandler<SettingsUpdatedEventArgs> temp = m_SettingsUpdated;
            if (temp != null)
            {
                temp.Invoke(this, e);
            }
        }

        #endregion
    }
}
