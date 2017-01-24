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
using System.Threading;

namespace _487Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Member Vairables of the Game
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState oldState;

        Texture2D background;
        Rectangle backgroundframe;

        Player player;
        List<Projectile> playerProjectileList;//A list of all player projectiles objects that exist.


        //Constructor for the Game
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1680;
            graphics.PreferredBackBufferHeight = 920;
            Content.RootDirectory = "Content";
        }


        //Methods of the Game
        protected override void Initialize()
        {
            oldState = Keyboard.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            playerProjectileList = new List<Projectile>();
            player = new Player(graphics,Content.Load<Texture2D>("PlayerSprite"));

            background = Content.Load<Texture2D>("starbackground");
            backgroundframe = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {

            //----Human Input------------------------------------------------------------
            KeyboardState newState = Keyboard.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || newState.IsKeyDown(Keys.Escape))
                this.Exit();


            if (newState.IsKeyDown(Keys.W) || newState.IsKeyDown(Keys.Up))
            {
                player.movePlayerUp();
            }
            if (newState.IsKeyDown(Keys.A) || newState.IsKeyDown(Keys.Left))
            {
                player.movePlayerLeft();
            }
            if (newState.IsKeyDown(Keys.S) || newState.IsKeyDown(Keys.Down))
            {
                player.movePlayerDown();
            }
            if (newState.IsKeyDown(Keys.D) || newState.IsKeyDown(Keys.Right))
            {
                player.movePlayerRight();
            }

            if (newState.IsKeyDown(Keys.LeftShift) || newState.IsKeyDown(Keys.RightShift))//Frezzes the player in place. Why? Why not?
            {
                player.spriteSpeed.X = 0;
                player.spriteSpeed.Y = 0;
            }

            if (newState.IsKeyDown(Keys.Space))
            {//Passes the projectile list to player so it can add it's new projectiles to the list once it makes them.
                player.shoot(playerProjectileList, Content);
            }
            if (newState.IsKeyDown(Keys.NumPad1))
            {
                Random rand = new Random();
                playerProjectileList.Add(new ArcingProjectile(graphics, Content.Load<Texture2D>("projectileUp"), player.spritePosition.X, player.spritePosition.Y, player.RotationAngle, (float)(rand.NextDouble()-0.5)*0.05f));
            }

            if (newState.IsKeyDown(Keys.Q))
            {
                player.RotationAngle -= 0.2f;
            }
            if (newState.IsKeyDown(Keys.E))
            {
                player.RotationAngle += 0.2f;
            }



            //----Updating Game Entities----------------------------------------------------------------------------------
            player.update(gameTime);//Update the player

            foreach(Projectile entity in playerProjectileList)
            {
                entity.update(gameTime);//Updates all the player's projectiles.
            }



            //----Disposing Game Entities------------------------------------------------------------------
            for(int i = 0; i < playerProjectileList.Count; i++)
            {
                if (playerProjectileList[i].toDelete)
                {//Once all the updates are done, go through the list and remove any that need deletion.
                    playerProjectileList.Remove(playerProjectileList[i]);//Once removed from the list the projectile should be without references, and so disposed of.
                    i--;
                }
            }


            // Update saved state. Storing two states are only nessisary to see if a button has just been pressed.
            oldState = newState; 

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(background, backgroundframe, Color.White);//Draw the background.
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            foreach (Projectile entity in playerProjectileList)//Draw the player's projectiles.
            { spriteBatch.Draw(entity.myTexture, entity.spritePosition, null, Color.White, entity.RotationAngle, entity.origin, entity.scale, SpriteEffects.None, 0f); }
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);//Draw the player
            spriteBatch.Draw(player.myTexture, player.spritePosition, null, Color.White, player.RotationAngle, player.origin, player.scale, SpriteEffects.None, 0f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
