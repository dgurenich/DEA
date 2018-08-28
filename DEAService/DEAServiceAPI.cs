using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;


/// <summary>
/// The DEA srevice has two operational contracts:
/// Version (method GET) that returns the version signature of the service's DLL, and
/// Json/DEA (method POST) that posts the JSON string containing the problem's formulation and retuns JSON string of its solution
/// </summary>

[OperationContract]
[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "version")]
Stream GetVersion();

[OperationContract]
[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "json/DEA")]
Stream RunDEAnalysis(Stream request);


/// <summary>
/// The core of both request and response JSON-ised classes in DEA is the DEAContext class.
/// It use two satellite classes: InOutVar and Projects.
/// The JSON structure of the class is shown below.
/// </summary>
public class DEAContext
{
    // Name of the scenario (default": "DEAContext")
    [JsonProperty(PropertyName = "Name", Order = 10)]
    public string Name { get => name; set => name = value; }
    private string name = "DEAContext";
   
    // Small positive number, used by the LP model.  Default = 1.0e-18
    [JsonProperty(PropertyName = "Epsilon", Order = 20)]
    public double Epsilon { get => epsilon; set => epsilon = value; }
    private double epsilon = 1.0e-18;

    // Number of input variables (costs, resouces, etc.)
    [JsonProperty(PropertyName = "NInputs", Order = 30)]
    private int NInputs = 0;

    // Number of output variables (goals, benefits, etc.)
    [JsonProperty(PropertyName = "NOutputs", Order = 40)]
    private int NOutputs = 0;

    // Dictionary of variables, key is variable's name, value - see the definition of the InOutVar class
    [JsonProperty(PropertyName = "Variables", Order = 50)]
    private Dictionary<string, InOutVar> InOutVarDictionary = new Dictionary<string, InOutVar>();

    // Array of constraints. The size of the array must match the value of NInputs.  May be null if only project ranking is required.
    [JsonProperty(PropertyName = "Constraints", Order = 60)]
    private double[] CostConstraint = null;

    // List of projects (see the definition of the Project class)
    [JsonProperty(PropertyName = "Projects", Order = 70)]
    private List<Project> ProjectList = new List<Project>();
}

public class InOutVar
{
    // Must be specified "I" - input (cost, resource), "O" - output (benefit, goal).
    [JsonProperty(PropertyName = "Type", Order = 10)]
    public string VarType;      
    
    // Scaling factor to be applied; e.g., if the costs are in millions and benefits in fractions of 1 then it makes sense
    // to set Scale to 0.000001.
    // Default is 1.0;
    [JsonProperty(PropertyName = "Scale", Order = 20, NullValueHandling = NullValueHandling.Ignore)]
    public double Scale = 1.0;  // Multiplier to apply to the entry from the CSV file

    /// <summary>
    /// The following four members are used only in the response and only for testing.
    /// They do not have to be set in the request JSON (i.e. left to be nulls, and can be ignored in  the response if present).
    /// </summary>
    [JsonProperty(PropertyName = "MinWeightApplied", Order = 22, NullValueHandling = NullValueHandling.Ignore)]
    public double? MinWeightApplied = null;

    [JsonProperty(PropertyName = "MaxWeightApplied", Order = 24, NullValueHandling = NullValueHandling.Ignore)]
    public double? MaxWeightApplied = null;

    [JsonProperty(PropertyName = "MinWeightActual", Order = 42, NullValueHandling = NullValueHandling.Ignore)]
    public double? MinWeightActual = null;

    [JsonProperty(PropertyName = "MaxWeightActual", Order = 44, NullValueHandling = NullValueHandling.Ignore)]
    public double? MaxWeightActual = null;
}

public class Project
{ 
    // Unique project name or ID.  Must be specified
    [JsonProperty(PropertyName = "Name", Order = 10, NullValueHandling = NullValueHandling.Ignore)]
    public string Name;         

    // Array of costs (resource) values associated with the project
    [JsonProperty(PropertyName = "Cost", Order = 20, NullValueHandling = NullValueHandling.Ignore)]
    public double[] Cost;

    // Array of benefit (resource) values associated with the project
    [JsonProperty(PropertyName = "Benefit", Order = 30, NullValueHandling = NullValueHandling.Ignore)]
    public double[] Benefit;

    // Coefficients obtained for the benefit variables (only in the response; null in request)
    [JsonProperty(PropertyName = "U", Order = 40, NullValueHandling = NullValueHandling.Ignore)]
    public double[] U;

    // Coefficients obtained for the  cost variables (only in the response; null in request)
    [JsonProperty(PropertyName = "V", Order = 50, NullValueHandling = NullValueHandling.Ignore)]
    public double[] V;

    // Relative efficiency of the project (onlyh in the response; null in request)
    [JsonProperty(PropertyName = "RelativeEfficiency", Order = 60, NullValueHandling = NullValueHandling.Ignore)]
    public double? R;   

    // True if the project has been selected in the constrained scenario.  False - if not selected
    // Null if constraints were not applied.  Used only in the response. 
    [JsonProperty(PropertyName = "Selected", Order = 70, NullValueHandling = NullValueHandling.Ignore)]
    public bool? Sel;   // True if project has been selected under constraints, False - if not, null - if cosntrais have never been applied

    // Used only in the response. '*' if the LP model for the project could vbe obtained in only approximate form (may happen in special situations)
    // '\0' otherwise.
    [JsonProperty(PropertyName = "Approximate", Order = 80, NullValueHandling = NullValueHandling.Ignore)]
    public char Approx;
}



/// <summary>
/// The response object includes the DEAContext class in its response form plus two more members:
/// OK and ErrorMessage
/// </summary>
public class DEAResponse
{
    // True if DEA successed, False otherwise
    [JsonProperty(PropertyName = "OK", Order = 10)]
    public bool OK { get; set; }

    // Error message if OK=False, null otherwise
    [JsonProperty(PropertyName = "ErrorMessage", Order = 20, NullValueHandling = NullValueHandling.Ignore)]
    public string errorMessage { get; set; }

    // DEAContext object will include everything from the request object plus the values of Us and Vs, RelativeEfficience, Selected and Approximate
    // indicators for the projects.  The project list in the response object comes out descendingly sorted by relative effciciency.
    [JsonProperty(PropertyName = "Context", Order = 30, NullValueHandling = NullValueHandling.Ignore)]
    public DEAContext context { get; set; }
}

