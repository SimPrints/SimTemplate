using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel
    {
        public class Uninitialised : MainWindowState
        {
            public Uninitialised(MainWindowViewModel outer) : base(outer)
            { }

            public override void BeginInitialise()
            {
                TransitionTo(typeof(Initialising));
            }
        }
    }
}
