using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateBuilder.Model.Database
{
    public interface IDataController
    {
        /// <summary>
        /// Connects the controller to the SQLite database using the provided configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>true if successful, false otherwise</returns>
        bool Connect(DataControllerConfig config);

        /// <summary>
        /// Gets the next image file to process by iterating the results of the SQL query and
        /// searching for it on the local machine.
        /// </summary>
        /// <returns>filepath of the local file, null if no image can be found.</returns>
        string GetImageFile();
    }
}
