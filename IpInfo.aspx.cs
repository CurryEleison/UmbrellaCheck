using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class IpInfo : System.Web.UI.Page
{
    protected string headers = String.Empty;
    protected string servervariables = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {

        // If you want it formated in some other way.
        // var headers = String.Empty;
        foreach (var key in Request.Headers.AllKeys)
            headers += key + "=" + Request.Headers[key] + Environment.NewLine;

        
        foreach (var key in Request.ServerVariables.AllKeys)
            servervariables += key + "=" + Request.ServerVariables[key] + Environment.NewLine;

    }
}