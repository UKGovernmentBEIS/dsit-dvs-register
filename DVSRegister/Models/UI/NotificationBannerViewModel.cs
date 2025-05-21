namespace DVSRegister.Models
{
    public class NotificationBannerViewModel
    {
        public string Title { get; set; }
        public List<NotificationContent> NotificationContent { get; set; }
      
    }

    public class NotificationContent
    {
        public string Heading { get; set; }
        public string HtmlContent { get; set; }
    }
}