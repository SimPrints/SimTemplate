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
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Model.Database
{
    [Table(Name = "Capture")]
    public class CaptureDb
    {
        private EntityRef<PersonDb> m_Person;

        [Association(Storage = "m_Person", ThisKey = "PersonId")]
        public PersonDb Person
        {
            get { return this.m_Person.Entity; }
            set { this.m_Person.Entity = value; }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public Int64 Id { get; set; }

        /// <summary>
        /// Gets the name of the scanner.
        /// </summary>
        [Column()]
        public string ScannerName { get; set; }

        /// <summary>
        /// Gets the finger number.
        /// </summary>
        [Column()]
        public string FingerNumber { get; set; }

        /// <summary>
        /// Gets the capture number.
        /// </summary>
        [Column()]
        public int CaptureNumber { get; set; }

        [Column(Name = "SimAfisTemplate")]
        public byte[] GoldTemplate { get; set; }

        public string HumanId
        {
            get
            {
                return String.Format("{0}-{1}-{2}-{3}",
                    Person.Pid,
                    ScannerName,
                    FingerNumber,
                    CaptureNumber);
            }
        }


        /// <summary>
        /// Gets the person identifier.
        /// </summary>
        [Column(Name = "Person_Id")]
        public Int64 PersonId { get; set; }
    }
}
