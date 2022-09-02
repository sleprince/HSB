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
    class FighterScreen : PicScreen
    {
        ContentManager Content;
        Texture2D[] preview = new Texture2D[6];
        Rectangle position1 = new Rectangle(25, 150, 275, 425);
        Rectangle position2 = new Rectangle(750, 150, 275, 425);
        /// <summary>
        /// Constructor.
        /// </summary>
        public FighterScreen()
            : base("Fighter Selection")
        {
            PicEntry Pic1Entry = new PicEntry(1);
            Pic1Entry.Selected += ConfirmEntrySelected;
            PicEntries.Add(Pic1Entry);

            PicEntry Pic2Entry = new PicEntry(2);
            Pic2Entry.Selected += ConfirmEntrySelected;
            PicEntries.Add(Pic2Entry);

            PicEntry Pic3Entry = new PicEntry(3);
            Pic3Entry.Selected += ConfirmEntrySelected;
            PicEntries.Add(Pic3Entry);

            PicEntry Pic4Entry = new PicEntry(4);
            Pic4Entry.Selected += ConfirmEntrySelected;
            PicEntries.Add(Pic4Entry);

            PicEntry Pic5Entry = new PicEntry(5);
            Pic5Entry.Selected += ConfirmEntrySelected;
            PicEntries.Add(Pic5Entry);

            PicEntry Pic6Entry = new PicEntry(6);
            Pic6Entry.Selected += ConfirmEntrySelected;
            PicEntries.Add(Pic6Entry);
        }

        public override void LoadContent()
        {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content/Menu");
            preview[0] = Content.Load<Texture2D>("Body1");
            preview[1] = Content.Load<Texture2D>("Body2");
            preview[2] = Content.Load<Texture2D>("Body3");
            preview[3] = Content.Load<Texture2D>("Body4");
            preview[4] = Content.Load<Texture2D>("Body5");
            preview[5] = Content.Load<Texture2D>("Body6");
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            if (selection.player1 == 0)
                spriteBatch.Draw(preview[0], position1, Color.White);
            else if (selection.player1 == 1)
                spriteBatch.Draw(preview[1], position1, Color.White);
            else if (selection.player1 == 2)
                spriteBatch.Draw(preview[2], position1, Color.White);
            else if (selection.player1 == 3)
                spriteBatch.Draw(preview[3], position1, Color.White);
            else if (selection.player1 == 4)
                spriteBatch.Draw(preview[4], position1, Color.White);
            else if (selection.player1 == 5)
                spriteBatch.Draw(preview[5], position1, Color.White);

            if (selection.player2 == 0)
                spriteBatch.Draw(preview[0], position2, Color.White);
            else if (selection.player2 == 1)
                spriteBatch.Draw(preview[1], position2, Color.White);
            else if (selection.player2 == 2)
                spriteBatch.Draw(preview[2], position2, Color.White);
            else if (selection.player2 == 3)
                spriteBatch.Draw(preview[3], position2, Color.White);
            else if (selection.player2 == 4)
                spriteBatch.Draw(preview[4], position2, Color.White);
            else if (selection.player2 == 5)
                spriteBatch.Draw(preview[5], position2, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
