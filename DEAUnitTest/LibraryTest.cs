using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DEALib;
using System.Data;

namespace DEAUnitTest
{
    [TestClass]
    public class LibraryTest
    {
        [TestMethod]
        public void Test20Depots()
        {
            string errorMessage = null;
            string warningMessage = null;

            string csvFilePath = @"C:\Projects\NCHRP 08-103\DEA-Example-20-depots.csv";
            string xmlFilePath = @"C:\Projects\NCHRP 08-103\DEA-Example-20-depots.xml";
            string xmlFilePath2 = @"C:\Projects\NCHRP 08-103\DEA-Example-20-depots-2.xml";

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
                ok = context.AddVariable("REQS", "O", 1.0, out errorMessage);
     
            if (ok)
            {
                ok = context.UploadCsvFile(csvFilePath, out warningMessage, out errorMessage);
            }

            if (ok)
            {
                ok = context.RunDEA(out errorMessage);

            }
            
            if (ok)
            {
                ok = context.AddCostConstraint("STOCK", 27.0, out errorMessage);
                if (ok)
                    ok = context.AddCostConstraint("WAGES", 40, out errorMessage);
            }

            if (ok)
            {
                ok = context.ApplyCostConstraintsToProjectSelection(out errorMessage);
            }
            Assert.IsTrue(ok, errorMessage);

            if (ok)
            {
                DataSet ds = context.ToDataSet(out errorMessage);
                Assert.IsNotNull(ds, errorMessage);
                ok = ds != null;
            }

            if (ok)
            {
                ok = context.SaveToXmlFile(xmlFilePath, out errorMessage);
                Assert.IsTrue(ok, errorMessage);
            }

            if (ok)
            {
                DEAContext context2 = DEAContext.CreateFromXmlFile(xmlFilePath, out errorMessage);
                Assert.IsNotNull(context2, errorMessage);
                if (context2 != null)
                {
                    ok = context2.SaveToXmlFile(xmlFilePath2, out errorMessage);
                    Assert.IsTrue(ok, errorMessage);
                }
            }

            if (ok)
            {
                string json = context.ToJsonString(out errorMessage);
                Assert.IsNotNull(json, errorMessage);
            }
        }


        [TestMethod]
        public void Test20DepotsConstrainedWeights()
        {
            string errorMessage = null;
            string warningMessage = null;

            string csvFilePath = @"C:\Projects\NCHRP 08-103\DEA-Example-20-depots.csv";
            string xmlFilePath = @"C:\Projects\NCHRP 08-103\DEA-Example-20-depots.xml";
            string xmlFilePath2 = @"C:\Projects\NCHRP 08-103\DEA-Example-20-depots-2.xml";

            bool ok = true;
            DEAContext context = new DEAContext("DEAContext", "Depot");
            ok = context.AddVariable("STOCK", "I",  out errorMessage);
            if (ok)
                ok = context.AddVariable("WAGES", "I",  out errorMessage);
            if (ok)
                ok = context.AddVariable("ISSUES", "O", 0.0, 1.0, out errorMessage);
            if (ok)
                ok = context.AddVariable("RECEIPTS", "O", 0.0, 1.0, out errorMessage);
            if (ok)
                ok = context.AddVariable("REQS", "O", 0.0, 1.0, out errorMessage);

            if (ok)
            {
                ok = context.UploadCsvFile(csvFilePath, out warningMessage, out errorMessage);
            }

            if (ok)
            {
                ok = context.RunDEA(out errorMessage);

            }

            if (ok)
            {
                ok = context.AddCostConstraint("STOCK", 27.0, out errorMessage);
                if (ok)
                    ok = context.AddCostConstraint("WAGES", 40, out errorMessage);
            }

            if (ok)
            {
                ok = context.ApplyCostConstraintsToProjectSelection(out errorMessage);
            }
            Assert.IsTrue(ok, errorMessage);

            if (ok)
            {
                DataSet ds = context.ToDataSet(out errorMessage);
                Assert.IsNotNull(ds, errorMessage);
                ok = ds != null;
            }

            if (ok)
            {
                ok = context.SaveToXmlFile(xmlFilePath, out errorMessage);
                Assert.IsTrue(ok, errorMessage);
            }

            if (ok)
            {
                DEAContext context2 = DEAContext.CreateFromXmlFile(xmlFilePath, out errorMessage);
                Assert.IsNotNull(context2, errorMessage);
                if (context2 != null)
                {
                    ok = context2.SaveToXmlFile(xmlFilePath2, out errorMessage);
                    Assert.IsTrue(ok, errorMessage);
                }
            }

            if (ok)
            {
                string json = context.ToJsonString(out errorMessage);
                Assert.IsNotNull(json, errorMessage);
            }
        }

        [TestMethod]
        public void TestJsonSerialization()
        {
            string errorMessage = null;
            string xmlInputFilePath = @"C:\Projects\NCHRP 08-103\DEA-Example-20-depots.xml";
            string xmlOutputFilePath = @"C:\Projects\NCHRP 08-103\DEA-Example-20-depots-3.xml";
            DEAContext context = DEAContext.CreateFromXmlFile(xmlInputFilePath, out errorMessage);
            Assert.IsNotNull(context, errorMessage);
            if (context != null)
            {
                string json = context.ToJsonString(out errorMessage);
                Assert.IsNotNull(json, errorMessage);
                if (json != null)
                {
                    DEAContext context2 = DEAContext.CreateFromJsonString(json, out errorMessage);
                    Assert.IsNotNull(context2, errorMessage);
                    if (context2 != null)
                    {
                        bool ok = context2.SaveToXmlFile(xmlOutputFilePath, out errorMessage);
                        Assert.IsTrue(ok, errorMessage);
                    }
                }
            }
        }

        [TestMethod]
        public void TestXmlSerializationInRequestMode()
        {
            string errorMessage = null;
            string warningMessage = null;

            string csvFilePath = @"C:\Projects\NCHRP 08-103\DEA-Example-20-depots.csv";
            string xmlFilePath = @"C:\Projects\NCHRP 08-103\DEA-Example-20-depots-4.xml";
            string xmlFilePath2 = @"C:\Projects\NCHRP 08-103\DEA-Example-20-depots-5.xml";

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
                ok = context.AddVariable("REQS", "O", 1.0, out errorMessage);

            if (ok)
            {
                ok = context.UploadCsvFile(csvFilePath, out warningMessage, out errorMessage);
            }
    
         
            if (ok)
            {
                ok = context.SaveToXmlFile(xmlFilePath, out errorMessage);
                Assert.IsTrue(ok, errorMessage);
            }

            if (ok)
            {
                DEAContext context2 = DEAContext.CreateFromXmlFile(xmlFilePath, out errorMessage);
                Assert.IsNotNull(context2, errorMessage);
                if (context2 != null)
                {
                    ok = context2.SaveToXmlFile(xmlFilePath2, out errorMessage);
                    Assert.IsTrue(ok, errorMessage);
                }
            }
        }

        [TestMethod]
        public void TestCaltransBad()
        {
            string errorMessage = null;
            string warningMessage = null;

            string csvFilePath = @"C:\Projects\NCHRP 08-103\CALTRANS_PROJECTS.csv";
            string xmlFilePath = @"C:\Projects\NCHRP 08-103\CALTRANS_PROJECTS.xml";
            

            DEAContext context = new DEAContext("DEAContext", "Project");
            bool ok = context.AddVariable("Cost", "I", out errorMessage);
            if (ok)
                ok = context.AddVariable("Goal 1", "O", out errorMessage);
            if (ok)
                ok = context.AddVariable("Goal 2", "O", out errorMessage);
            if (ok)
                ok = context.AddVariable("Goal 3", "O", out errorMessage);
            if (ok)
                ok = context.AddVariable("Goal 4", "O", out errorMessage);
            if (ok)
                ok = context.AddVariable("Goal 5", "O", out errorMessage);

            if (ok)
            {
                ok = context.UploadCsvFile(csvFilePath, out warningMessage, out errorMessage);
            }

            if (ok)
            {
                ok = context.RunDEA(out errorMessage);
            }

            if (ok)
            {
                ok = context.SaveToXmlFile(xmlFilePath, out errorMessage);
            }

            Assert.IsTrue(ok, errorMessage);

        }

        [TestMethod]
        public void TestJsonSerializationInRequestMode()
        {
            string errorMessage = null;
            string warningMessage = null;

            string csvFilePath = @"C:\Projects\NCHRP 08-103\DEA-Example-20-depots.csv";
            string xmlFilePath = @"C:\Projects\NCHRP 08-103\DEA-Example-20-depots-6.xml";
           

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
                ok = context.AddVariable("REQS", "O", 1.0, out errorMessage);

            if (ok)
            {
                ok = context.UploadCsvFile(csvFilePath, out warningMessage, out errorMessage);
            }

            if (ok)
            {
                string json = context.ToJsonString(out errorMessage);
                Assert.IsNotNull(json, errorMessage);
                if (json != null)
                {
                    DEAContext context2 = DEAContext.CreateFromJsonString(json, out errorMessage);
                    Assert.IsNotNull(context2, errorMessage);
                    ok = (context2 != null);

                    if (ok)
                    {
                        ok = context2.RunDEA(out errorMessage);

                    }

                    if (ok)
                    {
                        ok = context2.AddCostConstraint("STOCK", 27.0, out errorMessage);
                        if (ok)
                            ok = context2.AddCostConstraint("WAGES", 40, out errorMessage);
                    }

                    if (ok)
                    {
                        ok = context2.ApplyCostConstraintsToProjectSelection(out errorMessage);
                    }
                   

                    if (ok)
                    {
                        ok = context2.SaveToXmlFile(xmlFilePath, out errorMessage);
                    }

                    Assert.IsTrue(ok, errorMessage);
                }
            }

           


        }
    }
}
