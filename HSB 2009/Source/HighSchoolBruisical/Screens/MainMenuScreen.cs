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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
#endregion

namespace HighSchoolBruisical
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : PicScreen3
    {
        ContentManager Content;
        Texture2D title;
        Rectangle position = new Rectangle(375, 0, 290, 300);

        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("")
        {
            // Create our menu entries.
            PicEntry PlayMenuEntry = new PicEntry(1);
            PlayMenuEntry.Selected += FighterMenuEntrySelected;
            PicEntries.Add(PlayMenuEntry);

            PicEntry HowMenuEntry = new PicEntry(2);
            HowMenuEntry.Selected += ControlMenuEntrySelected;
            PicEntries.Add(HowMenuEntry);

            PicEntry CreditMenuEntry = new PicEntry(3);
            CreditMenuEntry.Selected += CreditMenuEntrySelected;
            PicEntries.Add(CreditMenuEntry);

            PicEntry ExitMenuEntry = new PicEntry(4);
            ExitMenuEntry.Selected += OnCancel;
            PicEntries.Add(ExitMenuEntry);
        }


        #endregion

        #region Handle Input


        void FighterMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new FighterScreen(), e.PlayerIndex);
        }

        void ControlMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new ControlScreen(), e.PlayerIndex);
        }

        void CreditMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new CreditScreen(), e.PlayerIndex);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.AddScreen(new ConfirmationScreen(true), playerIndex);
        }

        public override void LoadContent()
        {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");
            title = Content.Load<Texture2D>("Menu/logo");
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(title, position, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }


        #endregion
    }
}
