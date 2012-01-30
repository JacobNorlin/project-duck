using System;
using System.IO;
using Jitter.Dynamics;
using DuckEngine.Interfaces;
using System.Xml;
using Jitter.LinearMath;
using System.Reflection;
using System.Collections.Generic;
namespace DuckEngine.Storage
{
    /// <summary>
    /// A class which handles reading and writing of files to save and load data.
    /// </summary>
    public class StorageManager
    {
        public static void Save(Engine engine)
        {
            String path = engine.Content.RootDirectory+"/save0.xml";

            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("root");
            doc.AppendChild(root);
            XmlElement lookup = doc.CreateElement("lookup");
            root.AppendChild(lookup);
            XmlElement saved = doc.CreateElement("saved");
            root.AppendChild(saved);

            //a list would do..
            Dictionary<string, string> classNameToAqName = new Dictionary<string, string>();
            foreach (ISave saveable in engine.Saveables)
            {
                String className = saveable.GetType().Name;
                if (!classNameToAqName.ContainsKey(className))
                {
                    String aqName = saveable.GetType().AssemblyQualifiedName;
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

        public static IEnumerable<ISave> Load(Engine engine)
        {
            String path = engine.Content.RootDirectory + "/save0.xml";
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

            XmlNode saved = root.SelectSingleNode("saved");
            List<ISave> loadedObjects = new List<ISave>();
            foreach (XmlNode node in saved.ChildNodes)
            {
                string className = node.LocalName;
                string aqName;
                if (classNameToAqName.TryGetValue(className, out aqName))
                {
                    Type type = Type.GetType(aqName);
                    MethodInfo loadMethod = type.GetMethod("Load");
                    ISave loadedObject = (ISave)loadMethod.Invoke(null, new object[]{engine,node});
                    loadedObjects.Add(loadedObject);
                    Console.WriteLine("Loaded {0}.", className);
                }
                else
                {
                    Console.WriteLine("Error loading {0}, class not in lookup table.", className);
                }
            }

            engine.removeAll(engine.Saveables);
            foreach (object o in loadedObjects)
            {
                engine.addAll(o);
            }
            
            Console.WriteLine("Loaded from: " + path);
            return loadedObjects;
        }
    }
}
