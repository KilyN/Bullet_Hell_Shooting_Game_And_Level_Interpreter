using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _487Game
{
    /// <summary>
    /// Abstract base class for all projectiles. More logistical than pratical, though the polymorphism is nice.
    /// </summary>
    abstract public class Projectile : GameObject
    {
        //Methods
        /// <summary>
        /// Creates and returns a vector representing the difference between two points.
        /// </summary>
        /// <param name="spawnX">X value of the starting point.</param>
        /// <param name="spawnY">Y value of the starting point.</param>
        /// <param name="targetX">X value of the target point.</param>
        /// <param name="targetY">Y value of the target point.</param>
        /// <returns></returns>
        public static Vector2 createVector(float spawnX, float spawnY, float targetX, float targetY)
        {
            return new Vector2(targetX - spawnX, targetY - spawnY);
        }

        /// <summary>
        /// Creates and returns a vector representing the difference between a point and the given player.
        /// </summary>
        /// <param name="spawnX">X value of the starting point.</param>
        /// <param name="spawnY">Y value of the starting point.</param>
        /// <param name="player">The player object to be the target of the vector.</param>
        /// <returns></returns>
        public static Vector2 createVectorToPlayer(float spawnX, float spawnY, Player player)
        {
            return new Vector2(player.X - spawnX, player.Y - spawnY);
        }

        /// <summary>
        /// Checks the position of the given projectile and returns true if said projectile is off the screen. Marks the projectile's 'toDelete' flag to true if off the screen.
        /// </summary>
        /// <param name="bullet">The projectile whom you wish to check the position of.</param>
        /// <param name="graphics">The data structure that contains the size of the window.</param>
        public static void checkOutOfBounds(Projectile bullet, GraphicsDeviceManager graphics)
        {
            float boundingBox = 0;
            if (bullet.myTexture.Height > bullet.myTexture.Width)
                boundingBox = bullet.myTexture.Height;
            else
                boundingBox = bullet.myTexture.Width;

            //Checks if it is offscreen, note this is intentionally made to leave a very large amout of wiggle room such that a projecile can be offscreen and 
            // not be deleted if it has a possability of coming back.
            if (bullet.spritePosition.Y + boundingBox < 0 - graphics.GraphicsDevice.Viewport.Height/4 || bullet.spritePosition.Y - boundingBox > graphics.GraphicsDevice.Viewport.Height * 1.25
                || bullet.spritePosition.X + boundingBox < 0 - graphics.GraphicsDevice.Viewport.Width / 4 || bullet.spritePosition.X - boundingBox > graphics.GraphicsDevice.Viewport.Width * 1.25)
            {
                bullet.toDelete = true;//If the projetile is off screen, mark it to be removed.
            }
        }

        /// <summary>
        /// A function which is called when a projectile collides with any object.
        /// Note a projectile should only collide with objects it is hostile to, 
        /// like a player projectile intersetcing the player shouldn't trigger Collide().
        /// </summary>
        public override void Collide()
        {//We might need more sophisitcated 'dying' logic later.
            this.toDelete = true;
        }
    }


    public class TrollProjectile : Projectile
    {
        private int index = 0;

        public TrollProjectile(Texture2D texture, float startingX, float startingY, Queue<Movement> movementQueue)
        {
            myTexture = texture;
            spritePosition.X = startingX;
            spritePosition.Y = startingY;
            hitbox = new CircularHitBox(myTexture.Width / 2);
            origin.X = myTexture.Width / 2;
            origin.Y = myTexture.Height / 2;
            this.movementQueue = movementQueue;

            Vector2 temp = new Vector2();
            if (!float.IsNaN(movementQueue.Peek().initalLaunchTrajectory.X))
                trajectory = movementQueue.Peek().initalLaunchTrajectory;
            temp.X = trajectory.X;
            temp.Y = trajectory.Y;
            temp.Normalize();
            if (temp.Y > 0)//Derives the rotation angle from the given trajectory.
                RotationAngle = (float)Math.Acos(Vector2.Dot(temp, Vector2.UnitX));
            else
                RotationAngle = (float)(Math.PI * 2 - Math.Acos(Vector2.Dot(temp, Vector2.UnitX)));
        }

        //Methods
        public override void update(GameTime gameTime)
        {
            if (!toDelete)//Don't update the sprite if it needs to be deleted. (This should never need to be used.)
            {
                if (movementQueue == null || movementQueue.Count == 0)
                {
                    toDelete = true; return;//Don't know how to move? Then you must be finished and ready to move on.
                }

                movementQueue.Peek().Move(this, gameTime);

                if (movementQueue.Peek().finished)
                    movementQueue.Dequeue();

                if (index == 100)
                {
                    BulletFactory bulletFactory = BulletFactory.Instance();

                    bulletFactory.requestTrollProjectile(X, Y, (float)((0 * 2 * Math.PI) / 16), 0.06f, movementQueue.Peek().speed*2);
                    bulletFactory.requestTrollProjectile(X, Y, (float)((1 * 2 * Math.PI) / 16), 0.06f, movementQueue.Peek().speed * 2);
                    bulletFactory.requestTrollProjectile(X, Y, (float)((2 * 2 * Math.PI) / 16), 0.06f, movementQueue.Peek().speed * 2);
                    bulletFactory.requestTrollProjectile(X, Y, (float)((3 * 2 * Math.PI) / 16), 0.06f, movementQueue.Peek().speed * 2);
                    bulletFactory.requestTrollProjectile(X, Y, (float)((4 * 2 * Math.PI) / 16), 0.06f, movementQueue.Peek().speed * 2);
                    bulletFactory.requestTrollProjectile(X, Y, (float)((5 * 2 * Math.PI) / 16), 0.06f, movementQueue.Peek().speed * 2);
                    bulletFactory.requestTrollProjectile(X, Y, (float)((6 * 2 * Math.PI) / 16), 0.06f, movementQueue.Peek().speed * 2);
                    bulletFactory.requestTrollProjectile(X, Y, (float)((7 * 2 * Math.PI) / 16), 0.06f, movementQueue.Peek().speed * 2);
                    bulletFactory.requestTrollProjectile(X, Y, (float)((8 * 2 * Math.PI) / 16), 0.06f, movementQueue.Peek().speed * 2);
                    bulletFactory.requestTrollProjectile(X, Y, (float)((9 * 2 * Math.PI) / 16), 0.06f, movementQueue.Peek().speed * 2);
                    bulletFactory.requestTrollProjectile(X, Y, (float)((10 * 2 * Math.PI) / 16), 0.06f, movementQueue.Peek().speed * 2);
                    bulletFactory.requestTrollProjectile(X, Y, (float)((11 * 2 * Math.PI) / 16), 0.06f, movementQueue.Peek().speed * 2);
                    bulletFactory.requestTrollProjectile(X, Y, (float)((12 * 2 * Math.PI) / 16), 0.06f, movementQueue.Peek().speed * 2);
                    bulletFactory.requestTrollProjectile(X, Y, (float)((13 * 2 * Math.PI) / 16), 0.06f, movementQueue.Peek().speed * 2);
                    bulletFactory.requestTrollProjectile(X, Y, (float)((14 * 2 * Math.PI) / 16), 0.06f, movementQueue.Peek().speed * 2);
                    bulletFactory.requestTrollProjectile(X, Y, (float)((15 * 2 * Math.PI) / 16), 0.06f, movementQueue.Peek().speed * 2);
                    toDelete = true;
                }


                index++;
            }
        }

        public override void Collide()
        {
            return;
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            if (!toDelete)
            {
                if (!float.IsNaN(RotationAngle))
                    spriteBatch.Draw(myTexture, spritePosition, null, Color.White, RotationAngle, origin, scale, SpriteEffects.None, 0f);
                else
                    spriteBatch.Draw(myTexture, spritePosition, null, Color.White, 0, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }

    public class TrollProjectile2 : Projectile
    {
        private int index = 0;

        public TrollProjectile2(Texture2D texture, float startingX, float startingY, Queue<Movement> movementQueue)
        {
            myTexture = texture;
            spritePosition.X = startingX;
            spritePosition.Y = startingY;
            hitbox = new CircularHitBox(myTexture.Width / 2);
            origin.X = myTexture.Width / 2;
            origin.Y = myTexture.Height / 2;
            this.movementQueue = movementQueue;

            Vector2 temp = new Vector2();
            if (!float.IsNaN(movementQueue.Peek().initalLaunchTrajectory.X))
                trajectory = movementQueue.Peek().initalLaunchTrajectory;
            temp.X = trajectory.X;
            temp.Y = trajectory.Y;
            temp.Normalize();
            if (temp.Y > 0)//Derives the rotation angle from the given trajectory.
                RotationAngle = (float)Math.Acos(Vector2.Dot(temp, Vector2.UnitX));
            else
                RotationAngle = (float)(Math.PI * 2 - Math.Acos(Vector2.Dot(temp, Vector2.UnitX)));
        }

        //Methods
        public override void update(GameTime gameTime)
        {
            if (!toDelete)//Don't update the sprite if it needs to be deleted. (This should never need to be used.)
            {
                if (movementQueue == null || movementQueue.Count == 0)
                {
                    toDelete = true; return;//Don't know how to move? Then you must be finished and ready to move on.
                }

                movementQueue.Peek().Move(this, gameTime);

                if (movementQueue.Peek().finished)
                    movementQueue.Dequeue();

                if (index % 20 == 0 && index != 0)
                {
                    BulletFactory bulletFactory = BulletFactory.Instance();

                    Vector2 temp = new Vector2(trajectory.X,trajectory.Y);
                    bulletFactory.requestTrollProjectile2(X, Y, Vector2.Transform(temp, Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.PI / 8))),0, movementQueue.Peek().speed* 1.2f);
                    temp = new Vector2(trajectory.X, trajectory.Y);
                    bulletFactory.requestTrollProjectile2(X, Y, Vector2.Transform(temp, Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)-(Math.PI /8)))), 0, movementQueue.Peek().speed *1.2f);

                    //toDelete = true;
                }


                index++;
            }
        }

        public override void Collide()
        {
            return;
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            if (!toDelete)
            {
                if (!float.IsNaN(RotationAngle))
                    spriteBatch.Draw(myTexture, spritePosition, null, Color.White, RotationAngle, origin, scale, SpriteEffects.None, 0f);
                else
                    spriteBatch.Draw(myTexture, spritePosition, null, Color.White, 0, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
