using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NailsChekin.Models.ListModel
{
    public class CartItemModel
    {
        public string cart_order_id = "";
        public string cart_item_id = "";

        public string item_id = "";
        public string item_name = "";

        public string quantity = "1";
        public string price = "0";
        public string discount = "";

        public string isPromotion = "0";
        public string scheme = "";

        public CartItemModel() { }

        public CartItemModel(string item_id, string item_name, string quantity, string price, string discount = "0")
        {
            this.item_id = item_id;
            this.item_name = item_name;
            this.quantity = quantity;
            this.price = price;
            this.discount = discount;
        }
    }
}
