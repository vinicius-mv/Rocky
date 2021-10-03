namespace Rocky.Utility
{
    public static class WebConstants
    {
        public static class Paths
        {
            public const string ProductImages = @"\images\product\";
        }

        public static class Sessions
        {
            public const string ShoppingCartList = "ShoppingCartSession";
            public const string InquiryHeaderId = "InquiryHeaderSession";
        }

        public static class Notifications
        {
            public const string Success = "Success";
            public const string Error = "Error";
        }

        public static class Roles
        {
            public const string Admin = "Admin";
            public const string Customer = "Customer";
        }

        public static class Settings
        {
            public const string EmailAdmin = "EmailAdmin";
        }
    }
}
