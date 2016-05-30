using SimTemplate.Model.DataControllers.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Model.DataControllers
{
    public interface IDataController
    {
        /// <summary>
        /// Connects the controller to the database using the provided configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="progress">The progress.</param>
        void BeginInitialise(DataControllerConfig config);

        /// <summary>
        /// Gets an untemplated capture from the database.
        /// </summary>
        /// <param name="scannerType">Type of the scanner.</param>
        /// <param name="isTemplated">if set to <c>true</c> returns a capture that is templated.</param>
        /// <returns>unique identifier for the request.</returns>
        Guid BeginGetCapture(ScannerType scannerType, bool isTemplated);

        /// <summary>
        /// Cancels the capture requeset with the corresponding Guid.
        /// </summary>
        /// <param name="guid">The unique identifier of the request.</param>
        void AbortRequest(Guid guid);

        /// <summary>
        /// Saves the template to the database.
        /// </summary>
        /// <param name="dbId">The capture's primary key in the database.</param>
        /// <param name="template">The template in ISO standard form.</param>
        /// <returns>unique identifier for the request</returns>
        Guid BeginSaveTemplate(long dbId, byte[] template);

        /// <summary>
        /// Occurs when initialisation is complete.
        /// </summary>
        event EventHandler<InitialisationCompleteEventArgs> InitialisationComplete;

        /// <summary>
        /// Occurs when a request to get a capture is complete.
        /// </summary>
        event EventHandler<GetCaptureCompleteEventArgs> GetCaptureComplete;

        /// <summary>
        /// Occurs when a request to save a templte to a capture is complete.
        /// </summary>
        event EventHandler<SaveTemplateEventArgs> SaveTemplateComplete;
    }
}
