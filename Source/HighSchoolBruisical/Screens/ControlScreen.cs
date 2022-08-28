#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace HighSchoolBruisical
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class ControlScreen : MenuScreen
    {
        ContentManager Content;
        Texture2D preview;
        Vector2 position = new Vector2(0, 0);
        /// <summary>
        /// Constructor.
        /// </summary>
        public ControlScreen()
            : base("")
        {
            MenuEntry noMenuEntry = new MenuEntry("");
            noMenuEntry.Selected += OnCancel;
            MenuEntries.Add(noMenuEntry);
        }

        public override void LoadContent()
        {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");
            preview = Content.Load<Texture2D>("Menu/Controls");
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(preview, position, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}