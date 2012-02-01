using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DuckEngine;

namespace DuckGame.Maps
{
    class LoadedMap : Map
    {
        public LoadedMap(Engine _engine, string _fromFile)
            : base(_engine, _fromFile) { }

        public static LoadedMap Load(Engine engine, String fromFile, XmlNode node)
        {
            return new LoadedMap(engine, fromFile);
        }

        public override void Save(XmlDocument doc, XmlElement currNode) { }
    }
}
