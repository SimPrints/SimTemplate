using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateBuilder.ViewModel
{
    public enum OpenImageResult
    {
        None = 0,
        Running,
        Successful,
        Failed,
        InvalidFileCountExceeded,
    }
}
