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
    /// The abstract base class for all objects with a physical presence within the game.
    /// </summary>
    public abstract class GameObject
    {
        //Variables
        /// <summary>
        /// A vector that represnts the speed and direction of the object on the 2D plain.
        /// Keep in mind that for the game, up is -y.
        /// </summary>
        public Vector2 trajectory = Vector2.Zero;
        /// <summary>
        /// A vector representing the postion of the object on the screen, value corresponds to the center of the graphhical sprite.
        /// </summary>
        public Vector2 spritePosition;
        /// <summary>
        /// The texture to be used in drawing the object.
        /// </summary>
        public Texture2D myTexture;
        /// <summary>
        /// The queue which determies how the GameObject will move.
        /// </summary>
        protected Queue<Movement> movementQueue;
        /// <summary>
        /// A vector that represents the spot to be used for any rotation, genneraly it should be the center of the sprite. 
        /// Mesured from the upper-left corner to the center.
        /// </summary>
        public Vector2 origin = Vector2.Zero;
        /// <summary>
        /// The value in radians relating to the angle the object is pointing.
        /// Derived from the spriteSpeed vector, not the other way around.
        /// </summary>
        public float RotationAngle = 0f;
        /// <summary>
        /// The reltive size to the default size of the image the sprite should be drawn as, has not baring on speed or trajectory.
        /// For now it is always 1 for normal but it might not always be so.
        /// </summary>
        protected float scale = 1f;
        /// <summary>
        /// True when you want this GameObject to be deleted, like a projectile that goes off screen.
        /// </summary>
        public bool toDelete = false;
        /// <summary>
        /// The hitbox of the object.
        /// </summary>
        public Hitbox hitbox;


        //Properties
        /// <summary>
        /// The X position at the center of the sprite
        /// </summary>
        public float X
        {
            get { return spritePosition.X; }
            set { spritePosition.X = value; }
        }
        /// <summary>
        /// Y Position at center of sprite
        /// </summary>
        public float Y
        {
            get { return spritePosition.Y; }
            set { spritePosition.Y = value; }
        }
        

        //Methods
        /// <summary>
        /// When overridden in a derived class it handles all the things that should happen to the given object per one update.
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void update(GameTime gameTime);

        /// <summary>
        /// The function which is called when an object collides with another.
        /// </summary>
        public abstract void Collide();

        /// <summary>
        /// The function which when called on a object will draw itself to the spriteBatch.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public abstract void draw(SpriteBatch spriteBatch);
    }
}
