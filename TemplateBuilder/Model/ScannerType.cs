using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Model
{
    public enum ScannerType
    {
        [Description("All")]
        None = 0,

        [Description("Hamster")]
        Hamster,

        [Description("LES")]
        LES,

        [Description("UareU")]
        UareU,

        [Description("Next")]
        Next,

        [Description("Lumidigm")]
        Lumidigm,

        [Description("Eikon")]
        Eikon,
    }
}
