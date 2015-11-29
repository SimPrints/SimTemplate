using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateBuilder.Model.Database
{
    [Table(Name = "Capture")]
    public class Capture
    {
        private EntityRef<Person> m_Person;

        [Association(Storage = "m_Person", ThisKey = "PersonId")]
        public Person Person
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

        public string ImageFileName
        {
            get
            {
                return String.Format("{0}-{1}{2}-{3}",
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
