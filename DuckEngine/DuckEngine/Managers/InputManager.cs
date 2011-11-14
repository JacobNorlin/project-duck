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

        KeyboardState currentKeyboardState;
        KeyboardState lastKeyboardState;
        
        GamePadState[] lastGamePadStates = new GamePadState[4];
        GamePadState[] currentGamePadStates = new GamePadState[4];

        void Update()
        {
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
