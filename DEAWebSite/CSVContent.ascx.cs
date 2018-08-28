using System;
using DEALib;
using DEAService.Responses;
using System.Data;
using System.Web.UI.WebControls;
using RestSharp;
using System.Configuration;
using Newtonsoft.Json;

public partial class CSVContent : System.Web.UI.UserControl
{
    private string _contextKey = "DEACONTEXT";

    protected void Page_Load(object sender, EventArgs e)
    {
        ButtonRun.Visible = false;
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
                    GridViewParams.DataSource = ds.Tables["PARAMETERS"];
                    GridViewParams.DataBind();

                    GridViewVars.DataSource = ds.Tables["VARIABLES"];
                    GridViewVars.DataBind();

                    GridViewConst.DataSource = ds.Tables["CONSTRAINTS"];
                    GridViewConst.DataBind();

                    if (ds.Tables["CONSTRAINTS"].Rows == null || ds.Tables["CONSTRAINTS"].Rows.Count < 1)
                    {
                        LabelConst.Visible = false;
                    }

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
                    GridViewProjects.DataSource = ds.Tables["PROJECTS"];
                    GridViewProjects.DataBind();
                }
            }
        }
        catch(Exception ex)
        {
            errorMessage = ex.Message;
            LabelError.Text = errorMessage;
            LabelError.Visible = true;
            ButtonRun.Visible = false;
        }

        if (string.IsNullOrEmpty(errorMessage))
        {
            ButtonRun.Visible = true;
            if (ds != null)
            {
                if (ds.Tables["CONSTRAINTS"].Rows != null && ds.Tables["CONSTRAINTS"].Rows.Count > 0)
                {
                    ButtonRun.ToolTip = "Click to rank projects by their relative efficiency and apply constraints to their selection.";
                }
                else
                {
                    ButtonRun.ToolTip = "Click to rank projects by their relative efficiency.";
                }
            }


            if (!string.IsNullOrEmpty(err))
            {
                LabelError.Text = err;
                LabelError.Visible = true;
            }
        }
    }


    protected void ButtonRun_Click(object sender, EventArgs e)
    {
        string errorMessage = null;
        try
        {
            DEAContext context = Session[_contextKey] as DEAContext;
            string jsonIn = context.ToJsonString(out errorMessage);
            if (string.IsNullOrEmpty(jsonIn))
            {
                throw new Exception(errorMessage);
            }
            string serviceUrl = ConfigurationManager.AppSettings["deaServiceUrl"];
            var client = new RestClient();
            client.BaseUrl = new Uri(serviceUrl);
            RestRequest deaRequest = new RestRequest();
            deaRequest.Method = Method.POST;
            deaRequest.AddParameter("text/plain", jsonIn, ParameterType.RequestBody);
            deaRequest.Resource = "json/DEA";
            var response = client.Execute(deaRequest);
            if (response.IsSuccessful && response.ResponseStatus == ResponseStatus.Completed)
            {
                string jsonOut = response.Content;
                DEAResponse deaResponse = JsonConvert.DeserializeObject<DEAResponse>(jsonOut);
                if (deaResponse == null)
                {
                    throw new Exception("Unable to de-serialize JSON response.  Please check the log of the service.");
                }
                if (deaResponse.OK == false)
                {
                    throw new Exception(deaResponse.errorMessage);
                }
                Session[_contextKey] = deaResponse.context;
                DEAEventArgs args = new DEAEventArgs();
                args.Cargo = "DEAContextOK";
                RaiseBubbleEvent(this, args);
            }
            else
            {
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    throw new Exception(response.ErrorMessage);
                }
                else
                {
                    throw new Exception(response.ResponseStatus.ToString());
                }
            }
        }
        catch (Exception ex)
        {
            LabelError.Text = ex.Message;
            LabelError.Visible = true;
            DEAEventArgs args = new DEAEventArgs();
            args.Cargo = "ERROR: " + ex.Message;
            RaiseBubbleEvent(this, args);
        }
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Session.Remove(_contextKey);
        Response.Redirect("~/Default.aspx");
    }
}