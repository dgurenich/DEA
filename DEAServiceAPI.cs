using System;
using Newtonsoft.Json;

/// <summary>
/// Thecore of both request and response JSON-ised classes in DEA is the DEAContext class, the JSON structure of which is as follows:
/// </summary>
public class DEAContext
{
   
    [JsonProperty(PropertyName = "Name", Order = 10)]
    public string Name { get => name; set => name = value; }
   
    [JsonProperty(PropertyName = "Epsilon", Order = 20)]
    public double Epsilon { get => epsilon; set => epsilon = value; }

    [JsonProperty(PropertyName = "NInputs", Order = 30)]
    private int NInputs = 0;

    [JsonProperty(PropertyName = "NOutputs", Order = 40)]
    private int NOutputs = 0;

    [JsonProperty(PropertyName = "Variables", Order = 50)]
    private Dictionary<string, InOutVar> InOutVarDictionary = new Dictionary<string, InOutVar>();

    [JsonProperty(PropertyName = "Constraints", Order = 60)]
    private double[] CostConstraint = null;

    [JsonProperty(PropertyName = "Projects", Order = 70)]
    private List<Project> ProjectList = new List<Project>();

}




