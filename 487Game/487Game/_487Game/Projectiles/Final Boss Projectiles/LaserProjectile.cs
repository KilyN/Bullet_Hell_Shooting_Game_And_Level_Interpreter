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
    /// A projectile for the final boss, it corisponds to the D1 attack. Note, this projectile is very different from others.
    /// </summary>
    class LaserProjectile : Projectile
    {
        //Member Variables
        /// <summary>
        /// How long the projectile is currently.
        /// </summary>
        private int length;
        /// <summary>
        /// The maximum length the lazer is allowed to be.
        /// </summary>
        private const int maxLength = 300;
        /// <summary>
        /// The width of the laser.
        /// </summary>
        private int thickness = 5; 
        /// <summary>
        /// The visual rectangle representing the laser.
        /// </summary>
        private Rectangle rect;
        /// <summary>
        /// The color of the laser.
        /// </summary>
        private Color color = Color.Red;
        private Vector2 spriteSpeed;
        private float projectileSpeed = 100;

        //Constructors
        /// <summary>
        /// Constructor that takes a launch angle rather than a trajectory vector as one of it's parameters.
        /// Has an optional speed parameter.
        /// </summary>
        /// <param name="startingX">The X value you wish to spawn the center of the bullet on.</param>
        /// <param name="startingY">The Y value you wish to spawn the center of the bullet on.</param>
        /// <param name="launchAngle">The angle, in radians, clockwise from the postive x axis, that the projectile moves on.</param>
        /// <param name="speed">An optional parameter where you can customize the speed of the bullet.</param>
        public LaserProjectile(ContentManager Content, float startingX, float startingY, float launchAngle, float speed = 100f)
        {
            myTexture = Content.Load<Texture2D>("blankTexture");

            spritePosition.X = startingX;
            spritePosition.Y = startingY;
            origin.X = myTexture.Width / 2;
            origin.Y = myTexture.Height / 2;

            projectileSpeed = speed;

            RotationAngle = launchAngle;

            spriteSpeed.X = (float)Math.Cos(RotationAngle);
            spriteSpeed.Y = (float)Math.Sin(RotationAngle);
            spriteSpeed *= projectileSpeed;
        }

        /// <summary>
        /// Constructor that takes a Vector rather than an angle as one of it's parameters.
        /// Has an optional speed parameter.
        /// </summary>
        /// <param name="startingX">The X value you wish to spawn the center of the bullet on.</param>
        /// <param name="startingY">The Y value you wish to spawn the center of the bullet on.</param>
        /// <param name="trajectory">The vector direction the bullet travels in. Scale is irrelevant. WARNNING:The given vetcor is modified during the construction of the bullet! </param>
        /// <param name="speed">An optional parameter where you can customize the speed of the bullet.</param>
        public LaserProjectile(ContentManager Content, float startingX, float startingY, Vector2 trajectory, float speed = 100f)
        {
            myTexture = Content.Load<Texture2D>("blankTexture");

            spritePosition.X = startingX;
            spritePosition.Y = startingY;
            origin.X = myTexture.Width / 2;
            origin.Y = myTexture.Height / 2;

            projectileSpeed = speed;

            trajectory.Normalize();
            if (trajectory.Y > 0)//Derives the rotation angle from the given trajectory.
                RotationAngle = (float)Math.Acos(Vector2.Dot(trajectory, Vector2.UnitX));
            else
                RotationAngle = (float)(Math.PI * 2 - Math.Acos(Vector2.Dot(trajectory, Vector2.UnitX)));

            spriteSpeed = trajectory * projectileSpeed;
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
                Vector2 step = new Vector2();
                step.X = spriteSpeed.X;
                step.Y = spriteSpeed.Y;
                step.Normalize();

                spriteSpeed += (step * 2);

                length = (int)Vector2.Distance(spritePosition, spriteSpeed + spritePosition);

                if (length > maxLength)
                {
                    spritePosition += (step * 2);
                    length = maxLength;
                }
                rect = new Rectangle((int)spritePosition.X, (int)spritePosition.Y, length, thickness);
                hitbox = new RectangularHitBox(rect.Height, rect.Width, RotationAngle);
            }
        }

        /// <summary>
        /// As this projecile has to create and modify it's own sprite it has extra special drawing logic.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void draw(SpriteBatch spriteBatch)
        {
            if (!toDelete)
            {
                spriteBatch.Draw(myTexture, rect, null, color, RotationAngle, Vector2.Zero, SpriteEffects.None, 0.0f);
            }
        }
    }
}
