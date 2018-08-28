using System;
using System.Linq;
using DEALib;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.IO;

public partial class DEAOutputExcel : System.Web.UI.Page
{
    private string _contextKey = "DEACONTEXT";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (GridViewProjects != null)
            {
                Populate();
                ExportToExcel();
            }
        }
    }

    void Populate()
    {
        string errorMessage = null;
        if (Session[_contextKey] != null)
        {
            DEAContext context = Session[_contextKey] as DEAContext;
            DataSet ds = context.ToDataSet(out errorMessage);
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


    private void ExportToExcel()
    {
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=DEARankedProjectExport.xls");
        Response.Charset = "";
        Response.ContentType = "application/vnd.ms-excel";

        using (StringWriter sw = new StringWriter())
        {
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            //To Export all pages
            GridViewProjects.AllowPaging = false;

            GridViewProjects.HeaderRow.BackColor = Color.White;
            foreach (TableCell cell in GridViewProjects.HeaderRow.Cells)
            {
                cell.BackColor = GridViewProjects.HeaderStyle.BackColor;
            }
            foreach (GridViewRow row in GridViewProjects.Rows)
            {
                row.BackColor = Color.White;
                foreach (TableCell cell in row.Cells)
                {
                    if (row.RowIndex % 2 == 0)
                    {
                        cell.BackColor = GridViewProjects.AlternatingRowStyle.BackColor;
                    }
                    else
                    {
                        cell.BackColor = GridViewProjects.RowStyle.BackColor;
                    }
                    cell.CssClass = "textmode";
                }
            }

            GridViewProjects.RenderControl(hw);

            //style to format numbers to string
            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Response.Write(style);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        /* Verifies that the control is rendered */
    }

}