using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.Dynamics;
using Jitter.LinearMath;
using DuckEngine.Helpers;
using Microsoft.Xna.Framework;

namespace DuckEngine.MapEditor
{
    class Selection : ICollection<RigidBody>, IEnumerable<RigidBody>
    {
        public static readonly Selection Empty = new Selection();

        private HashSet<RigidBody> selected = new HashSet<RigidBody>();
        private RigidBody highlighted;
        public RigidBody Highlighted { get { return highlighted; } }
        private JBBox box;
        public float BoundingBoxSize { get { return (box.Max - box.Min).Length() / 2; } }
        public Vector3 Position
        {
            get { return Conversion.ToXNAVector(box.Center); }
            set {
                JVector change = Conversion.ToJitterVector(value) - box.Center;
                //Console.WriteLine(change);
                foreach (RigidBody body in selected)
                {
                    body.Position += change;
                }
                box.Max += change;
                box.Min += change;
            }
        }

        public bool Active
        {
            set
            {
                foreach (RigidBody body in selected)
                {
                    if (body.Tag is Entity)
                    {
                        Entity entity = (Entity)body.Tag;
                        entity.Active = value;
                    }
                }
            }
        }

        public Selection()
        {
        }
        
        public Selection copy(bool active)
        {
            Selection copy = new Selection();
            RigidBody copyHighlighted = null;
            foreach (RigidBody body in selected)
            {
                if (body.Tag is PhysicalEntity)
                {
                    PhysicalEntity newEntity = ((PhysicalEntity)body.Tag).Clone();
                    if (highlighted == body)
                    {
                        copyHighlighted = newEntity.Body;
                    }
                    copy.Add(newEntity.Body);
                }
            }
            copy.highlighted = copyHighlighted;
            copy.Active = active;
            return copy;
        }

        /// <summary>
        /// Select or deselect given body depending on whether
        /// it was previously selected or deselected.
        /// </summary>
        /// <param name="body"></param>
        /// <returns>true if body was deselected</returns>
        public bool toggleSelection(RigidBody body)
        {
            highlighted = body;
            if (selected.Remove(body))
            {
                return true;
            }
            else
            {
                Add(body);
                return false;
            }
        }

        private void reconstructBox()
        {
            box = new JBBox();
            foreach (RigidBody body in selected)
            {
                box = JBBox.CreateMerged(box, body.BoundingBox);
            }
        }

        IEnumerator<RigidBody> IEnumerable<RigidBody>.GetEnumerator()
        {
            return selected.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return selected.GetEnumerator();
        }

        public void Add(RigidBody body)
        {
            highlighted = body;
            if (selected.Add(body))
            {
                reconstructBox();
            }
        }

        public void Add(IEnumerable<RigidBody> bodies)
        {
            foreach (RigidBody body in bodies)
            {
                Add(body);
            }
        }

        public void Clear()
        {
            selected.Clear();
        }

        public bool Contains(RigidBody body)
        {
            return selected.Contains(body);
        }

        public void CopyTo(RigidBody[] array, int arrayIndex)
        {
            selected.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return selected.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(RigidBody body)
        {
            return selected.Remove(body);
        }
    }
}
