#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
#endregion

namespace HighSchoolBruisical
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        NetworkSession networkSession;
        
        SpriteBatch spriteBatch;
        ContentManager content;

        GamePadState gamepadState, lastGamepadState;
        KeyboardState keyboardState, lastKeyboardState;

        // Global content.
        private SpriteFont hudFont;

        private Texture2D winOverlay;
        private Texture2D loseOverlay;
        private Texture2D diedOverlay;

        // Meta-level game state.
        private int levelIndex = -1;
        private Level level;

        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(15);
        
        private const Buttons ContinueButton = Buttons.Start;

        #endregion

        #region Properties


        /// <summary>
        /// The logic for deciding whether the game is paused depends on whether
        /// this is a networked or single player game. If we are in a network session,
        /// we should go on updating the game even when the user tabs away from us or
        /// brings up the pause menu, because even though the local player is not
        /// responding to input, other remote players may not be paused. In single
        /// player modes, however, we want everything to pause if the game loses focus.
        /// </summary>
        new bool IsActive
        {
            get
            {
                if (networkSession == null)
                {
                    // Pause behavior for single player games.
                    return base.IsActive;
                }
                else
                {
                    // Pause behavior for networked games.
                    return !IsExiting;
                }
            }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(NetworkSession networkSession)
        {
            this.networkSession = networkSession;
            
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            gamepadState = lastGamepadState = GamePad.GetState(PlayerIndex.One);
            keyboardState = lastKeyboardState = Keyboard.GetState();

        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content/Gameplay");

            spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);

            // Load fonts
            hudFont = content.Load<SpriteFont>("Fonts/Hud");

            // Load overlay textures
            winOverlay = content.Load<Texture2D>("Overlays/P1WINS");
            loseOverlay = content.Load<Texture2D>("Overlays/P2WINS");
            diedOverlay = content.Load<Texture2D>("Overlays/TIMEOUT");

            //MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(Content.Load<Song>("Sounds/Music"));

            LoadNextLevel();

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            if (IsActive)
            {
                HandleInput();
                level.Update(gameTime);
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public void HandleInput()
        {
            keyboardState = Keyboard.GetState();
            gamepadState = GamePad.GetState(PlayerIndex.One);

            // Exit the game when back is pressed.
            if ((!level.RoundOver) && 
                ((gamepadState.Buttons.Start == ButtonState.Pressed && lastGamepadState.Buttons.Start == ButtonState.Released) ||
                 (keyboardState.IsKeyDown(Keys.Escape) && lastKeyboardState.IsKeyUp(Keys.Escape))))
            {               
                ScreenManager.AddScreen(new PauseMenuScreen(/*networkSession*/), null);
            }
            
            bool continuePressed =
                keyboardState.IsKeyDown(Keys.Space) ||
                gamepadState.IsButtonDown(ContinueButton);

            // Perform the appropriate action to advance the game and
            // to get the player back to playing.
            if (continuePressed)
            {
                if (!level.Player1.IsAlive || !level.Player2.IsAlive)
                {
                    level.StartNewLife();
                }
                else if (level.TimeRemaining == TimeSpan.Zero)
                {
                    ReloadCurrentLevel();
                }
            }

            lastGamepadState = gamepadState;
            lastKeyboardState = keyboardState;

        }

        private void LoadNextLevel()
        {
            // Find the path of the next level.
            string levelPath;

            // Loop here so we can try again when we can't find a level.
            while (true)
            {
                // Try to find the next level. They are sequentially numbered txt files.
                levelPath = String.Format("Levels/{0}.txt", ++levelIndex);
                levelPath = Path.Combine(StorageContainer.TitleLocation, "Content/Gameplay/" + levelPath);
                if (File.Exists(levelPath))
                    break;

                // If there isn't even a level 0, something has gone wrong.
                if (levelIndex == 0)
                    throw new Exception("No levels found.");

                // Whenever we can't find a level, start over again at 0.
                levelIndex = -1;
            }

            // Unloads the content for the current level before loading the next one.
            if (level != null)
                level.Dispose();

            // Load the level.
            level = new Level(content, levelPath, selection.GetLevel(), selection.GetPlayer1(), selection.GetPlayer2());
        }

        private void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
        }

        /// <summary>
        /// Draws the game from background to foreground.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            level.Draw(gameTime, spriteBatch);

            DrawHud();

            spriteBatch.End();
            
            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);

            base.Draw(gameTime);
        }

        private void DrawHud()
        {
            Rectangle titleSafeArea = ScreenManager.GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            // Draw time remaining. Uses modulo division to cause blinking when the
            // player is running out of time.
            string timeString = "TIME: " + level.TimeRemaining.Minutes.ToString("00") + ":" + level.TimeRemaining.Seconds.ToString("00");
            Color timeColor;
            if (level.TimeRemaining > WarningTime ||
                level.RoundOver ||
                (int)level.TimeRemaining.TotalSeconds % 2 == 0)
            {
                timeColor = Color.Yellow;
            }
            else
            {
                timeColor = Color.Red;
            }
            DrawShadowedStringCentre(hudFont, timeString, hudLocation + new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2.0f, 5.0f), timeColor);

            // Draw score
            float timeHeight = hudFont.MeasureString(timeString).Y;
            Color healthStatusColor = Color.GreenYellow;
            if (level.Player1.Health < 60) healthStatusColor = Color.Yellow;
            if (level.Player1.Health < 30) healthStatusColor = Color.Red;
            if (level.Player1.Health <= 0) healthStatusColor = Color.Silver;
            DrawShadowedStringLeft(hudFont, "P1: " + level.Player1.Health.ToString("G3") + "%", hudLocation + new Vector2(5.0f, 5.0f), healthStatusColor);

            healthStatusColor = Color.GreenYellow;
            if (level.Player2.Health < 60) healthStatusColor = Color.Yellow;
            if (level.Player2.Health < 30) healthStatusColor = Color.Red;
            if (level.Player2.Health <= 0) healthStatusColor = Color.Silver;
            DrawShadowedStringRight(hudFont, "P2: " + level.Player2.Health.ToString("G3") + "%", hudLocation + new Vector2((float)ScreenManager.GraphicsDevice.Viewport.Width - 10.0f, 5.0f), healthStatusColor);

            // Determine the status overlay message to show.
            Texture2D status = null;
            if (level.RoundOver)
            {
                if (level.TimeRemaining.Seconds < 1)
                    status = diedOverlay;
                else
                    if (!level.Player2.IsAlive)
                    {
                        status = winOverlay;
                    }
                    else
                    {
                        status = loseOverlay;
                    }
            }

            if (status != null)
            {
                // Draw status message.
                Vector2 statusSize = new Vector2(status.Width, status.Height);
                spriteBatch.Draw(status, center - statusSize / 2, Color.White);
            }
        }

        private void DrawShadowedStringCentre(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(2.0f, 2.0f), Color.Black, 0.0f, new Vector2(font.MeasureString(value).X / 2, 0), 2.0f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(font, value, position, color, 0.0f, new Vector2(font.MeasureString(value).X / 2, 0), 2.0f, SpriteEffects.None, 0.0f);
        }

        private void DrawShadowedStringLeft(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(2.0f, 2.0f), Color.Black, 0.0f, new Vector2(0, 0), 2.0f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(font, value, position, color, 0.0f, new Vector2(0, 0), 2.0f, SpriteEffects.None, 0.0f);
        }

        private void DrawShadowedStringRight(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(2.0f, 2.0f), Color.Black, 0.0f, new Vector2(font.MeasureString(value).X, 0), 2.0f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(font, value, position, color, 0.0f, new Vector2(font.MeasureString(value).X, 0), 2.0f, SpriteEffects.None, 0.0f);
        }


        #endregion
    }
}
