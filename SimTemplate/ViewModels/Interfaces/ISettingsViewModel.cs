using SimTemplate.DataTypes.Enums;

namespace SimTemplate.ViewModels.Interfaces
{
    public interface ISettingsViewModel
    {
        string ApiKey { get; }

        ViewModelStatus Result { get; }

        void Refresh();
    }
}
