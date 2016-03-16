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
            public Uninitialised(TemplateBuilderViewModel outer) : base(outer)
            { }


        }
    }
}
