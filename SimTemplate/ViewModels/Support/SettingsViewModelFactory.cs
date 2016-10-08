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
using SimTemplate.Utilities;
using SimTemplate.ViewModels.Interfaces;
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
