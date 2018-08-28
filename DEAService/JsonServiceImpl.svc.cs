using System;
using System.Configuration;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Text;
using System.IO;
using log4net;
using DEALib;
using DEAService.Responses;
using Newtonsoft.Json;

namespace DEAService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "JsonServiceImpl" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select JsonServiceImpl.svc or JsonServiceImpl.svc.cs at the Solution Explorer and start debugging.
   [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class JsonServiceImpl : IJsonServiceImpl
    {

        private static ILog log = null;
        public static ILog LOG
        {
            get
            {
                if (log != null)
                {
                    return log;
                }

                try
                {
                    log4net.Config.XmlConfigurator.Configure();
                    string logName = ConfigurationManager.AppSettings["LogName"];
                    log = LogManager.GetLogger(logName);
                }
                catch(Exception)
                {
                    log = null; 
                }

                return log;
            }
        }
          
        public Stream GetVersion()
        {
          
            UriTemplateMatch utm = WebOperationContext.Current.IncomingRequest.UriTemplateMatch;
            LOG.Debug(utm.RequestUri.OriginalString);
         

            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            version += "\nBuilt: " + System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString();
            version += "\nHost: " + System.Environment.MachineName;
            Stream ms = new System.IO.MemoryStream(ASCIIEncoding.Default.GetBytes("DEA Service Assembly\nVersion: " + version));
            return ms;
        }

        public Stream RunDEAnalysis(Stream request)
        {
            bool ok = true;
            string errorMessage = null;
            DEAContext context = null;
            string jsonRequest = null;
            string jsonResponse = null;
            const string fn = "RunDEAnalysis";

            LOG.DebugFormat("{0} - started", fn);

            UriTemplateMatch utm = WebOperationContext.Current.IncomingRequest.UriTemplateMatch;
            LOG.Debug(utm.RequestUri.OriginalString);

            try
            {
                using (StreamReader reader = new StreamReader(request))
                {
                    jsonRequest = reader.ReadToEnd();
                }

                LOG.DebugFormat("Request: {0}", jsonRequest);

                context = DEAContext.CreateFromJsonString(jsonRequest, out errorMessage);
                ok = (context != null);
                if (ok)
                {
                    ok = context.RunDEA(out errorMessage);
                    if (ok && context.ConstraintsSet)
                    {
                        ok = context.ApplyCostConstraintsToProjectSelection(out errorMessage);
                    }
                    if (ok)
                    {
                        jsonResponse = context.ToJsonString(out errorMessage);
                        ok = (jsonResponse != null);
                    }
                }
            }
            catch(Exception ex)
            {
                ok = false;
                errorMessage = ex.Message;
            }

            DEAResponse response = new DEAResponse();
            response.OK = ok;
            response.errorMessage = errorMessage;
            response.context = context;

            jsonResponse = JsonConvert.SerializeObject(response, Formatting.Indented);

            LOG.DebugFormat("Response: {0}", jsonResponse);
            Stream ms = new MemoryStream(ASCIIEncoding.Default.GetBytes(s: jsonResponse));
            LOG.DebugFormat("{0} - ended", fn);

            return ms;
        }
       
    }
}
