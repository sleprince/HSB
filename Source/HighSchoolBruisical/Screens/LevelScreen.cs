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
    class LevelScreen : PicScreen2
    {
        ContentManager Content;
        Texture2D[] preview = new Texture2D[4];
        Rectangle position = new Rectangle(50, 125, 400, 450);
        /// <summary>
        /// Constructor.
        /// </summary>
        public LevelScreen()
            : base("Level Selection")
        {
            PicEntry Pic1Entry = new PicEntry(1);
            Pic1Entry.Selected += LevelEntrySelected;
            PicEntries.Add(Pic1Entry);

            PicEntry Pic2Entry = new PicEntry(2);
            Pic2Entry.Selected += LevelEntrySelected;
            PicEntries.Add(Pic2Entry);

            PicEntry Pic3Entry = new PicEntry(3);
            Pic3Entry.Selected += LevelEntrySelected;
            PicEntries.Add(Pic3Entry);

            PicEntry Pic4Entry = new PicEntry(4);
            Pic4Entry.Selected += LevelEntrySelected;
            PicEntries.Add(Pic4Entry);
        }

        public override void LoadContent()
        {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content/Menu");
            preview[0] = Content.Load<Texture2D>("level_room");
            preview[1] = Content.Load<Texture2D>("level_yard");
            preview[2] = Content.Load<Texture2D>("level_gym");
            preview[3] = Content.Load<Texture2D>("level_hallway");
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            if (selection.level == 0)
                spriteBatch.Draw(preview[0], position, Color.White);
            else if (selection.level == 1)
                spriteBatch.Draw(preview[1], position, Color.White);
            else if (selection.level == 2)
                spriteBatch.Draw(preview[2], position, Color.White);
            else if (selection.level == 3)
                spriteBatch.Draw(preview[3], position, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
