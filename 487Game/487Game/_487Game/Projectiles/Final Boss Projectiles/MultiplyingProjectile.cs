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
    /// One of the final boss' projectiles, corisponds to attack C1 in my document.
    /// Note that they don't move exactly like the projectiles in the demo, but that's just for extra style.
    /// </summary>
    class MultiplyingProjectile : Projectile
    {
        //Member Variables
        /// <summary>
        /// How many updates the projectile has been though. Used for staging.
        /// </summary>
        private int index = 0;
        /// <summary>
        /// So the bullet can tell if it is a parent or child bullet, as they behave differently.
        /// </summary>
        private int type;
        private Vector2 spriteSpeed;
        private float projectileSpeed = 100;

        //Constructors
        /// <summary>
        /// Constructor that takes a launch angle rather than a trajectory vector as one of it's parameters.
        /// </summary>
        /// <param name="texture">The texture you wish the bullet to have.</param>
        /// <param name="startingX">The X value you wish to spawn the center of the bullet on.</param>
        /// <param name="startingY">The Y value you wish to spawn the center of the bullet on.</param>
        /// <param name="trajectory">The vector direction the bullet travels in. Scale is irrelevant. WARNNING:The given vetcor is modified during the construction of the bullet! </param>
        /// <param name="startingIndex">The index used to spesify what stage the projectile starts in, it is optional and defaults to zero.</param>
        /// <param name="Type">Indicates what type the projectile is, parent or child. 0 == parent, 1 == child</param>
        public MultiplyingProjectile(Texture2D texture, float startingX, float startingY, float launchAngle, int startingIndex = 0, int Type = 0)
        {
            myTexture = texture;
            spritePosition.X = startingX;
            spritePosition.Y = startingY;
            hitbox = new CircularHitBox(myTexture.Width / 2);
            index = startingIndex;
            type = Type;

            origin.X = myTexture.Width / 2;
            origin.Y = myTexture.Height / 2;

            RotationAngle = launchAngle;

            spriteSpeed.X = (float)Math.Cos(RotationAngle);
            spriteSpeed.Y = (float)Math.Sin(RotationAngle);
            spriteSpeed *= projectileSpeed;
        }

        /// <summary>
        /// Constructor that takes a Vector rather than an angle as one of it's parameters.
        /// </summary>
        /// <param name="texture">The texture you wish the bullet to have.</param>
        /// <param name="startingX">The X value you wish to spawn the center of the bullet on.</param>
        /// <param name="startingY">The Y value you wish to spawn the center of the bullet on.</param>
        /// <param name="launchAngle">The angle, in radians, clockwise from the postive x axis, that the projectile moves on.</param>
        /// <param name="startingIndex">The index used to spesify what stage the projectile starts in, it is optional and defaults to zero.</param>
        /// <param name="Type">Indicates what type the projectile is, parent or child. 0 == parent, 1 == child</param>
        public MultiplyingProjectile(Texture2D texture, float startingX, float startingY, Vector2 trajectory, int startingIndex = 0, int Type = 0)
        {
            myTexture = texture;
            spritePosition.X = startingX;
            spritePosition.Y = startingY;
            hitbox = new CircularHitBox(myTexture.Width / 2);
            index = startingIndex;
            type = Type;

            origin.X = myTexture.Width / 2;
            origin.Y = myTexture.Height / 2;

            trajectory.Normalize();
            if (trajectory.Y > 0)//Derives the rotation angle from the given trajectory.
                RotationAngle = (float)Math.Acos(Vector2.Dot(trajectory, Vector2.UnitX));
            else
                RotationAngle = (float)(Math.PI * 2 - Math.Acos(Vector2.Dot(trajectory, Vector2.UnitX)));

            spriteSpeed = trajectory * projectileSpeed;
        }


        //Methods
        /// <summary>
        /// Updates the projectile's movment, handles everything from staging to orientation.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void update(GameTime gameTime)
        {
            if (!toDelete)//Don't update the sprite if it needs to be deleted. (This should never need to be used.)
            {
                index++;

                if(index == 1)//Begin Stage 1: Move outward.
                {
                    projectileSpeed = 100;
                    spriteSpeed.Normalize();
                    spriteSpeed *= projectileSpeed;
                }
                else if(index == 50)//Begin Stage 2: Split and reflect.
                {
                    //Create new child projectile.
                    Vector2 childVector = Vector2.Transform(spriteSpeed, Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)-Math.PI / 4)));
                    BulletFactory bulletFactory = BulletFactory.Instance();
                    bulletFactory.requestMultiplyingProjectile(spritePosition.X, spritePosition.Y, childVector, bulletFactory.textureContainer.redProjectile, 50, 1);

                    //Relfect yourself.
                    spriteSpeed = Vector2.Transform(spriteSpeed, Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.PI/4)));

                    if (spriteSpeed.Y > 0)//Updates the RotationAngle of the projectile so the sprite visual can know to point that direction as well.
                        RotationAngle = (float)Math.Acos(Vector2.Dot(spriteSpeed, Vector2.UnitX) / (spriteSpeed.Length() * Vector2.UnitX.Length()));
                    else
                        RotationAngle = (float)(Math.PI * 2 - Math.Acos(Vector2.Dot(spriteSpeed, Vector2.UnitX) / (spriteSpeed.Length() * Vector2.UnitX.Length())));
                }
                else if(index == 100)//Begin Stage 3: Reflect
                {
                    if (type == 0)//Reflects yourself, one condition for parent, one for child.
                        spriteSpeed = Vector2.Transform(spriteSpeed, Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)-Math.PI / 2)));
                    else
                        spriteSpeed = Vector2.Transform(spriteSpeed, Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.PI / 2)));

                    if (spriteSpeed.Y > 0)//Updates the RotationAngle of the projectile so the sprite visual can know to point that direction as well.
                        RotationAngle = (float)Math.Acos(Vector2.Dot(spriteSpeed, Vector2.UnitX) / (spriteSpeed.Length() * Vector2.UnitX.Length()));
                    else
                        RotationAngle = (float)(Math.PI * 2 - Math.Acos(Vector2.Dot(spriteSpeed, Vector2.UnitX) / (spriteSpeed.Length() * Vector2.UnitX.Length())));
                }
                else if(index == 150)//Begin Stage 4: Split and reflect
                {
                    if (type == 0)//Parent creates new child, reflects
                    {
                        Vector2 childVector = Vector2.Transform(spriteSpeed, Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)((Math.PI /2)+(Math.PI / 16)))));
                        BulletFactory bulletFactory = BulletFactory.Instance();
                        bulletFactory.requestMultiplyingProjectile(spritePosition.X, spritePosition.Y, childVector, bulletFactory.textureContainer.greenProjectile, 150, 0);

                        spriteSpeed = Vector2.Transform(spriteSpeed, Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)((Math.PI / 2) - (Math.PI / 16)))));
                    }
                    else//Childs simply reflects.
                    {
                        spriteSpeed = Vector2.Transform(spriteSpeed, Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)-Math.PI / 2)));
                        spriteSpeed *= 0.85f;
                    }

                    if (spriteSpeed.Y > 0)//Updates the RotationAngle of the projectile so the sprite visual can know to point that direction as well.
                        RotationAngle = (float)Math.Acos(Vector2.Dot(spriteSpeed, Vector2.UnitX) / (spriteSpeed.Length() * Vector2.UnitX.Length()));
                    else
                        RotationAngle = (float)(Math.PI * 2 - Math.Acos(Vector2.Dot(spriteSpeed, Vector2.UnitX) / (spriteSpeed.Length() * Vector2.UnitX.Length())));
                }

                // Move the sprite by speed, scaled by elapsed time.
                spritePosition += spriteSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        /// <summary>
        /// This is the generic draw function for most projectiles. Note it is different than other draw functions in main as it supports rotation.
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
