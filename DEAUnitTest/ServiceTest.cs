using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using System.Text;
using DEALib;
using RestSharp;

namespace DEAUnitTest
{
    [TestClass]
    public class ServiceTest
    {
        [TestMethod]
        public void TestVersion()
        {
            string url = @"http://localhost/DEAService/JsonServiceImpl.svc/version";
            try
            {
                WebRequest request = WebRequest.Create(url);
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string a = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void TestDataEnvelopedAnalysis()
        {
            string csvFilePath = @"C:\Projects\NCHRP 08-103\DEA-Example-20-depots.csv";
            string warningMessage, errorMessage;

       
            try
            {
           
                bool ok = true;
                DEAContext context = new DEAContext("DEAContext", "Depot");
                ok = context.AddVariable("STOCK", "I",  out errorMessage);
                if (ok)
                    ok = context.AddVariable("WAGES", "I",  out errorMessage);
                if (ok)
                    ok = context.AddVariable("ISSUES", "O",  out errorMessage);
                if (ok)
                    ok = context.AddVariable("RECEIPTS", "O",  out errorMessage);
                if (ok)
                    ok = context.AddVariable("REQS", "O",  out errorMessage);
                if (ok)
                    ok = context.UploadCsvFile(csvFilePath, out warningMessage, out errorMessage);
                if (ok)
                    ok = context.AddCostConstraint("STOCK", 27.0, out errorMessage);
                if (ok)
                    ok = context.AddCostConstraint("WAGES", 40, out errorMessage);

                if (ok)
                {
                    var client = new RestClient();
                    client.BaseUrl = new Uri(@"http://localhost/DEAJsonService/JsonServiceImpl.svc");
                    var request = new RestRequest();
                    request.Method = Method.POST;
                   // request.AddHeader("Accept", "application/json");
                   // request.RequestFormat = DataFormat.Json;
                    string json = context.ToJsonString(out errorMessage);
                    ok = (json != null);
                    if (ok)
                    {
                        request.AddParameter("text/plain", json, ParameterType.RequestBody);
                        request.Resource = "json/DEA";
                    }
                    var response = client.Execute(request);

                    int stop = 0;
                }

                Assert.IsTrue(ok, errorMessage);
                
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [TestMethod]
        public void TestDataEnvelopedAnalysisConstrainedWeights()
        {
            string csvFilePath = @"C:\Projects\NCHRP 08-103\DEA-Example-20-depots.csv";
            string warningMessage, errorMessage;


            try
            {

                bool ok = true;
                DEAContext context = new DEAContext("DEAContext", "Depot");
                ok = context.AddVariable("STOCK", "I",  out errorMessage);
                if (ok)
                    ok = context.AddVariable("WAGES", "I", out errorMessage);
                if (ok)
                    ok = context.AddVariable("ISSUES", "O", 0, 1, out errorMessage);
                if (ok)
                    ok = context.AddVariable("RECEIPTS", "O", 0, 1, out errorMessage);
                if (ok)
                    ok = context.AddVariable("REQS", "O", 0, 1, out errorMessage);
                if (ok)
                    ok = context.UploadCsvFile(csvFilePath, out warningMessage, out errorMessage);
                if (ok)
                    ok = context.AddCostConstraint("STOCK", 27.0, out errorMessage);
                if (ok)
                    ok = context.AddCostConstraint("WAGES", 40, out errorMessage);

                if (ok)
                {
                    var client = new RestClient();
                    client.BaseUrl = new Uri(@"http://localhost/DEAJsonService/JsonServiceImpl.svc");
                    var request = new RestRequest();
                    request.Method = Method.POST;
                    string json = context.ToJsonString(out errorMessage);
                    ok = (json != null);
                    if (ok)
                    {
                        request.AddParameter("text/plain", json, ParameterType.RequestBody);
                        request.Resource = "json/DEA";
                    }
                    var response = client.Execute(request);

                    int stop = 0;
                }

                Assert.IsTrue(ok, errorMessage);

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void TestDataEnvelopedAnalysisScaled()
        {
            string csvFilePath = @"C:\Projects\NCHRP 08-103\DEA-Example-20-depots.csv";
            string warningMessage, errorMessage;


            try
            {

                bool ok = true;
                DEAContext context = new DEAContext("DEAContext", "Depot");
                ok = context.AddVariable("STOCK", "I", 1.0, out errorMessage);
                if (ok)
                    ok = context.AddVariable("WAGES", "I", 1.0, out errorMessage);
                if (ok)
                    ok = context.AddVariable("ISSUES", "O", 1.0, out errorMessage);
                if (ok)
                    ok = context.AddVariable("RECEIPTS", "O", 1.0, out errorMessage);
                if (ok)
                    ok = context.AddVariable("REQS", "O", 19.0, out errorMessage);
                if (ok)
                    ok = context.UploadCsvFile(csvFilePath, out warningMessage, out errorMessage);
                if (ok)
                    ok = context.AddCostConstraint("STOCK", 27.0, out errorMessage);
                if (ok)
                    ok = context.AddCostConstraint("WAGES", 40, out errorMessage);

                if (ok)
                {
                    var client = new RestClient();
                    client.BaseUrl = new Uri(@"http://localhost/DEAJsonService/JsonServiceImpl.svc");
                    var request = new RestRequest();
                    request.Method = Method.POST;
                    string json = context.ToJsonString(out errorMessage);
                    ok = (json != null);
                    if (ok)
                    {
                        request.AddParameter("text/plain", json, ParameterType.RequestBody);
                        request.Resource = "json/DEA";
                    }
                    var response = client.Execute(request);

                    int stop = 0;
                }

                Assert.IsTrue(ok, errorMessage);

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
