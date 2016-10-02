using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.ViewModels.Support
{
    public interface ISettingsViewModelFactory
    {
        ISettingsViewModel CreateViewModel();
    }
}
