using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;

public partial class UmbrellaCallback : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentType = "application/json";


        // Hopefully we got exact coordinates from the client. html5 for the win!
        float lat = string.IsNullOrWhiteSpace(Request["lat"]) ? float.NaN : float.Parse(Request["lat"]);
        float lon = string.IsNullOrWhiteSpace(Request["lat"]) ? float.NaN : float.Parse(Request["lon"]);

        if (float.IsNaN(lat) || float.IsNaN(lon))
        {
            // Oh well. Let's do a geoip lookup and see if that works
            string ip = string.IsNullOrWhiteSpace(Request["ip"]) ? Request.UserHostAddress : Request["ip"];
            // This is because AppHarbor doesn't give me the ip address
            // if (ip != null && ip.StartsWith("10.") && !string.IsNullOrWhiteSpace(Request.ServerVariables["X-Forwarded-For"]))
            ip = Request.ServerVariables["X-Forwarded-For"];
            if (ip == "::1")
                ip = "87.72.246.106";
            Response.Write(ip);
            Response.Flush();
            WebClient ipwc = new WebClient();
            string locurl = String.Format("http://api.hostip.info/get_html.php?ip={0}&position=true", ip);
            string locinfo = ipwc.DownloadString(locurl);
            Regex reglat = new Regex(@"(Latitude\: )([0-9\.-]+)");
            Regex reglon = new Regex(@"(Longitude\: )([0-9\.-]+)");
            string slat = reglat.Match(locinfo).Groups[2].Value;
            string slon = reglon.Match(locinfo).Groups[2].Value;
            lat = float.Parse(slat);
            lon = float.Parse(slon);
        }

        // Now that we have lat and lon it's time to figure out the weather
        string weatherurl = string.Format("http://api.wunderground.com/auto/wui/geo/ForecastXML/index.xml?query={0},{1}", lat, lon);
        WebClient wc = new WebClient();
        string weatherxml = wc.DownloadString(weatherurl);
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(weatherxml);

        // We are only interested in a single particular day (today)
        XmlNodeList xnl = xml.SelectNodes("//simpleforecast/forecastday[period=1]");
        string conditions = (xnl != null && xnl.Count > 0) ? xnl.Item(0).SelectSingleNode("conditions").InnerText : "Argh!";

        // Knowing what we know should you bring an umbrella?
        string output = String.Empty;
        switch (conditions)
        {
            case "Chance of Flurries":
            case "Chance of Rain":
            case "Chance of Freezing Rain":
            case "Chance of Sleet":
            case "Chance of Snow":
            case "Chance of Thunderstorms":
            case "Chance of a Thunderstorm":
            case "Flurries":
            case "Freezing Rain":
            case "Rain":
            case "Sleet":
            case "Snow":
            case "Thunderstorms":
            case "Thunderstorm":
                output = "Yes";
                break;
            case "Sunny":
            case "Overcast":
            case "Scattered Clouds":
            case "Mostly Cloudy":
            case "Mostly Sunny":
            case "Partly Cloudy":
            case "Partly Sunny":
            case "Fog":
            case "Haze":
            case "Clear":
            case "Cloudy":
                output = "No";
                break;
            case "Unknown":
            default:
                output = "Uhh";
                break;



        }

        //Good. OUt it goes
        litOutput.Text = String.Format("{{\"main\": \"{3}\", \"sub\": \"Its going to be that bad.\", \"conditions\": \"{2}\", \"url\": \"{0}\"}}", weatherurl, weatherxml, conditions, output);
    }
}