using System;
using System.IO;
using System.Linq;
using Jitter.Dynamics;
using DuckEngine.Interfaces;
using DuckEngine.Helpers;
using System.Xml;
using Jitter.LinearMath;
using System.Reflection;
using System.Collections.Generic;
namespace DuckEngine.Storage
{
    /// <summary>
    /// A class which handles reading and writing of files to save and load data.
    /// </summary>
    public static class StorageManager
    {
        public static void Save(Map map)
        {
            String path = "Content/save0.xml";

            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("root");
            doc.AppendChild(root);
            XmlElement mapNode = doc.CreateElement("map");
            mapNode.SetAttribute("aq", map.GetType().AssemblyQualifiedName);
            root.AppendChild(mapNode);
            XmlElement lookup = doc.CreateElement("lookup");
            root.AppendChild(lookup);
            XmlElement saved = doc.CreateElement("saved");
            root.AppendChild(saved);

            //a list would do..
            Dictionary<string, string> classNameToAqName = new Dictionary<string, string>();
            foreach (ISave saveable in map.Tracker.Saveables)
            {
                String className = saveable.GetType().Name;
                String aqName = saveable.GetType().AssemblyQualifiedName;
                if (!classNameToAqName.ContainsKey(className))
                {
                    if (classNameToAqName.ContainsValue(aqName))
                    {
                        string otherAqName;
                        classNameToAqName.TryGetValue(className, out otherAqName);
                        throw new System.Exception("Name collision! The class " + aqName + " and " + otherAqName + " have the same local class name.");
                    }
                    classNameToAqName.Add(className, aqName);
                    XmlElement lookupEntry = doc.CreateElement(className);
                    lookupEntry.SetAttribute("aq", aqName);
                    lookup.AppendChild(lookupEntry);
                }
                XmlElement node = doc.CreateElement(className);
                saveable.Save(doc, node);
                saved.AppendChild(node);
            }
            doc.Save(path);
            Console.WriteLine("Saved to: " + path);
        }

        //private class XmlPro : XmlElement
        //{
        //    private XmlDocument doc;

        //    public XmlPro(XmlDocument _doc)
        //        : base(
        //    {
        //        doc = _doc;
        //    }
        //}

        public static Map Load(Engine engine)
        {
            String path = "Content/save0.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode root = doc.SelectSingleNode("root");
            XmlNode lookup = root.SelectSingleNode("lookup");

            Dictionary<string, string> classNameToAqName = new Dictionary<string, string>();
            foreach (XmlNode node in lookup.ChildNodes)
            {
                string className = node.LocalName;
                string aqName = node.Attributes.GetNamedItem("aq").InnerText;
                classNameToAqName.Add(className, aqName);
            }

            XmlNode mapNode = root.SelectSingleNode("map");
            Type mapType = Type.GetType(mapNode.Attributes.GetNamedItem("aq").InnerText);
            MethodInfo mapLoadMethod = mapType.GetMethod("Load");
            Map map = (Map)mapLoadMethod.Invoke(null, new object[] { engine, path, mapNode });
            
            XmlNode saved = root.SelectSingleNode("saved");
            foreach (XmlNode node in saved.ChildNodes)
            {
                string className = node.LocalName;
                string aqName;
                if (classNameToAqName.TryGetValue(className, out aqName))
                {
                    Type type = Type.GetType(aqName);
                    MethodInfo loadMethod = type.GetMethod("Load");
                    ISave loadedObject = (ISave)loadMethod.Invoke(null, new object[] { map.Engine, map.Tracker, node });
                    Console.WriteLine("Loaded {0}.", className);
                }
                else
                {
                    Console.WriteLine("Error loading {0}, class not in lookup table.", className);
                }
            }

            Console.WriteLine("Loaded from: " + path);
            return map;
        }
    }
}
