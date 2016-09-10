using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace PartialCacheService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
       
        [OperationContract]
        [WebGet(
            UriTemplate = "DownloadFile/{filename}",
            ResponseFormat = WebMessageFormat.Json)]
        Stream DownloadFile(string filename);

       // [OperationContract]
       // [WebGet]

       // CompositeType GetDataUsingDataContract(CompositeType composite);

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
        public int chunkNo { get; set; }
        public Chunk(byte[] bytes, int chunkNumber)
        {
            byteArray = bytes;
            chunkNo = chunkNumber;
        }

    }
}
