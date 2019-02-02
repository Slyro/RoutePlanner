using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RoutePlanner
{
    class AKDTools
    {
        public enum GetOrders
        {
            All,
            Planned
        }
        public static string Link { get; set; }
        public static string Login { get; set; }
        public static string Password { get; set; }
        public static int CenterID { get; set; }
        public static string Territories { get; set; }
        private static readonly string script = "vehicles=JourneyUtil.getVehicles(),srch=function(e,r,n){for(var s=0;s<n.length&&e!=n[s].name;)s+=1;if(s<n.length){if(!n[s].isValid)return 4;for(var i=!0,l=0;l<r.length;l+=1)if(r[l]==n[s].schedulingZoneId){i=!1;break}return i?0:(Ct.Selection.selectOrder(n[s]),1)}for(var t=0;t<vehicles.length;t+=1)for(var h=0;h<vehicles[t].runs.length;h+=1)for(var c=0;c<vehicles[t].runs[h].orders.length;c+=1){try{if(e==vehicles[t].runs[h].orders[c].name&&vehicles[t].runs[h].orders[c].failReason.search(\"не подходят\")>0)return 5}catch(e){return 2}if(e==vehicles[t].runs[h].orders[c].name)return 2}return 3};";
        private static readonly string getunplannedorders = "alldrops=function(){for(var n=[],e=0,r=0;r<Ct.UnplannedOrdersPanel.Panel.orders.length;r++)n[e]=Ct.UnplannedOrdersPanel.Panel.orders[r].name,e++;return n};";
        private static readonly string getplannedorders = "vehicles=JourneyUtil.getVehicles(),getPlannedOrders=function(){for(var e=[],r=0,n=0;n<vehicles.length;n+=1)for(var s=0;s<vehicles[n].runs.length;s+=1)for(var l=0;l<vehicles[n].runs[s].orders.length;l+=1)e[r]=vehicles[n].runs[s].orders[l].name,r+=1;return e};";
        public static bool IsBusy => !(bool)DriverManager.ExecuteScript("return Schedule.isPlanningProcess") && !(bool)DriverManager.ExecuteScript("return Ct.Loader.isShowing") ? false : true;
        public static bool HaveSelectedOrder => !((bool)DriverManager.ExecuteScript("return Ct.Selection.selectedOrder == null"));
        public static bool IsReady => (bool)DriverManager.ExecuteScript("return $.isReady");
        public static void ScriptInject() => DriverManager.ExecuteScript(script);
        public static int SelectOrder(string order_track, int[] zoneID)
        {
            string zones = "[";
            for (int i = 0; i < zoneID.Length - 1; i++)
            {
                new StringBuilder().Append($"{zoneID[i]},");
            }
            zones += $"{new StringBuilder().ToString()}{zoneID[zoneID.Length - 1]}]";
            return Convert.ToInt32(DriverManager.ExecuteScript($"return srch(\"{order_track.ToUpperInvariant()}\",{zones},Ct.UnplannedOrdersPanel.Panel.orders)"));
        }
        public static int GetCenterID() => Convert.ToInt32(DriverManager.ExecuteScript("return Ct.Globalfilter.distributionCentresList[0].id"));
        public static List<string> GetOrderList(GetOrders orders)
        {
            IEnumerable<object> lst = null;
            switch (orders)
            {
                case (GetOrders.All):
                    DriverManager.ExecuteScript(getunplannedorders);
                    lst = (ReadOnlyCollection<object>)DriverManager.ExecuteScript("return alldrops()");
                    break;
                case (GetOrders.Planned):
                    DriverManager.ExecuteScript(getplannedorders);
                    lst = (ReadOnlyCollection<object>)DriverManager.ExecuteScript("return getPlannedOrders()");
                    break;
                default:
                    break;
            }       
            foreach (object item in lst)
            {
                new List<string>().Add(item.ToString());
            }
            return new List<string>();
        }      
        public static string GetTerritories()
        {
            if (!IsBusy && IsReady)
            {
                DriverManager.Url = $"{DriverManager.Url.Remove(DriverManager.Url.LastIndexOf(".ru") + 3)}/gt/gt-api/scheduling-zones/?aocId={CenterID}";
            }
            DriverManager.Back();
            return PropertiesCollections.driver.FindElement(By.TagName("pre")).Text;
        }
        public static void JQLoaderWait()
        {
            try
            {
                while (true)
                {
                    if (!IsBusy)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(500);
                }
            }
            catch (WebDriverException) { }
            
        } //Работает и ладно...
        public static void Renew() => DriverManager.ExecuteScript("OpenAjax.hub.publish(\"data.reload\")");
    }
}