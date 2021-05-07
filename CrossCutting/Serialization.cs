using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace CrossCutting
{
    /// <summary>
    /// The serialization class.
    /// </summary>
    public static class Serialization
    {
        /// <summary>
        /// Deserializes the specified to deserialize.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toDeserialize">To deserialize.</param>
        /// <returns></returns>
        public static T DeserializeXML<T>(this String toDeserialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringReader textReader = new StringReader(toDeserialize);
            return (T)xmlSerializer.Deserialize(textReader);
        }

        /// <summary>
        /// Serializes the specified to serialize.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSerialize">To serialize.</param>
        /// <returns></returns>
        public static String SerializeXML<T>(this T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }

        /// <summary>
        /// Serializes the object XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static String SerializeObjectXML<T>(T value)
        {
            if (value == null)
            {
                return null;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(false, false); 
            settings.Indent = false;
            settings.OmitXmlDeclaration = false;
            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, value);
                }
                return textWriter.ToString();
            }
        }

        /// <summary>
        /// Deserializes the object XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static T DeserializeObjectXML<T>(String xml)
        {
            if (String.IsNullOrEmpty(xml))
            {
                return default(T);
            }
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlReaderSettings settings = new XmlReaderSettings();
            using (StringReader textReader = new StringReader(xml))
            {
                using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
                {
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
        }

        /// <summary>
        /// Deserializes the json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toDeserialize">To deserialize.</param>
        /// <returns></returns>
        public static T DeserializeJSON<T>(this String toDeserialize)
        {
            if (string.IsNullOrEmpty(toDeserialize))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(toDeserialize);
        }

        /// <summary>
        /// Serializes the json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSerialize">To serialize.</param>
        /// <returns></returns>
        public static String SerializeJSON<T>(this T toSerialize)
        {
            if (toSerialize == null)
            {
                return null;
            }
            return JsonConvert.SerializeObject(toSerialize);
        }
    }
}
