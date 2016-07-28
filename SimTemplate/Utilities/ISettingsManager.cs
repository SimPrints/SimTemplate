using SimTemplate.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Utilities
{
    public interface ISettingsManager
    {
        object GetSetting(Setting setting);

        bool ValidateCurrentSettings();

        bool ValidateQuerySetting(Setting setting, object queryValue);

        string SettingHelpText(Setting setting);
    }
}
