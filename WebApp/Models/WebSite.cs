namespace WebApp.Models
{
    using System.Net;
    using Etc;
    using Etc.EF;

    public class WebSite : Repository<WebSite, Storage>
    {
        public string Domain { get; set; }
        public long Interval { get; set; }
        public HttpStatusCode LastStatus { get; set; }
        public string Name { get; set; }

        public bool IsAvailable => (int) LastStatus == 200 || (int) LastStatus == 418;
    }
}