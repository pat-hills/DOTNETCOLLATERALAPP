using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CRL.Infrastructure.Helpers
{
    public static class SerializerHelper
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
        public static byte[] Serialize<T>(T obj)
        {
            BinaryFormatter bf = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }


        public static T DeSerilizeFSView<T>(byte[] items)
        {
            BinaryFormatter bf = new BinaryFormatter();
            T _list;


            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(items, 0, items.Length);
                    ms.Position = 0;

                    _list = (T)bf.Deserialize(ms);

                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _list;

        }

        public static T DeSerializeXML<T>(HttpPostedFileBase file)
        {
            T _list;
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(object));
                TextReader reader = new StreamReader(file.InputStream, System.Text.Encoding.GetEncoding("UTF-8"));
                object obj = deserializer.Deserialize(reader);
                _list = (T)obj;
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _list;
        }

        public static byte[] SerializeObj(object obj)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(stream, obj);

                byte[] bytes = stream.ToArray();
                stream.Flush();

                return bytes;
            }
        }

        public static T Deserialize<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
            {
                return null;
            }
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(byteArray, 0, byteArray.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = (T)binForm.Deserialize(memStream);
                return obj;
            }
        }

        /// <summary>
        /// populate a class with xml data 
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        /// <param name="input">xml data</param>
        /// <returns>Object Type</returns>
        public static T DeserializeBatch<T>(byte[] arrayItems) where T : class
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T), new XmlRootAttribute("FSBatch"));

            using (MemoryStream sr = new MemoryStream(arrayItems))
            {
                return (T)ser.Deserialize(sr);
            }
        }


        //Dealing with flat xml

        private static readonly Regex NodeWithPropertyValuesExpression = new Regex("[a-zA-Z]+_[a-zA-Z]+_\\d");
        private static readonly Regex NodeWithoutPropertyValuesExpression = new Regex("[a-zA-Z]+_\\d");

        public static string RestructureFlatXmlToTree(string xml)
        {
            var doc = XDocument.Parse(xml);

            var nodesToBeReserialized = IdentifyNodesToBeReserialized(doc);
            var nodeIdentifiers = GetParentNodesForTree(nodesToBeReserialized);

            foreach (var nodeIdentifier in nodeIdentifiers)
            {
                var newlyIdentifiedChildNodes = nodesToBeReserialized.Where(x => x.Name.LocalName.Contains(nodeIdentifier)).ToList();

                if (!newlyIdentifiedChildNodes.Any())
                {
                    return xml;
                }

                var reserializedChildNodes = CreateReserializedChildNodes(newlyIdentifiedChildNodes, nodeIdentifier);

                if (doc.Root == null)
                {
                    continue;
                }

                doc.Root.Add(new XElement(string.Format("{0}s", nodeIdentifier), reserializedChildNodes));
            }

            return doc.ToString();
        }

        private static IEnumerable<string> GetParentNodesForTree(IEnumerable<XElement> nodesToBeReserialized)
        {
            return nodesToBeReserialized.Select(node => node.Name.LocalName).GroupBy(g => g).Select(x => x.Key).ToList();
        }

        private static List<XElement> IdentifyNodesToBeReserialized(XContainer doc)
        {
            return doc.Descendants().Where(node => node.Name.LocalName.Equals("FirstName") || node.Name.LocalName.Equals("Surname")).ToList();
        }

        private static List<XElement> CreateReserializedChildNodes(List<XElement> newlyIdentifiedChildNodes, string nodeIdentifier)
        {
            var reserializedChildNodes = new List<XElement>();

            var newChildNodesCount = newlyIdentifiedChildNodes.Select(node => node.Name.LocalName).Select(x => x.Substring(x.Length - 1, 1)).GroupBy(g => g).Count();

            for (var i = 0; i < newChildNodesCount; i++)
            {
                var itemNumber = (i + 1).ToString();
                var newChildNodeGrouping = newlyIdentifiedChildNodes.Where(x => x.Name.LocalName.Contains(itemNumber)).ToList();

                XElement newItemNode;
                if (NodeWithPropertyValuesExpression.IsMatch(newChildNodeGrouping[0].Name.LocalName))
                {
                    var nodeProperties = newChildNodeGrouping.Select(node => node.Name.LocalName.Split('_')[1]).ToList();
                    var nestedPropertyNodes = nodeProperties.Select(property => new XElement(property, newChildNodeGrouping.Where(x => x.Name.LocalName.Contains(property)).Select(x => x.Value))).ToList();
                    newItemNode = new XElement(nodeIdentifier, nestedPropertyNodes);
                }
                else
                {
                    newItemNode = new XElement(nodeIdentifier, newChildNodeGrouping[0].Value);
                }
                reserializedChildNodes.Add(newItemNode);
            }

            return reserializedChildNodes;
        }
    }

}

