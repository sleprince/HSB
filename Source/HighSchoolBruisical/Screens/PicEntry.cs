#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace HighSchoolBruisical
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    class PicEntry : GameScreen
    {
        #region Fields

        //ContentManager Content;
        //Texture2D pic;
        int texNum;
        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        float selectionFade;

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        /*public Texture2D Pic
        {
            get { return pic; }
            //set { pic = value; }
        }*/


        #endregion

        #region Events


        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }

        public bool EntrySelected()
        {
            if (Selected != null)
                return true;
            else
                return false;
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public PicEntry(int tex)
        {
            texNum = tex;
        }


        #endregion

        /*public virtual void LoadContent(string name)
        {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");
            pic = Content.Load<Texture2D>(name);
        }

        public override void UnloadContent()
        {
            Content.Unload();
        }*/

        #region Update and Draw


        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(PicScreen screen, bool isSelected,
                                                      GameTime gameTime)
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }

        public virtual void Update(PicScreen2 screen, bool isSelected,
                                                      GameTime gameTime)
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }

        public virtual void Update(PicScreen3 screen, bool isSelected,
                                                      GameTime gameTime)
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(PicScreen screen, Rectangle position,
                                 bool isSelected, Color colour, GameTime gameTime)
        {
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? colour : Color.White;

            // Modify the alpha to fade text out during transitions.
            color = new Color(color.R, color.G, color.B, screen.TransitionAlpha);

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Texture2D pic;
            if (texNum == 1)
                pic = screenManager.Tex1;
            else if (texNum == 2)
                pic = screenManager.Tex2;
            else if (texNum == 3)
                pic = screenManager.Tex3;
            else if (texNum == 4)
                pic = screenManager.Tex4;
            else if (texNum == 5)
                pic = screenManager.Tex5;
            else
                pic = screenManager.Tex6;

            spriteBatch.Draw(pic, position, color);
        }

        public virtual void Draw(PicScreen2 screen, Rectangle position,
                                 bool isSelected, GameTime gameTime)
        {
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Red : Color.White;

            // Modify the alpha to fade text out during transitions.
            color = new Color(color.R, color.G, color.B, screen.TransitionAlpha);

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Texture2D pic;
            if (texNum == 1)
                pic = screenManager.Tex11;
            else if (texNum == 2)
                pic = screenManager.Tex12;
            else if (texNum == 3)
                pic = screenManager.Tex13;
            else
                pic = screenManager.Tex14;

            spriteBatch.Draw(pic, position, color);
        }

        public virtual void Draw(PicScreen3 screen, Rectangle position,
                                 bool isSelected, GameTime gameTime)
        {
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Red : Color.White;

            // Modify the alpha to fade text out during transitions.
            color = new Color(color.R, color.G, color.B, screen.TransitionAlpha);

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            Texture2D pic;
            if (texNum == 1)
                pic = screenManager.Tex7;
            else if (texNum == 2)
                pic = screenManager.Tex8;
            else if (texNum == 3)
                pic = screenManager.Tex9;
            else
                pic = screenManager.Tex10;

            spriteBatch.Draw(pic, position, color);
        }

        #endregion
    }
}