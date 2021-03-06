#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using GameStateManagement;
#endregion

namespace GameStateManagementSample
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class ControlsMenuScreen : MenuScreen
    {
        #region Fields


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public ControlsMenuScreen()
            : base("Controls")
        {
            MenuEntry back = new MenuEntry(string.Empty);

            // Hook up menu event handlers.
           
            back.Selected += backSelected;
            
            // Add entries to the menu.
            MenuEntries.Add(back);
        }

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);
        }

        void backSelected(object sender, PlayerIndexEventArgs e)
        {
            this.OnCancel(e.PlayerIndex);
            ScreenManager.ScreenInCounter = 0;
        }
        #endregion
    }
}
