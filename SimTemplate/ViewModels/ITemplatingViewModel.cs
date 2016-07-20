using SimTemplate.DataTypes;
using SimTemplate.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.ViewModels
{
    public interface ITemplatingViewModel
    {
        void BeginInitialise();

        /// <summary>
        /// Gets or sets a value indicating whether the 'save tempalte' command is active.
        /// </summary>
        bool IsSaveTemplatePermitted { get; }

        MinutiaType InputMinutiaType { set; }

        void EscapeAction();

        string PromptText { set; }

        Uri StatusImage { set; }

        void BeginTemplating(CaptureInfo capture);

        CaptureInfo Capture { get; }

        byte[] FinaliseTemplate();

        void QuitTemplating();
    }
}
