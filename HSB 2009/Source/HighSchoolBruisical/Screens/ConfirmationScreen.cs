#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
#endregion

namespace HighSchoolBruisical
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class ConfirmationScreen : MenuScreen2
    {
        #region Fields


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public ConfirmationScreen(bool toexitgame)
            : base("")
        {
            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;

            // Add the Yes and No menu entry.
            MenuEntry YesMenuEntry = new MenuEntry("Yes");
            if (toexitgame)
                YesMenuEntry.Selected += ConfirmExitAccepted;
            else
                YesMenuEntry.Selected += ConfirmQuitAccepted;
            MenuEntries.Add(YesMenuEntry);

            MenuEntry NoMenuEntry = new MenuEntry("No");
            NoMenuEntry.Selected += OnCancel;
            MenuEntries.Add(NoMenuEntry);
        }


        #endregion


        #region Draw


        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            string message = "Are you sure?";

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width-210, viewport.Height-200);
            Vector2 textSize = font.MeasureString(message);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            // Fade the popup alpha during transitions.
            Color color = new Color(255, 255, 255, TransitionAlpha);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, message, textPosition, color);
            spriteBatch.End();

            base.Draw(gameTime);
        }


        #endregion
    }
}