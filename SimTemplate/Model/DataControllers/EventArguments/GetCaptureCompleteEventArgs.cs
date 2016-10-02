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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimTemplate.Utilities;
using SimTemplate.DataTypes;

namespace SimTemplate.Model.DataControllers.EventArguments
{
    public class GetCaptureCompleteEventArgs : EventArgs
    {
        private CaptureInfo m_Capture;
        private Guid m_RequestId;
        private DataRequestResult m_Result;

        public CaptureInfo Capture { get { return m_Capture; } }

        public Guid RequestId { get { return m_RequestId; } }

        public DataRequestResult Result { get { return m_Result; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetCaptureCompleteEventArgs"/> class in 
        /// the case where the request was successful.
        /// </summary>
        /// <param name="capture">The capture.</param>
        /// <param name="requestId">The request unique identifier.</param>
        public GetCaptureCompleteEventArgs(CaptureInfo capture, Guid requestId, DataRequestResult result)
        {
            IntegrityCheck.IsNotNull(requestId);
            IntegrityCheck.IsNotNull(result);
            if (result == DataRequestResult.Success)
            {
                IntegrityCheck.IsNotNull(capture);
            }

            m_Capture = capture;
            m_RequestId = requestId;
            m_Result = result;
        }
    }
}
