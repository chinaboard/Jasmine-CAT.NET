using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace CAT.Util
{
    public class JsonSerializer
    {
        public static string Serializer<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            return jsonString;
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        public static T Deserialize<T>(string text)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(text));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }
    }
}