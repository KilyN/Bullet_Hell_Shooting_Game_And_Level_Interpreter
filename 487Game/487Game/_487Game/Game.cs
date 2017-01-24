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
using System.Diagnostics;
// commit testing
namespace _487Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        // Member fields (_ denotes non-default field)
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Color backColor = Color.DeepSkyBlue;
        KeystrokeInterpreter _interpreter;
        Player _player;
        SoundEffects _soundEffects;
        GameSettings _settings;
        Background _background; 
        Rectangle _bgFrame;
        EnemySpawner _spawner;
        BulletFactory bulletFactory;
        CollisionChecker collisionChecker;
        LevelInterpreter _levelInterpreter;
        bool gameStarted = false;
        Texture2D _splashScreen;
        
        public Game()
        {
            // Initialize resolution and settings
            _settings = new GameSettings();
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = _settings.Height;
            graphics.PreferredBackBufferWidth = _settings.Width;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _soundEffects = SoundEffects.Instance(Content);
            // Initialize player, interpreter, and background and call starting sound effects
            _player = new Player(Content, new Vector2(_settings.Width / 2, _settings.Height - 100), _settings);
            _interpreter = new KeystrokeInterpreter(_player);
            _levelInterpreter = new LevelInterpreter(@"..\..\..\..\_487GameContent\LevelConfigs", new EnemyGroupFactory(_player, _settings, Content), Content);
            _background = new Background(Content.Load<Texture2D>("starbackground"), _settings, _player);
            _bgFrame = new Rectangle(0, 0, _settings.Width, _settings.Height);
            _spawner = new EnemySpawner();
            bulletFactory = BulletFactory.Instance(Content, _player);
            collisionChecker = new CollisionChecker(_player, _spawner, graphics);
            _splashScreen = Content.Load<Texture2D>("Splash Screen");
            _soundEffects.theBossSong= _soundEffects._bossSong.CreateInstance();
            _soundEffects.theBossSong.IsLooped = true;
            _soundEffects.theBossSong.Volume = 0.9f;
            _soundEffects.theBossSong.Play();
            // Load up level 1
            _spawner.LoadLevel(_levelInterpreter.LoadLevel());

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (!gameStarted)
            {
                if(Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    gameStarted = true;
                    _spawner.splashScreentime.Stop();
                }
            }

            if (!(_player.isfinalBossDie || _player.lives <= 0) && gameStarted)
            {
                // Check Keystrokes
                _interpreter.PerformAction(Keyboard.GetState());

                _player.update(gameTime);

                collisionChecker.collisionCheck();

                // Call Enemy creation function
                _spawner.SpawnAndMoveEnemies(gameTime);

                //Updates all projectiles.
                bulletFactory.updateProjectiles(gameTime);

                _soundEffects.update();

            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

                // Draw background here
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                _background.draw(spriteBatch);
                spriteBatch.End();

                // Draw Enemies here
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                _spawner.DrawEnemies(spriteBatch);
                spriteBatch.End();


                // Draw bullets here
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                bulletFactory.drawProjectiles(spriteBatch);
                spriteBatch.End();

                // Draw player here
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                _player.draw(spriteBatch);
                spriteBatch.End();

            if (!gameStarted)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(_splashScreen, Vector2.Zero, Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
