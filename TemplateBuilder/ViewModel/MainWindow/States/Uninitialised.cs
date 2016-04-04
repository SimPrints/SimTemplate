using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateBuilder.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        public class Uninitialised : TemplateBuilderBaseState
        {
            // TODO: Implement this state and add BeginInitialize as a method.
            public Uninitialised(TemplateBuilderViewModel outer) : base(outer)
            { }
        }
    }
}
