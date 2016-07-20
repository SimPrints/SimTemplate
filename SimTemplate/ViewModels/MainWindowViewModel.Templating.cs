using SimTemplate.Helpers;
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel
    {
        public class Templating : Initialised
        {
            #region Constructor

            public Templating(MainWindowViewModel outer) : base(outer)
            { }

            #endregion

            public override void LoadFile()
            {
                Outer.m_TemplatingViewModel.QuitTemplating();

                TransitionTo(typeof(Loading));
            }

            public override void SaveTemplate()
            {
                if (Outer.m_TemplatingViewModel.IsSaveTemplatePermitted)
                {
                    TransitionTo(typeof(Saving));
                }
                else
                {
                    Log.Warn("SaveTemplate() called when IsSaveTemplatePermitted. Improper usage!");
                }
            }

            public override void SetScannerType(ScannerType type)
            {
                // TODO: Prompt if user wants to save their work?
                // if (Outer.m_Minutia)
                // {
                // }
                // TransitionTo(typeof(Loading));
            }
        }
    }
}
