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

namespace DuckEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Engine : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private List<IDraw2D> AllDraw2D = new List<IDraw2D>();
        private List<IDraw3D> AllDraw3D = new List<IDraw3D>();
        private List<IInput>  AllInput  = new List<IInput>();
        private List<ILogic>  AllLogic  = new List<ILogic>();

        private DebugDrawer debugDrawer;

        World world;
        public World World { get { return world; } }
        Camera camera;
        public Camera Camera { get { return camera; } }
        Helper3D helper3D;
        public Helper3D Helper3D { get { return helper3D; } }

        public bool multithread = true;
        
        public Engine()
        {
            this.IsMouseVisible = true;
            world = new World(new CollisionSystemSAP());
            graphics = new GraphicsDeviceManager(this);
            debugDrawer = new DebugDrawer(this);
            helper3D = new Helper3D(this);
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }
            foreach (IInput e in AllInput)
            {
                e.Input(gameTime);
            }
            foreach (ILogic e in AllLogic)
            {
                e.Update(gameTime);
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

            foreach (IDraw3D e in AllDraw3D)
            {
                e.Draw3D(gameTime);
            }
            foreach (IDraw2D e in AllDraw2D)
            {
                e.Draw2D(gameTime);
            }
            
            base.Draw(gameTime);
        }

        public void addDraw2D(IDraw2D e)
        {
            AllDraw2D.Add(e);
        }
        public void removeDraw2D(IDraw2D e)
        {
            AllDraw2D.Remove(e);
        }
        public void addDraw3D(IDraw3D e)
        {
            AllDraw3D.Add(e);
        }
        public void removeDraw3D(IDraw3D e)
        {
            AllDraw3D.Remove(e);
        }
        public void addLogic(ILogic e)
        {
            AllLogic.Add(e);
        }
        public void removeLogic(ILogic e)
        {
            AllLogic.Remove(e);
        }
        public void addInput(IInput e)
        {
            AllInput.Add(e);
        }
        public void removeInput(IInput e)
        {
            AllInput.Remove(e);
        }
    }
}
