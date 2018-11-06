namespace WebApp.Etc
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading.Tasks;
    using Flurl.Http;

    public static class Tools
    {
        public static bool IsAvailable(string url, out HttpStatusCode code) // TODO https
        {
            var result = $"http://{url}/".
                WithTimeout(15).
                AllowAnyHttpStatus().
                HeadAsync().ContinueWith(x => x.IsFaulted ? null : x.Result).Result;
            code = result?.StatusCode ?? HttpStatusCode.ServiceUnavailable;
            return code == HttpStatusCode.OK || code == (HttpStatusCode)418;
        }
    }
}