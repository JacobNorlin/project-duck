using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckEngine.Interfaces;
using Microsoft.Xna.Framework;
using DuckEngine.Input;
using Microsoft.Xna.Framework.Input;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using Jitter.Dynamics.Constraints;

namespace DuckEngine
{
    class Player : Entity, ILogic, IInput, IDraw3D
    {
        #region Keyboard directions
        //  these are the directions in which a player with a keybord
        //  may move depending on which keys are down.
        private static float S = 1 / (float)Math.Sqrt(2);
        private static float[,] keyboardDirection = {
                //  { x, y}     <   ^   v   >            ----legend----
                    { 0, 0},//                          . = button down
                    {-1, 0},//              .           x = button down
                    { 0,-1},//          .                   but ignored
                    {-S,-S},//          .   .
                    { 0, 1},//      .        
                    {-S, S},//      .       .
                    { 0, 0},//      x   x    
                    {-1, 0},//      x   x   .
                    { 1, 0},//  .            
                    { 0, 0},//  x           x
                    { S,-S},//  .       .    
                    { 0,-1},//  x       .   x
                    { S, S},//  .   .        
                    { 0, 1},//  x   .       x
                    { 1, 0},//  .   x   x    
                    { 0, 0}};// x   x   x   x
        #endregion
        private static JVector size = new JVector(1, 2, 1);

        private float hp;
        public float HP { get { return hp; } }

        private RigidBody body;
        public RigidBody Body { get { return body; } }

        private List<Weapon> weapons = new List<Weapon>();
        public List<Weapon> Weapons { get { return weapons; } }

        public Player(Engine _owner)
            : base(_owner)
        {
            owner.addDraw3D(this);
            owner.addInput(this);
            Shape boxShape = new BoxShape(size);
            body = new RigidBody(boxShape);
            owner.World.AddBody(body);
            Constraint upright = new Jitter.Dynamics.Constraints.SingleBody.FixedAngle(body);
            owner.World.AddConstraint(upright);
            body.AllowDeactivation = false;
        }
        ~Player()
        {
            owner.removeDraw3D(this);
            owner.World.RemoveBody(body);
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Input(GameTime gameTime, InputManager input)
        {
            bool up = input.Keyboard_IsKeyDown(Keys.Up);
            bool down = input.Keyboard_IsKeyDown(Keys.Down);
            bool left = input.Keyboard_IsKeyDown(Keys.Left);
            bool right = input.Keyboard_IsKeyDown(Keys.Right);

            int i = 0;
            if (left)   i |= 1;
            if (up)     i |= 2;
            if (down)   i |= 4;
            if (right)  i |= 8;

            float xMovement = keyboardDirection[i, 0];
            float yMovement = keyboardDirection[i, 1];

            body.LinearVelocity = new JVector(xMovement*5, body.LinearVelocity.Y, yMovement*5);
            if (input.Keyboard_WasKeyPressed(Keys.Space))
                body.LinearVelocity += new JVector(0, 10, 0);
        }

        public void Draw3D(GameTime gameTime)
        {
            owner.Helper3D.DrawBoxBody(body);
        }
    }
}
