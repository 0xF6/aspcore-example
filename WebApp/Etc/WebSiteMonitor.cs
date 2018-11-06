namespace WebApp.Etc
{
    using System.Net;
    using Fluent.Task;
    using Fluent.Task.Enum;
    using Models;
    using StorageType = System.Collections.Generic.Dictionary<System.Guid, WebSiteMonitor>;

    // TODO: Move to middleware\service
    public class WebSiteMonitor
    {
        public static StorageType Storage = new StorageType();
        private WebSiteMonitor(WebSite site) => Metadata = site;
        public WebSite Metadata { get; }
        public Schedule Schedule { get; set; }
        public void Action()
        {
            Tools.IsAvailable(Metadata.Domain, out var code);
            Metadata.LastStatus = code;
            Metadata.Update();
        }

        public static WebSiteMonitor From(WebSite site, TaskScheduler scheduler)
        {
            lock (Guarder)
            {
                var action = new WebSiteMonitor(site);
                action.Schedule = Schedule
                    .Instance(o => action.Action())
                    .SetFrequencyTime((int)site.Interval)
                    .SetStartImmediately()
                    .RunLoop(scheduler);
                Storage.Add(site.UID, action);
                return action;
            }
        }

        public static void RemoveAndStop(WebSite site)
        {
            lock (Guarder)
            {
                if (!Storage.ContainsKey(site.UID))
                    return;
                var action = Storage[site.UID];
                action.Schedule.LoopSettings.IsLoop = false;
                action.Schedule.Restart();
                Storage.Remove(action.Metadata.UID);
            }
        }


        private static readonly object Guarder = new object();
    }
}