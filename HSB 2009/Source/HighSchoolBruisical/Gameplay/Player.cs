using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace HighSchoolBruisical
{
    class Player
    {
        // Animations

        // Active, change depending on crouched state
        private Animation activeIdleAnimation;
        private Animation activeForwardAnimation;
        private Animation activeBackwardAnimation;
        private Animation activeBlockAnimation;
        private Animation activePunchAnimation;
        private Animation activeKickAnimation;
        
        // Universal, ignore crouching state
        private Animation jumpAnimation;
        private Animation jumpKickAnimation;
        private Animation jumpPunchAnimation;
        private Animation headButtAnimation;
        private Animation doubleKickAnimation;
        private Animation dieAnimation;

        private bool victoryTaunted;
        private List<Animation> victoryAnimations = new List<Animation>();

        // General, referenced by active animations
        private Animation standingIdleAnimation;
        private Animation standingForwardAnimation;
        private Animation standingBackwardAnimation;
        private Animation standingBlockAnimation;
        private Animation standingPunchAnimation;
        private Animation standingKickAnimation;
        private Animation crouchingIdleAnimation;
        private Animation crouchingForwardAnimation;
        private Animation crouchingBackwardAnimation;
        private Animation crouchingBlockAnimation;
        private Animation crouchingPunchAnimation;
        private Animation crouchingKickAnimation;

        private SpriteEffects flip = SpriteEffects.None;
        private AnimationPlayer sprite;
        private Texture2D boundingBox;

        // Sounds
        private List<SoundEffect> victorySounds;
        private List<SoundEffect> headButtSounds;
        private List<SoundEffect> jumpKickSounds;
        private List<SoundEffect> jumpPunchSounds;
        private List<SoundEffect> jumpSounds;
        private List<SoundEffect> punchSounds;
        private List<SoundEffect> kickSounds;
        private List<SoundEffect> impactSounds;
        private List<SoundEffect> hitSounds;
        private List<SoundEffect> blockSounds;

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
            set
            {
                if (value > 0) health = value;
                else health = 0;
            }
        }
        int health;

        public int MaxHealth
        {
            get { return MaxHealth; }
        }
        int maxHealth;

        private float previousBottom;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        bool facingRight;

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

        public String CharacterName
        {
            get { return characterName; }
        }
        private String characterName;

        // Constants for controling horizontal movement
        private const float MoveAcceleration = 15000.0f;
        private const float MaxMoveSpeed = 3000.0f;
        private const float GroundDragFactor = 0.58f;
        private const float AirDragFactor = 0.65f;

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -4000.0f;
        private const float GravityAcceleration = 3500.0f;
        private const float MaxFallSpeed = 700.0f;
        private const float JumpControlPower = 0.24f;

        // Collision boxes
        public Rectangle AttackBoxLow;
        public Rectangle AttackBoxHigh;
        public Rectangle BodyBoxLow;
        public Rectangle BodyBoxHigh;

        // Input configuration
        private const float MoveStickScale = 1.0f;
   
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

        private Random randomizer = new Random(73592);

        // Action states

        // Jumping
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;

        // Jump kicking
        private bool isJumpKicking;
        private bool wasJumpKicking;
        private bool jumpKicked;
        private float jumpKickAttackTime;
        private float maxJumpKickTime;
        private float jumpKickHitMoment;

        // Jump punching
        private bool isJumpPunching;
        private bool wasJumpPunching;
        private bool jumpPunched;
        private float jumpPunchAttackTime;
        private float maxJumpPunchTime;
        private float jumpPunchHitMoment;
        
        // Punching
        private bool isPunching;
        private bool wasPunching;
        private bool punched;
        private float punchAttackTime;
        private float maxPunchTime;
        private float punchHitMoment;

        // Kicking
        private bool isKicking;
        private bool wasKicking;
        private bool kicked;
        private float kickAttackTime;
        private float maxKickTime;
        private float kickHitMoment;

        // Head butting
        private bool isHeadButting;
        private bool wasHeadButting;
        private bool headButted;
        private float headButtAttackTime;
        private float maxHeadButtTime;
        private float headButtHitMoment;

        // Double kicking
        private bool isDoubleKicking;
        private bool wasDoubleKicking;
        private bool doubleKicked;
        private float doubleKickAttackTime;
        private float maxDoubleKickTime;
        private float doubleKickHitMoment;
        
        // Blocking
        private bool isBlocking;
        private bool wasBlocking;

        // Crounching
        private bool isCrouching;
        private bool wasCrouching;

        public void ReceiveLowToLowHit()
        {
            if (!wasBlocking)
            {
                Health -= 5;
                hitSounds[randomizer.Next(hitSounds.Count)].Play();
                if (randomizer.NextDouble() < 0.15)
                    impactSounds[randomizer.Next(impactSounds.Count)].Play();
            }
            else
            {
                blockSounds[randomizer.Next(blockSounds.Count)].Play();
                Health -= 2;
            }
        }

        public void ReceiveHighToLowHit()
        {
            if (!wasBlocking)
            {
                Health -= 4;
                hitSounds[randomizer.Next(hitSounds.Count)].Play();
                if (randomizer.NextDouble() < 0.15)
                    impactSounds[randomizer.Next(impactSounds.Count)].Play();
            }
            else
            {
                blockSounds[randomizer.Next(blockSounds.Count)].Play();
                Health -= 1;
            }
        }

        public void ReceiveLowToHighHit()
        {
            if (!wasBlocking)
            {
                Health -= 9;
                hitSounds[randomizer.Next(hitSounds.Count)].Play();
                if (randomizer.NextDouble() < 0.15)
                    impactSounds[randomizer.Next(impactSounds.Count)].Play();
            }
            else
            {
                blockSounds[randomizer.Next(blockSounds.Count)].Play();
                Health -= 3;
            }
        }

        public void ReceiveHighToHighHit()
        {
            if (!wasBlocking)
            {
                Health -= 6;
                hitSounds[randomizer.Next(hitSounds.Count)].Play();
                if (randomizer.NextDouble() < 0.15)
                    impactSounds[randomizer.Next(impactSounds.Count)].Play();
            }
            else
            {
                blockSounds[randomizer.Next(blockSounds.Count)].Play();
                Health -= 2;
            }
        }

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
        public Player(Level level, Vector2 position, PlayerIndex index, String name)
        {
            playerIndex = index;
            characterName = name;

            health = maxHealth = 100;
            
            if (playerIndex == PlayerIndex.One)
                facingRight = true;
            else if (playerIndex == PlayerIndex.Two)
                facingRight = false;

            this.level = level;

            LoadContent();

            // Construct the master list of moves.

            moves = new Move[]
                {
                    new Move("Block",       Buttons.Y) { IsSubMove = true },
                    new Move("Jump",        Buttons.A) { IsSubMove = true },
                    new Move("Jump",        Buttons.A, Buttons.A),
                    new Move("Punch",       Buttons.X) { IsSubMove = true },
                    new Move("Punch",       Buttons.X, Buttons.X),
                    new Move("Kick",        Buttons.B) { IsSubMove = true },
                    new Move("Kick",        Buttons.B, Buttons.B),
                    new Move("Jump Kick",   Buttons.A, Buttons.B),
                    new Move("Jump Kick",   Buttons.B, Buttons.A),
                    new Move("Jump Kick",   Buttons.A | Buttons.B),
                    new Move("Jump Punch",  Buttons.A, Buttons.X),
                    new Move("Jump Punch",  Buttons.X, Buttons.A),
                    new Move("Jump Punch",  Buttons.A | Buttons.X),
                    new Move("Double Kick", Buttons.Y, Buttons.B),
                    new Move("Double Kick", Buttons.B, Buttons.Y),
                    new Move("Double Kick", Buttons.Y | Buttons.B),
                    new Move("Head Butt",   Buttons.Y, Buttons.X),
                    new Move("Head Butt",   Buttons.X, Buttons.Y),
                    new Move("Head Butt",   Buttons.Y | Buttons.X),
                };

            // Construct a move list which will store its own copy of the moves array.
            moveList = new MoveList(moves);

            // Create an InputManager for each player with a sufficiently large buffer.
            inputManager = new InputManager(playerIndex, moveList.LongestMoveLength);
            
            Reset(position);

            UpdateCrouchedState(false);
        }

        public void LoadAllSounds(ref List<SoundEffect> effects, String path)
        {
            int iterator = 0;
            //File.OpenRead(subpath);
            while (File.Exists("Content/Gameplay/" + path + iterator.ToString() + ".xnb"))
            {
                effects.Add(Level.Content.Load<SoundEffect>(path + iterator.ToString()));
                iterator++;
            }
        }


        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        public void LoadContent()
        {
            boundingBox = Level.Content.Load<Texture2D>("Sprites/box");

            activeIdleAnimation = standingIdleAnimation         = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Standing Idel"), 0.12f, true);
            activeForwardAnimation  = standingForwardAnimation  = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Standing forwards walk"), 0.1f, true);
            activeBackwardAnimation = standingBackwardAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Standing backwars walk"), 0.1f, true);
            activeBlockAnimation    = standingBlockAnimation    = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Standing Block"), 0.03f, false);
            activePunchAnimation    = standingPunchAnimation    = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Standing Punch"), 0.04f, false);
            activeKickAnimation     = standingKickAnimation     = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Standing Kick"), 0.07f, false);

            crouchingBackwardAnimation  = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Standing forwards walk"), 0.15f, true);
            crouchingForwardAnimation   = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Standing backwars walk"), 0.15f, true);
            crouchingBlockAnimation     = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Crouch block"), 0.03f, false);
            crouchingIdleAnimation      = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Crouching Idel"), 0.1f, true);
            crouchingKickAnimation      = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Crouch round house kick"), 0.08f, false);
            crouchingPunchAnimation     = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Crouch punch"), 0.05f, false);
            
            jumpAnimation       = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Standing jump"), 0.1f, false);
            jumpKickAnimation   = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Standing jump kick"), 0.12f, false);
            jumpPunchAnimation  = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Jumping swing"), 0.1f, false);
            headButtAnimation   = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Standing Headbutt"), 0.06f, false);
            doubleKickAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/standing hight kick"), 0.1f, false);
            dieAnimation        = new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Death Animation"), 0.09f, false);

            victoryAnimations.Add(new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Breathing Fire"), 0.13f, false));
            victoryAnimations.Add(new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Backflip"), 0.1f, false));
            victoryAnimations.Add(new Animation(Level.Content.Load<Texture2D>("Sprites/" + characterName + "/Whipping Arse"), 0.12f, false));
            
            punchHitMoment = 0.01f;
            jumpKickHitMoment = 0.25f;
            jumpPunchHitMoment = 0.25f;
            doubleKickHitMoment = 0.25f;
            kickHitMoment = 0.25f;
            headButtHitMoment = 0.3f;

            // Calculate bounds within texture size.            
            int width = (int)(activeIdleAnimation.FrameWidth * 0.4);
            int left = (activeIdleAnimation.FrameWidth - width) / 2;
            int height = (int)(activeIdleAnimation.FrameWidth * 0.8);
            int top = activeIdleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

            // Create sound lists
            victorySounds = new List<SoundEffect>();
            headButtSounds = new List<SoundEffect>();
            jumpKickSounds = new List<SoundEffect>();
            jumpPunchSounds = new List<SoundEffect>();
            jumpSounds = new List<SoundEffect>();
            punchSounds = new List<SoundEffect>();
            kickSounds = new List<SoundEffect>();
            impactSounds = new List<SoundEffect>();
            hitSounds = new List<SoundEffect>();
            blockSounds = new List<SoundEffect>();

            // Load sounds
            LoadAllSounds(ref victorySounds, "Sounds/" + characterName + "/victory");
            LoadAllSounds(ref jumpSounds, "Sounds/" + characterName + "/jump");
            LoadAllSounds(ref jumpKickSounds, "Sounds/" + characterName + "/jump");
            LoadAllSounds(ref jumpPunchSounds, "Sounds/" + characterName + "/jump");
            LoadAllSounds(ref punchSounds, "Sounds/" + characterName + "/punch");
            LoadAllSounds(ref kickSounds, "Sounds/" + characterName + "/kick");
            LoadAllSounds(ref headButtSounds, "Sounds/" + characterName + "/headbutt");
            LoadAllSounds(ref impactSounds, "Sounds/" + characterName + "/impact");
            LoadAllSounds(ref hitSounds, "Sounds/hit");
            LoadAllSounds(ref blockSounds, "Sounds/block");

        }

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        public void Reset(Vector2 position)
        {
            Position = position;
            health = maxHealth;
            Velocity = Vector2.Zero;
            victoryTaunted = false;
            isAlive = true;
            sprite.PlayAnimation(activeIdleAnimation);
        }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// Checks for collisions with the target player.
        /// </summary>
        public void Update(GameTime gameTime, Player target)
        {
            GetInput(gameTime);

            ApplyPhysics(gameTime, target);

            isIdle = true;
            
            if (!IsAlive || !IsOnGround) isIdle = false;

            if (isPunching || punchAttackTime > 0.0f) isIdle = false;
            if (isKicking || kickAttackTime > 0.0f) isIdle = false;
            if (isHeadButting || headButtAttackTime > 0.0f) isIdle = false;
            if (isDoubleKicking || doubleKickAttackTime > 0.0f) isIdle = false;

            if (isBlocking) isIdle = false;

            if (IsIdle)
            {
                if (Math.Abs(Velocity.X) - 0.02f > 0)
                {
                    if (Velocity.X > 0)
                        sprite.PlayAnimation(activeForwardAnimation);
                    else
                        sprite.PlayAnimation(activeBackwardAnimation);
                }
                else
                {
                    sprite.PlayAnimation(activeIdleAnimation);
                }
            }

            if (target.Position.X > this.Position.X)
                facingRight = true;
            else facingRight = false;

            // Clear input.
            movement = 0.0f;
            isJumping = false;
            isJumpKicking = false;
            isJumpPunching = false;
            isDoubleKicking = false;
            isHeadButting = false;
            isPunching = false;
            isKicking = false;
            isBlocking = false;
            isCrouching = false;
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
                isKicking = (move.Name == "Kick");
                isJumping = (move.Name == "Jump");
                isPunching = (move.Name == "Punch");
                isHeadButting = (move.Name == "Head Butt");
                isJumpPunching = (move.Name == "Jump Punch");
                isJumpKicking = (move.Name == "Jump Kick");
                isDoubleKicking = (move.Name == "Double Kick");
                isBlocking = (move.Name == "Block");
            }

            if (wasBlocking &&
                (inputManager.GamePadState.IsButtonDown(Buttons.Y) ||
                (inputManager.KeyboardState.IsKeyDown(Keys.Y))))
                isBlocking = true;

            if ((inputManager.GamePadState.IsButtonDown(Buttons.DPadDown)) ||
                (inputManager.KeyboardState.IsKeyDown(Keys.Down)) ||
                (inputManager.GamePadState.ThumbSticks.Left.Y < -0.2))
                isCrouching = true;
                
        }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime, Player target)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            velocity.X += movement * MoveAcceleration * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            DoCrouch();

            velocity.X = DoBlock(velocity.X, gameTime);

            if (!wasBlocking)
            {
                velocity.X = DoPunch(velocity.X, gameTime, target);
                velocity.X = DoKick(velocity.X, gameTime, target);
                velocity.X = DoHeadButt(velocity.X, gameTime, target);
                velocity.X = DoDoubleKick(velocity.X, gameTime, target);

                velocity.Y = DoJump(velocity.Y, gameTime);
                velocity.Y = DoJumpKick(velocity.Y, gameTime, target);
                velocity.Y = DoJumpPunch(velocity.Y, gameTime, target);
            }

            if (wasCrouching) velocity.X = 0;           
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
        /// Checks the collision of low hits
        /// </summary>
        /// <param name="target"></param>
        public bool CheckLowHit(Player target)
        {
            if (RectangleExtensions.GetIntersectionDepth(target.BodyBoxHigh, this.AttackBoxLow) != Vector2.Zero)
            {
                target.ReceiveLowToHighHit();
                return true;
            }

            if (RectangleExtensions.GetIntersectionDepth(target.BodyBoxLow, this.AttackBoxLow) != Vector2.Zero)
            {
                target.ReceiveLowToLowHit();
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Checks the collision of high hits
        /// </summary>
        /// <param name="target"></param>
        public bool CheckHighHit(Player target)
        {
            if (RectangleExtensions.GetIntersectionDepth(target.BodyBoxHigh, this.AttackBoxHigh) != Vector2.Zero)
            {
                target.ReceiveHighToHighHit();
                return true;
            }

            if (RectangleExtensions.GetIntersectionDepth(target.BodyBoxLow, this.AttackBoxHigh) != Vector2.Zero)
            {
                target.ReceiveHighToLowHit();
                return true;
            }

            return false;
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
            if (isJumping || isJumpKicking || isJumpPunching)
            {
                // Begin or continue a jump
                if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
                {
                    if (jumpTime == 0.0f)
                    {
                        wasCrouching = false;
                        jumpSounds[randomizer.Next(jumpSounds.Count)].Play();
                    }

                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (!isJumpKicking && !isJumpPunching) sprite.PlayAnimation(jumpAnimation);
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

        private float DoBlock(float velocityX, GameTime gameTime)
        {
            if (isBlocking)
            {
                sprite.PlayAnimation(activeBlockAnimation);
                wasBlocking = true;
                if (IsOnGround) return 0.0f;
                else return velocityX;
            }
            else
            {
                wasBlocking = false;
                return velocityX;
            }
        }

        private void DoCrouch()
        {
            if (isIdle && isOnGround)
            {
                if (isCrouching && isOnGround)
                {
                    wasCrouching = true;
                    UpdateCrouchedState(true);
                }
                else
                {
                    wasCrouching = false;
                    UpdateCrouchedState(false);
                }
            }
            UpdateCollisionBoxes(wasCrouching);
        }

        private float DoPunch(float velocityX, GameTime gameTime, Player target)
        {
            if (isOnGround)
            {
                // If the player wants to jump
                if (isPunching && punchAttackTime == 0.0f)
                {
                    punched = false;

                    punchSounds[randomizer.Next(punchSounds.Count)].Play();

                    sprite.RestartAnimation(activePunchAnimation);
                    maxPunchTime = activePunchAnimation.FrameTime * activePunchAnimation.FrameCount;
                    punchAttackTime += 0.001f;
                }


                if (wasPunching || (0.0f < punchAttackTime && punchAttackTime < maxPunchTime))
                {
                    punchAttackTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if ((punchAttackTime > punchHitMoment) && (!punched))
                    {
                        punched = CheckHighHit(target);
                    }
                }

                if (0.0f < punchAttackTime && punchAttackTime <= maxPunchTime)
                {
                    velocityX = 0.0f;
                }
                else
                {
                    punchAttackTime = 0.0f;
                }

                if (sprite.Animation != activePunchAnimation)
                    punchAttackTime = 0.0f;
                
                wasPunching = isPunching;
            }
            return velocityX;
        }

        private float DoKick(float velocityX, GameTime gameTime, Player target)
        {
            if (isOnGround)
            {
                if (isKicking && kickAttackTime == 0.0f)
                {
                    kicked = false;                    

                    kickSounds[randomizer.Next(kickSounds.Count)].Play();
                    sprite.RestartAnimation(activeKickAnimation);

                    kickAttackTime += 0.001f;
                    maxKickTime = activeKickAnimation.FrameTime * activeKickAnimation.FrameCount;
                }


                if (wasKicking || (0.0f < kickAttackTime && kickAttackTime < maxKickTime))
                {
                    kickAttackTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if ((kickAttackTime > kickHitMoment) && (!kicked))
                    {
                        kicked = CheckLowHit(target);
                    }
                }

                if (0.0f < kickAttackTime && kickAttackTime <= maxKickTime)
                {
                    velocityX = 0.0f;
                }
                else
                {
                    kickAttackTime = 0.0f;
                }

                if (sprite.Animation != activeKickAnimation)
                    kickAttackTime = 0.0f;

                wasKicking = isKicking;
            }
            return velocityX;
        }

        private float DoDoubleKick(float velocityX, GameTime gameTime, Player target)
        {
            if (isOnGround)
            {
                if (isDoubleKicking && doubleKickAttackTime == 0.0f)
                {                    
                    doubleKicked = false;
                    wasCrouching = false;

                    kickSounds[randomizer.Next(kickSounds.Count)].Play();
                    sprite.RestartAnimation(doubleKickAnimation);

                    doubleKickAttackTime += 0.001f;
                    maxDoubleKickTime = doubleKickAnimation.FrameTime * doubleKickAnimation.FrameCount;
                }


                if (wasDoubleKicking || (0.0f < doubleKickAttackTime && doubleKickAttackTime < maxDoubleKickTime))
                {
                    doubleKickAttackTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if ((doubleKickAttackTime > doubleKickHitMoment) && (!doubleKicked))
                    {
                        doubleKicked = CheckHighHit(target);
                        doubleKicked = CheckHighHit(target);
                    }
                }

                if (0.0f < doubleKickAttackTime && doubleKickAttackTime <= maxDoubleKickTime)
                {
                    velocityX = 0.0f;
                }
                else
                {
                    doubleKickAttackTime = 0.0f;
                }

                if (sprite.Animation != doubleKickAnimation)
                    doubleKickAttackTime = 0.0f;

                wasDoubleKicking = isDoubleKicking;
            }
            return velocityX;
        }

        private float DoHeadButt(float velocityX, GameTime gameTime, Player target)
        {
            if (isOnGround)
            {
                if (isHeadButting && headButtAttackTime == 0.0f)
                {
                    headButted = false;
                    wasCrouching = false;

                    headButtSounds[randomizer.Next(headButtSounds.Count)].Play();
                    sprite.RestartAnimation(headButtAnimation);

                    headButtAttackTime += 0.001f;
                    maxHeadButtTime = headButtAnimation.FrameTime * headButtAnimation.FrameCount;
                }


                if (wasHeadButting || (0.0f < headButtAttackTime && headButtAttackTime < maxHeadButtTime))
                {
                    headButtAttackTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if ((headButtAttackTime > headButtHitMoment) && (!headButted))
                    {
                        headButted = CheckHighHit(target);
                        headButted = CheckHighHit(target);
                    }
                }

                if (0.0f < headButtAttackTime && headButtAttackTime <= maxHeadButtTime)
                {
                    velocityX = 0.0f;
                }
                else
                {
                    headButtAttackTime = 0.0f;
                }

                if (sprite.Animation != headButtAnimation)
                    headButtAttackTime = 0.0f;

                wasHeadButting = isHeadButting;
            }
            return velocityX;
        }

        private float DoJumpKick(float velocityY, GameTime gameTime, Player target)
        {
            if (isJumpKicking && jumpKickAttackTime == 0.0f)
            {
                jumpKicked = false;
                wasCrouching = false;

                jumpKickSounds[randomizer.Next(jumpKickSounds.Count)].Play();
                sprite.RestartAnimation(jumpKickAnimation);

                jumpKickAttackTime += 0.001f;
                maxJumpKickTime = jumpKickAnimation.FrameTime * jumpKickAnimation.FrameCount * 2;
            }
            
            if (wasJumpKicking || (0.0f < jumpKickAttackTime && jumpKickAttackTime < maxJumpKickTime))
            {
                jumpKickAttackTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if ((jumpKickAttackTime > jumpKickHitMoment) && (!jumpKicked))
                {
                    jumpKicked = CheckLowHit(target);
                }
            }

            if (jumpKickAttackTime >= maxJumpKickTime)
            {
                jumpKickAttackTime = 0.0f;
            }

            wasJumpKicking = isJumpKicking;

            return velocityY;
        }

        private float DoJumpPunch(float velocityY, GameTime gameTime, Player target)
        {
            if (isJumpPunching && jumpPunchAttackTime == 0.0f)
            {
                jumpPunched = false;
                wasCrouching = false;

                jumpPunchSounds[randomizer.Next(jumpPunchSounds.Count)].Play();
                sprite.RestartAnimation(jumpPunchAnimation);

                jumpPunchAttackTime += 0.001f;
                maxJumpPunchTime = jumpPunchAnimation.FrameTime * jumpPunchAnimation.FrameCount * 2;
            }

            if (wasJumpPunching || (0.0f < jumpPunchAttackTime && jumpPunchAttackTime < maxJumpPunchTime))
            {
                jumpPunchAttackTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if ((jumpPunchAttackTime > jumpPunchHitMoment) && (!jumpPunched))
                {
                    jumpPunched = CheckHighHit(target);
                }
            }

            if (jumpPunchAttackTime >= maxJumpPunchTime)
            {
                jumpPunchAttackTime = 0.0f;
            }

            wasJumpPunching = isJumpPunching;

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
            sprite.PlayAnimation(dieAnimation);
        }

        public void OnVictory()
        {
            if (!victoryTaunted)
            {
                victorySounds[randomizer.Next(victorySounds.Count)].Play();
                sprite.PlayAnimation(victoryAnimations[randomizer.Next(victoryAnimations.Count)]);
                victoryTaunted = true;
            }
        }

        /// <summary>
        /// Called when this player reaches the level's exit.
        /// </summary>
        public void OnReachedExit()
        {
            sprite.PlayAnimation(activePunchAnimation);
        }

        /// <summary>
        /// Draws the animated player.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Flip the sprite to face the way we are moving.
            if (facingRight)
                flip = SpriteEffects.FlipHorizontally;
            else
                flip = SpriteEffects.None;

            // Draw that sprite.
            sprite.Draw(gameTime, spriteBatch, Position, flip);
        #if DEBUG
            // Draw collision bounding boxes
            spriteBatch.Draw(boundingBox, BodyBoxLow, Color.Yellow);
            spriteBatch.Draw(boundingBox, BodyBoxHigh, Color.Orange);
            spriteBatch.Draw(boundingBox, AttackBoxLow, Color.Green);
            spriteBatch.Draw(boundingBox, AttackBoxHigh, Color.Red);
            
        #endif
        }

        private void UpdateCollisionBoxes(bool crouched)
        {
            if (!crouched)
            {
                if (facingRight)
                {
                    BodyBoxHigh = new Rectangle((int)position.X - 80, (int)position.Y - 350, 100, 100);
                    BodyBoxLow = new Rectangle((int)position.X - 70, (int)position.Y - 250, 100, 150);
                    AttackBoxHigh = new Rectangle((int)position.X + 30, (int)position.Y - 350, 100, 100);
                    AttackBoxLow = new Rectangle((int)position.X + 45, (int)position.Y - 270, 140, 90);
                }
                else
                {
                    BodyBoxHigh = new Rectangle((int)position.X - 20, (int)position.Y - 350, 100, 100);
                    BodyBoxLow = new Rectangle((int)position.X - 30, (int)position.Y - 250, 100, 150);
                    AttackBoxHigh = new Rectangle((int)position.X - 130, (int)position.Y - 350, 100, 100);
                    AttackBoxLow = new Rectangle((int)position.X - 185, (int)position.Y - 270, 140, 90);
                }
            }
            else
            {
                if (facingRight)
                {
                    BodyBoxHigh = new Rectangle((int)position.X - 50, (int)position.Y - 260, 100, 100);
                    BodyBoxLow = new Rectangle((int)position.X - 60, (int)position.Y - 150, 120, 60);
                    AttackBoxHigh = new Rectangle((int)position.X + 60, (int)position.Y - 240, 100, 50);
                    AttackBoxLow = new Rectangle((int)position.X + 45, (int)position.Y - 200, 140, 90);
                }
                else
                {
                    BodyBoxHigh = new Rectangle((int)position.X - 50, (int)position.Y - 260, 100, 100);
                    BodyBoxLow = new Rectangle((int)position.X - 60, (int)position.Y - 150, 120, 60);
                    AttackBoxHigh = new Rectangle((int)position.X - 160, (int)position.Y - 240, 100, 50);
                    AttackBoxLow = new Rectangle((int)position.X - 185, (int)position.Y - 200, 140, 90);
                }
            }
        }

        private void UpdateCrouchedState(bool crouched)
        {
            if (!crouched)
            {
                activeBackwardAnimation = standingBackwardAnimation;
                activeBlockAnimation = standingBlockAnimation;
                activeForwardAnimation = standingForwardAnimation;
                activeIdleAnimation = standingIdleAnimation;
                activeKickAnimation = standingKickAnimation;
                activePunchAnimation = standingPunchAnimation;
            }
            else
            {
                activePunchAnimation = crouchingPunchAnimation;
                activeKickAnimation = crouchingKickAnimation;
                activeIdleAnimation = crouchingIdleAnimation;
                activeForwardAnimation = crouchingForwardAnimation;
                activeBlockAnimation = crouchingBlockAnimation;
                activeBackwardAnimation = crouchingBackwardAnimation;
            }
        }

    }
}
