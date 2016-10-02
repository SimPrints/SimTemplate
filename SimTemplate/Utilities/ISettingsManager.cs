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
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.Utilities
{
    public interface ISettingsManager
    {
        /// <summary>
        /// Gets the current value of the setting.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <returns>The value of the setting</returns>
        object GetCurrentSetting(Setting setting);

        /// <summary>
        /// Validates all of the current setting values.
        /// </summary>
        /// <returns>[true] if and only if all the settings are valid, else [false]</returns>
        bool ValidateCurrentSettings();

        /// <summary>
        /// Validates the query value for the setting.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <param name="queryValue">The query value.</param>
        /// <returns>[true] if and only if the query value is valid, else [false]</returns>
        bool ValidateQuerySetting(Setting setting, object queryValue);

        /// <summary>
        /// Gets the help text for providing a valid value for the setting.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <returns>The help text</returns>
        string ValidationHelpText(Setting setting);

        /// <summary>
        /// Updates the setting.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <param name="newValue">The query value.</param>
        /// <returns>[true] if and only if the setting was successful updated, else [false]</returns>
        bool UpdateSetting(Setting setting, object newValue);

        /// <summary>
        /// Saves the current settings.
        /// </summary>
        void SaveSettings();
    }
}
