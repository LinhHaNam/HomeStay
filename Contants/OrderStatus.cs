namespace HomestayWeb.Contants
{
    public class OrderStatus
    {

        public static string PENDING_CONFIRM = "PENDING CONFIRM";
        public static string CONFIRMED = "CONFIRMED";
        public static string CHECKED_IN = "CHECKED IN";
        public static string CHECKED_OUT = "CHECKED OUT";

        public static List<string> status = new List<string>()
        {
            PENDING_CONFIRM, CONFIRMED, CHECKED_IN, CHECKED_OUT
        };

    }
}
