#region File Description
//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
#endregion

namespace HighSchoolBruisical
{
    /// <summary>
    /// Sample showing how to manage the different game states involved in
    /// implementing a networked game, with menus for creating, searching,
    /// and joining sessions, a lobby screen, and the game itself. This main
    /// game class is extremely simple: all the interesting stuff happens
    /// in the ScreenManager component.
    /// </summary>
    public class HighSchoolBruisicalGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        private const int TargetFrameRate = 60;

        #endregion

        #region Initialization


        /// <summary>
        /// The main game constructor.
        /// </summary>
        public HighSchoolBruisicalGame()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            // Create components.
            screenManager = new ScreenManager(this);
            screenManager.TraceEnabled = true;

            Components.Add(screenManager);
            Components.Add(new MessageDisplayComponent(this));

            // Activate the first screens.
            MessageBoxScreen confirmStartMessageBox = new MessageBoxScreen("", false, 2);
            confirmStartMessageBox.Accepted += Intro1EntrySelected;
            screenManager.AddScreen(confirmStartMessageBox, null);

            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);
            //screenManager.AddScreen(new BackgroundScreen(), null);
            //screenManager.AddScreen(new MainMenuScreen(), null);

            // Listen for invite notification events.
            //NetworkSession.InviteAccepted += (sender, e)
                //=> NetworkSessionComponent.InviteAccepted(screenManager, e);

            // To test the trial mode behavior while developing your game,
            // uncomment this line:

            // Guide.SimulateTrialMode = true;
        }


        #endregion

        protected void Intro1EntrySelected(object sender, PlayerIndexEventArgs e)
        {
            MessageBoxScreen confirmStartMessageBox = new MessageBoxScreen("", false, 3);
            confirmStartMessageBox.Accepted += Intro2EntrySelected;
            screenManager.AddScreen(confirmStartMessageBox, null);
        }

        protected void Intro2EntrySelected(object sender, PlayerIndexEventArgs e)
        {
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
        }

        #region Draw


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }


        #endregion
    }


    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (HighSchoolBruisicalGame game = new HighSchoolBruisicalGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}
