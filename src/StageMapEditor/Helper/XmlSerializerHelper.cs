using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace StageMapEditor.Helper
{
    public class XmlSerializerHelper
    {
        /// <summary>
        /// 書き出し先にXmlファイルをシリアライズ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writeFileInfo"></param>
        /// <param name="xml"></param>
        public static void Serialize<T>(FileInfo writeFileInfo, T xml) where T : class
        {
            Serialize(writeFileInfo.FullName, xml);
        }

        /// <summary>
        /// 書き出し先にXmlファイルをシリアライズ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writeFilepath"></param>
        /// <param name="xml"></param>
        public static void Serialize<T>(string writeFilepath, T xml) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var fs = new FileStream(writeFilepath, FileMode.Create))
            {
                serializer.Serialize(fs, xml);
            }
        }

        /// <summary>
        /// ファイルからデシリアライズ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static T Deserialize<T>(FileInfo fileInfo) where T : class
        {
            return Deserialize<T>(fileInfo.FullName);
        }

        /// <summary>
        /// ファイルからデシリアライズ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string filepath) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var fs = new FileStream(filepath, FileMode.Open))
            {
                var xml = serializer.Deserialize(fs) as T;
                return xml;
            }
        }
    }
}
