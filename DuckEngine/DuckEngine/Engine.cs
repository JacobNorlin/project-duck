using System;
using System.Collections.Generic;
using DuckEngine.Helpers;
using DuckEngine.Input;
using DuckEngine.Interfaces;
using DuckEngine.Network;
using DuckEngine.Sound;
using DuckEngine.Storage;
using Jitter;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DuckEngine.Managers;

namespace DuckEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Engine : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        StartupObject startup;

        internal readonly Tracker Tracker;
        
        DebugDrawer debugDrawer;
        public DebugDrawer DebugDrawer { get { return debugDrawer; } }

        //The physics world
        World physics;
        public World Physics { get { return physics; } }

        //Camera to handle view/projection matrices
        Camera camera;
        public Camera Camera
        {
            get { return camera; }
            set
            {
                if (camera != value)
                {
                    if (camera != null)
                    {
                        camera.Active = false;
                    }
                    if (value != null)
                    {
                        value.Active = true;
                    }
                    camera = value;
                }
            }
        }

        private Map map = null;
        public Map Map
        {
            get
            {
                return map;
            }
            set
            {
                if (map != null)
                {
                    Tracker.Untrack(map.Tracker);
                    map.Dispose();
                }
                if (value != null)
                {
                    Tracker.Track(value.Tracker);
                }
                map = value;
            }
        }

        Helper3D helper3D;
        public Helper3D Helper3D { get { return helper3D; } }

        #region Managers
        InputManager inputManager;
        public InputManager Input { get { return inputManager; } }

        MouseEventManager mouseEventManager;
        public MouseEventManager MouseEventManager { get { return mouseEventManager; } }

        NetworkManager networkManager;
        public NetworkManager Network { get { return networkManager; } }

        SoundManager soundManager;
        public SoundManager Sound { get { return soundManager; } }

        //StorageManager storageManager;
        //public StorageManager Storage { get { return storageManager; } }
        #endregion

        public bool multithread = true;
        private bool physicsEnabled = true;
        public bool PhysicsEnabled
        {
            get { return physicsEnabled; }
            set
            {
                if (value && !physicsEnabled)
                {
                    ActivateAllPhysicalBodies();
                }
                physicsEnabled = value;
            }
        }

        public Engine(StartupObject _startup)
        {
            this.IsMouseVisible = true;

            startup = _startup;

            //Physics
            physics = new World(new CollisionSystemSAP());
            physics.CollisionSystem.CollisionDetected += new CollisionDetectedHandler(CollisionDetected);
            physics.CollisionSystem.PassedBroadphase += new PassedBroadphaseHandler(PassedBroadphase);
            physics.AllowDeactivation = true;

            //Managers
            Tracker = new Tracker(this);
            graphics = new GraphicsDeviceManager(this);
            inputManager = new InputManager(this);
            mouseEventManager = new MouseEventManager(this);
            networkManager = new NetworkManager();
            soundManager = new SoundManager();
            //storageManager = new StorageManager();

            //Helpers
            debugDrawer = new DebugDrawer(this);
            helper3D = new Helper3D(this);

            //IsFixedTimeStep = false;
            //TODO: unlock update speed
            TargetElapsedTime = TimeSpan.FromMilliseconds(8); //Update every 8 ms
            Window.ClientSizeChanged += new EventHandler<EventArgs>(ClientSizeChanged);
            Content.RootDirectory = "Content";
        }

        void ClientSizeChanged(object sender, EventArgs e)
        {
            camera.WindowSizeChanged();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //BlendState blendState = new BlendState();
            //blendState.ColorBlendFunction = BlendFunction.Add;
            //blendState.AlphaBlendFunction = BlendFunction.Add;
            //blendState.ColorSourceBlend = Blend.SourceAlpha;
            //blendState.AlphaSourceBlend = Blend.SourceAlpha;
            //blendState.ColorDestinationBlend = Blend.InverseSourceAlpha;
            //blendState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            //GraphicsDevice.BlendState.
            debugDrawer.Initialize();
            startup.Initialize(this);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            helper3D.LoadContent();
            startup.LoadContent(this);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Input.UpdateInput();
            Tracker.Input(gameTime, Input);
            Tracker.Update(gameTime);
            //Physics if enabled
            if (physicsEnabled)
            {
                float step = (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (step > 0.01f) step = 0.01f;
                physics.Step(step, multithread);
            }

            mouseEventManager.ExecuteMouseEvents(gameTime, Input);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Helper3D.BasicEffect.View = Camera.View;
            Helper3D.BasicEffect.Projection = Camera.Projection;

            Tracker.Draw3D(gameTime);

            Tracker.Draw2D(spriteBatch);

            debugDrawer.Draw();
            //foreach (RigidBody body in physics.RigidBodies)
            //{
            //    Helper3D.DrawBody(body, Color.SandyBrown, true);
            //}
            base.Draw(gameTime);
        }

        #region Physics
        void CollisionDetected(RigidBody body1, RigidBody body2, JVector point1, JVector point2, JVector normal, float penetration)
        {
            if (body1.Tag != null && body2.Tag != null)
            {
                if (body1.Tag is ICollideEvent)
                {
                    ((ICollideEvent)body1.Tag).Collide((Entity)body2.Tag);
                }

                if (body2.Tag is ICollideEvent)
                {
                    ((ICollideEvent)body2.Tag).Collide((Entity)body1.Tag);
                }
            }
        }

        bool PassedBroadphase(IBroadphaseEntity entity1, IBroadphaseEntity entity2)
        {
            RigidBody body1 = (RigidBody)entity1;
            RigidBody body2 = (RigidBody)entity2;
            if (entity1 is RigidBody && entity2 is RigidBody)
            {
                if (body1.Tag != null && body2.Tag != null)
                {
                    bool filter1 = true;
                    bool filter2 = true;
                    if (body1.Tag is ICollideEvent)
                    {
                        filter1 = ((ICollideEvent)body1.Tag).BroadPhaseFilter((Entity)body2.Tag);
                    }

                    if (body2.Tag is ICollideEvent)
                    {
                        filter2 = ((ICollideEvent)body2.Tag).BroadPhaseFilter((Entity)body1.Tag);
                    }

                    return (filter1 && filter2);
                }
            }

            return true;
        }
        #endregion

        private void ActivateAllPhysicalBodies()
        {
            foreach (RigidBody body in Physics.RigidBodies)
            {
                body.IsActive = true;
            }
            //How do I activate a softbody? problem for future me //Björn
            //foreach (SoftBody body in Physics.SoftBodies)
            //{
            //    body.IsActive = true;
            //}
        }
    }
}
