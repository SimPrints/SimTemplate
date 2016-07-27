using SimTemplate.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Utilities
{
    public interface IWindowService
    {
        void Show(object dataContext);

        bool? ShowDialog(object dataContext);
    }
}
