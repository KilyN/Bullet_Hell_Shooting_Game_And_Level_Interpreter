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
    /// An invisible entitiy that a boss creates to remotly spawn bullets for it.
    /// </summary>
    public class BossBulletSpawner : GameObject
    {

        //Member Variables
        /// <summary>
        /// TurnningAngle should be anywhere from 0 to about 0.05. Zero = straight. Postive = arc right. Negitive = arc left.
        /// </summary>
        private float turningAngle;
        /// <summary>
        /// Which attack this spawner should perform, 1 = A1, 2 = A2, 3 = A3.
        /// </summary>
        private readonly int type;
        /// <summary>
        /// Speed at wich the spawner moves.
        /// </summary>
        private float speed;
        /// <summary>
        /// Number representing the number of updates, to date. Used for staging.
        /// </summary>
        private int index = 0;
        /// <summary>
        /// A reference to the player, nessesary if the bullets are to beable to track the player.
        /// </summary>
        private Player player;
        private Vector2 spriteSpeed;


        //Constructors
        /// <summary>
        /// A odd constructor which takes multiple reference to things in main, which are all nessesary to spawn things.
        /// </summary>
        /// <param name="origin">Location of boss at time of creation.</param>
        /// <param name="type">Which attack type this spawner is to create.</param>
        /// <param name="trajectory">Inital direction the spanwer is moving in.</param>
        /// <param name="player">A reference to the player.</param>
        /// <param name="clockwise">A bool representing if the spanwer is spiraling clockwise or not.</param>
        public BossBulletSpawner(Vector2 origin, int type, Vector2 trajectory, Player player, bool clockwise = true)
        {
            this.speed = 200;
            this.origin = origin;
            this.turningAngle = 0.3f;
            trajectory.Normalize();
            spriteSpeed = trajectory * speed;
            this.player = player;
            this.type = type;

            spritePosition.X = this.origin.X;
            spritePosition.Y = this.origin.Y;

            if (!clockwise)
                turningAngle *= -1;
        }


        //Methods
        /// <summary>
        /// Bullet Spawners have no sprite to draw, for now.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void draw(SpriteBatch spriteBatch)
        {
            throw new InvalidOperationException();
        }
        
        /// <summary>
        /// This update's the spawner's position based upon it's type and creates projectiles.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void update(GameTime gameTime)
        {
            if (!toDelete)//Don't update the object if it needs to be deleted. (This should never need to be used.)
            {
                index++;

                switch (type)
                {
                    case 1: typeOneSpiral(); break;
                    case 2: typeTwoSpiral(); break;
                    case 3: typeThreeSpiral(); break;
                    default: throw new Exception();
                }


                spriteSpeed = Vector2.Transform(spriteSpeed, Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, turningAngle)));

                spritePosition += spriteSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;//Move object

                if (type == 1 || type == 2)
                {
                    if (index % 2 == 0)
                    {
                        if(type == 1)
                            SoundEffects.Instance()._bullet.Play(0.1f,-0.8f,0);
                        else
                            SoundEffects.Instance()._bullet.Play(0.2f, -0.8f, 0);
                        spawnBullets();
                    }
                } 
                else if(type == 3)
                {
                    SoundEffects.Instance()._bullet.Play(0.03f, -0.8f, 0);
                    spawnBullets();
                }
            }
        }

        /// <summary>
        /// This object does not collide with anything, as such this function should never be called.
        /// </summary>
        public override void Collide()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// The spiral movement assocated with attack A1.
        /// </summary>
        private void typeOneSpiral()
        {
            Vector2 additon = new Vector2(spriteSpeed.X, spriteSpeed.Y);
            additon.Normalize();

            if (index < 40)
                spriteSpeed += additon * 10;


            if (index < 10)
                turningAngle *= 0.95f;
            else if (index < 20)
                turningAngle *= 0.97f;
            else if (index < 30)
                turningAngle *= 0.97f;
            else if (index > 55 && index < 62)
                turningAngle *= 0.93f;

            if (index > 80)
                toDelete = true;
        }

        /// <summary>
        /// The spiral movement associated with attack A2.
        /// </summary>
        private void typeTwoSpiral()
        {
            Vector2 additon = new Vector2(spriteSpeed.X, spriteSpeed.Y);
            additon.Normalize();

            if (index < 40)
                spriteSpeed += additon * 10;


            if (index < 20)
                turningAngle *= 0.98f;
            else if (index > 30 && index < 60)
                turningAngle *= 0.985f;
            else if(index > 60 && index < 100)
                turningAngle *= 0.99f;

            if (index > 130)
                toDelete = true;
        }

        /// <summary>
        /// The spiral movement associated with attack A3.
        /// </summary>
        private void typeThreeSpiral()
        {
            Vector2 additon = new Vector2(spriteSpeed.X, spriteSpeed.Y);
            additon.Normalize();

            if (index < 40)
                spriteSpeed += additon * 20;


            if (index < 20)
                turningAngle *= 0.98f;
            else if (index > 30 && index < 60)
                turningAngle *= 0.99f;
            else if (index > 60 && index < 100)
                //turningAngle *= 0.9f;


            if (index > 80)
                toDelete = true;
        }

        /// <summary>
        /// A function which spawns linear projectiles based upon the type of the spawner.
        /// </summary>
        private void spawnBullets()
        {
            BulletFactory bulletFactory = BulletFactory.Instance();

            switch (type)
            {
                case 1:
                    bulletFactory.requestLinearProjectile(spritePosition.X, spritePosition.Y, Projectile.createVectorToPlayer(spritePosition.X, spritePosition.Y, player), bulletFactory.textureContainer.yellowProjectile, 100);
                    break;

                case 2:
                    Vector2 playerVector = Projectile.createVectorToPlayer(spritePosition.X, spritePosition.Y, player);
                    bulletFactory.requestLinearProjectile(spritePosition.X, spritePosition.Y, playerVector, bulletFactory.textureContainer.yellowProjectile, 200);
                    playerVector = Vector2.Transform(playerVector, Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)(120 * (Math.PI / 180)))));
                    bulletFactory.requestLinearProjectile(spritePosition.X, spritePosition.Y, playerVector, bulletFactory.textureContainer.yellowProjectile, 200);
                    playerVector = Vector2.Transform(playerVector, Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)(120 * (Math.PI / 180)))));
                    bulletFactory.requestLinearProjectile(spritePosition.X, spritePosition.Y, playerVector, bulletFactory.textureContainer.yellowProjectile, 200);
                    break;

                case 3:
                    Random rand = new Random();
                    Vector2 randomVector = new Vector2(rand.Next(-100, 100), rand.Next(-100, 100));
                    bulletFactory.requestLinearProjectile(spritePosition.X, spritePosition.Y, randomVector, bulletFactory.textureContainer.yellowProjectile, 100);
                    break;

                default:
                    throw new Exception();

            }
        }
    }
}
