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
    /// Enemy class
    /// </summary>
    public abstract class Enemy : GameObject
    {
        // Fields
        // Player reference
        protected Player _player;
        // Amount of health an enemy has. 
        public int health = 10;
        // Queue of projectiles to be fired by player
        public Queue<Tuple<Projectile, int>> projectilesQueue;
        // Spawn delay index
        public int Delay { get; set; }
        // index to keep track of how long enemy object has been alive.
        public int _index;
        //Draw can be overriden in derived classes if nessisary, (I think)
        public override void draw(SpriteBatch spriteBatch)
        {
            if (!float.IsNaN(RotationAngle))
                spriteBatch.Draw(myTexture, spritePosition, null, Color.White, RotationAngle, origin, scale, SpriteEffects.None, 0f);
            else
                spriteBatch.Draw(myTexture, spritePosition, null, Color.White, (float)Math.PI/2f, origin, scale, SpriteEffects.None, 0f);
        }

        public override void Collide()
        {
            if (!toDelete)
            {
                health -= 10;
                if (health <= 0)
                {
                    toDelete = true;
                }
            }
        }

        // Lots of hardcoding in these methods that can be made more flexible by using variables and letting other parts of the program alter these trajectories
        // Fine for now, but will make these changes at least during second deliverable
    }

    public class BasicEnemy : Enemy
    {
        /// <summary>
        /// Constructor to create a new Basic Enemy
        /// </summary>
        /// <param name="newTexture"></param>
        /// <param name="initPosition"></param>
        /// <param name="player"></param>
        /// <param name="movementQueue"></param>
        /// <param name="delay">optional</param>
        /// <param name="pQueue">optional</param>
        public BasicEnemy(Texture2D newTexture, Vector2 initPosition, Player player, Queue<Movement> movementQueue, int delay = 0, Queue<Tuple<Projectile, int>> pQueue = null, int newHealth = 10)
        {
            myTexture = newTexture;
            spritePosition = initPosition;
            _player = player;
            this.movementQueue = movementQueue;
            this.projectilesQueue = pQueue;
            if (pQueue == null)
            {
                this.projectilesQueue = new Queue<Tuple<Projectile, int>>();
            }
            if (myTexture.Height > 42)
            {
                scale = 42f / myTexture.Height;
            }
            hitbox = new CircularHitBox((myTexture.Width * scale) / 2);
            origin.X = myTexture.Width / 2;
            origin.Y = myTexture.Height / 2;
            Delay = delay;
            _index = 0;
            health = newHealth;
        }

        public override void update(GameTime time)
        {
            // movement logic
            if (movementQueue == null || movementQueue.Count == 0)
            {
                toDelete = true; return;//Don't know how to move? Then you must be finished and ready to move on.
            }

            movementQueue.Peek().Move(this, time);

            if (movementQueue.Peek().finished)//Current Movement finished? Go to the next.
                movementQueue.Dequeue();

            // projectile logic
            if (projectilesQueue.Count > 0 && projectilesQueue.Peek().Item2 == _index)
                SoundEffects.Instance()._bullet.Play(0.5f,0,0);
            while (projectilesQueue.Count > 0 && projectilesQueue.Peek().Item2 == _index)
            {
                  var proj = projectilesQueue.Dequeue().Item1;
                  proj.spritePosition.X = spritePosition.X;
                  proj.spritePosition.Y = spritePosition.Y;
                  BulletFactory.Instance().putProjectile(proj);
            }

            _index++;
        }
    }

    //public class StraightPathEnemy : Enemy
    //{
    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="newTexture">new texture</param>
    //    /// <param name="initPosition">initial position</param>
    //    /// <param name="player">player reference</param>
    //    /// <param name="delay">delay timer</param>
    //    public StraightPathEnemy(Texture2D newTexture, Vector2 initPosition, Player player, int delay)
    //    {
    //        myTexture = newTexture;
    //        spritePosition = initPosition;
    //        _player = player;
    //        _delay = delay;
    //        _index = 0;
    //        active = false;
    //        hitbox = new CircularHitBox(myTexture.Width / 2);
    //        origin.X = myTexture.Width / 2;
    //        origin.Y = myTexture.Height / 2;
    //    }

    //    /// <summary>
    //    /// Stright trajectory
    //    /// </summary>
    //    protected override void MoveEnemy(GameTime time)
    //    {
    //        spritePosition.Y += 3;
    //    }
    //}

    //public class ArcLeftEnemy : Enemy
    //{
    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="newTexture">new texture</param>
    //    /// <param name="initPosition">initial position</param>
    //    /// <param name="player">player reference</param>
    //    /// <param name="delay">delay timer</param>
    //    public ArcLeftEnemy(Texture2D newTexture, Vector2 initPosition, Player player, int delay)
    //    {
    //        myTexture = newTexture;
    //        spritePosition = initPosition;
    //        _player = player;
    //        _delay = delay;
    //        _index = 0;
    //        active = false;
    //        hitbox = new CircularHitBox(myTexture.Width / 2);
    //        origin.X = myTexture.Width / 2;
    //        origin.Y = myTexture.Height / 2;
    //    }

    //    protected override void MoveEnemy(GameTime time)
    //    {
    //        if (_index < 50)
    //        {
    //            spritePosition.Y += 3;
    //        }
    //        else if (_index >= 50 && _index < 230)
    //        {
    //            spritePosition.Y += (float)(3 * Math.Cos((_index - 50) * Math.PI / 360));
    //            spritePosition.X -= (float)(3 * Math.Sin((_index - 50) * Math.PI / 360));
    //        }
    //        else if (_index >= 230)
    //        {
    //            spritePosition.X -= 3;
    //        }
    //    }
    //}

    //public class ArcRightEnemy : Enemy
    //{
    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="newTexture">new texture</param>
    //    /// <param name="initPosition">initial position</param>
    //    /// <param name="player">player reference</param>
    //    /// <param name="delay">delay timer</param>
    //    public ArcRightEnemy(Texture2D newTexture, Vector2 initPosition, Player player, int delay)
    //    {
    //        myTexture = newTexture;
    //        spritePosition = initPosition;
    //        _player = player;
    //        _delay = delay;
    //        _index = 0;
    //        active = false;
    //        hitbox = new CircularHitBox(myTexture.Width / 2);
    //        origin.X = myTexture.Width / 2;
    //        origin.Y = myTexture.Height / 2;
    //    }

    //    protected override void MoveEnemy(GameTime time)
    //    {
    //        if (_index < 50)
    //        {
    //            spritePosition.Y += 3;
    //        }
    //        else if (_index >= 50 && _index < 230)
    //        {
    //            spritePosition.Y += (float)(3 * Math.Cos((_index - 50) * Math.PI / 360));
    //            spritePosition.X += (float)(3 * Math.Sin((_index - 50) * Math.PI / 360));
    //        }
    //        else if (_index >= 230)
    //        {
    //            spritePosition.X += 3;
    //        }
    //    }
    //}

    //public class DownAndLeftEnemy : Enemy
    //{
    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="newTexture">new texture</param>
    //    /// <param name="initPosition">initial position</param>
    //    /// <param name="player">player reference</param>
    //    /// <param name="delay">delay timer</param>
    //    public DownAndLeftEnemy(Texture2D newTexture, Vector2 initPosition, Player player, int delay)
    //    {
    //        myTexture = newTexture;
    //        spritePosition = initPosition;
    //        _player = player;
    //        _delay = delay;
    //        _index = 0;
    //        active = false;
    //        hitbox = new CircularHitBox(myTexture.Width / 2);
    //        origin.X = myTexture.Width / 2;
    //        origin.Y = myTexture.Height / 2;
    //    }

    //    protected override void MoveEnemy(GameTime time)
    //    {

    //        if (_index < 150)
    //        {
    //            spritePosition.Y += 2;
    //        }
    //        else if (_index > 350)
    //        {
    //            spritePosition.X -= 1;
    //        }

    //        // Draw Projectiles
    //        if (_index == 200)
    //        {
    //            BulletFactory bulletFactory = BulletFactory.Instance();
    //            bulletFactory.requestLinearProjectile(X, Y, Projectile.createVectorToPlayer(X, Y, _player));
    //        }
    //        if (_index == 220)
    //        {
    //            BulletFactory bulletFactory = BulletFactory.Instance();
    //            bulletFactory.requestLinearProjectile(X - 5, Y, Projectile.createVectorToPlayer(X - 5, Y, _player));
    //            bulletFactory.requestLinearProjectile(X + 5, Y, Projectile.createVectorToPlayer(X + 5, Y, _player));
    //        }
    //        if (_index == 240)
    //        {
    //            BulletFactory bulletFactory = BulletFactory.Instance();
    //            bulletFactory.requestLinearProjectile(X - 10, Y, Projectile.createVectorToPlayer(X - 10, Y, _player));
    //            bulletFactory.requestLinearProjectile(X + 10, Y, Projectile.createVectorToPlayer(X + 10, Y, _player));
    //            bulletFactory.requestLinearProjectile(X, Y, Projectile.createVectorToPlayer(X, Y, _player));
    //        }
    //    }
    //}

    //public class DownAndRightEnemy : Enemy
    //{
    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="newTexture">new texture</param>
    //    /// <param name="initPosition">initial position</param>
    //    /// <param name="player">player reference</param>
    //    /// <param name="delay">delay timer</param>
    //    public DownAndRightEnemy(Texture2D newTexture, Vector2 initPosition, Player player, int delay)
    //    {
    //        myTexture = newTexture;
    //        spritePosition = initPosition;
    //        _player = player;
    //        _delay = delay;
    //        _index = 0;
    //        active = false;
    //        hitbox = new CircularHitBox(myTexture.Width / 2);
    //        origin.X = myTexture.Width / 2;
    //        origin.Y = myTexture.Height / 2;
    //    }

    //    protected override void MoveEnemy(GameTime time)
    //    {
    //        if (_index < 150)
    //        {
    //            spritePosition.Y += 2;
    //        }
    //        else if (_index > 350)
    //        {
    //            spritePosition.X += 1;
    //        }

    //        // Draw Projectiles
    //        if (_index == 200)
    //        {
    //            BulletFactory bulletFactory = BulletFactory.Instance();
    //            bulletFactory.requestLinearProjectile(X, Y, Projectile.createVectorToPlayer(X, Y, _player));
    //        }
    //        if (_index == 220)
    //        {
    //            BulletFactory bulletFactory = BulletFactory.Instance();
    //            bulletFactory.requestLinearProjectile(X - 5, Y, Projectile.createVectorToPlayer(X - 5, Y, _player));
    //            bulletFactory.requestLinearProjectile(X + 5, Y, Projectile.createVectorToPlayer(X + 5, Y, _player));
    //        }
    //        if (_index == 240)
    //        {
    //            BulletFactory bulletFactory = BulletFactory.Instance();
    //            bulletFactory.requestLinearProjectile(X - 10, Y, Projectile.createVectorToPlayer(X - 10, Y, _player));
    //            bulletFactory.requestLinearProjectile(X + 10, Y, Projectile.createVectorToPlayer(X + 10, Y, _player));
    //            bulletFactory.requestLinearProjectile(X, Y, Projectile.createVectorToPlayer(X, Y, _player));
    //        }
    //    }
    //}

    //public class DownAndBackEnemy : Enemy
    //{
    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="newTexture">new texture</param>
    //    /// <param name="initPosition">initial position</param>
    //    /// <param name="player">player reference</param>
    //    /// <param name="delay">delay timer</param>
    //    public DownAndBackEnemy(Texture2D newTexture, Vector2 initPosition, Player player, int delay)
    //    {
    //        myTexture = newTexture;
    //        spritePosition = initPosition;
    //        _player = player;
    //        _delay = delay;
    //        _index = 0;
    //        active = false;
    //        hitbox = new CircularHitBox(myTexture.Width / 2);
    //        origin.X = myTexture.Width / 2;
    //        origin.Y = myTexture.Height / 2;
    //    }

    //    protected override void MoveEnemy(GameTime time)
    //    {
    //        if (_index < 100)
    //        {
    //            spritePosition.Y += 2;
    //        }
    //        else if(_index == 100)
    //        {
    //            BulletFactory bulletFactory = BulletFactory.Instance();

    //            bulletFactory.requestLinearProjectile(X, Y, Vector2.UnitY, bulletFactory.textureContainer.redProjectile, 50, false);
    //        }
    //        else if (_index > 300)
    //        {
    //            spritePosition.Y -= 2;
    //        }
    //    }
    //}

}
