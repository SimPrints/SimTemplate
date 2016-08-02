using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.DataTypes.Enums
{
    public enum Activity
    {
        None = 0,
        Uninitialised,
        Idle,
        Transitioning,
        Loading,
        Templating,
        Fault,
    }
}
