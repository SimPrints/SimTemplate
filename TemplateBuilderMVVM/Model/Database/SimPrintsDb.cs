using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateBuilder.Model.Database
{
    [Database]
    public class SimPrintsDb : DataContext
    {
        public Table<Capture> Captures;
        public Table<Person> People;
        public SimPrintsDb(string connection) : base(connection) { }
        public SimPrintsDb(SQLiteConnection connection) : base(connection) { }
    }
}
