using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace CarbonCube
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        static public GraphicsDeviceManager sGraphics;
        static public SpriteBatch sSpriteBatch;
        static public ContentManager sContent;

        static public InputWrapper sControles;

        int mWindowWidth = 1000;
        int mWindowHeight = 700;

        GameManager myGame;

        public Game1()
        {
            this.Window.Title = "Carbon Cube";
            Content.RootDirectory = "Content";
            sContent = Content;

            sGraphics = new GraphicsDeviceManager(this);

            sGraphics.PreferredBackBufferWidth = mWindowWidth;
            sGraphics.PreferredBackBufferHeight = mWindowHeight;

            sControles = new InputWrapper();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            sSpriteBatch = new SpriteBatch(GraphicsDevice);

            myGame = new GameManager();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Game1.sControles.Update();

            if (sControles["Echap"].Etat2 == ButtonState.Pressed)
                Exit();

            myGame.UpdateGame();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            if (Camera.Width < 18f)
            {
                sSpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            }
            else
            {
                sSpriteBatch.Begin();
            }

            myGame.DrawGame();

            sSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
