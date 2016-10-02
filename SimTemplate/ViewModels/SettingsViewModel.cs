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
using System;
using System.Windows.Input;
using SimTemplate.ViewModels.Commands;
using System.ComponentModel;
using SimTemplate.Utilities;
using SimTemplate.DataTypes.Enums;
using System.Collections;
using System.Collections.Generic;

namespace SimTemplate.ViewModels
{
    public partial class SettingsViewModel :
        ViewModel,
        ISettingsViewModel,
        IDialogViewModel,
        IDataErrorInfo
    {
        private const string TITLE = "Settings";

        // Commands
        private ICommand m_UpdateSettingsCommand;

        // Dependencies
        private readonly ISettingsManager m_SettingsManager;

        // View-Driven Properties
        private SettingCompare m_ApiKey;
        private SettingCompare m_RootUrl;

        // ViewModel-Driven Properties
        private ViewModelStatus m_Result;

        #region Constructor

        public SettingsViewModel(ISettingsManager settingsManager)
        {
            m_SettingsManager = settingsManager;

            InitialiseCommands();
        }

        #endregion

        #region Directly Bound Properties

        public string Title { get { return TITLE; } }

        public string ApiKey
        {
            get { return (string)m_ApiKey.QueryValue; }
            set
            {
                if (!m_ApiKey.QueryValue.Equals(value))
                {
                    m_ApiKey.QueryValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string RootUrl
        {
            get { return (string)m_RootUrl.QueryValue; }
            set
            {
                if (!m_RootUrl.QueryValue.Equals(value))
                {
                    m_RootUrl.QueryValue = value;
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
            m_SettingsManager.UpdateSetting(Setting.ApiKey, m_ApiKey.QueryValue);
            m_SettingsManager.UpdateSetting(Setting.RootUrl, m_RootUrl.QueryValue);
            m_SettingsManager.SaveSettings();
            Result = ViewModelStatus.Complete;
        }

        private bool UpdateCanExecute()
        {
           return m_SettingsManager.ValidateQuerySetting(Setting.ApiKey, m_ApiKey.QueryValue) &&
                m_SettingsManager.ValidateQuerySetting(Setting.RootUrl, m_RootUrl.QueryValue) &&
                (m_ApiKey.HasChanged || m_RootUrl.HasChanged);
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
                        if (!m_SettingsManager.ValidateQuerySetting(
                            Setting.ApiKey,
                            m_ApiKey.QueryValue))
                        {
                            errorMessage = m_SettingsManager.ValidationHelpText(Setting.ApiKey);
                        }
                        break;

                    case "RootUrl":
                        if (!m_SettingsManager.ValidateQuerySetting(
                            Setting.RootUrl,
                            m_RootUrl.QueryValue))
                        {
                            errorMessage = m_SettingsManager.ValidationHelpText(Setting.RootUrl);
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
            m_ApiKey = new SettingCompare(m_SettingsManager.GetCurrentSetting(Setting.ApiKey));
            m_RootUrl = new SettingCompare(m_SettingsManager.GetCurrentSetting(Setting.RootUrl));
            m_Result = ViewModelStatus.Running;
        }

        #endregion

        #region Private Methods

        private void InitialiseCommands()
        {
            m_UpdateSettingsCommand = new RelayCommand(x => UpdateSettings(), x => UpdateCanExecute());
        }

        #endregion

        public class SettingCompare
        {
            private readonly object m_CurrentValue;
            private object m_QueryValue;

            public SettingCompare(object currentValue)
            {
                m_CurrentValue = currentValue;
                m_QueryValue = currentValue;
            }

            public object CurrentValue { get { return m_CurrentValue; } }

            public object QueryValue
            {
                get { return m_QueryValue; }
                set { m_QueryValue = value; }
            }

            public bool HasChanged
            {
                get
                {
                    return !m_CurrentValue.Equals(m_QueryValue);
                }
            }
        }
    }
}
