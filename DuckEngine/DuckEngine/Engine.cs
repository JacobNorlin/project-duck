using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Jitter;
using DuckEngine.Helpers;
using Jitter.Collision;
using DuckEngine.Input;
using DuckEngine.Network;
using DuckEngine.Physics;
using DuckEngine.Sound;
using DuckEngine.Storage;
using DuckEngine.Interfaces;
using Jitter.Dynamics;
using Jitter.LinearMath;

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

        #region Objects
        private List<IDraw2D> AllDraw2D = new List<IDraw2D>();
        private List<IDraw3D> AllDraw3D = new List<IDraw3D>();
        private List<IInput>  AllInput  = new List<IInput>();
        private List<ILogic>  AllLogic  = new List<ILogic>();
        #endregion
        

        DebugDrawer debugDrawer;
        public DebugDrawer DebugDrawer { get { return debugDrawer; } }

        //The physics world
        World world;
        public World World { get { return world; } }

        //Camera to handle view/projection matrices
        Camera camera;
        public Camera Camera { get { return camera; } }

        Helper3D helper3D;
        public Helper3D Helper3D { get { return helper3D; } }

        #region Managers
        InputManager inputManager;
        public InputManager Input { get { return inputManager; } }

        NetworkManager networkManager;
        public NetworkManager Network { get { return networkManager; } }

        PhysicsManager physicsManager;
        public PhysicsManager Physics { get { return physicsManager; } }

        SoundManager soundManager;
        public SoundManager Sound { get { return soundManager; } }

        StorageManager storageManager;
        public StorageManager Storage { get { return storageManager; } }
        #endregion

        public bool multithread = true;
        
        public Engine(StartupObject _startup)
        {
            this.IsMouseVisible = true;

            startup = _startup;

            world = new World(new CollisionSystemSAP());
            world.CollisionSystem.CollisionDetected += new CollisionDetectedHandler(CollisionDetected);
            world.CollisionSystem.PassedBroadphase += new PassedBroadphaseHandler(PassedBroadphase);
            graphics = new GraphicsDeviceManager(this);
            debugDrawer = new DebugDrawer(this);
            helper3D = new Helper3D(this);

            inputManager = new InputManager();
            networkManager = new NetworkManager();
            physicsManager = new PhysicsManager();
            soundManager = new SoundManager();
            storageManager = new StorageManager();

            camera = new Camera(this);
            camera.Position = new Vector3(0, 3, 10);

            Window.ClientSizeChanged += new EventHandler<System.EventArgs>(Window_ClientSizeChanged);
            Content.RootDirectory = "Content";
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
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
            camera.WindowSizeChanged();
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
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            foreach (IInput entity in AllInput)
            {
                entity.Input(gameTime, Input);
            }

            foreach (ILogic entity in AllLogic)
            {
                entity.Update(gameTime);
            }

            float step = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (step > 0.01f) step = 0.01f;
            world.Step(step, multithread);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (IDraw3D entity in AllDraw3D)
            {
                entity.Draw3D(gameTime);
            }

            foreach (IDraw2D entity in AllDraw2D)
            {
                entity.Draw2D(spriteBatch);
            }
            
            base.Draw(gameTime);

        }

        void CollisionDetected(RigidBody body1, RigidBody body2, JVector point1, JVector point2, JVector normal, float penetration)
        {
            if (body1.Tag != null && body2.Tag != null)
            {
                if (body1.Tag is ICollide)
                {
                    ((ICollide)body1.Tag).Collide((Entity)body2.Tag);
                }

                if (body2.Tag is ICollide)
                {
                    ((ICollide)body2.Tag).Collide((Entity)body1.Tag);
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
                    if (body1.Tag is ICollide)
                    {
                        filter1 = ((ICollide)body1.Tag).BroadPhaseFilter((Entity)body2.Tag);
                    }

                    if (body2.Tag is ICollide)
                    {
                        filter2 = ((ICollide)body2.Tag).BroadPhaseFilter((Entity)body1.Tag);
                    }

                    return (filter1 && filter2);
                }
            }

            return true;
        }

        #region Add & remove objects
        /// <summary>
        /// Add an object of type IDraw2D to the engine.
        /// </summary>
        /// <param name="e">The object to be added</param>
        public void addDraw2D(IDraw2D e)
        {
            AllDraw2D.Add(e);
        }

        /// <summary>
        /// Remove an object of type IDraw2D to the engine.
        /// </summary>
        /// <param name="e">The object to be removed</param>
        public void removeDraw2D(IDraw2D e)
        {
            AllDraw2D.Remove(e);
        }

        /// <summary>
        /// Add an object of type IDraw3D to the engine.
        /// </summary>
        /// <param name="e">The object to be added</param>
        public void addDraw3D(IDraw3D e)
        {
            AllDraw3D.Add(e);
        }

        /// <summary>
        /// Remove an object of type IDraw3D to the engine.
        /// </summary>
        /// <param name="e">The object to be removed</param>
        public void removeDraw3D(IDraw3D e)
        {
            AllDraw3D.Remove(e);
        }

        /// <summary>
        /// Add an object of type ILogic to the engine.
        /// </summary>
        /// <param name="e">The object to be added</param>
        public void addLogic(ILogic e)
        {
            AllLogic.Add(e);
        }

        /// <summary>
        /// Remove an object of type ILogic to the engine.
        /// </summary>
        /// <param name="e">The object to be removed</param>
        public void removeLogic(ILogic e)
        {
            AllLogic.Remove(e);
        }

        /// <summary>
        /// Add an object of type IInput to the engine.
        /// </summary>
        /// <param name="e">The object to be added</param>
        public void addInput(IInput e)
        {
            AllInput.Add(e);
        }

        /// <summary>
        /// Remove an object of type IInput to the engine.
        /// </summary>
        /// <param name="e">The object to be removed</param>
        public void removeInput(IInput e)
        {
            AllInput.Remove(e);
        }
        #endregion
    }
}
