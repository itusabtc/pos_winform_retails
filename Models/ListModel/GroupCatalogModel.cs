using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NailsChekin.Models.ListModel
{
    class GroupCatalogModel
    {
        public string id = "";
        public string code = "";
        public string name = "";
        public string shortName = "";
        public string image = "";

        public GroupCatalogModel() { }

        public GroupCatalogModel(string id, string code, string name, string shortName, string image)
        {
            this.id = id;
            this.code = code;
            this.name = name;
            this.shortName = shortName;
            this.image = image;
        }

    }
}
