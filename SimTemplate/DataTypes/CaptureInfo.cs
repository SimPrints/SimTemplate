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
using System.Windows.Media.Imaging;

namespace SimTemplate.DataTypes
{
    public class CaptureInfo
    {
        private readonly long m_DbId;
        private readonly byte[] m_ImageData;
        private readonly byte[] m_TemplateData;

        public long DbId { get { return m_DbId; } }
        public byte[] ImageData { get { return m_ImageData; } }
        public byte[] TemplateData { get { return m_TemplateData; } }

        public CaptureInfo(long dbId, byte[] imageData, byte[] templateData)
        {
            m_DbId = dbId;
            m_ImageData = imageData;
            m_TemplateData = templateData;
        }
    }
}
