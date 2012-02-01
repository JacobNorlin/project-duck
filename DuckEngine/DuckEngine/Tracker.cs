using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckEngine.Interfaces;
using DuckEngine.Input;
using DuckEngine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Jitter.Dynamics;

namespace DuckEngine
{
    /// <summary>
    /// A class for bundling the calls to classes implementing
    /// IPhysical, IDraw2D, IDraw3D, IInput, ILogic and/or ISave interfaces.
    /// </summary>
    public class Tracker : IDraw2D, IDraw3D, IInput, ILogic
    {
        private readonly List<IPhysical> PhysicalList = new List<IPhysical>();
        private readonly List<IDraw2D> Draw2DList = new List<IDraw2D>();
        private readonly List<IDraw3D> Draw3DList = new List<IDraw3D>();
        private readonly List<IInput> InputList = new List<IInput>();
        private readonly List<ILogic> LogicList = new List<ILogic>();
        private readonly List<ISave> SaveList = new List<ISave>();
        private readonly Engine Engine;

        public IEnumerable<RigidBody> Bodies
        {
            get
            {
                List<RigidBody> bodies = PhysicalList.Bodies();
                PhysicalList.ForEach(physical =>
                {
                    if (physical is Tracker)
                        bodies.AddRange(((Tracker)physical).Bodies);
                });
                return bodies;
            }
        }
        public IEnumerable<ISave> Saveables
        {
            get
            {
                List<ISave> toSave = new List<ISave>(SaveList);
                SaveList.ForEach(e =>
                {
                    if (e is Tracker)
                        toSave.AddRange(((Tracker)e).Saveables);
                });
                return toSave;
            }
        }

        /// <summary>
        /// Create a tracker
        /// </summary>
        /// <param name="_engine">bound to the given Engines physics engine</param>
        public Tracker(Engine _engine)
        {
            Engine = _engine;
        }

        #region All
        /// <summary>
        /// Checks which of the interfaces IPhysical, IDraw2D, IDraw3D
        /// ILogic, IInput and ISave the given object
        /// implements, and adds it to the lists.
        /// </summary>
        /// <param name="e"></param>
        public void Track(Object e)
        {
            if (e is IPhysical) TrackPhysical((IPhysical)e);
            if (e is IDraw2D) TrackDraw2D((IDraw2D)e);
            if (e is IDraw3D) TrackDraw3D((IDraw3D)e);
            if (e is ILogic) TrackLogic((ILogic)e);
            if (e is IInput) TrackInput((IInput)e);
            if (e is ISave) TrackSave((ISave)e);
        }

        /// <summary>
        /// Checks which of the interfaces IPhysical, IDraw2D, IDraw3D
        /// ILogic, IInput and ISave the given object
        /// implements, and removes it from the lists.
        /// </summary>
        /// <param name="e"></param>
        public void Untrack(Object e)
        {
            if (e is IPhysical) UntrackPhysical((IPhysical)e);
            if (e is IDraw2D) UntrackDraw2D((IDraw2D)e);
            if (e is IDraw3D) UntrackDraw3D((IDraw3D)e);
            if (e is ILogic) UntrackLogic((ILogic)e);
            if (e is IInput) UntrackInput((IInput)e);
            if (e is ISave) UntrackSave((ISave)e);
        }
        #endregion
        #region Physical
        /// <summary>
        /// Keep track of Physical objects, and add them to the physics engine.
        /// </summary>
        /// <param name="e"></param>
        public void TrackPhysical(IPhysical e)
        {
            if (e.Body != null)
            {
                Engine.Physics.AddBody(e.Body);
                PhysicalList.Add(e);
            }
        }

        /// <summary>
        /// Stop keep track of Physical objects, and remove them to the physics engine.
        /// </summary>
        /// <param name="e"></param>
        public void UntrackPhysical(IPhysical e)
        {
            Engine.Physics.RemoveBody(e.Body);
            PhysicalList.Remove(e);
        }

        /// <summary>
        /// If the given object is already Tracked "Physically",
        /// update which body should be in the Physics Engine.
        /// </summary>
        /// <param name="e">Given object</param>
        /// <param name="oldBody">
        /// The old objects Body can only be null if the object is not tracked
        /// by this tracker physically, in which case calling this method does nothing.
        /// </param>
        public void UpdatePhysicalTrack(IPhysical e, RigidBody oldBody)
        {
            if (PhysicalList.Contains(e)) { //only if it already tracked, which also means
                Engine.Physics.RemoveBody(oldBody); //it's in the physics engine, so remove it
                if (e.Body == null) //If the new body is null,
                {
                    PhysicalList.Remove(e); //stop Physical tracking
                }
                else //otherwise
                {
                    Engine.Physics.AddBody(e.Body); //add the new body to the physics engine
                }
            }
        }
        #endregion
        #region 2D
        /// <summary>
        /// Add an object of type IDraw2D to the engine.
        /// </summary>
        /// <param name="e"></param>
        public void TrackDraw2D(IDraw2D e)
        {
            Draw2DList.Add(e);
        }

        /// <summary>
        /// Remove an object of type IDraw2D to the engine.
        /// </summary>
        /// <param name="e"></param>
        public void UntrackDraw2D(IDraw2D e)
        {
            Draw2DList.Remove(e);
        }
        #endregion
        #region 3D
        /// <summary>
        /// Add an object of type IDraw3D to the engine.
        /// </summary>
        /// <param name="e"></param>
        public void TrackDraw3D(IDraw3D e)
        {
            Draw3DList.Add(e);
        }

        /// <summary>
        /// Remove an object of type IDraw3D to the engine.
        /// </summary>
        /// <param name="e"></param>
        public void UntrackDraw3D(IDraw3D e)
        {
            Draw3DList.Remove(e);
        }
        #endregion
        #region Logic
        /// <summary>
        /// Add an object of type ILogic to the engine.
        /// </summary>
        /// <param name="e"></param>
        public void TrackLogic(ILogic e)
        {
            LogicList.Add(e);
        }

        /// <summary>
        /// Remove an object of type ILogic to the engine.
        /// </summary>
        /// <param name="e"></param>
        public void UntrackLogic(ILogic e)
        {
            LogicList.Remove(e);
        }
        #endregion
        #region Input
        /// <summary>
        /// Add an object of type IInput to the engine.
        /// </summary>
        /// <param name="e"></param>
        public void TrackInput(IInput e)
        {
            InputList.Add(e);
        }

        /// <summary>
        /// Remove an object of type IInput to the engine.
        /// </summary>
        /// <param name="e"></param>
        public void UntrackInput(IInput e)
        {
            InputList.Remove(e);
        }
        #endregion
        #region Save
        /// <summary>
        /// Keep track of this object, in case the user wants to save it.
        /// </summary>
        /// <param name="e"></param>
        public void TrackSave(ISave e)
        {
            SaveList.Add(e);
        }

        /// <summary>
        /// Stop keeping track of this object, it will no
        /// longer be listed as a saveable item.
        /// </summary>
        /// <param name="e"></param>
        public void UntrackSave(ISave e)
        {
            SaveList.Remove(e);
        }
        #endregion
        #region Forward interface calls to all the children
        public void Draw2D(SpriteBatch spriteBatch)
        {
            Draw2DList.ForEach(e => e.Draw2D(spriteBatch));
        }
        public void Draw3D(GameTime gameTime)
        {
            Draw3DList.ForEach(e => e.Draw3D(gameTime));
        }
        public void Input(GameTime gameTime, InputManager input)
        {
            InputList.ForEach(e => e.Input(gameTime, input));
        }
        public void Update(GameTime gameTime)
        {
            LogicList.ForEach(e => e.Update(gameTime));
        }
        #endregion

        /// <summary>
        /// Untrack ALL objects {IDraw2D, IDraw3D, IInput, ILogic, ISave};
        /// </summary>
        public void UntrackAll()
        {
            PhysicalList.ForEach(e => UntrackPhysical(e));
            Draw2DList.ForEach(e => UntrackDraw2D(e));
            Draw3DList.ForEach(e => UntrackDraw3D(e));
            InputList.ForEach(e => UntrackInput(e));
            LogicList.ForEach(e => UntrackLogic(e));
            SaveList.ForEach(e => UntrackSave(e));
        }
    }
}
