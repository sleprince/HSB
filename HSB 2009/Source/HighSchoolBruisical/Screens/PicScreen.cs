#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace HighSchoolBruisical
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class PicScreen : GameScreen
    {
        #region Fields

        List<PicEntry> picEntries = new List<PicEntry>();
        int selectedEntry = 0;
        int selectedEntry2 = 2;
        string picTitle;

        #endregion

        #region Properties


        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<PicEntry> PicEntries
        {
            get { return picEntries; }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public PicScreen(string picTitle)
        {
            this.picTitle = picTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            // Accept or cancel the menu? We pass in our null, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex;

            // Move to the next menu entry up?
            if (input.IsMenuUp(PlayerIndex.One))
            {
                selectedEntry -= 3;
                soundBank.PlayCue("menumove");
                if (selectedEntry < 0)
                    selectedEntry = picEntries.Count - 1;
                if (selectedEntry == selectedEntry2)
                {
                    selectedEntry -= 1;
                    if (selectedEntry < 0)
                        selectedEntry = picEntries.Count - 1;
                }
            }

            // Move to the next menu entry down?
            if (input.IsMenuDown(PlayerIndex.One))
            {
                selectedEntry += 3;
                soundBank.PlayCue("menumove");
                if (selectedEntry >= picEntries.Count)
                    selectedEntry = 0;
                if (selectedEntry == selectedEntry2)
                {
                    selectedEntry += 1;
                    if (selectedEntry >= picEntries.Count)
                        selectedEntry = 0;
                }
            }
            // Move to the previous menu entry left?
            if (input.IsMenuLeft(PlayerIndex.One))
            {
                selectedEntry--;
                soundBank.PlayCue("menumove");
                if (selectedEntry < 0)
                    selectedEntry = picEntries.Count - 1;
                if (selectedEntry == selectedEntry2)
                {
                    selectedEntry -= 1;
                    if (selectedEntry < 0)
                        selectedEntry = picEntries.Count - 1;
                }
            }

            // Move to the next menu entry right?
            if (input.IsMenuRight(PlayerIndex.One))
            {
                selectedEntry++;
                soundBank.PlayCue("menumove");
                if (selectedEntry >= picEntries.Count)
                    selectedEntry = 0;
                if (selectedEntry == selectedEntry2)
                {
                    selectedEntry += 1;
                    if (selectedEntry >= picEntries.Count)
                        selectedEntry = 0;
                }
            }

            selection.SetPlayer1(selectedEntry);

            if (input.IsMenuSelect(PlayerIndex.One, out playerIndex))
            {
                soundBank.PlayCue("menuselect");
                OnSelectEntry(selectedEntry, playerIndex);
            }
            else if (input.IsMenuCancel(PlayerIndex.One, out playerIndex))
            {
                soundBank.PlayCue("menuback");
                OnCancel(playerIndex);
            }

            // Move to the next menu entry up?
            if (input.IsMenuUp(PlayerIndex.Two))
            {
                selectedEntry2 -= 3;
                soundBank.PlayCue("menumove");
                if (selectedEntry2 < 0)
                    selectedEntry2 = picEntries.Count - 1;
                if (selectedEntry2 == selectedEntry)
                {
                    selectedEntry2 -= 1;
                    if (selectedEntry2 < 0)
                        selectedEntry2 = picEntries.Count - 1;
                }
            }

            // Move to the next menu entry down?
            if (input.IsMenuDown(PlayerIndex.Two))
            {
                selectedEntry2 += 3;
                soundBank.PlayCue("menumove");
                if (selectedEntry2 >= picEntries.Count)
                    selectedEntry2 = 0;
                if (selectedEntry2 == selectedEntry)
                {
                    selectedEntry2 += 1;
                    if (selectedEntry2 >= picEntries.Count)
                        selectedEntry2 = 0;
                }
            }
            // Move to the previous menu entry left?
            if (input.IsMenuLeft(PlayerIndex.Two))
            {
                selectedEntry2--;
                soundBank.PlayCue("menumove");
                if (selectedEntry2 < 0)
                    selectedEntry2 = picEntries.Count - 1;
                if (selectedEntry2 == selectedEntry)
                {
                    selectedEntry2 -= 1;
                    if (selectedEntry2 < 0)
                        selectedEntry2 = picEntries.Count - 1;
                }
            }

            // Move to the next menu entry right?
            if (input.IsMenuRight(PlayerIndex.Two))
            {
                selectedEntry2++;
                soundBank.PlayCue("menumove");
                if (selectedEntry2 >= picEntries.Count)
                    selectedEntry2 = 0;
                if (selectedEntry2 == selectedEntry)
                {
                    selectedEntry2 += 1;
                    if (selectedEntry2 >= picEntries.Count)
                        selectedEntry2 = 0;
                }
            }

            selection.SetPlayer2(selectedEntry2);

            if (input.IsMenuSelect(PlayerIndex.Two, out playerIndex))
            {
                soundBank.PlayCue("menuselect");
                OnSelectEntry(selectedEntry2, playerIndex);
            }
            else if (input.IsMenuCancel(PlayerIndex.Two, out playerIndex))
            {
                soundBank.PlayCue("menuback");
                OnCancel(playerIndex);
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            picEntries[selectedEntry].OnSelectEntry(playerIndex);
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }


        protected void ConfirmEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen("", false, 1);

            confirmQuitMessageBox.Accepted += FighterEntrySelected;

            ScreenManager.AddScreen(confirmQuitMessageBox, null);
        }


        protected void FighterEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new LevelScreen(), e.PlayerIndex);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < picEntries.Count; i++)
            {
                bool isSelected = IsActive && ((i == selectedEntry) || (i == selectedEntry2));

                picEntries[i].Update(this, isSelected, gameTime);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            Rectangle texPosition = new Rectangle(325, 150, 100, 100);

            int picSpacing = 150;

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            /*if (ScreenState == ScreenState.TransitionOn)
                position.Y -= transitionOffset * 256;
            else
                position.Y += transitionOffset * 512;*/

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < picEntries.Count; i++)
            {
                PicEntry picEntry = picEntries[i];
                Color colour;

                bool isSelected = IsActive && ((i == selectedEntry) || (i == selectedEntry2));

                if (i == selectedEntry)
                    colour = Color.Red;
                else if (i == selectedEntry2)
                    colour = Color.Blue;
                else
                    colour = Color.White;

                picEntry.Draw(this, texPosition, isSelected, colour, gameTime);

                texPosition.X += picSpacing;

                if (i == 2)
                {
                    texPosition.X = 325;
                    texPosition.Y += picSpacing;
                }
            }

            // Draw the menu title.
            Vector2 titlePosition = new Vector2(533, 80);
            Vector2 titleOrigin = font.MeasureString(picTitle) / 2;
            Color titleColor = new Color(192, 192, 192, TransitionAlpha);
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, picTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }


        #endregion
    }
}
