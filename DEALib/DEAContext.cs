using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.SolverFoundation.Services;
using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Solvers;
using System.Data;
using Newtonsoft.Json;

namespace DEALib
{

    /// <summary>
    /// Data Eneveloped Analysis, project prioritization and project selection occur here
    /// </summary>
    public class DEAContext
    {
        [JsonIgnore]
        private string name = null;                     // Optional name that may be given to the context

        [JsonIgnore]
        private string projectIDFieldName = null;       // Name if the column in the header of the CSV file that contains project names (IDs)

        [JsonIgnore]
        private double epsilon = 10e-6;                 // Small value to be used as the lower bound for the decistion variables in DEA equivalent LP

        [JsonProperty(PropertyName = "Constraints", Order = 60)]
        private double[] CostConstraint = null;

        [JsonIgnore]
        private string CSVFilePath = null;

        [JsonIgnore]
        private int ProjectIDFieldIx = -1;

        [JsonProperty (PropertyName = "Variables", Order = 50)]
        private Dictionary<string, InOutVar> InOutVarDictionary = new Dictionary<string, InOutVar>();

        [JsonProperty(PropertyName = "Projects", Order = 70)]
        private List<Project> ProjectList = new List<Project>();

        [JsonProperty(PropertyName = "NInputs", Order = 30)]
        private int NInputs = 0;

        [JsonProperty(PropertyName = "NOutputs", Order = 40)]
        private int NOutputs = 0;

        [JsonProperty (PropertyName = "Name", Order = 10)]
        public string Name { get => name; set => name = value; }

        [JsonIgnore]
        public string ProjectIDFieldName { get => projectIDFieldName; set => projectIDFieldName = value; }

        [JsonProperty(PropertyName = "Epsilon", Order = 20)]
        public double Epsilon { get => epsilon; set => epsilon = value; }

        [JsonIgnore]
        public bool ConstraintsSet
        {
            get
            {
                if (CostConstraint != null)
                {
                    return true;
                }
                return false;
            }
        }

        public DEAContext()
        {
            Name = "DEAContext";
            ProjectIDFieldName = "Project";
        }

        public DEAContext(string contextName, string projectIdFieldName)
        {
            Name = contextName;
            ProjectIDFieldName = projectIdFieldName;
        }

        /// <summary>
        /// Adds variables to the context
        /// </summary>
        /// <param name="name">Variable's name, must be unique, case-sensitive</param>
        /// <param name="typ">"I" for input Cost), "O" for output (Benefit)</param>
        /// <param name="scale">Scaling fator to be applied to the value obtained from the CSV file. Usually 1.0</param>
        /// <param name="errorMessage">Error message, null if none</param>
        /// <returns>True on success, False on failure</returns>
        public bool AddVariable(string name, string typ, double scale, out string errorMessage)
        {
            bool ok = true;
            errorMessage = null;

            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new Exception("The name of the input or output variable may not be null or empty.");
                }
                if (InOutVarDictionary.ContainsKey(name))
                {
                    throw new Exception("Duplicate variable name: '" + name + "'");
                }
                if (string.IsNullOrEmpty(typ))
                {
                    throw new Exception("The type for the variable '" + name + "' is missing");
                }
                switch (typ.ToUpper())
                {
                    case "I":
                        InOutVar v = new InOutVar();
                        v.VarType = "I";
                        v.Index = NInputs++;
                        v.CsvPosIx = -1;    // To be resolved later
                        v.Scale = scale;
                        InOutVarDictionary.Add(name, v);
                        break;

                    case "O":
                        InOutVar u = new InOutVar();
                        u.VarType = "O";
                        u.Index = NOutputs++;
                        u.CsvPosIx = -1;    // To be resolved later
                        u.Scale = scale;
                        InOutVarDictionary.Add(name, u);
                        break;

                    default:
                        throw new Exception("The type of the variable '" + name + "' is specified as '" + typ
                            + "'.  The valid entries for the type are I<nput> and O<output>");
                }
            }
            catch (Exception ex)
            {
                ok = false;
                errorMessage = ex.Message;
            }
            return ok;
        }

        protected bool AddProject(Project proj, out string errorMessage)
        {
            bool ok = true;
            errorMessage = null;

            try
            {
                ProjectList.Add(proj);
            }
            catch (Exception ex)
            {
                ok = false;
                errorMessage = ex.Message;
            }

            return (ok);
        }

        protected bool ParseCSVHeader(string header, out int numErrors, out int numWarnings, out string errorMessage, out string warningMessage)
        {
            bool ok = true;
            errorMessage = null;
            warningMessage = null;
            numErrors = 0;
            numWarnings = 0;

            StringBuilder sbError = new StringBuilder();
            StringBuilder sbWarning = new StringBuilder();

            try
            {
                string[] fieldNames = header.Split(new char[] { ',' });
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    string name = fieldNames[i];
                    if (string.Compare(name, ProjectIDFieldName, true) == 0)
                    {
                        ProjectIDFieldIx = i;
                    }
                    else if (InOutVarDictionary.ContainsKey(name))
                    {
                        InOutVarDictionary[name].CsvPosIx = i;
                    }
                    else
                    {
                        sbWarning.Append(string.Format(@"Field {0} is not registered among the input or output variables.  
  Neither it is a name for the project ID column.<br/>", name));
                        numWarnings++;
                    }
                }

                if (ProjectIDFieldIx < 0)
                {
                    sbError.Append(string.Format("Column not found in the CSV header for the project ID field '{0}'. <br/>", ProjectIDFieldName));
                    ok = false;
                    numErrors++;
                }

                foreach (string key in InOutVarDictionary.Keys)
                {
                    if (InOutVarDictionary[key].CsvPosIx < 0)
                    {
                        sbError.Append(string.Format("Column not found in the CSV header for the variable '{0}'. <br/>", key));
                        ok = false;
                        numErrors++;
                    }
                }

                if (!ok)
                {
                    errorMessage = sbError.ToString();
                }

                if (sbWarning.Length > 0)
                {
                    warningMessage = sbWarning.ToString();
                }
            }
            catch (Exception ex)
            {
                ok = false;
                errorMessage = ex.Message;
            }

            return (ok);
        }


        protected bool ParseLine(string line, int i, out string errorMessage)
        {
            bool ok = true;
            errorMessage = null;

            double[] input = new double[NInputs];
            double[] output = new double[NOutputs];
            string projName = null;

            try
            {
                string[] token = line.Split(new char[] { ',' });
                if (0 <= ProjectIDFieldIx && ProjectIDFieldIx < token.Length)
                {
                    projName = token[ProjectIDFieldIx];
                }
                else
                {
                    errorMessage = string.Format("Unable to parse the CSV line # {0} [{1}], Project ID field (index = {2}) not found.",
                        i, line, ProjectIDFieldIx);
                    ok = false;
                }
                if (ok)
                {
                    foreach (string varName in InOutVarDictionary.Keys)
                    {
                        InOutVar var = InOutVarDictionary[varName];
                        string t = token[var.CsvPosIx];
                        double d;
                        if (!double.TryParse(t, out d))
                        {
                            errorMessage = string.Format("Unable to parse the CSV line # {0} [{1}], Illegal value of '{2}' found for the variable {3}.",
                                i, line, t, varName);
                            ok = false;
                            break;
                        }
                        if (var.VarType == "I")
                        {
                            input[var.Index.Value] = d * var.Scale;
                        }
                        else
                        {
                            output[var.Index.Value] = d * var.Scale;
                        }
                    }

                    if (ok)
                    {
                        Project project = new Project(projName, input, output);
                        ok = AddProject(project, out errorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                ok = false;
            }

            return ok;
        }

        /// <summary>
        /// Uploads projects from a CSV file.
        /// </summary>
        /// <param name="csvFilePath">Fully qualified path name of the CSV file</param>
        /// <param name="warningMessage">out warnings, null if none</param>
        /// <param name="errorMessage">out errors, null if none</param>
        /// <returns>true on success, false on faulue, when ok=true the warningMessage may still contain messages.  Worth checking.</returns>
        public bool UploadCsvFile(string csvFilePath, out string warningMessage, out string errorMessage)
        {
            bool ok = true;
            errorMessage = null;
            warningMessage = null;

            try
            {
                string[] Line = File.ReadAllLines(csvFilePath);
                int numErrors = 0;
                int numWarnings = 0;
                ok = ParseCSVHeader(Line[0], out numErrors, out numWarnings, out errorMessage, out warningMessage);
                for (int i = 1; ok && i < Line.Length; i++)
                {
                    string csv = Line[i];
                    ok = ParseLine(csv, i, out errorMessage);
                }
            }
            catch (Exception ex)
            {
                ok = false;
                errorMessage = ex.Message;
            }

            if (ok)
            {
                CSVFilePath = csvFilePath;
            }
            return ok;
        }

     
        /// <summary>
        /// Runs Data Enveloped Analysis
        /// </summary>
        /// <param name="errorMessage">out error message, null if no errors</param>
        /// <returns>True on success, False on failure</returns>
        public bool RunDEA(out string errorMessage)
        {
            bool ok = true;
            errorMessage = null;

            try
            {
                foreach (Project p in ProjectList)
                {
                    ok = SolveModelForProjectAsLP(p, out errorMessage);
                    if (!ok)
                    {
                        break;
                    }
                }

                if (ok)
                {
                    ProjectList.Sort();
                }
            }
            catch (Exception ex)
            {
                ok = false;
                errorMessage = ex.Message;
            }

            return (ok);
        }


        private bool SolveModelForProjectAsLP(Project project, out string errorMessage)
        {
            bool ok = true;
            errorMessage = null;

            try
            {
                SimplexSolver solver = new SimplexSolver();
                int[] var = new int[InOutVarDictionary.Count];

                int iVar = 0;
                int[] varEps = new int[InOutVarDictionary.Count];

                foreach (string varName in InOutVarDictionary.Keys)
                {
                    InOutVar v = InOutVarDictionary[varName];
                    solver.AddVariable(varName, out var[iVar]);
                    solver.SetBounds(var[iVar], 0, Rational.PositiveInfinity);
                    v.VarIndex = iVar++;
                }


                int pVar = 0;
                int[] uConst = new int[ProjectList.Count];
                foreach (Project p in ProjectList)
                {
                    solver.AddRow(p.Name, out uConst[pVar]);
                    foreach (string varName in InOutVarDictionary.Keys)
                    {
                        InOutVar v = InOutVarDictionary[varName];
                        if (v.VarType == "O")
                        {
                            solver.SetCoefficient(uConst[pVar], var[v.VarIndex.Value], p.Benefit[v.Index.Value]);
                        }
                        else
                        {
                            solver.SetCoefficient(uConst[pVar], var[v.VarIndex.Value], -p.Cost[v.Index.Value]);
                        }
                    }
                    solver.SetUpperBound(uConst[pVar], 0.0);
                    pVar++;
                }

                int vConst = 0;
                solver.AddRow("SumV", out vConst);

                int vObjective = 0;
                solver.AddRow("Objective", out vObjective);

                foreach (string varName in InOutVarDictionary.Keys)
                {
                    InOutVar v = InOutVarDictionary[varName];
                    if (v.VarType == "O")
                    {
                        solver.SetCoefficient(vConst, var[v.VarIndex.Value], 0);
                        solver.SetCoefficient(vObjective, var[v.VarIndex.Value], project.Benefit[v.Index.Value]);
                    }
                    else
                    {
                        solver.SetCoefficient(vConst, var[v.VarIndex.Value], project.Cost[v.Index.Value]);
                        solver.SetCoefficient(vObjective, var[v.VarIndex.Value], 0.0);
                    }
                }
                solver.SetBounds(vConst, 100.0, 100.0);

                foreach (string varName in InOutVarDictionary.Keys)
                {
                    InOutVar v = InOutVarDictionary[varName];
                    solver.AddRow("Eps-" + v.VarIndex.ToString(), out varEps[v.VarIndex.Value]);
                    solver.SetCoefficient(varEps[v.VarIndex.Value], v.VarIndex.Value, 1);
                    solver.SetLowerBound(varEps[v.VarIndex.Value], epsilon);
                }

                solver.AddGoal(vObjective, 1, false);
                solver.Solve(new SimplexSolverParams());

                if (solver.SolutionQuality == LinearSolutionQuality.None)
                {
                    errorMessage = "LP solution could not be obtained for " + project.Name + ".  " + solver.ToString();
                    ok = false;
                }

                if (ok)
                {
                    double Nominator = 0.0;
                    double Denominator = 0.0;


                    foreach (string varName in InOutVarDictionary.Keys)
                    {
                        InOutVar v = InOutVarDictionary[varName];
                        double d = solver.GetValue(var[v.VarIndex.Value]).ToDouble();

                        if (v.VarType == "I")
                        {
                            project.V[v.Index.Value] = d;
                            Denominator += d * project.Cost[v.Index.Value];
                        }
                        else
                        {
                            project.U[v.Index.Value] = d;
                            Nominator += d * project.Benefit[v.Index.Value];
                        }
                    }

                    project.R = Nominator / Denominator;
                }

            }
            catch (Exception ex)
            {
                ok = false;
                errorMessage = ex.Message;
            }

            return (ok);
        }

        /// <summary>
        /// Adds upper bound for the input variable associated with a cost
        /// </summary>
        /// <param name="costName">Name of the cost variable.  Must be the same as the one used in the AddVariable method</param>
        /// <param name="boundary">Upper bound for the cost</param>
        /// <param name="errorMessage">error message, null if no errors</param>
        /// <returns>true on success, false on failure</returns>
        public bool AddCostConstraint(string costName, double boundary, out string errorMessage)
        {
            bool ok = true;
            errorMessage = null;

            try
            {
                if (NInputs <= 0)
                {
                    throw new Exception("No cost (input) variables have been defined.");
                }

                if (CostConstraint == null)
                {
                    CostConstraint = new double[NInputs];
                    for (int i=0; i<NInputs; i++)
                    {
                        CostConstraint[i] = double.MaxValue;    // For costs that do not have constraints set explicitly no constraints are assumed.
                    }
                }

                if (!InOutVarDictionary.ContainsKey(costName))
                {
                    throw new Exception(string.Format("Cost input {0} has not been registered among the variables.", costName));
                }

                InOutVar v = InOutVarDictionary[costName];
                if (v.VarType != "I")
                {
                    throw new Exception(string.Format("Identifier {0} is associated with a benefit (output) metric, not with a cost (input)", costName));
                }

                if (v.Index.Value <0 || v.Index.Value >= NInputs)
                {
                    throw new Exception(string.Format("Cost (input) index {0} of associated with {1} is out of range [0 - {2}]", v.Index, costName, NInputs-1));
                }

                CostConstraint[v.Index.Value] = boundary;
            }
            catch(Exception ex)
            {
                CostConstraint = null;
                errorMessage = ex.Message;
                ok = false;
            }


            return (ok);
        }


        /// <summary>
        /// Applies project selection algorithm to the projects using the given constraints
        /// </summary>
        /// <param name="errorMessage">Error message, null if no errors</param>
        /// <returns>True on success, False on failure</returns>
        public bool ApplyCostConstraintsToProjectSelection(out string errorMessage)
        {
            bool ok = true;
            errorMessage = null;

            try
            {
                if (CostConstraint == null)
                {
                    throw new Exception("Cost constraints have not been initialized.");
                }

                SimplexSolver solver = new SimplexSolver();
                int[] selVar = new int[ProjectList.Count];
                int pCount = 0;
                foreach(Project p in ProjectList)
                {
                    solver.AddVariable("Sel-" + pCount.ToString(), out selVar[pCount]);
                    solver.SetBounds(selVar[pCount], 0, 1);
                    pCount++;
                }

                int[] cConst = new int[NInputs];
                for (int i= 0; i < NInputs; i++)
                {
                    solver.AddRow("Cost-" + i.ToString(), out cConst[i]);
                    for (int j = 0; j < ProjectList.Count; j++)
                    {
                        solver.SetCoefficient(cConst[i], selVar[j], ProjectList[j].Cost[i]);
                    }
                    solver.SetUpperBound(cConst[i], CostConstraint[i]);
                }

                int vObjective;
                solver.AddRow("Objective", out vObjective);
                for (int j = 0; j < ProjectList.Count; j++)
                {
                    solver.SetCoefficient(vObjective, selVar[j], ProjectList[j].R.Value);
                }

                solver.AddGoal(vObjective, 1, false);

                SimplexSolverParams param = new SimplexSolverParams();
                param.MixedIntegerGenerateCuts = true;
                solver.Solve(param);

                if (solver.SolutionQuality != LinearSolutionQuality.Exact)
                {
                    throw new Exception("Solver failed to find the exact solution for the cost-effective project selection problem.");
                }

                for (int i = 0; i < ProjectList.Count; i++)
                {
                    ProjectList[i].Sel = solver.GetValue(selVar[i]).IsOne;
                }
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                ok = false;
            }

            return (ok);
        }

        /// <summary>
        /// Returns itself as a data set object (collection of tables)
        /// </summary>
        /// <param name="errorMessage">out Error message, null if no erros</param>
        /// <returns>Created data set, null if error</returns>
        public DataSet ToDataSet(out string errorMessage)
        {
            DataSet ds = null;
            errorMessage = null;

            try
            {
                ds = new DataSet(Name);

                // Scalars
                DataTable dtParam = new DataTable("PARAMETERS");
                dtParam.Columns.Add("NAME", typeof(string));
                dtParam.Columns.Add("VALUE", typeof(double));
                DataRow r = dtParam.NewRow();
                r["NAME"] = "EPSILON";
                r["VALUE"] = epsilon;
                dtParam.Rows.Add(r);
                r = dtParam.NewRow();
                r["NAME"] = "NINPUTS";
                r["VALUE"] = NInputs;
                dtParam.Rows.Add(r);
                r = dtParam.NewRow();
                r["NAME"] = "NOUTPUTS";
                r["VALUE"] = NOutputs;
                dtParam.Rows.Add(r);
                ds.Tables.Add(dtParam);

                // Variables
                DataTable dtVars = new DataTable("VARIABLES");
                dtVars.Columns.Add("NAME", typeof(string));
                dtVars.Columns.Add("TYPE", typeof(string));
                dtVars.Columns.Add("SCALE", typeof(double));
                dtVars.Columns.Add("VARIX", typeof(int));
                dtVars.Columns.Add("INDEX", typeof(int));
                foreach (string varName in InOutVarDictionary.Keys)
                {
                    InOutVar v = InOutVarDictionary[varName];
                    r = dtVars.NewRow();
                    r["NAME"] = varName;
                    r["TYPE"] = v.VarType;
                    r["SCALE"] = v.Scale;
                    if (v.VarIndex.HasValue)
                    {
                        r["VARIX"] = v.VarIndex.Value;
                    }
                    else
                    {
                        r["VARIX"] = DBNull.Value;
                    }
                    if (v.Index.HasValue)
                    {
                        r["INDEX"] = v.Index.Value;
                    }
                    else
                    {
                        r["INDEX"] = DBNull.Value;
                    }
                    dtVars.Rows.Add(r);
                }
                ds.Tables.Add(dtVars);

                // Constraints
                DataTable dtConst = new DataTable("CONSTRAINTS");
                dtConst.Columns.Add("NAME", typeof(string));
                dtConst.Columns.Add("BOUND", typeof(double));
                if (CostConstraint != null && CostConstraint.Length == NInputs)
                {
                    foreach (string varName in InOutVarDictionary.Keys)
                    {
                        InOutVar v = InOutVarDictionary[varName];
                        if (v.VarType == "I")
                        {
                            r = dtConst.NewRow();
                            r["NAME"] = varName;
                            r["BOUND"] = CostConstraint[v.Index.Value];
                            dtConst.Rows.Add(r);
                        }
                    }
                }
                ds.Tables.Add(dtConst);

                // Projects
                DataTable dtProject = new DataTable("PROJECTS");
                dtProject.Columns.Add("NAME", typeof(string));
                foreach (string varName in InOutVarDictionary.Keys)
                {
                    InOutVar v = InOutVarDictionary[varName];
                    dtProject.Columns.Add(varName, typeof(double));
                    if (v.VarType == "I")
                    {
                        dtProject.Columns.Add("V-" + varName, typeof(double));
                    }
                    else
                    {
                        dtProject.Columns.Add("U-" + varName, typeof(double));
                    }
                }
                dtProject.Columns.Add("RELATIVE_EFFICIENCY", typeof(double));
                dtProject.Columns.Add("SELECTED", typeof(bool));
                foreach(Project p in ProjectList)
                {
                    r = dtProject.NewRow();
                    r["NAME"] = p.Name;
                    foreach (string varName in InOutVarDictionary.Keys)
                    {
                        InOutVar v = InOutVarDictionary[varName];
                        if (v.Index.HasValue)
                        {
                            if (v.VarType == "I")
                            {
                                r[varName] = p.Cost[v.Index.Value];
                                r["V-" + varName] = p.V[v.Index.Value];
                            }
                            else
                            {
                                r[varName] = p.Benefit[v.Index.Value];
                                r["U-" + varName] = p.U[v.Index.Value];
                            }
                        }
                        else
                        {
                            if (v.VarType == "I")
                            {
                                r[varName] = DBNull.Value;
                                r["V-" + varName] = DBNull.Value;
                            }
                            else
                            {
                                r[varName] = DBNull.Value;
                                r["U-" + varName] =DBNull.Value;
                            }
                        }
                    }
                    if (p.R.HasValue)
                    {
                        r["RELATIVE_EFFICIENCY"] = p.R;
                    }
                    else
                    {
                        r["RELATIVE_EFFICIENCY"] = DBNull.Value;
                    }

                    if (p.Sel.HasValue)
                    {
                        r["SELECTED"] = p.Sel.Value;
                    }
                    else
                    {
                        r["SELECTED"] = DBNull.Value;
                    }
                    dtProject.Rows.Add(r);
                }
                ds.Tables.Add(dtProject);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                ds = null;
            }

            return ds;
        }

        /// <summary>
        /// Saves context object as an XML file converting it the dataset
        /// </summary>
        /// <param name="xmlFilePath">Fully qualified pathname for the XML file</param>
        /// <param name="errorMessage">out Error message, null if no errors</param>
        /// <returns>True on success, False on failure</returns>
        public bool SaveToXmlFile(string xmlFilePath, out string errorMessage)
        {
            bool ok = true;
            errorMessage = null;

            try
            {
                DataSet ds = ToDataSet(out errorMessage);
                ok = (ds != null);
                if (ok)
                {
                    ds.WriteXml(xmlFilePath, XmlWriteMode.WriteSchema);
                }
            }
            catch(Exception ex)
            {
                ok = false;
                errorMessage = ex.Message;
            }

            return (ok);
        }
        
        /// <summary>
        /// Static method that creates and returns a new DEAContext object populating it from a dataset supplied as the argument
        /// </summary>
        /// <param name="ds">DataSet object to populate from</param>
        /// <param name="errorMessage">out Error message, null if no erros</param>
        /// <returns>New DEAContext object, null if error</returns>
        public static DEAContext CreateFromDataSet(DataSet ds, out string errorMessage)
        {
            errorMessage = null;
            DEAContext context = null;
            
            try
            {
                context = new DEAContext();
                context.Name = ds.DataSetName;
                DataTable dt = ds.Tables["PARAMETERS"];
                foreach(DataRow r in dt.Rows)
                {
                    switch(r["NAME"].ToString())
                    {
                        case "EPSILON":
                            context.Epsilon = Convert.ToDouble(r["VALUE"]);
                            break;
                        case "NINPUTS":
                            context.NInputs = Convert.ToInt32(r["VALUE"]);
                            break;
                        case "NOUTPUTS":
                            context.NOutputs = Convert.ToInt32(r["VALUE"]);
                            break;
                    }
                }

                dt = ds.Tables["VARIABLES"];
                foreach(DataRow r in dt.Rows)
                {
                    InOutVar v = new InOutVar();
                    string varName = r["NAME"].ToString();
                    v.VarType = r["TYPE"].ToString();
                    v.Scale = Convert.ToDouble(r["SCALE"]);
                    if (r["VARIX"] != null && r["VARIX"] != DBNull.Value) {
                        v.VarIndex = Convert.ToInt32(r["VARIX"]);
                    }
                    else
                    {
                        v.VarIndex = null;
                    }

                    if (r["INDEX"] != null && r["INDEX"] != DBNull.Value)
                    {
                        v.Index = Convert.ToInt32(r["INDEX"]);
                    }
                    else
                    {
                        v.Index = null;
                    }
                    context.InOutVarDictionary.Add(varName, v);
                }

                dt = ds.Tables["CONSTRAINTS"];
                if (dt.Rows.Count > 0)
                {
                    foreach(DataRow r in dt.Rows)
                    {
                        string varName = r["NAME"].ToString();
                        double value = Convert.ToDouble(r["BOUND"]);
                        bool ok = context.AddCostConstraint(varName, value, out errorMessage);
                        if (!ok)
                        {
                            throw new Exception(errorMessage);
                        }
                    }
                }

                dt = ds.Tables["PROJECTS"];
                foreach (DataRow r in dt.Rows)
                {
                    Project p = new Project();
                    if (context.NInputs > 0)
                    {
                        p.Cost = new double[context.NInputs];
                        p.V = new double[context.NInputs];
                    }

                    if (context.NInputs > 0) {
                        p.Benefit = new double[context.NOutputs];
                        p.U = new double[context.NOutputs];
                    }

                    p.Name = r["NAME"].ToString();
                    foreach(string varName in context.InOutVarDictionary.Keys)
                    {
                        InOutVar v = context.InOutVarDictionary[varName];
                        if (v.Index.HasValue)
                        {
                            if (v.VarType == "I")
                            {
                                p.Cost[v.Index.Value] = Convert.ToDouble(r[varName]);
                                p.V[v.Index.Value] = Convert.ToDouble(r["V-" + varName]);
                            }
                            else
                            {
                                p.Benefit[v.Index.Value] = Convert.ToDouble(r[varName]);
                                p.U[v.Index.Value] = Convert.ToDouble(r["U-" + varName]);
                            }
                        }
                    }

                    if (r["RELATIVE_EFFICIENCY"] != null && r["RELATIVE_EFFICIENCY"] != DBNull.Value)
                    {
                        p.R = Convert.ToDouble(r["RELATIVE_EFFICIENCY"]);
                    }
                    else
                    {
                        p.R = null;
                    }

                    if (!string.IsNullOrEmpty(r["SELECTED"].ToString()))
                    {
                        p.Sel = Convert.ToBoolean(r["SELECTED"]);
                    }
                    else
                    {
                        p.Sel = null;
                    }
                    context.ProjectList.Add(p);
                }
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                context = null;
            }

            return context;
        }

        /// <summary>
        /// Static method that creates a new DEAContext object initializing it from the XML file
        /// </summary>
        /// <param name="xmlFileName">Fully qualified path name to of the XML file that must exist</param>
        /// <param name="errorMessage">out Error message, null if no error</param>
        /// <returns>Newly created DEAContext object, null if there were errors</returns>
        public static DEAContext CreateFromXmlFile(string xmlFileName, out string errorMessage)
        {
            errorMessage = null;
            DEAContext context = null;

            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(xmlFileName, XmlReadMode.ReadSchema);
                context = DEAContext.CreateFromDataSet(ds, out errorMessage);
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                context = null;
            }

            return context;
        }

        /// <summary>
        /// Generates a JSON string from the object
        /// </summary>
        /// <param name="errorMessage">out Error message, null if no errors.</param>
        /// <returns>JSON string, null on error</returns>
        public string ToJsonString(out string errorMessage)
        {
            errorMessage = null;
            string json = null;

            try
            {
                json = JsonConvert.SerializeObject(this, Formatting.Indented);
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                json = null;

            }
            return json;
        }

        /// <summary>
        /// Static method that creates a new DEAContext object from a JSON string
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <param name="errorMessage">out Error message, null if no errors</param>
        /// <returns>Newly created DEAContext object or null if there were any errors</returns>
        public static DEAContext CreateFromJsonString(string json, out string errorMessage)
        {
            errorMessage = null;
            DEAContext context = null;

            try
            {
                context = JsonConvert.DeserializeObject<DEAContext>(json);
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                context = null;
            }
            return context;
        }
    }
}
