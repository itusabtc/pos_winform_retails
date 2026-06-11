using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NailsChekin.Models.ListModel
{
    class PendingTurnModel
    {
        public string id = "";
        public string staffId = "";
        public string customerId = "";
        public string jService = "";
        public string is_same_time = "";

        public PendingTurnModel() { }

        public PendingTurnModel(string id, string staffId, string customerId, string jService, string is_same_time)
        {
            this.id = id;
            this.staffId = staffId;
            this.customerId = customerId;
            this.jService = jService;
            this.is_same_time = is_same_time;
        }

    }

}
