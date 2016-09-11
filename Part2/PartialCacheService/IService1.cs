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

namespace PartialCacheService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        [WebGet(
            UriTemplate = "DownloadFile/{filename}",
            ResponseFormat = WebMessageFormat.Xml,
            RequestFormat = WebMessageFormat.Xml)]
        Stream DownloadFile(string filename);
       // Chunk DownloadFile(string filename);
       // [OperationContract]
       // [WebGet]

       // CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    //[Serializable]
    [KnownType(typeof(Chunk))]
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
        Dictionary<int, byte[]> chunks { get; set; }
        public Chunk(Dictionary<int, byte[]> chunks)
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
