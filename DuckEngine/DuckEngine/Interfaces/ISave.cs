using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DuckEngine.Interfaces
{
    /// <summary>
    /// An interface allowing for saving/restoring your object to/from an XML save file.<br />
    /// <br />
    /// <b>Please note!</b><br />
    /// When implementing this interface, You must also create a method
    /// "static object Load(Engine engine, XmlNode node)"
    /// corresponding to your Save method implementation.
    /// </summary>
    public interface ISave
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="node"></param>
        void Save(XmlDocument doc, XmlElement currNode);
        //static object Load(Engine engine, XmlNode node);
    }
}
