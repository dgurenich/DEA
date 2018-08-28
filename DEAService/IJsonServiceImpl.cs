using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;

namespace DEAService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IJsonServiceImpl" in both code and config file together.
    [ServiceContract]
    public interface IJsonServiceImpl
    {
          
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "version")]
        Stream GetVersion();

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "json/DEA")]
        Stream RunDEAnalysis(Stream request);
   
    }
}
