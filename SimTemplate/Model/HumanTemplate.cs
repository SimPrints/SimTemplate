using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.ViewModel
{
    public class HumanTemplate
    {
        public HumanTemplate(long id, string base64Image)
        {
            Id = id;
            Base64Image = base64Image;
        }

        public long Id;
        public string Base64Image;
    }
}
