using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.LinearMath;
using System.Xml;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;

namespace DuckEngine.Storage
{
    public static class StorageExtensions
    {
        public static void Save(this JVector v, XmlDocument doc, XmlElement node)
        {
            node.InnerText = v.X + " " + v.Y + " " + v.Z;
        }
        public static JVector LoadJVector(XmlNode node)
        {
            string[] coords = node.InnerText.Split(' ');
            float x = float.Parse(coords[0]);
            float y = float.Parse(coords[1]);
            float z = float.Parse(coords[2]);
            return new JVector(x, y, z);
        }

        public static void Save(this JMatrix m, XmlDocument doc, XmlElement node)
        {
            node.InnerText =
                m.M11 + " " + m.M12 + " " + m.M13 + " " +
                m.M21 + " " + m.M22 + " " + m.M23 + " " +
                m.M31 + " " + m.M32 + " " + m.M33;
        }
        public static JMatrix LoadJMatrix(XmlNode node)
        {
            string[] cells = node.InnerText.Split(' ');
            float[] m = new float[9];
            for (int i = 8; i >= 0; i--)
            {
                m[i] = float.Parse(cells[i]);
            }
            return new JMatrix(m[0], m[1], m[2], m[3], m[4], m[5], m[6], m[7], m[8]);
        }
        public static void Save(this RigidBody body, XmlDocument doc, XmlElement node)
        {
            node.SetAttribute("mass", body.Mass.ToString());
            XmlElement posNode = doc.CreateElement("pos");
            XmlElement oriNode = doc.CreateElement("ori");
            XmlElement shapeNode = doc.CreateElement("shape");
            body.Position.Save(doc, posNode);
            body.Orientation.Save(doc, oriNode);
            body.Shape.Save(doc, shapeNode);
            node.AppendChild(posNode);
            node.AppendChild(oriNode);
            node.AppendChild(shapeNode);
        }
        public static RigidBody LoadRigidBody(XmlNode node)
        {
            XmlNode shapeNode = node.SelectSingleNode("shape");
            XmlNode posNode = node.SelectSingleNode("pos");
            XmlNode oriNode = node.SelectSingleNode("ori");

            Shape shape = LoadShape(shapeNode);
            RigidBody body = new RigidBody(shape);
            body.Position = LoadJVector(posNode);
            body.Orientation = LoadJMatrix(oriNode);
            return body;
        }

        public static void Save(this Shape shape, XmlDocument doc, XmlElement node)
        {
            if (shape is BoxShape) ((BoxShape)shape).Save(doc, node);
            else if (shape is CapsuleShape) ((CapsuleShape)shape).Save(doc, node);
            else
            {
                throw new NotImplementedException("Saving of " + shape.GetType().Name + " shapes not yet implemented.");
            }
        }
        public static void Save(this BoxShape boxShape, XmlDocument doc, XmlElement node)
        {
            node.SetAttribute("shape", "box");
            XmlElement sizeNode = doc.CreateElement("size");
            boxShape.Size.Save(doc, sizeNode);
            node.AppendChild(sizeNode);
        }
        public static void Save(this CapsuleShape capShape, XmlDocument doc, XmlElement node)
        {
            node.SetAttribute("shape", "capsule");
            node.SetAttribute("length", capShape.Length.ToString());
            node.SetAttribute("radius", capShape.Radius.ToString());
        }
        public static Shape LoadShape(XmlNode node)
        {
            string shape = node.Attributes.GetNamedItem("shape").InnerText;
            switch (shape)
            {
                case "box":
                    return new BoxShape(LoadJVector(node.SelectSingleNode("size")));
            }
            throw new FormatException("No suitable shape found inside XML node:\n" + node.InnerText);
        }
    }
}
