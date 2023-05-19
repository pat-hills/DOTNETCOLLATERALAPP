using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CRL.Model.Search
{
    public static class SearchParameterHelper
    {

     

        public static string GenerateXML<T>(T searchParam)  //Again this should be in the service class and we have to use an interface for the cloning and also for the serialising
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, searchParam);
                stringWriter.Flush();
                return stringWriter.ToString();
            }



        }

        public static T GetObjectFromXML<T>(string xmlString)  //Again this should be in the service class and we have to use an interface for the cloning and also for the serialising
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(xmlString);
            object obj = deserializer.Deserialize(reader);
            T XmlData = (T)obj;
            reader.Close();
            return XmlData;



        }
    }

}
