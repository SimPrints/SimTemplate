using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateBuilder.Model.Database;

namespace TemplateBuilder.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        public class Uninitialised : TemplateBuilderBaseState
        {
            public Uninitialised(TemplateBuilderViewModel outer) : base(outer)
            { }

            public override void DataController_InitialisationComplete(InitialisationCompleteEventArgs e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
