using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class TestAppData : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lit.Text = System.IO.File.GetLastWriteTime(Server.MapPath("~/App_Data/TextFile1.txt")).ToString(); // System.IO.File.ReadAllText(Server.MapPath("~/App_Data/TextFile.txt"));
        System.IO.File.WriteAllText(Server.MapPath("~/App_Data/TextFile1.txt"), "slidujh lkajsd hflkadshf alkhf alk");
    }
}