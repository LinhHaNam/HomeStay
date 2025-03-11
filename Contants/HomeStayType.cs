namespace HomestayWeb.Contants
{
    public class HomeStayType
    {

        public static string ORDERED = "ORDERED";
        public static string AVAILABLE = "AVAILABLE";
        public static string CLEANING = "CLEANING";

        private static List<string> instance = null;

        public static List<string> Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new List<string>()
                    {
                        ORDERED, AVAILABLE, CLEANING
                    };
                }
                return instance;
            }
        }
    }
}
