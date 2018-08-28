using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            FileUploader1.Visible = true;
            CSVContent1.Visible = false;
            DEAOutput1.Visible = false;
        }
    }

    protected override bool OnBubbleEvent(object source, EventArgs args)
    {
        if (source == FileUploader1 && args is DEAEventArgs && (args as DEAEventArgs).Cargo == "DEAContextOK")
        {
            FileUploader1.Visible = false;
            DEAOutput1.Visible = false;
            CSVContent1.Populate();
            CSVContent1.Visible = true;
        }
        else if (source == CSVContent1 && args is DEAEventArgs && (args as DEAEventArgs).Cargo.StartsWith("ERROR:"))
        {
            FileUploader1.Visible = false;
            DEAOutput1.Visible = false;
            CSVContent1.Populate((args as DEAEventArgs).Cargo);
            CSVContent1.Visible = true;
        }
        else if (source == CSVContent1 && args is DEAEventArgs && (args as DEAEventArgs).Cargo == "DEAContextOK")
        {
            FileUploader1.Visible = false;
            CSVContent1.Visible = false;
            DEAOutput1.Populate();
            DEAOutput1.Visible = true;
        }
        return base.OnBubbleEvent(source, args);
    }

}