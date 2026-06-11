namespace NailsChekin.Models
{
    public class Enums
    {

    }

    public enum P5_CONNECTTION_TYPE
    {
        WLAN_LAN,
        PAIR_MODE,
        USB,
        CLOUD,
        NONE
    }

    public enum CODEPAY_DEVICE
    {
        P5,
        T2
    }

    public enum CREDIT_DEVICE_TYPE
    {
        CLOVER,
        CODE_PAY,
        NONE
    }

    public enum PAYMENT_MODE
    {
        USING_SAVE,
        USING_SERVICE_NOW,
        NONE
    }

    public enum LAYOUT_MODE
    {
        STANDARD,
        MINI_SCREEN,
        LARGE_SCREEN
    }

    public enum PAYMENT_NOW_MODE
    {
        TICKET,
        GIFT_CARD
    }

    public enum SYSTEM_MODE
    {
        TEST,
        REAL
    }

    public enum POS_ROLE
    {
        PRIMARY,
        SECONDARY
    }

    public enum SYSTEM_MENU
    {
        FULL_MENU,
        QUICK_MENU
    }

}
