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
    class CreditScreen : MenuScreen
    {
        ContentManager Content;
        Texture2D preview;
        float y = 500;
        //Vector2 position = new Vector2(0, 0);
        /// <summary>
        /// Constructor.
        /// </summary>
        public CreditScreen()
            : base("")
        {
            MenuEntry noMenuEntry = new MenuEntry("");
            noMenuEntry.Selected += OnCancel;
            MenuEntries.Add(noMenuEntry);
        }

        public override void LoadContent()
        {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content/Menu");
            preview = Content.Load<Texture2D>("Credits");
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(preview, new Vector2(400, y -= 0.5f), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}