using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace DuckEngine.Input
{
    /// <summary>
    /// A class which holds current and recent input data.
    /// </summary>
    public class InputManager : Entity
    {
        public enum MouseButton { Left, Middle, Right };

        PlayerIndex activePlayer;

        MouseState currentMouseState;
        public MouseState CurrentMouseState { get { return currentMouseState; } }
        MouseState lastMouseState;
        public MouseState LastMouseState { get { return lastMouseState; } }
        #region Mouse getters
        public bool Mouse_IsButtonDown(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left:
                    return currentMouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return currentMouseState.MiddleButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return currentMouseState.RightButton == ButtonState.Pressed;
            }
            return false;
        }
        public bool Mouse_WasButtonDown(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left:
                    return lastMouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return lastMouseState.MiddleButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return lastMouseState.RightButton == ButtonState.Pressed;
            }
            return false;
        }
        public bool Mouse_WasButtonPressed(MouseButton mouseButton)
        {
            return Mouse_IsButtonDown(mouseButton) &&
                !Mouse_WasButtonDown(mouseButton);
        }
        public bool Mouse_WasButtonReleased(MouseButton mouseButton)
        {
            return !Mouse_IsButtonDown(mouseButton) &&
                Mouse_WasButtonDown(mouseButton);
        }
        private Point mouseMovement;
        public Point MouseMovement { get { return mouseMovement; } }
        public bool Mouse_IsOnScreen()
        {
            return Owner.Window.ClientBounds.Contains(
                currentMouseState.X, currentMouseState.Y);
        }
        public Point Mouse_Coords() {
            return new Point(currentMouseState.X, currentMouseState.Y);
        }
        private Ray mouseRay = new Ray();
        public Ray MouseRay { get { return mouseRay; } }

        private int mouseScrollChange = 0;
        public int MouseScroll { get { return mouseScrollChange; } }

        private Point mouseFrozenAt;
        private bool mouseFrozen;
        public bool MouseFrozen
        {
            get { return mouseFrozen; }
            set
            {
                if (value && !mouseFrozen)
                {
                    mouseFrozenAt = Mouse_Coords();
                }
                mouseFrozen = value;
            }
        }
        #endregion

        KeyboardState lastKeyboardState;
        KeyboardState currentKeyboardState;
        #region Keyboard getters
                //Actually I'd like to name these methods
                //  Keyboard.Is_____() but I don't know how to do it nicely //Björn
        public bool Keyboard_IsKeyDown(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }
        public bool Keyboard_IsKeyUp(Keys key)
        {
            return currentKeyboardState.IsKeyUp(key);
        }
        public bool Keyboard_WasKeyPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) &
                      lastKeyboardState.IsKeyUp(key);
        }
        public bool Keyboard_WasKeyReleased(Keys key)
        {
            return currentKeyboardState.IsKeyUp(key) &
                      lastKeyboardState.IsKeyDown(key);
        }
        #endregion
        
        GamePadState[] lastGamePadStates = new GamePadState[4];
        public GamePadState[] LastGamePadStates { get { return lastGamePadStates; } }
        GamePadState[] currentGamePadStates = new GamePadState[4];
        public GamePadState[] CurrentGamePadStates  { get { return currentGamePadStates; } }

        public InputManager(Engine _owner)
            : base(_owner)
        {
        }

        /// <summary>
        /// Updates the input manager to hold the latest input information.
        /// </summary>
        public void Update()
        {
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            mouseMovement.X = currentMouseState.X - lastMouseState.X;
            mouseMovement.Y = currentMouseState.Y - lastMouseState.Y;
            if (mouseFrozen)
            {
                Mouse.SetPosition(mouseFrozenAt.X, mouseFrozenAt.Y);
                currentMouseState = Mouse.GetState(); //refresh mouse state
            }

            mouseScrollChange = currentMouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue;

            mouseRay.Position = Owner.Camera.Position;
            mouseRay.Direction = RayTo(currentMouseState.X, currentMouseState.Y);

            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            for (int i = 0; i < 4; i++ )
            {
                lastGamePadStates[i] = currentGamePadStates[i];
                currentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }
        }

        // Helper method to get the 3d ray
        private Vector3 RayTo(int x, int y)
        {
            Vector3 nearSource = new Vector3(x, y, 0);
            Vector3 farSource = new Vector3(x, y, 1);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);
            Vector3 nearPoint = Owner.GraphicsDevice.Viewport.Unproject(nearSource,
                Owner.Camera.Projection, Owner.Camera.View, world);
            Vector3 farPoint = Owner.GraphicsDevice.Viewport.Unproject(farSource,
                Owner.Camera.Projection, Owner.Camera.View, world);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return direction;
        }
    }
}
