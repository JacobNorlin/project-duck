using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace DuckEngine.Input
{
    public class InputManager
    {
        PlayerIndex activePlayer;

        MouseState currentMouseState;
        public MouseState CurrentMouseState { get { return currentMouseState; } }
        MouseState lastMouseState;
        public MouseState LastMouseState { get { return lastMouseState; } }

        KeyboardState currentKeyboardState;
        public KeyboardState CurrentKeyboardState { get { return currentKeyboardState; } }
        KeyboardState lastKeyboardState;
        public KeyboardState LastKeyboardState {  get { return lastKeyboardState; } }

        GamePadState[] lastGamePadStates = new GamePadState[4];
        public GamePadState[] LastGamePadStates { get { return lastGamePadStates; } }
        GamePadState[] currentGamePadStates = new GamePadState[4];
        public GamePadState[] CurrentGamePadStates  { get { return currentGamePadStates; } }

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
