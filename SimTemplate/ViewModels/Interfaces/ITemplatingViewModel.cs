using SimTemplate.DataTypes;
using SimTemplate.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.ViewModels.Interfaces
{
    public interface ITemplatingViewModel
    {
        void BeginInitialise();

        /// <summary>
        /// Gets or sets a value indicating whether saving the template is permitted
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is saving the template permitted; otherwise, <c>false</c>.
        /// </value>
        bool IsSaveTemplatePermitted { get; }

        /// <summary>
        /// Sets the type of the input minutia.
        /// </summary>
        /// <value>
        /// The type of the input minutia.
        /// </value>
        MinutiaType InputMinutiaType { set; }

        /// <summary>
        /// Acts upon a call to escape.
        /// </summary>
        void EscapeAction();

        /// <summary>
        /// Begins templating with the provided capture.
        /// </summary>
        /// <param name="capture">The capture.</param>
        void BeginTemplating(CaptureInfo capture);

        /// <summary>
        /// Gets the capture currently being templated.
        /// </summary>
        /// <value>
        /// The capture.
        /// </value>
        CaptureInfo Capture { get; }

        /// <summary>
        /// Finalises templating.
        /// </summary>
        /// <returns>the template in ISO format</returns>
        byte[] FinaliseTemplate();

        /// <summary>
        /// Quits templating without finalising.
        /// </summary>
        void QuitTemplating();

        event EventHandler<UserActionRequiredEventArgs> UserActionRequired;
    }
}
