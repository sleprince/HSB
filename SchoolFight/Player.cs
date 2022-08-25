using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace SchoolFight
{
    /// <summary>
    /// Our fearless adventurer!
    /// </summary>
    class Player
    {
        // Animations
        private Animation idleAnimation;
        private Animation runAnimation;
        private Animation jumpAnimation;
        private Animation punchAnimation;
        private Animation dieAnimation;
        private SpriteEffects flip = SpriteEffects.None;
        private AnimationPlayer sprite;

        // Sounds
        private SoundEffect killedSound;
        private SoundEffect jumpKickSound;
        private SoundEffect jumpSound;
        private SoundEffect punchSound;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        // Physics state
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        public int Health
        {
            get { return health; }
            set { health = value; }
        }
        int health;

        public int MaxHealth
        {//NEW
            get { return maxHealth; }
            set { maxHealth = value; }
        }
        int maxHealth;

        private float previousBottom;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        // This is the master list of moves in logical order. This array is kept
        // around in order to draw the move list on the screen in this order.
        Move[] moves;
        // The move list used for move detection at runtime.
        MoveList moveList;

        // The move list is used to match against an input manager for each player.
        InputManager inputManager;
        // Stores each players' most recent move and when they pressed it.
        Move playerMove;
        TimeSpan playerMoveTime;

        // Time until the currently "active" move dissapears from the screen.
        readonly TimeSpan MoveTimeOut = TimeSpan.FromSeconds(0.1);

        private PlayerIndex playerIndex;


#if ZUNE
        // Constants for controling horizontal movement
        private const float MoveAcceleration = 7000.0f;
        private const float MaxMoveSpeed = 1000.0f;
        private const float GroundDragFactor = 0.38f;
        private const float AirDragFactor = 0.48f;

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -2000.0f;
        private const float GravityAcceleration = 1700.0f;
        private const float MaxFallSpeed = 450.0f;
        private const float JumpControlPower = 0.13f;

        // Input configuration
        private const float MoveStickScale = 0.0f;
        private const Buttons JumpButton = Buttons.B;        
#else
        // Constants for controling horizontal movement
        private const float MoveAcceleration = 14000.0f;
        private const float MaxMoveSpeed = 2000.0f;
        private const float GroundDragFactor = 0.58f;
        private const float AirDragFactor = 0.65f;

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -4000.0f;
        private const float GravityAcceleration = 3500.0f;
        private const float MaxFallSpeed = 600.0f;
        private const float JumpControlPower = 0.14f;

        // Other constants
        private const float MaxPunchTime = 1.07f;
        private const float MaxJumpKickTime = 0.75f;
        
        // Collision areas
        private const float MinXLowDistance = 30.0f;
        private const float MaxXLowDistance = 90.0f;
        private const float MinYLowDistance = 0.0f;
        private const float MaxYLowDisatnce = 60.0f;

        private const float MinXHighDistance = 30.0f;
        private const float MaxXHighDistance = 90.0f;
        private const float MinYHighDistance = 0.0f;
        private const float MaxYHighDisatnce = 60.0f;

        // Input configuration
        private const float MoveStickScale = 1.0f;
   
#endif

        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        public bool IsIdle
        {
            get { return isIdle; }
        }
        bool isIdle;

        /// <summary>
        /// Current user movement input.
        /// </summary>
        private float movement;

        // Action states
        private float attackTime;

        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;

        private bool isJumpKicking;
        private bool wasJumpKicking;
        private float jumpKickAttackTime;

        private bool isPunching;
        private bool wasPunching;
        private float punchAttackTime;

        private Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this player in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        /// <summary>
        /// Constructors a new player.
        /// </summary>
        public Player(Level level, Vector2 position, PlayerIndex index)
        {
            playerIndex = index;
            this.level = level;
            health = 100;

            LoadContent();

            // Construct the master list of moves.

            moves = new Move[]
                {
                    new Move("Jump",        Buttons.A) { IsSubMove = true },
                    new Move("Jump",        Buttons.A, Buttons.A),
                    new Move("Punch",       Buttons.X),
                    new Move("Jump Kick",   Buttons.A, Buttons.X),
                    new Move("Jump Kick",   Buttons.A | Buttons.X),
                    
                };

            // Construct a move list which will store its own copy of the moves array.
            moveList = new MoveList(moves);

            // Create an InputManager for each player with a sufficiently large buffer.
            inputManager = new InputManager(playerIndex, moveList.LongestMoveLength);
            
            Reset(position);
        }

        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        public void LoadContent()
        {
            // Load animated textures.
            idleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Idle"), 0.1f, true);
            runAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Run"), 0.1f, true);
            jumpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Jump"), 0.1f, false);
            punchAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Celebrate"), 0.1f, false);
            dieAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Die"), 0.1f, false);

            // Calculate bounds within texture size.            
            int width = (int)(idleAnimation.FrameWidth * 0.4);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.8);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

            // Load sounds.            
            killedSound = Level.Content.Load<SoundEffect>("Sounds/PlayerKilled");
            jumpSound = Level.Content.Load<SoundEffect>("Sounds/PlayerJump");
            punchSound = Level.Content.Load<SoundEffect>("Sounds/MonsterKilled");
            jumpKickSound = Level.Content.Load<SoundEffect>("Sounds/PlayerKilled");
        }

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            isAlive = true;
            sprite.PlayAnimation(idleAnimation);
        }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// Checks for collisions with the target player.
        /// </summary>
        public void Update(GameTime gameTime, Player target)
        {
            GetInput(gameTime);

            ApplyPhysics(gameTime);

            CheckCollisions(target);

            isIdle = true;
            
            if (!IsAlive || !IsOnGround) isIdle = false;

            if (isPunching || punchAttackTime > 0.0f) isIdle = false;

            if (IsIdle)
            {
                if (Math.Abs(Velocity.X) - 0.02f > 0)
                {
                    sprite.PlayAnimation(runAnimation);
                }
                else
                {
                    sprite.PlayAnimation(idleAnimation);
                }
            }

            // Clear input.
            movement = 0.0f;
            isJumping = false;
            isJumpKicking = false;
            isPunching = false;
        }

        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        private void GetInput(GameTime gameTime)
        {
            // Expire old moves.
            if (gameTime.TotalRealTime - playerMoveTime > MoveTimeOut)
            {
                playerMove = null;
            }

            inputManager.Update(gameTime);

            // Detection and record the current player's most recent move.
            Move newMove = moveList.DetectMove(inputManager);
            if (newMove != null)
            {
                playerMove = newMove;
                playerMoveTime = gameTime.TotalRealTime;
            }

            // Get analog horizontal movement.
            movement = inputManager.GamePadState.ThumbSticks.Left.X * MoveStickScale;

            // Ignore small movements to prevent running in place.
            if (Math.Abs(movement) < 0.5f)
                movement = 0.0f;

            // If any digital horizontal movement input is found, override the analog movement.
            if (inputManager.GamePadState.IsButtonDown(Buttons.DPadLeft) ||
                inputManager.KeyboardState.IsKeyDown(Keys.Left))
            {
                movement = -1.0f;
            }
            else if (inputManager.GamePadState.IsButtonDown(Buttons.DPadRight) ||
                     inputManager.KeyboardState.IsKeyDown(Keys.Right))
            {
                movement = 1.0f;
            }

            Move move = playerMove;

            // Check if the player wants to jump.
            if (move != null)
            {
                isJumping = (move.Name == "Jump");
                isPunching = (move.Name == "Punch");
                isJumpKicking = (move.Name == "Jump Kick");
            }
                
        }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            attackTime = 0.0f;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            velocity.X += movement * MoveAcceleration * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            velocity.Y = DoJump(velocity.Y, gameTime);
            velocity.Y = DoJumpKick(velocity.Y, gameTime);

            velocity.X = DoHighPunch(velocity.X, gameTime);

            // Apply pseudo-drag horizontally.
            if (IsOnGround)
                velocity.X *= GroundDragFactor;
            else
                velocity.X *= AirDragFactor;

            // Prevent the player from running faster than his top speed.            
            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Apply velocity.
            Position += velocity * elapsed;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            // If the player is now colliding with the level, separate them.
            HandleCollisions();

            // If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == previousPosition.X)
                velocity.X = 0;

            if (Position.Y == previousPosition.Y)
                velocity.Y = 0;
        }

        /// <summary>
        /// Checks the collision with the target
        /// </summary>
        /// <param name="target"></param>
        public void CheckCollisions(Player target)
        {
            Vector2 distance = new Vector2(0, 0);
            distance.X = Math.Abs(this.Position.X - target.Position.X);
            distance.Y = Math.Abs(this.Position.Y - target.Position.Y);



        }


        /// <summary>
        /// Calculates the Y velocity accounting for jumping and
        /// animates accordingly.
        /// </summary>
        /// <remarks>
        /// During the accent of a jump, the Y velocity is completely
        /// overridden by a power curve. During the decent, gravity takes
        /// over. The jump velocity is controlled by the jumpTime field
        /// which measures time into the accent of the current jump.
        /// </remarks>
        /// <param name="velocityY">
        /// The player's current velocity along the Y axis.
        /// </param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        private float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (isJumping || isJumpKicking)
            {
                                    //NEW
                    if ((health > 0) && (jumpTime == 0.0f))
                    {
                        health = health - 10;
                    }
                // Begin or continue a jump
                if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
                {
                    if (jumpTime == 0.0f)
                        jumpSound.Play();

                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    sprite.PlayAnimation(jumpAnimation);
                }

                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    jumpTime = 0.0f;

                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;

            return velocityY;
        }

        private float DoHighPunch(float velocityX, GameTime gameTime)
        {
            if (isOnGround)
            {
                // If the player wants to jump
                if (isPunching && attackTime == 0.0f)
                {
                    punchSound.Play();
                    sprite.RestartAnimation(punchAnimation);
                }


                if (wasPunching || (0.0f < punchAttackTime && punchAttackTime < MaxPunchTime))
                {
                    punchAttackTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (0.0f < punchAttackTime && punchAttackTime <= MaxPunchTime)
                {
                    velocityX = 0.0f;
                }
                else
                {
                    punchAttackTime = 0.0f;
                }

                if (sprite.Animation != punchAnimation)
                    punchAttackTime = 0.0f;
                
                attackTime = Math.Max(attackTime, punchAttackTime);
                wasPunching = isPunching;
            }
            return velocityX;
        }

        private float DoJumpKick(float velocityY, GameTime gameTime)
        {
            if (isJumpKicking && jumpKickAttackTime == 0.0f)
            {
                jumpKickSound.Play();
                sprite.RestartAnimation(dieAnimation);
            }
            
            if (wasJumpKicking || (0.0f < jumpKickAttackTime && jumpKickAttackTime < MaxJumpKickTime))
            {
                jumpKickAttackTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (jumpKickAttackTime >= MaxJumpKickTime)
            {
                jumpKickAttackTime = 0.0f;
            }

            attackTime = Math.Max(attackTime, jumpKickAttackTime);
            wasJumpKicking = isJumpKicking;

            return velocityY;
        }

        /// <summary>
        /// Detects and resolves all collisions between the player and his neighboring
        /// tiles. When a collision is detected, the player is pushed away along one
        /// axis to prevent overlapping. There is some special logic for the Y axis to
        /// handle platforms which behave differently depending on direction of movement.
        /// </summary>
        private void HandleCollisions()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            // Reset flag to search for ground collision.
            isOnGround = false;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = Level.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = Level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;

                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable || IsOnGround)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable) // Ignore platforms.
                            {
                                // Resolve the collision along the X axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }

            // Save the new bounds bottom.
            previousBottom = bounds.Bottom;
        }

        /// <summary>
        /// Called when the player has been killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This parameter is null if the player was
        /// not killed by an enemy (fell into a hole).
        /// </param>
        public void OnKilled()
        {
            isAlive = false;
            killedSound.Play();

            sprite.PlayAnimation(dieAnimation);
        }

        /// <summary>
        /// Called when this player reaches the level's exit.
        /// </summary>
        public void OnReachedExit()
        {
            sprite.PlayAnimation(punchAnimation);
        }

        /// <summary>
        /// Draws the animated player.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Flip the sprite to face the way we are moving.
            if (Velocity.X > 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Velocity.X < 0)
                flip = SpriteEffects.None;

            // Draw that sprite.
            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }

    }
}
