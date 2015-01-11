#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using GameStateManagementSample;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class ControlsScreen : GameScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public ControlsScreen()
        {
           

        }


        #endregion

        #region Handle Input

        //public override void HandleInput(InputState input)
        //{
        //    if (input == null)
        //        throw new ArgumentNullException("input");

        //    int playerIndex = (int)ControllingPlayer.Value;
        //    KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
        //     GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

        //     if (gamePadState.IsButtonDown(Buttons.B))
        //     {
        //         ScreenManager.BControls = false;
        //         ExitScreen();
        //     }
        //}
       

        #endregion
    }
}
