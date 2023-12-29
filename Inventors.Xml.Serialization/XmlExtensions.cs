using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Inventors.Xml.Serialization
{
    public static class XmlExtensions
    {
        /// <summary>
        /// Convert a XML encoded object to an object
        /// </summary>
        /// <typeparam name="T">The type of the object to convert the XML to</typeparam>
        /// <param name="self">The XML string to convert to an object</param>
        /// <returns>An object that has been deserialized from the XML string</returns>
        public static T ToObject<T>(this string self)
            where T : class
        {
            using var text = new StringReader(self);
            using var reader = XmlReader.Create(text);

            var serializer = new XmlSerializer(typeof(T));
            return serializer.Deserialize(reader) as T ?? throw new InvalidOperationException("Deserialization resulted in null");
        }

        /// <summary>
        /// Convert an object to an XML encoded string.
        /// </summary>
        /// <typeparam name="T">The type of the object to convert</typeparam>
        /// <param name="x">The object to convert</param>
        /// <returns>A string containing the XML representation of the object</returns>
        public static string ToXml<T>(this T x)
            where T : class
        {
            using TextWriter writer = new StringWriter();

            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, x);
            return writer.ToString() ?? throw new InvalidOperationException("Serialization of {x} returned a null string");
        }

        public static string Base64Encode(this string self)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(self);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string self)
        {
            var base64EncodedBytes = Convert.FromBase64String(self);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

}
