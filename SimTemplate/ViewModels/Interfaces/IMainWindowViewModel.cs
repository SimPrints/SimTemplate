using SimTemplate.DataTypes.Enums;
using SimTemplate.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimTemplate.ViewModels.Interfaces
{
    public interface IMainWindowViewModel
    {
        void BeginInitialise();

        #region Commands

        ICommand LoadFileCommand { get; }

        ICommand SaveTemplateCommand { get; }

        ICommand TerminationButtonPressCommand { get; }

        ICommand BifuricationButtonPressCommand { get; }

        ICommand EscapePressCommand { get; }

        ICommand ToggleSettingsCommand { get; }

        ICommand ReinitialiseCommand { get; }

        #endregion

        #region Bound Properties
        
        string PromptText { get; }

        IEnumerable<ScannerType> ScannerTypes { get; }

        ScannerType FilteredScannerType { get; set; }

        ITemplatingViewModel TemplatingViewModel { get; }

        #endregion

        event EventHandler<ActivityChangedEventArgs> ActivityChanged;
    }
}
