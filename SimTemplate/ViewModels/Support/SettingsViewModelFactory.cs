using SimTemplate.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.ViewModels.Support
{
    public class SettingsViewModelFactory : ISettingsViewModelFactory
    {
        private ISettingsManager m_SettingsManager;

        public SettingsViewModelFactory(ISettingsManager settingsManager)
        {
            m_SettingsManager = settingsManager;
        }

        public ISettingsViewModel CreateViewModel()
        {
            return new SettingsViewModel(m_SettingsManager);
        }
    }
}
