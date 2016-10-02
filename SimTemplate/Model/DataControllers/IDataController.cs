// Copyright 2016 Sam Briggs
//
// This file is part of SimTemplate.
//
// SimTemplate is free software: you can redistribute it and/or modify it under the
// terms of the GNU General Public License as published by the Free Software 
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// SimTemplate is distributed in the hope that it will be useful, but WITHOUT ANY 
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
// A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// SimTemplate. If not, see http://www.gnu.org/licenses/.
//
using SimTemplate.DataTypes.Enums;
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
        Guid BeginInitialise(DataControllerConfig config);

        /// <summary>
        /// Gets an untemplated capture from the database.
        /// </summary>
        /// <param name="scannerType">Type of the scanner.</param>
        /// <param name="isTemplated">if set to <c>true</c> returns a capture that is templated.</param>
        /// <returns>unique identifier for the request.</returns>
        Guid BeginGetCapture(ScannerType scannerType);

        /// <summary>
        /// Saves the template to the database.
        /// </summary>
        /// <param name="dbId">The capture's primary key in the database.</param>
        /// <param name="template">The template in ISO standard form.</param>
        /// <returns>unique identifier for the request</returns>
        Guid BeginSaveTemplate(long dbId, byte[] template);

        /// <summary>
        /// Cancels the capture requeset with the corresponding Guid.
        /// </summary>
        /// <param name="guid">The unique identifier of the request.</param>
        void AbortRequest(Guid guid);

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
