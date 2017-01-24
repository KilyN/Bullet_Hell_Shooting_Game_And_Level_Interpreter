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
    class CollisionChecker
    {
        /// <summary>
        /// Member variable references to check for collisions between objects
        /// </summary>
        //private List<Enemy> enemyListRef;
        private Player playerRef;
        //private List<Projectile> playerProjectilesRef;
        //public List<Projectile> enemyProjectilesRef;
        private EnemySpawner spawnerReference;
        private BulletFactory bulletFactoryRef;
        private GraphicsDeviceManager graphics;
        
        /// <summary>
        /// Constructor that takes the references to a player and the lists of items we need
        /// </summary>
        /// <param name="enemies"></param>
        /// <param name="player"></param>
        /// <param name="playerProjectiles"></param>
        /// <param name="enemyProjectiles"></param>
        //public CollisionChecker(List<Enemy> enemies, Player player, List<Projectile> playerProjectiles, List<Projectile> enemyProjectiles)
        //{
        //    enemyListRef = enemies;
        //    playerRef = player;
        //    playerProjectilesRef = playerProjectiles;
        //    enemyProjectilesRef = enemyProjectiles;
        //}

        /// <summary>
        /// Constructor that takes references to a player, Enemy Spawner, and Bullet Factory.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="spawner"></param>
        /// <param name="bfactory"></param>
        public CollisionChecker(Player player, EnemySpawner spawner, GraphicsDeviceManager graphics)
        {
            playerRef = player;
            bulletFactoryRef = BulletFactory.Instance();
            spawnerReference = spawner;
            this.graphics = graphics;
        }

        /// <summary>
        /// Void function to check each objects for collisions
        /// </summary>
        public void collisionCheck()
        {
            // Check player against each enemy projectile
            foreach (Projectile p in bulletFactoryRef.enemyProjectileList)
            {
                if(DoesCollide(p, playerRef))
                {
                    break;
                }
            }

            // Check player against each enemy
            foreach(Enemy e in spawnerReference._enemies)
            {
                DoesCollide(playerRef, e);
            }

            // Check each player projectile against each enemy
            foreach(Projectile playerProjectile in bulletFactoryRef.playerProjectileList)
            {
                foreach(Enemy enemy in spawnerReference._enemies)
                {
                    DoesCollide(enemy, playerProjectile);
                }
            }

            // when the player's Bomb collide with enermy, call wipeProjectile()
            foreach (Projectile playerProjectileBomb in bulletFactoryRef.playerProjectileListBomb)
            {
                foreach (Enemy enemy in spawnerReference._enemies)
                {
                    if (DoesCollide(enemy, playerProjectileBomb)||playerRef.bombDisplayTime<=0)
                    {
                         // bulletFactoryRef.wipeProjetiles();
                        BulletFactory.Instance().wipeProjetiles();
                        enemy.health--;
                    }
                }
            }




            // Check each projectile against the bounds of the window.
            foreach (Projectile proj in bulletFactoryRef.enemyProjectileList)
                Projectile.checkOutOfBounds(proj, graphics);//Checks to see if they are out of bounds.

            foreach (Projectile proj in bulletFactoryRef.playerProjectileList)
                Projectile.checkOutOfBounds(proj, graphics);//Checks to see if they are out of bounds.
            
        }

        private bool DoesCollide(GameObject item1, GameObject item2)
        {
            bool collision;

            if(item1.hitbox is RectangularHitBox)
            {
                collision = RectangularCollisionCheck(item1, item2);
            }
            else
            {
                collision = CirclularCollisionCheck(item1, item2);
            }

            if (collision && !item1.toDelete && !item2.toDelete)
            {
                item1.Collide();
                item2.Collide();
            }
            else
                collision = false;

            return collision;
        }

        private bool CirclularCollisionCheck(GameObject item1, GameObject item2)
        {
            if((item1.hitbox as CircularHitBox).Radius + (item2.hitbox as CircularHitBox).Radius > Math.Sqrt(Math.Pow(Math.Abs(item1.X - item2.X), 2.0) + Math.Pow(Math.Abs(item1.Y - item2.Y), 2.0)))
            {
                return true;
            }

            return false;
        }

        // Parameter 1 here is rectangle
        private bool RectangularCollisionCheck(GameObject item1, GameObject item2)
        {
            RectangularHitBox rec = item1.hitbox as RectangularHitBox;
            CircularHitBox circ = item2.hitbox as CircularHitBox;

            var corners = rec.GetCorners(item1.X, item1.Y);

            if(ClosestPointCheck(corners[0].Item1, corners[0].Item2, corners[1].Item1, corners[1].Item2, item2.X, item2.Y, circ.Radius))
            {
                return true;
            }
            if (ClosestPointCheck(corners[1].Item1, corners[1].Item2, corners[2].Item1, corners[2].Item2, item2.X, item2.Y, circ.Radius))
            {
                return true;
            }
            if (ClosestPointCheck(corners[2].Item1, corners[2].Item2, corners[3].Item1, corners[3].Item2, item2.X, item2.Y, circ.Radius))
            {
                return true;
            }
            if (ClosestPointCheck(corners[3].Item1, corners[3].Item2, corners[0].Item1, corners[0].Item2, item2.X, item2.Y, circ.Radius))
            {
                return true;
            }

            return false;
        }

        private bool ClosestPointCheck(float x1, float y1, float x2, float y2, float x, float y, float r)
        {
            float A = x - x1;
            float B = y - y1;
            float C = x2 - x1;
            float D = y2 - y1;

            float dot = A * C + B * D;
            float len_sq = C * C + D * D;
            float param = -1;

            if (len_sq != 0)
            {
                param = dot / len_sq;
            }

            float xx, yy;

            if (param < 0)
            {
                xx = x1;
                yy = y1;
            }
            else if (param > 1)
            {
                xx = x2;
                yy = y2;
            }
            else
            {
                xx = x1 + param * C;
                yy = y1 + param * D;
            }

            float dx = x - xx;
            float dy = y - yy;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            
            if(r > dist)
            {
                return true;
            }

            return false;
        }
    }
}
