using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace BackendPartialService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface BackendPartialServiceContract
    {

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<string> GetAvailableFilenames();

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "GetFileChunks/{filename}")]
        Stream GetFileChunks(string filename, Stream s);

        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
    [DataContract]
    public class Chunk
    {
        [DataMember]
        public byte[] byteArray { get; set; }
        [DataMember]
        public int chunkNo { get; set; }
        public Chunk(byte[] bytes, int chunkNumber)
        {
            byteArray = bytes;
            chunkNo = chunkNumber;
        }

    }
}
