using System;
using System.Web.UI.WebControls;
using System.Configuration;
using DEALib;
using System.IO;

public partial class VarUploader : System.Web.UI.UserControl
{
    
   
    public string VarFilePath
    {
        set
        {
            ViewState["VAR_FILE"] = value;
        }

        get
        {
            return ViewState["VAR_FILE"].ToString();
        }
    }

    public string ProjFilePath
    {
        set
        {
            ViewState["PROJ_FILE"] = value;
        }

        get
        {
            return ViewState["PROJ_FILE"].ToString();
        }
    }

    public string ConstFilePath
    {
        set
        {
            ViewState["CONST_FILE"] = value;
        }

        get
        {
            return ViewState["CONST_FILE"].ToString();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        LabelError.Visible = false;
        LabelContextError.Visible = false;
    }

    protected void LinkButton1_Command(object sender, CommandEventArgs e)
    {
        try
        {
            string err = string.Empty;
            if (FileUploadVar.HasFile)
            {
                VarFilePath = MapPath("~/App_Data/" + Session.SessionID.Trim() + "-" + FileUploadVar.FileName);
                FileUploadVar.SaveAs(VarFilePath);
            }
            else
            {
                err += "Please select a CSV file with variables' specifications.";
            }

            if (FileUploadProj.HasFile)
            {
                ProjFilePath = MapPath("~/App_Data/" + Session.SessionID.Trim() + "-" + FileUploadProj.FileName);
                FileUploadProj.SaveAs(ProjFilePath);
            }
            else
            {
                if (!string.IsNullOrEmpty(err))
                {
                    err += "<br/><br/>";
                }
                err += "Please select a CSV file with the projects' specifications.";
            }

            if (FileUploadConstraints.HasFile)
            {
                ConstFilePath = MapPath("~/App_Data/" + Session.SessionID.Trim() + "-" + FileUploadConstraints.FileName);
                FileUploadConstraints.SaveAs(ConstFilePath);
            }
            else
            {
                ConstFilePath = string.Empty;
            }

            if (!string.IsNullOrEmpty(err))
            {
                LabelContextError.Text = err;
                LabelContextError.Visible = true;
            }
            else
            {
                bool ok = UploadFiles(out err);
                if (!ok)
                {
                    LabelContextError.Text = err;
                    LabelContextError.Visible = true;
                }
                else
                {
                    File.Delete(VarFilePath);
                    File.Delete(ProjFilePath);
                    if (!string.IsNullOrEmpty(ConstFilePath))
                    {
                        File.Delete(ConstFilePath);
                    }
                    DEAEventArgs arg = new DEAEventArgs();
                    arg.Cargo = "DEAContextOK";
                    RaiseBubbleEvent(this, arg);
                }
            }
        }
        catch(Exception ex)
        {
            LabelError.Text = ex.Message;
            LabelError.Visible = true;
        }

    }

    bool UploadFiles(out string errorMessage)
    {
        errorMessage = null;
        bool ok = true;
        string contextKey = "DEACONTEXT";

        try
        {
            string warningMessage = null; 
            Session.Remove(contextKey);
            string projColName = ConfigurationManager.AppSettings["csvProjectIDColumnName"];
            if (string.IsNullOrEmpty(projColName))
            {
                projColName = "Project";
            }
            DEAContext context = new DEAContext("DEAContext", projColName);

            ok = context.LoadVariablesFromCsvFile(VarFilePath, out errorMessage);
            if (ok)
            {
                ok = context.UploadCsvFile(ProjFilePath, out warningMessage, out errorMessage);
            }
            if (ok && !string.IsNullOrEmpty(ConstFilePath))
            {
                ok = context.LoadCsvConstraints(ConstFilePath, out errorMessage);
            }

            Session[contextKey] = context;
        }
        catch(Exception ex)
        {
            errorMessage = ex.Message;
            ok = false;
        }
        return ok;
    }

   
}