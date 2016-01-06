using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace JumpTest
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // XNA defined stuff
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        // Setting up the screen resolution
        private int ScreenHeight = 600;
        private int ScreenWidth = 800;

        // Define textures
        private Texture2D background;
        private Texture2D Player;
        private Texture2D platform;
        private Texture2D tehEnd;
        private SpriteFont MainFont;

        // Initialize the player
        float PlayerX = 400; // TODO: Make both things more dynamic - AKA allow resolution change.
        float PlayerY = 550; //ScreenHeight - Player.height

        // Get controller state
        private MouseState mouse;

        // Platform coordinate array.
        Random random = new Random((int)DateTime.Now.Ticks);
        public float[] platformCoordinates = new float[20]; // TODO: dynamic arrays ahoy! *ahem* make the size bigger...

        // Physics!
        private float gravity = 2.0f;
        private float movingSpeed = 20.0f;
        private float playerVelocityY = 0.0f;
        private float platformVelocityY = 2.0f;

        private bool jumpOn = false; // False = no jumping, true  = jumping. Determines if the person is jumping or not.

        // FPS Counter (via http://forums.create.msdn.com/forums/p/28402/157777.aspx )
        private int numOfFrames;
        private int FPS;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        void Jump()
        {
            checkCollision();
            if (PlayerY + playerVelocityY <= ScreenHeight-Player.Height/2)
            {
                PlayerY += playerVelocityY;
                playerVelocityY += gravity;
            }
            else { jumpOn = false; }
        }
        float getDistance(Point PlayerCenter, Point PlatformCenter)
        {
            int x = PlayerCenter.X - PlatformCenter.X;
            int y = PlayerCenter.Y - PlatformCenter.Y;
            return (float) (x*x) + (y*y);
        }
        void checkCollision()
        {
            Point PlayerCenter = new Point((int)PlayerX+(Player.Width/2),(int)PlayerY-(Player.Height/2));
            Point PlatformCenter;
            // Obtains the center values and checks them for proximity
            for (int iii=0; iii < platformCoordinates.Length; iii++)
            {
                if (iii%2==0)
                {
                    PlatformCenter = new Point((int)platformCoordinates[iii]+platform.Width/2,(int)platformCoordinates[iii+1]-platform.Height/2);
                    if (getDistance(PlayerCenter, PlatformCenter) < 800.0f)
                    {
                        playerVelocityY = -35.0f;
                    }
                }
            }
        }

        protected override void Initialize()
        {
            // Set the screen resolution 
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.ApplyChanges();
           

            // Load the textures
            background = Content.Load<Texture2D>("background");
            Player = Content.Load<Texture2D>("Player");
            platform = Content.Load<Texture2D>("platform");
            MainFont = Content.Load<SpriteFont>("MainFont");
            tehEnd = Content.Load<Texture2D>("TehEnd");

            this.IsMouseVisible = true;

            loadPlatforms();

            base.Initialize();
        }

        void loadPlatforms()
        {
            // This function loops through platformCoordinates array, and fills it with random coordinates.
            // Note that re-generating new platforms happens further down the code. This function is only
            // used when the game is loading!
            for (int iii = 0; iii < platformCoordinates.Length; iii++)
            {
                    platformCoordinates[iii] = random.Next(0, 600);
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Get controller state 
            mouse = Mouse.GetState();

            // Make the 'esc' key an exit button
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape) == true)
                this.Exit();

            // FPS counter update logic
            if (gameTime.TotalGameTime.Milliseconds == 0)
            {
                FPS = numOfFrames;
                numOfFrames = 0;
            }

            // Mouse movement : movement
            if (mouse.X > PlayerX + (Player.Width / 2) + 10)
            {
                PlayerX += movingSpeed;
            }
            if (mouse.X < PlayerX + (Player.Width / 2) - 10)
            {
                PlayerX -= movingSpeed;
            }

            // Making sure that the player doesn't go out of the game area
            if (PlayerX + Player.Width > ScreenWidth)
            {
                PlayerX = ScreenWidth - Player.Width;
            }
            if (PlayerX <= 0)
            {
                PlayerX = 0;
            }

            // Mouse movement : jumping
            if (jumpOn == true)
            {
                Jump();
            }
            if (!jumpOn && mouse.LeftButton == ButtonState.Pressed)
            {
                playerVelocityY = -35.0f;
                jumpOn = true;
            }
            if (PlayerY <= ScreenHeight/2)
            {
                for (int iii=0; iii < platformCoordinates.Length; iii++)
                {
                    if (iii%2!=0)
                    platformCoordinates[iii] += platformVelocityY;
                    if (platformCoordinates[iii] > ScreenHeight)
                    {
                        platformCoordinates[iii] = 0-platform.Height;
                        platformCoordinates[iii - 1] = random.Next(0, 600);
                    }
                        
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            numOfFrames++; //FPS Counter

            float cords1 = 100.0f; // TODO: Remove
            float cords2 = 50.0f;

            // Drawing
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);

            spriteBatch.DrawString(MainFont, "Press 'esc' to exit!", new Vector2(50, 25), Color.White);
            spriteBatch.DrawString(MainFont, "FPS: " + FPS, new Vector2(50, 50), Color.White);
            spriteBatch.DrawString(MainFont, "Player Pos. (X;Y): " + PlayerX + ";" + PlayerY, new Vector2(50,75),Color.White);          

            // Draw the platforms every frame. For debugging purposes it also tells me the status of each platform.
            for (int iii = 0; iii < platformCoordinates.Length;iii++ )
            {
                if (iii % 2 == 0)
                {
                    spriteBatch.Draw(platform,new Vector2(platformCoordinates[iii],platformCoordinates[iii+1]), Color.White);
                    // DEBUG!
                    if (platformCoordinates[iii] == 0 && platformCoordinates[iii + 1] == 0)
                    {
                        spriteBatch.DrawString(MainFont,
                   "Platform "+ iii/2 + " FAIL. Coordinates: 0;0. ", new Vector2(cords2, cords1), Color.White);
                    }
                    else
                    {
                        spriteBatch.DrawString(MainFont,
                   "Platform " + iii/2 + " OK. coordinates (X;Y): " + platformCoordinates[iii] + ";" +
                   platformCoordinates[iii + 1], new Vector2(cords2, cords1), Color.White);
                    }

                    cords1 += 25f;
                }
            }
            spriteBatch.Draw(Player, new Vector2(PlayerX, PlayerY), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
                                                      