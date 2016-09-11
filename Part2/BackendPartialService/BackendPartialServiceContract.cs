using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;

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
            UriTemplate = "GetFileChunks/{filename}",
            ResponseFormat = WebMessageFormat.Xml,
            RequestFormat = WebMessageFormat.Xml)]
        //Stream GetFileChunks(string filename, Stream s);
        string GetFileChunks(string filename, Stream s);
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
    //[Serializable]
    [KnownType(typeof(Chunk))]
    [DataContract]
    public class Chunk
    {
        [DataMember]
        public Dictionary<string, byte[]> chunks { get; set; }
        //public byte[] byteArray { get; set; }
        
       // public int chunkNo { get; set; }
       
        public Chunk(Dictionary<string, byte[]> chunks)
        {
            this.chunks = chunks;
        }

    }
    public class KeyValuePairJsonConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> deserializedJSObjectDictionary, Type targetType, JavaScriptSerializer javaScriptSerializer)
        {
            Object targetTypeInstance = Activator.CreateInstance(targetType);

            FieldInfo[] targetTypeFields = targetType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo fieldInfo in targetTypeFields)
                fieldInfo.SetValue(targetTypeInstance, deserializedJSObjectDictionary[fieldInfo.Name]);

            return targetTypeInstance;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            Dictionary<string, byte[]> dictionaryInput = obj as Dictionary<string, byte[]>;

            if (dictionaryInput == null)
            {
                throw new InvalidOperationException("Object must be of Dictionary<string, MyClass> type.");
            }

            foreach (KeyValuePair<string, byte[]> pair in dictionaryInput)
                result.Add(pair.Key, pair.Value);

            return result;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new ReadOnlyCollection<Type>(new Type[] { typeof(Dictionary<string, byte[]>) });
            }
        }
    }
}
