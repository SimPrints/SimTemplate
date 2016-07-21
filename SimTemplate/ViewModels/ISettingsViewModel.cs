using SimTemplate.ViewModels.CustomEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.ViewModels
{
    public interface ISettingsViewModel
    {
        void Refresh();

        event EventHandler<SettingsUpdatedEventArgs> SettingsUpdated;
    }
}
