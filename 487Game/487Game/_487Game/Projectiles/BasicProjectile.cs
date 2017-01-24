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
    /// <summary>
    /// Basic Projectile object
    /// </summary>
    public class BasicProjectile : Projectile
    {
        //Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture">The texture you wish the bullet to have.</param>
        /// <param name="startingX">The X value you wish to spawn the center of the bullet on.</param>
        /// <param name="startingY">The Y value you wish to spawn the center of the bullet on.</param>
        /// <param name="movementQueue">The movement queue for the object. </param>
        public BasicProjectile(Texture2D texture, float startingX, float startingY, Queue<Movement> movementQueue)
        {
            myTexture = texture;
            spritePosition.X = startingX;
            spritePosition.Y = startingY;
            hitbox = new CircularHitBox(myTexture.Width / 2);
            origin.X = myTexture.Width / 2;
            origin.Y = myTexture.Height / 2;
            this.movementQueue = movementQueue;

            Vector2 temp = new Vector2();
            if(!float.IsNaN(movementQueue.Peek().initalLaunchTrajectory.X))
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
        /// <summary>
        /// For now this merely updates the bullets position, nothing else.
        /// </summary>
        /// <param name="gameTime"></param>
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
            }
        }

        /// <summary>
        /// This is the generic draw function for most projectiles. Note it is different than other draw functions as it supports rotation.
        /// </summary>
        /// <param name="spriteBatch"></param>
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
