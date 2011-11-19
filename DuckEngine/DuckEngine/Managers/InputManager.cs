using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DuckEngine.Input
{
    /// <summary>
    /// A class which holds current and recent input data.
    /// </summary>
    public class InputManager
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
        public Point Mouse_Movement()
        {
            return new Point(
                currentMouseState.X - lastMouseState.X,
                currentMouseState.Y - lastMouseState.Y);
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

        /// <summary>
        /// Updates the input manager to hold the latest input information.
        /// </summary>
        public void Update()
        {
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            for (int i = 0; i < 4; i++ )
            {
                lastGamePadStates[i] = currentGamePadStates[i];
                currentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }
        }
    }
}
