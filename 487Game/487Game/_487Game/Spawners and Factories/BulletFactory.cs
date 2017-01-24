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
    /// The central object in which all projectiles are maintained and disposed of.
    /// Note that this is a singleton class, as such there can only ever be but a single BulletFactory.
    /// </summary>
    public class BulletFactory
    {
        /// <summary>
        /// A custom container class which holds loaded projectile textures. 
        /// Because it is public requestors of projectiles can access it to spesify what texture they want their projectiles to be without having to load it themselves.
        /// It also saves the time it takes to load a texture, whatever that may be.
        /// </summary>
        public class ProjectileTextureContainer
        {
            //Member Variables
            public readonly Texture2D defaultProjectile;
            public readonly Texture2D redProjectile;
            public readonly Texture2D orangeProjectile;
            public readonly Texture2D yellowProjectile;
            public readonly Texture2D greenProjectile;
            public readonly Texture2D blueProjectile;
            public readonly Texture2D purpleProjectile;
            public readonly Texture2D bomb;

            //Contructor
            public ProjectileTextureContainer(ContentManager Content)
            {
                defaultProjectile = Content.Load<Texture2D>("projectileUp");
                redProjectile = Content.Load<Texture2D>("projectileRed");
                orangeProjectile = Content.Load<Texture2D>("projectileOrange");
                yellowProjectile = Content.Load<Texture2D>("projectileYellow");
                greenProjectile = Content.Load<Texture2D>("projectileGreen");
                blueProjectile = Content.Load<Texture2D>("projectileBlue");
                purpleProjectile = Content.Load<Texture2D>("projectilePurple");
                bomb = Content.Load<Texture2D>("bomb");
            }
        }


        //Member Variables
        /// <summary>
        /// A reference to the single possible BulletFactory in existence.
        /// </summary>
        private static BulletFactory instance;
        /// <summary>
        /// The one and only list of enemy projectiles, it is BulletFactoy's job to maintain it.
        /// </summary>
        public List<Projectile> enemyProjectileList;
        /// <summary>
        /// The one and only list of the player's projectiles, it is BulletFactoy's job to maintain it.
        /// </summary>
        public List<Projectile> playerProjectileList;
        /// <summary>
        /// player projectileListBomb
        /// </summary>
        public List<Projectile> playerProjectileListBomb;

        /// <summary>
        /// Refernece to main's Content object, used for loading textures.
        /// </summary>
        private ContentManager Content;
        /// <summary>
        /// Reference to the player, needed to know when the player is invuruable and so when bullets cannot be created.
        /// </summary>
        private Player player;
        /// <summary>
        /// A custom container class which holds loaded projectile textures. 
        /// Because it is public requestors of projectiles can access it to spesify what texture they want their projectiles to be without having to load it themselves.
        /// </summary>
        public ProjectileTextureContainer textureContainer;


        //Constructor
        /// <summary>
        /// It is private so no one besides a method of the BulletFactory can create it.
        /// </summary>
        private BulletFactory(ContentManager Content, Player player)
        {
            this.Content = Content;
            this.player = player;

            enemyProjectileList = new List<Projectile>();
            playerProjectileList = new List<Projectile>();
            playerProjectileListBomb = new List<Projectile>();
            textureContainer = new ProjectileTextureContainer(Content);
        }


        //Sudo-Constructors
        /// <summary>
        /// Used to gain a reference to main's unique BulletFactory from which objects can request bullets. Must be initalized in main first.
        /// </summary>
        /// <returns>Returns a reference to the single bullet factory. Request bullets from it.</returns>
        public static BulletFactory Instance()
        {
            if (instance == null)
                throw new NullReferenceException();

            return instance;
        }

        /// <summary>
        /// Overload of Instance which is used once in main to initalize the first and only BulletFactory object.
        /// This overload should never be called more than once and will throw an exception if it is.
        /// </summary>
        /// <returns>Returns a reference to the single bullet factory. Request bullets from it.</returns>
        public static BulletFactory Instance(ContentManager Content, Player player)
        {
            if (instance != null)
                throw new Exception();
            
            instance = new BulletFactory(Content, player);
            return instance;
        }


        //Methods
        //-----------Main's Update Methods----------------------------------------------------------------------------------------
        /// <summary>
        /// Updates and cleans out all the projectiles in existence.
        /// </summary>
        /// <param name="gameTime"></param>
        public void updateProjectiles(GameTime gameTime)
        {
            for (int i = 0; i < enemyProjectileList.Count; i++)
            {
                enemyProjectileList[i].update(gameTime);//Updates all the projectiles.
                if (enemyProjectileList[i].toDelete)
                {//Remove any that need deletion.
                    enemyProjectileList.RemoveAt(i);//Once removed from the list the projectile should be without references, and so disposed of.
                    i--;
                }
            }
            for (int i = 0; i < playerProjectileList.Count; i++)
            {
                playerProjectileList[i].update(gameTime);//Updates all the projectiles.
                if (playerProjectileList[i].toDelete)
                {//Remove any that need deletion.
                    playerProjectileList.RemoveAt(i);//Once removed from the list the projectile should be without references, and so disposed of.
                    i--;
                }
            }
            for (int i = 0; i < playerProjectileListBomb.Count; i++)
            {
                playerProjectileListBomb[i].update(gameTime);//Updates all the projectiles.

                if (playerProjectileListBomb[i].toDelete||player.bombDisplayTime<=0)
                {//Remove any that need deletion.
                    playerProjectileListBomb.RemoveAt(i);//Once removed from the list the projectile should be without references, and so disposed of.
                    i--;
                }
            }
        }

        /// <summary>
        /// Simply calls each projectile's individual draw function.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void drawProjectiles(SpriteBatch spriteBatch)
        {
            foreach (Projectile entity in enemyProjectileList)
                entity.draw(spriteBatch);

            foreach (Projectile entity in playerProjectileList)
                entity.draw(spriteBatch);

            foreach (Projectile entity in playerProjectileListBomb)
                entity.draw(spriteBatch);
        }


        //----------Utility Control Methods----------------------------------------------------------------------------------------
        /// <summary>
        /// Gets rid of all projectiles on the screen. Used when the player dies for example and they need to respawn.
        /// </summary>
        public void wipeProjetiles()
        {
            enemyProjectileList.Clear();
            playerProjectileList.Clear();
        }

        //-------Bullet Creation Methods-------------------------------------------------------------------------------------------
        /// <summary>
        /// A function that takes a already made projectile and adds it to the lists. Since the lists are public you could just do this yourself but functions are nice.
        /// </summary>
        /// <param name="newProjectile">The projectile you wish to add.</param>
        /// <param name="players">Am optional boolean representing if this is the player's projectile.</param>
        public void putProjectile(Projectile newProjectile, bool players = false)
        {
            if (player.invulnerabilityTimer != 0) { return; }

            if (!players)
                enemyProjectileList.Add(newProjectile);
            else
                playerProjectileList.Add(newProjectile);
        }

        /// <summary>
        /// Creates a LinearProjectile at the set position and trajectory.
        /// </summary>
        /// <param name="xSpawn">X value of the place to create the projectile.</param>
        /// <param name="ySpawn">Y value of the place to create the projectile.</param>
        /// <param name="trajectoryAngle">The angle, in radians, clockwise from the postiive X axis, which defines a trajectory.</param>
        /// <param name="texture">Optional: A refernece to a texture to be used in place of the default texture.</param>
        /// <param name="speed">Optional: The speed at which the projectile should move.</param>
        /// <param name="players">Optional: States whether the projectile was shot by the player.</param>
        public void requestLinearProjectile(float xSpawn, float ySpawn, float trajectoryAngle, Texture2D texture = null, float speed = 100, bool players = false)
        {
            if(player.invulnerabilityTimer != 0) { return; }

            if(texture == null)
                texture = textureContainer.defaultProjectile;

            Queue<Movement> movementQueue = new Queue<Movement>();
            movementQueue.Enqueue(new Movement(trajectoryAngle,-1,0,speed));

            if (players)
                playerProjectileList.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
            else
                enemyProjectileList.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
        }

        /// <summary>
        /// Creates a LinearProjectile at the set position and trajectory.
        /// </summary>
        /// <param name="xSpawn">X value of the place to create the projectile.</param>
        /// <param name="ySpawn">Y value of the place to create the projectile.</param>
        /// <param name="trajectoryVector">A vector representing the trajectory the projectile should move in. Scale is irrelivent. Vector is consumed during creation.</param>
        /// <param name="texture">Optional: A refernece to a texture to be used in place of the default texture.</param>
        /// <param name="speed">Optional: The speed at which the projectile should move.</param>
        /// <param name="players">Optional: States whether the projectile was shot by the player.</param>
        public void requestLinearProjectile(float xSpawn, float ySpawn, Vector2 trajectoryVector, Texture2D texture = null, float speed = 100, bool players = false)
        {
            if (player.invulnerabilityTimer != 0) { return; }

            if (texture == null)
                texture = textureContainer.defaultProjectile;

            Queue<Movement> movementQueue = new Queue<Movement>();
            movementQueue.Enqueue(new Movement(trajectoryVector, -1, 0, speed));

            if (players)
                playerProjectileList.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
            else
                enemyProjectileList.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
        }

        /// <summary>
        /// Creates a ArcingProjectile at the set position and trajectory.
        /// </summary>
        /// <param name="xSpawn">X value of the place to create the projectile.</param>
        /// <param name="ySpawn">Y value of the place to create the projectile.</param>
        /// <param name="trajectoryAngle">The angle, in radians, clockwise from the postiive X axis, which defines a trajectory.</param>
        /// <param name="turningAngle">The angle, in radians, that the projectile should arc by each update. Should be anywhere from 0 to about 0.05. 
        /// Zero = straight. Postive = arc right. Negitive = arc left.</param>
        /// <param name="texture">Optional: A refernece to a texture to be used in place of the default texture.</param>
        /// <param name="speed">Optional: The speed at which the projectile should move.</param>
        /// <param name="players">Optional: States whether the projectile was shot by the player.</param>
        public void requestArcingProjectile(float xSpawn, float ySpawn, float trajectoryAngle, float turningAngle, Texture2D texture = null, float speed = 100, bool players = false)
        {
            if (player.invulnerabilityTimer != 0) { return; }

            if (texture == null)
                texture = textureContainer.defaultProjectile;

            Queue<Movement> movementQueue = new Queue<Movement>();
            movementQueue.Enqueue(new Movement(trajectoryAngle, -1, turningAngle, speed));

            if (players)
                playerProjectileList.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
            else
                enemyProjectileList.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
        }

        /// <summary>
        /// Creates a ArcingProjectile at the set position and trajectory.
        /// </summary>
        /// <param name="xSpawn">X value of the place to create the projectile.</param>
        /// <param name="ySpawn">Y value of the place to create the projectile.</param>
        /// <param name="trajectoryVector">A vector representing the trajectory the projectile should move in. Scale is irrelivent. Vector is consumed during creation.</param>
        /// <param name="turningAngle">The angle, in radians, that the projectile should arc by each update. Should be anywhere from 0 to about 0.05. 
        /// Zero = straight. Postive = arc right. Negitive = arc left.</param>
        /// <param name="texture">Optional: A refernece to a texture to be used in place of the default texture.</param>
        /// <param name="players">Optional: States whether the projectile was shot by the player.</param>
        public void requestArcingProjectile(float xSpawn, float ySpawn, Vector2 trajectoryVector, float turningAngle, Texture2D texture = null, float speed = 100, bool players = false)
        {
            if (player.invulnerabilityTimer != 0) { return; }

            if (texture == null)
                texture = textureContainer.defaultProjectile;

            Queue<Movement> movementQueue = new Queue<Movement>();
            movementQueue.Enqueue(new Movement(trajectoryVector, -1, turningAngle, speed));

            if (players)
                playerProjectileList.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
            else
                enemyProjectileList.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
        }

        /// <summary>
        /// Creates a CicrularSlowProjectile at the set position and trajectory.
        /// </summary>
        /// <param name="xSpawn">X value of the place to create the projectile.</param>
        /// <param name="ySpawn">Y value of the place to create the projectile.</param>
        /// <param name="trajectoryAngle">The angle, in radians, clockwise from the postiive X axis, which defines a trajectory.</param>
        /// <param name="turningAngle">The angle, in radians, that the projectile should arc by each update. Should be anywhere from 0 to about 0.05. 
        /// Zero = straight. Postive = arc right. Negitive = arc left.</param>
        /// <param name="texture">Optional: A refernece to a texture to be used in place of the default texture.</param>
        /// <param name="players">Optional: States whether the projectile was shot by the player.</param>
        public void requestCircularSlowProjetile(float xSpawn, float ySpawn, float trajectoryAngle, float turningAngle, Texture2D texture = null, bool players = false)
        {
            if (player.invulnerabilityTimer != 0) { return; }

            if (texture == null)
                texture = textureContainer.blueProjectile;

            Queue<Movement> movementQueue = new Queue<Movement>();
            movementQueue.Enqueue(new Movement(trajectoryAngle, 25, 0, 300));
            movementQueue.Enqueue(new Movement(null, -1, turningAngle, 100, 0.995f));

            if (players)
                playerProjectileList.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
            else
                enemyProjectileList.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
        }

        /// <summary>
        /// Creates a CicrularSlowProjectile at the set position and trajectory.
        /// </summary>
        /// <param name="xSpawn">X value of the place to create the projectile.</param>
        /// <param name="ySpawn">Y value of the place to create the projectile.</param>
        /// <param name="trajectoryVector">A vector representing the trajectory the projectile should move in. Scale is irrelivent. Vector is consumed during creation.</param>
        /// <param name="turningAngle">The angle, in radians, that the projectile should arc by each update. Should be anywhere from 0 to about 0.05. 
        /// Zero = straight. Postive = arc right. Negitive = arc left.</param>
        /// <param name="texture">Optional: A refernece to a texture to be used in place of the default texture.</param>
        /// <param name="players">Optional: States whether the projectile was shot by the player.</param>
        public void requestCircularSlowProjetile(float xSpawn, float ySpawn, Vector2 trajectoryVector, float turningAngle, Texture2D texture = null, bool players = false)
        {
            if (player.invulnerabilityTimer != 0) { return; }

            if (texture == null)
                texture = textureContainer.blueProjectile;

            Queue<Movement> movementQueue = new Queue<Movement>();
            movementQueue.Enqueue(new Movement(trajectoryVector, 25, 0, 300));
            movementQueue.Enqueue(new Movement(null, -1, turningAngle, 100, 0.995f));

            if (players)
                playerProjectileList.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
            else
                enemyProjectileList.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
        }

        /// <summary>
        /// Creates a CicrularSlowProjectile2 at the set position and trajectory.
        /// </summary>
        /// <param name="xSpawn">X value of the place to create the projectile.</param>
        /// <param name="ySpawn">Y value of the place to create the projectile.</param>
        /// <param name="trajectoryAngle">The angle, in radians, clockwise from the postiive X axis, which defines a trajectory.</param>
        /// <param name="turningAngle">The angle, in radians, that the projectile should arc by each update. Should be anywhere from 0 to about 0.05. 
        /// Zero = straight. Postive = arc right. Negitive = arc left.</param>
        /// <param name="texture">Optional: A refernece to a texture to be used in place of the default texture.</param>
        /// <param name="players">Optional: States whether the projectile was shot by the player.</param>
        public void requestCircularSlowProjetile2(float xSpawn, float ySpawn, float trajectoryAngle, float turningAngle, Texture2D texture = null, bool players = false)
        {
            if (player.invulnerabilityTimer != 0) { return; }

            if (texture == null)
                texture = textureContainer.blueProjectile;

            Queue<Movement> movementQueue = new Queue<Movement>();
            movementQueue.Enqueue(new Movement(trajectoryAngle, 15, 0, 300));
            movementQueue.Enqueue(new Movement(null, 85, turningAngle * 7, 150));
            movementQueue.Enqueue(new Movement(null, -1, (turningAngle * 7) / 5, 150, 0.995f));

            if (players)
                playerProjectileList.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
            else
                enemyProjectileList.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
        }


        public void requestBomb(float xSpawn, float ySpawn, Vector2 trajectoryVector, Texture2D texture, float speed=100, bool players = false)
        {
           
            if (player.invulnerabilityTimer != 0) { return; }

            if (texture == null)
                texture = textureContainer.defaultProjectile;

            Queue<Movement> movementQueue = new Queue<Movement>();
            movementQueue.Enqueue(new Movement(trajectoryVector, -1, 0, speed));

            if (players)  // true
                playerProjectileListBomb.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
            
        }



        /// <summary>
        /// Creates a CicrularSlowProjectile2 at the set position and trajectory.
        /// </summary>
        /// <param name="xSpawn">X value of the place to create the projectile.</param>
        /// <param name="ySpawn">Y value of the place to create the projectile.</param>
        /// <param name="trajectoryVector">A vector representing the trajectory the projectile should move in. Scale is irrelivent. Vector is consumed during creation.</param>
        /// <param name="turningAngle">The angle, in radians, that the projectile should arc by each update. Should be anywhere from 0 to about 0.05. 
        /// Zero = straight. Postive = arc right. Negitive = arc left.</param>
        /// <param name="texture">Optional: A refernece to a texture to be used in place of the default texture.</param>
        /// <param name="players">Optional: States whether the projectile was shot by the player.</param>
        public void requestCircularSlowProjetile2(float xSpawn, float ySpawn, Vector2 trajectoryVector, float turningAngle, Texture2D texture = null, bool players = false)
        {
            if (player.invulnerabilityTimer != 0) { return; }

            if (texture == null)
                texture = textureContainer.blueProjectile;

            Queue<Movement> movementQueue = new Queue<Movement>();
            movementQueue.Enqueue(new Movement(trajectoryVector, 15, 0, 300));
            movementQueue.Enqueue(new Movement(null, 85, turningAngle * 7, 150));
            movementQueue.Enqueue(new Movement(null, -1, (turningAngle * 7) / 5, 150, 0.995f));

            if (players)
                playerProjectileList.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
            else
                enemyProjectileList.Add(new BasicProjectile(texture, xSpawn, ySpawn, movementQueue));
        }

        /// <summary>
        /// Creates a MultiplyingProjectile at the set position and trajectory.
        /// </summary>
        /// <param name="xSpawn">X value of the place to create the projectile.</param>
        /// <param name="ySpawn">Y value of the place to create the projectile.</param>
        /// <param name="trajectoryAngle">The angle, in radians, clockwise from the postiive X axis, which defines a trajectory.</param>
        /// <param name="texture">Optional: A refernece to a texture to be used in place of the default texture.</param>
        /// <param name="startingIndex">Optional: An internal variable used for staging, other classes should't mess with this.</param>
        /// <param name="Type">Optional: An internal variable affecting projectile's behaviour, other classses shouldn't mess with this.</param>
        /// <param name="players">Optional: States whether the projectile was shot by the player.</param>
        public void requestMultiplyingProjectile(float xSpawn, float ySpawn, float trajectoryAngle, Texture2D texture = null, int startingIndex = 0, int Type = 0, bool players = false)
        {
            if (player.invulnerabilityTimer != 0) { return; }

            if (texture == null)
                texture = textureContainer.blueProjectile;

            if (players)
                playerProjectileList.Add(new MultiplyingProjectile(texture, xSpawn, ySpawn, trajectoryAngle, startingIndex, Type));
            else
                enemyProjectileList.Add(new MultiplyingProjectile(texture, xSpawn, ySpawn, trajectoryAngle, startingIndex, Type));
        }

        /// <summary>
        /// Creates a MultiplyingProjectile at the set position and trajectory.
        /// </summary>
        /// <param name="xSpawn">X value of the place to create the projectile.</param>
        /// <param name="ySpawn">Y value of the place to create the projectile.</param>
        /// <param name="trajectoryVector">A vector representing the trajectory the projectile should move in. Scale is irrelivent. Vector is consumed during creation.</param>
        /// <param name="texture">Optional: A refernece to a texture to be used in place of the default texture.</param>
        /// <param name="startingIndex">Optional: An internal variable used for staging, other classes should't mess with this.</param>
        /// <param name="Type">Optional: An internal variable affecting projectile's behaviour, other classses shouldn't mess with this.</param>
        /// <param name="players">Optional: States whether the projectile was shot by the player.</param>
        public void requestMultiplyingProjectile(float xSpawn, float ySpawn, Vector2 trajectoryVector, Texture2D texture = null, int startingIndex = 0, int Type = 0, bool players = false)
        {
            if (player.invulnerabilityTimer != 0) { return; }

            if (texture == null)
                texture = textureContainer.blueProjectile;

            if (players)
                playerProjectileList.Add(new MultiplyingProjectile(texture, xSpawn, ySpawn, trajectoryVector, startingIndex, Type));
            else
                enemyProjectileList.Add(new MultiplyingProjectile(texture, xSpawn, ySpawn, trajectoryVector, startingIndex, Type));
        }

        /// <summary>
        /// Creates a LaserProjectile at the set position and trajectory.
        /// </summary>
        /// <param name="xSpawn">X value of the place to create the projectile.</param>
        /// <param name="ySpawn">Y value of the place to create the projectile.</param>
        /// <param name="trajectoryAngle">The angle, in radians, clockwise from the postiive X axis, which defines a trajectory.</param>
        /// <param name="speed">Optional: The speed at which the projectile should move.</param>
        /// <param name="players">Optional: States whether the projectile was shot by the player.</param>
        public void requestLaserProjetile(float xSpawn, float ySpawn, float trajectoryAngle, float speed = 100, bool players = false)
        {
            if (player.invulnerabilityTimer != 0) { return; }

            if (players)
                playerProjectileList.Add(new LaserProjectile(Content, xSpawn, ySpawn, trajectoryAngle, speed));
            else
                enemyProjectileList.Add(new LaserProjectile(Content, xSpawn, ySpawn, trajectoryAngle, speed));
        }

        /// <summary>
        /// Creates a LaserProjectile at the set position and trajectory.
        /// </summary>
        /// <param name="xSpawn">X value of the place to create the projectile.</param>
        /// <param name="ySpawn">Y value of the place to create the projectile.</param>
        /// <param name="trajectoryVector">A vector representing the trajectory the projectile should move in. Scale is irrelivent. Vector is consumed during creation.</param>
        /// <param name="speed">Optional: The speed at which the projectile should move.</param>
        /// <param name="players">Optional: States whether the projectile was shot by the player.</param>
        public void requestLaserProjetile(float xSpawn, float ySpawn, Vector2 trajectoryVector, float speed = 100, bool players = false)
        {
            if (player.invulnerabilityTimer != 0) { return; }

            if (players)
                playerProjectileList.Add(new LaserProjectile(Content, xSpawn, ySpawn, trajectoryVector, speed));
            else
                enemyProjectileList.Add(new LaserProjectile(Content, xSpawn, ySpawn, trajectoryVector, speed));
        }

        public void requestTrollProjectile(float xSpawn, float ySpawn, float trajectoryAngle, float turningAngle, float speed = 100)
        {
            Queue<Movement> movementQueue = new Queue<Movement>();
            movementQueue.Enqueue(new Movement(trajectoryAngle, -1, turningAngle, speed, 0.995f));

            playerProjectileList.Add(new TrollProjectile(textureContainer.orangeProjectile, xSpawn, ySpawn, movementQueue));

        }

        public void requestTrollProjectile2(float xSpawn, float ySpawn, Vector2 trajectoryVector, float turningAngle, float speed = 200)
        {
            Queue<Movement> movementQueue = new Queue<Movement>();
            movementQueue.Enqueue(new Movement(trajectoryVector, -1, turningAngle, speed, 0.995f));

            playerProjectileList.Add(new TrollProjectile2(textureContainer.redProjectile, xSpawn, ySpawn, movementQueue));

        }
    }
}

