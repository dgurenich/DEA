using System;
using System.Linq;
using System.Web.UI.WebControls;
using DEALib;
using System.Data;

public partial class DEAOutput : System.Web.UI.UserControl
{
    private string _contextKey = "DEACONTEXT";

    protected void Page_Load(object sender, EventArgs e)
    {
        hlinkExcel.Visible = false;
        LabelError.Visible = false;
        Populate();
    }


    public void Populate(string err = null)
    {
        string errorMessage = null;
        DataSet ds = null;
        try
        {
            if (Session[_contextKey] != null)
            {
                DEAContext context = Session[_contextKey] as DEAContext;
                ds = context.ToDataSet(out errorMessage);
                if (ds != null)
                {
                    GridViewProjects.Columns.Clear();
                    BoundField bf = new BoundField();
                    bf.HeaderText = context.ProjectIDFieldName;
                    bf.DataField = "NAME";
                    GridViewProjects.Columns.Add(bf);

                    foreach (DataRow r in ds.Tables["VARIABLES"].Rows)
                    {
                        string varName = r["NAME"].ToString();
                        bf = new BoundField();
                        bf.HeaderStyle.CssClass = "text-right";
                        bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                        bf.HeaderText = varName;
                        bf.DataField = varName;
                        bf.DataFormatString = "{0:f6}";
                        GridViewProjects.Columns.Add(bf);
                    }

                    bf = new BoundField();
                    bf.HeaderStyle.CssClass = "text-right";
                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    bf.HeaderText = "Relative Efficiency";
                    bf.DataField = "RELATIVE_Efficiency";
                    bf.DataFormatString = "{0:f6}";
                    GridViewProjects.Columns.Add(bf);

                    if (ds.Tables["CONSTRAINTS"].Rows != null && ds.Tables["CONSTRAINTS"].Rows.Count > 0)
                    {
                        bf = new BoundField();
                        bf.HeaderStyle.CssClass = "text-right";
                        bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                        bf.HeaderText = "Selected";
                        bf.DataField = "SELECTED";
                        bf.ReadOnly = true;
                        GridViewProjects.Columns.Add(bf);
                    }

                    bf = new BoundField();
                    bf.HeaderStyle.CssClass = "text-right";
                    bf.HeaderText = "Approximate";
                    bf.DataField = "APPROXIMATE";
                    bf.DataFormatString = "{0}";
                    GridViewProjects.Columns.Add(bf);

                    GridViewProjects.DataSource = ds.Tables["PROJECTS"];
                    GridViewProjects.DataBind();
                }
            }
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            LabelError.Text = errorMessage;
            LabelError.Visible = true;
            hlinkExcel.Visible = false;
        }

        if (string.IsNullOrEmpty(errorMessage))
        {
            hlinkExcel.Visible = true;
            if (!string.IsNullOrEmpty(err))
            {
                LabelError.Text = err;
                LabelError.Visible = true;
            }
        }
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Session.Remove(_contextKey);
        Response.Redirect("~/Default.aspx");
    }
}