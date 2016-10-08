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

namespace SimTemplate.Model.DataControllers.EventArguments
{
    public class InitialisationCompleteEventArgs : EventArgs
    {
        private readonly InitialisationResult m_Result;
        private readonly Guid m_RequestId;
        private readonly DataRequestResult m_RequestResult;

        public InitialisationResult Result { get { return m_Result; } }
        public Guid RequestId { get { return m_RequestId; } }

        public InitialisationCompleteEventArgs(InitialisationResult result, Guid requestId, DataRequestResult requestResult)
        {
            m_Result = result;
            m_RequestId = requestId;
            m_RequestResult = requestResult;
        }
    }
}
