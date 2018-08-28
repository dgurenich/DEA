
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DEALib;
using System.Data;
namespace DEAUnitTest
{
    [TestClass]
    public class VA_test
    {
        [TestMethod]
        public void TestVADataUnconstrained()
        {

            string errorMessage = null;
            string warningMessage = null;

            string csvFilePath = @"C:\Projects\NCHRP 08-103\VA_TEST.csv";
            string xmlFilePath = @"C:\Projects\NCHRP 08-103\VA_TEST.xml";
            string xmlFilePath2 = @"C:\Projects\NCHRP 08-103\VA_TEST-2.xml";

            bool ok = true;
            DEAContext context = new DEAContext("DEAContext", "Project");
            ok = context.AddVariable("Cost", "I", 1.0e-6, out errorMessage);
            if (ok)
                ok = context.AddVariable("Congestion", "O",  out errorMessage);
            if (ok)
                ok = context.AddVariable("Safety", "O", out errorMessage);
            if (ok)
                ok = context.AddVariable("Accessibility", "O",  out errorMessage);
            if (ok)
                ok = context.AddVariable("Environmental Quality", "O", out errorMessage);
            if (ok)
                ok = context.AddVariable("Economic Development", "O", out errorMessage);
            if (ok)
                ok = context.AddVariable("Land Use", "O", out errorMessage);
            if (ok)
            {
                ok = context.UploadCsvFile(csvFilePath, out warningMessage, out errorMessage);
            }

            if (ok)
            {
                ok = context.RunDEA(out errorMessage);

            }
            /*
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
            */
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
        public void TestVACsvDataUnconstrained()
        {

            string errorMessage = null;
            string warningMessage = null;

            string csvFilePath = @"C:\Projects\NCHRP 08-103\VA_TEST_PROJECTS.csv";
            string csvConstFilePath = @"C:\Projects\NCHRP 08-103\VA_TEST_CONSTRAINTS.csv";
            string csvVarFilePath = @"C:\Projects\NCHRP 08-103\VA_TEST_VARIABLES.csv";
            string xmlFilePath = @"C:\Projects\NCHRP 08-103\VA_TEST-3.xml";
            string xmlFilePath2 = @"C:\Projects\NCHRP 08-103\VA_TEST-4.xml";

            bool ok = true;
            DEAContext context = new DEAContext("DEAContext", "Project");
            ok = context.LoadVariablesFromCsvFile(csvVarFilePath, out errorMessage);
            if (ok)
            {
                ok = context.UploadCsvFile(csvFilePath, out warningMessage, out errorMessage);
            }

            if (ok)
            {
                ok = context.LoadCsvConstraints(csvConstFilePath, out errorMessage);
            }
            if (ok)
            {
                ok = context.RunDEA(out errorMessage);

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
    }

   
}

