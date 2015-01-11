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
    class CharacterMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry shipMenuEntry;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public CharacterMenuScreen(): base("Choose Your Ship")
        {
            // Create our menu entries.
            shipMenuEntry = new MenuEntry(string.Empty);

            
            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            shipMenuEntry.Selected += shipMenuEntrySelected;
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(shipMenuEntry);
            MenuEntries.Add(back);
        }

        public override void Activate(bool instancePreserved)
        {
            SetMenuEntryText();

            base.Activate(instancePreserved);
        }

        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            if (ScreenManager.shipchosenbool == true)
                shipMenuEntry.Text = "Blue Seal";
            else
                shipMenuEntry.Text = "Red Dragon";
        }


        #endregion

        #region Handle Input

        void shipMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AudioEnabled = !ScreenManager.AudioEnabled;

            SetMenuEntryText();
        }

        void AudioVolumeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AudioVolume += 0.1f;

            if (ScreenManager.AudioVolume >= 1.1f)
                ScreenManager.AudioVolume = 0.1f;

            SetMenuEntryText();
        }

        void SoundEffectMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.SFXVolume += 0.1f;

            if (ScreenManager.SFXVolume >= 1.1f)
                ScreenManager.SFXVolume = 0.1f;

            SetMenuEntryText();
        }

        void SplitScreenMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.ScreenHorizontal = !ScreenManager.ScreenHorizontal;

            SetMenuEntryText();
        }

        #endregion
    }
}
