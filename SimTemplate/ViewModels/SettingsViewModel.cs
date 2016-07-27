using System;
using System.Windows.Input;
using SimTemplate.ViewModels.Commands;
using System.ComponentModel;
using SimTemplate.Utilities;
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.ViewModels
{
    public partial class SettingsViewModel : ViewModel, ISettingsViewModel, IDialogViewModel, IDataErrorInfo
    {
        private const string TITLE = "Settings";

        // Commands
        private ICommand m_UpdateSettingsCommand;

        // View-Driven Properties
        private string m_ApiKey;

        // ViewModel-Driven Properties
        private ViewModelStatus m_Result;

        #region Constructor

        public SettingsViewModel()
        {
            InitialiseCommands();
        }

        #endregion

        #region Directly Bound Properties

        public string Title { get { return TITLE; } }

        public string ApiKey
        {
            get { return m_ApiKey; }
            set
            {
                if (m_ApiKey != value)
                {
                    m_ApiKey = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ViewModelStatus Result
        {
            get { return m_Result; }
            set
            {
                if (m_Result != value)
                {
                    m_Result = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region Command Methods

        private void UpdateSettings()
        {
            Log.Debug("UpdateSettings() called");
            // Update settings
            Properties.Settings.Default.ApiKey = m_ApiKey;
            Properties.Settings.Default.Save();
            Result = ViewModelStatus.Complete;
        }

        private bool IsUpdateSettingsPermitted
        {
            get
            {
                return (m_ApiKey != Properties.Settings.Default.ApiKey);
            }
        }

        #endregion

        #region Commands

        public ICommand UpdateSettingsCommand { get { return m_UpdateSettingsCommand; } }

        #endregion

        #region IDataErrorInfo Members

        public string Error
        {
            get { return String.Empty; }
        }

        public string this[string property]
        {
            get
            {
                String errorMessage = String.Empty;
                switch (property)
                {
                    case "ApiKey":
                        Guid result;
                        bool isValid = Guid.TryParse(ApiKey, out result);
                        if (!isValid)
                        {
                            errorMessage = "API Key be a valid 32 character GUID";
                        }

                        break;

                    default:
                        throw IntegrityCheck.FailUnexpectedDefault(property);
                }
                return errorMessage;
            }
        }

        #endregion

        #region ISettingsViewModel

        void ISettingsViewModel.Refresh()
        {
            m_ApiKey = Properties.Settings.Default.ApiKey;
            m_Result = ViewModelStatus.Running;
        }

        #endregion

        #region Private Methods

        private void InitialiseCommands()
        {
            m_UpdateSettingsCommand = new RelayCommand(
                x => UpdateSettings(),
                x => IsUpdateSettingsPermitted);
        }

        #endregion
    }
}
