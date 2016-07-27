using SimTemplate.DataTypes.Enums;

namespace SimTemplate.ViewModels
{
    public interface ISettingsViewModel
    {
        string ApiKey { get; }

        ViewModelStatus Result { get; }

        void Refresh();
    }
}
