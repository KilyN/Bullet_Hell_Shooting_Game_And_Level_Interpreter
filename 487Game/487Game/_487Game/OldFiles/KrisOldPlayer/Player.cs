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

namespace _487Game
{
    //The player object is a centralization of all data relating to the function and movment of the player in game.
    //It should try to have the player object handle most things about the player itself so we shouldn't regularly have...
    //outside game elements changing the sprite's position for example.
    class Player : GameObject
    {

        //----Member Variables--------------------------------------------------------------------------------------------------------------------------------------

        //--Constant Member Variables----------------
        private const float topSpeed = 400f;//Maximum speed of the player. (Can surpass this but past it no more velocity can bee added.)
        
        //A value that allows the player to more easaly change their momentum if they try to accelrate in the opposite direction of their movment.
        private const float reverseMomentumCoefficient = 0.8f;//The smaller the value the easier it is to change direction.

        private const float playerAcceleration = 20f;//The rate at which the player accelerates in a direction corrisponding to the pressed movement keys.

        //Determines how quickly the player loses speed when not under acceleration.
        private const float playerDragCoefficient = 0.99f;//If the value is below 1 they slow to a stop, above or equal 1 and they never stop moving.

        private const int bulletShootingCooldown = 0;//How long, total, the player has to wait before fireing again.

        //--Non-constant Member Variables------------       
        //Reference to the main app's Graphics Manager, nessisary for sprite to check and correct it's own collison with the window.
        private GraphicsDeviceManager graphics;

        private int bulletShootingDelay = 0; //How long, currently, till the player can fire again. 0 = free to fire.


        //----Constructors-------------------------------------------------------------------------------------------------------------------------------------
        public Player(GraphicsDeviceManager ngraphics, Texture2D texture)
        {
            graphics = ngraphics;
            myTexture = texture;

            origin.X = myTexture.Width / 2;
            origin.Y = myTexture.Height / 2;

            spritePosition.Y = graphics.GraphicsDevice.Viewport.Height/2;//Spawns the player by default in the center of the screen.
            spritePosition.X = graphics.GraphicsDevice.Viewport.Width / 2;
        }

        //----Methods------------------------------------------------------------------------------------------------------------------------------------------
        public override void update(GameTime gameTime)
        {
            // Move the sprite by speed, scaled by elapsed time.
            spritePosition += spriteSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            int MaxX = graphics.GraphicsDevice.Viewport.Width - myTexture.Width/2;
            int MinX = myTexture.Width / 2;
            int MaxY = graphics.GraphicsDevice.Viewport.Height - myTexture.Height/2;
            int MinY = myTexture.Height / 2;

            // Check for bounce.
            if (spritePosition.X > MaxX)
            {
                spriteSpeed.X *= -0.25f;
                spritePosition.X = MaxX;
            }

            else if (spritePosition.X < MinX)
            {
                spriteSpeed.X *= -0.25f;
                spritePosition.X = MinX;
            }

            if (spritePosition.Y > MaxY)
            {
                spriteSpeed.Y *= -0.25f;
                spritePosition.Y = MaxY;
            }

            else if (spritePosition.Y < MinY)
            {
                spriteSpeed.Y *= -0.25f;
                spritePosition.Y = MinY;
            }

            //Handles air drag, so player stops eventually.
            if (spriteSpeed.Length() > playerAcceleration)
            {
                spriteSpeed *= playerDragCoefficient;
            }
            else if (spriteSpeed.Length() < playerAcceleration)
            {
                spriteSpeed = Vector2.Zero;
            }

            //Incriments the shooting cooldown.
            if (bulletShootingDelay > 0)
            {
                bulletShootingDelay--;
            }
        }


        //These move methods are called when you press one of the corrisponding movement keys. (WASD)
        public void movePlayerUp()
        {
            if (spriteSpeed.Y > 0)
            {
                spriteSpeed.Y *= reverseMomentumCoefficient;
            }
            if (Math.Abs(spriteSpeed.Y) < topSpeed)
            {
                spriteSpeed.Y -= playerAcceleration;
            }
        }

        public void movePlayerDown()
        {
            if (spriteSpeed.Y < 0)
            {
                spriteSpeed.Y *= reverseMomentumCoefficient;
            }
            if (Math.Abs(spriteSpeed.Y) < topSpeed)
            {
                spriteSpeed.Y += playerAcceleration;
            }
        }

        public void movePlayerLeft()
        {
            if (spriteSpeed.X > 0)
            {
                spriteSpeed.X *= reverseMomentumCoefficient;
            }
            if (Math.Abs(spriteSpeed.X) < topSpeed)
            {
                spriteSpeed.X -= playerAcceleration;
            }
        }

        public void movePlayerRight()
        {
            if (spriteSpeed.X < 0)
            {
                spriteSpeed.X *= reverseMomentumCoefficient;
            }
            if (Math.Abs(spriteSpeed.X) < topSpeed)
            {
                spriteSpeed.X += playerAcceleration;
            }
        }

        public void shoot(List<Projectile> playerProjectileList, ContentManager Content)
        {
            if (bulletShootingDelay <= 0)
            {//Spwans a bullet traveling in the direction the player is facing.
                playerProjectileList.Add(new LinearProjectile(graphics, Content.Load<Texture2D>("projectileUp"), spritePosition.X, spritePosition.Y, RotationAngle));
                bulletShootingDelay = bulletShootingCooldown;
            }
        }

    }
}
