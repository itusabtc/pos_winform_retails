using NailsChekin.Models.Helper;

namespace NailsChekin.Models.Payments
{
    public class CloverLib
    {
        public static CLOVER_CONNECTTION_TYPE Get_ConecttionType_Setting()
        {
            string connection_type = Constants.clover_connection_type;
            if (connection_type.Equals("WLAN/LAN"))
                return CLOVER_CONNECTTION_TYPE.WLAN_LAN;
            else if (connection_type.Equals("PAIR MODE"))
                return CLOVER_CONNECTTION_TYPE.PAIR_MODE;
            else if (connection_type.Contains("USB"))
                return CLOVER_CONNECTTION_TYPE.USB;
            else if (connection_type.Equals("CLOUD"))
                return CLOVER_CONNECTTION_TYPE.CLOUD;

            return CLOVER_CONNECTTION_TYPE.NONE;
        }
    }

    public enum CLOVER_CONNECTTION_TYPE
    {
        WLAN_LAN,
        PAIR_MODE,
        USB,
        CLOUD,
        NONE
    }

}
