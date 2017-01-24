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

namespace _487Game
{
    /// <summary>
    /// PLayer class, holds all components relevent to the player pawn and methods used to alter it. Will inherit from pawn when implemented
    /// </summary>
    public class Player : GameObject
    {
        // Fields
        GameSettings _settings;
        int _speed;
        public int lives;
        public int invulnerabilityTimer = 0;
        public int bombDisplayTime;
        public bool bombIsDisplay = true;
        public int bulletCooldown = 0;

        // for player life display
        public Texture2D playerLife;
        public Rectangle healthRecPlayer;
        public Texture2D youLost;
        public Texture2D playerWin;
        public Texture2D playerImmune;
        public Texture2D bombOnScreen;
        public bool isfinalBossDie = false;
        public bool isBulletCheatOn = false;

        // Getters
        public Vector2 Position => spritePosition;
        public Texture2D Texture => myTexture;



        public Player(ContentManager Content, Vector2 newVector, GameSettings newSettings)
        {
            myTexture = Content.Load<Texture2D>("playerSprite");
            spritePosition = newVector;
            _settings = newSettings;
            _speed = 5;
            lives = 9;
            playerLife = Content.Load<Texture2D>("playerLife");
            if (myTexture.Width > 50)
                scale = 50f / myTexture.Width;
            hitbox = new CircularHitBox((myTexture.Width * scale) / 4);
            youLost = Content.Load<Texture2D>("gameover");
            playerWin = Content.Load<Texture2D>("youwin");
            playerImmune = Content.Load<Texture2D>("PlayerImmune");
            origin.X = myTexture.Width / 2;
            origin.Y = myTexture.Height / 2;
            healthRecPlayer = new Rectangle(0, 0, 20, 20);
            bombOnScreen = Content.Load<Texture2D>("bomb");
        }

        public void ToggleSpeed()
        {
            if (_speed == 5)
                _speed = 10;
            else
                _speed = 5;
        }

        // Method to trigger player cheat code when "Y" and "U" are simultaneously pushed.
        // Restores the player back to full health.
        public void triggerHealthCheatCode()
        {
            lives = 9;
        }


        /// <summary>
        /// Move player method. Accepts key stroke and make corresponding position adjustment (maybe velocity vector alteration in future)
        /// </summary>
        public void MovePlayer(Keys direction)
        {
            if (direction == Keys.Up && spritePosition.Y - (myTexture.Height / 2) > 0)
                spritePosition.Y -= _speed + 1;

            if (direction == Keys.Down && spritePosition.Y + (myTexture.Height / 2) < _settings.Height)
                spritePosition.Y += _speed - 1;

            if (direction == Keys.Left && spritePosition.X - (myTexture.Width / 2) > 0)
                spritePosition.X -= _speed;

            if (direction == Keys.Right && spritePosition.X + (myTexture.Width / 2) < _settings.Width)
                spritePosition.X += _speed;
        }

        //Update handles everything that needs to be updated every turn, such as the tickdown for the InvulnerabilityTimer.
        public override void update(GameTime gameTime)
        {
            if (invulnerabilityTimer > 0)
                invulnerabilityTimer--;
            if (bombDisplayTime > 0)
               { bombDisplayTime--; }
                
            if (bulletCooldown > 0)
                bulletCooldown--;
            if(spritePosition.Y + (myTexture.Height / 2) < _settings.Height)
                spritePosition.Y++;
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            // draw player red deadpool
            Texture2D drawingTexture;
            if (invulnerabilityTimer > 0)
                drawingTexture = playerImmune;
            else
                drawingTexture = myTexture;

            if (!float.IsNaN(RotationAngle))
                spriteBatch.Draw(drawingTexture, spritePosition, null, Color.White, RotationAngle, origin, scale, SpriteEffects.None, 0f);
            else
                spriteBatch.Draw(drawingTexture, spritePosition, null, Color.White, 0, origin, scale, SpriteEffects.None, 0f);

            // draw player live stars 
            for (int i = 1, j = lives; i <= lives; i++, j--)
                spriteBatch.Draw(playerLife, new Rectangle(healthRecPlayer.X + ((lives - j) * 21), _settings.Height - 20, 20, healthRecPlayer.Height), Color.White);

            // display playerWin
            if (isfinalBossDie==true)
                spriteBatch.Draw(playerWin, new Rectangle(_settings.Width / 2 - 180, _settings.Height / 5, 350, 300), Color.White);

            //  display "youLose"
            if (lives <= 0)
                spriteBatch.Draw(youLost, new Rectangle(_settings.Width / 2 - 200, _settings.Height / 5, 400, 300), Color.White);

            // bombDisplay as long as the bombIsDisplay==true
            if (bombIsDisplay==true)
                spriteBatch.Draw(bombOnScreen, new Rectangle(_settings.Width -50, _settings.Height -50, 40, 40), Color.White);
        

        }

        public void Shoot()
        {
            BulletFactory bulletFactory = BulletFactory.Instance();
            // If cheat for new projectils is not activated
            if (!isBulletCheatOn)
            {
                if (bulletCooldown == 0)
                {
                    SoundEffects.Instance()._bullet.Play(0.6f,0,0);
                    bulletFactory.requestLinearProjectile(X, Y, -Vector2.UnitY, bulletFactory.textureContainer.orangeProjectile, 900, true);
                    bulletCooldown += 15;
                }
            }

            // if cheat is activated
            if (isBulletCheatOn)
            {
                float radianval = 0;
                // Loop 16 times to create 16 projectiles around boss and fire outwards
                for (int i = 0; i < 16; i++)
                {
                    radianval += (float)(Math.PI * 2) / 16;
                    bulletFactory.requestLinearProjectile(X, Y, radianval, bulletFactory.textureContainer.orangeProjectile, 900, true);
                }
            }
        }

        public void shootBoom()

        {
            BulletFactory bulletFactory = BulletFactory.Instance();

            bombDisplayTime = 50;  // display the bomb for afew ticks
            bulletFactory.requestBomb(X, Y, -Vector2.UnitY, bulletFactory.textureContainer.bomb, 300, true);
            


        }
        public override void Collide()
        {
            if (invulnerabilityTimer == 0)
            {
                lives--;

                if (lives > 0)
                {
                    spritePosition.X = _settings.Width / 2;//Reset back to starting position.
                    spritePosition.Y = _settings.Height - 30;

                    invulnerabilityTimer += 40;//Make the player invulnerable.

                    BulletFactory.Instance().wipeProjetiles();//Get rid of all the projectiles on the screen.
                    
                }
            }
        }
    }
}
