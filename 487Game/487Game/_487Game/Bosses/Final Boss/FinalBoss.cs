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
    class FinalBoss : Enemy
    {
        List<BossBulletSpawner> bossSpanwers;

        //  add below fields for final boss health display
        public Texture2D midhealth;
        public Rectangle healthRectangle;
        public GameSettings _settings;
        private Queue<Tuple<int, char>> _bullets;

        private int index = 0;

        public FinalBoss(Texture2D newTexture, Vector2 initPosition, Player player, Queue<Movement> movementQueue, GameSettings newSetting, Texture2D newhealthMId, Queue<Tuple<int, char>> bullets)
        {
            myTexture = newTexture;
            spritePosition = initPosition;
            _player = player;
            this.movementQueue = movementQueue;
            health = 350;
            bossSpanwers = new List<BossBulletSpawner>();
            if (myTexture.Width > 100)
                scale = 100f / myTexture.Width;
            hitbox = new CircularHitBox((myTexture.Width * scale) / 3);
            origin.X = myTexture.Width / 2;
            origin.Y = myTexture.Height / 2;
            // final boss health display       
            midhealth = newhealthMId;
            _settings = newSetting;
            _bullets = bullets;
        }

        // draw final enemy and its health
        public override void draw(SpriteBatch spriteBatch)
        {
            //draw final boss
            if (!float.IsNaN(RotationAngle))
                spriteBatch.Draw(myTexture, spritePosition, null, Color.White, RotationAngle, origin, scale, SpriteEffects.None, 0f);
            else
                spriteBatch.Draw(myTexture, spritePosition, null, Color.White, (float)Math.PI / 2f, origin, scale, SpriteEffects.None, 0f);

            // draw final boss health
            spriteBatch.Draw(midhealth, healthRectangle, Color.White);
        }

        public override void Collide()
        {
            if (!toDelete)
            {
                health -= 1;
                if (health <= 0)
                {
                    toDelete = true;

                    //set finalBoss is true
                    _player.isfinalBossDie = true;
                }
            }
        }

        public void updateSpawners(GameTime gameTime)
        {
            // healthRectangle for final boss health
            healthRectangle = new Rectangle(_settings.Width - 40, 40, 20, health * 2);

            for (int i = 0; i < bossSpanwers.Count; i++)
            {
                bossSpanwers[i].update(gameTime);
                if (bossSpanwers[i].toDelete)
                {
                    bossSpanwers.Remove(bossSpanwers[i]);
                    i--;
                }
            }

            // display youWin when the final boss goes off screen
            BulletFactory bulletFactory = BulletFactory.Instance();
            if (spritePosition.Y < -150 && bulletFactory.enemyProjectileList.Count == 0)
                _player.isfinalBossDie = true;
        }

        public void attackA1()
        {
            bossSpanwers.Add(new BossBulletSpawner(new Vector2(X, Y), 1, Vector2.UnitY, _player));
            bossSpanwers.Add(new BossBulletSpawner(new Vector2(X, Y), 1, -Vector2.UnitY, _player));
        }

        public void attackA2()
        {
            bossSpanwers.Add(new BossBulletSpawner(new Vector2(X, Y), 2, Vector2.UnitY, _player, false));
        }

        public void attackA3()
        {
            bossSpanwers.Add(new BossBulletSpawner(new Vector2(X, Y), 3, -Vector2.UnitY, _player));
            bossSpanwers.Add(new BossBulletSpawner(new Vector2(X, Y), 3, Vector2.UnitY, _player, false));
        }

        public void attackB1(int sign)
        {
            SoundEffects.Instance()._bullet.Play(0.7f, -0.2f, 0);

            BulletFactory bulletFactory = BulletFactory.Instance();

            bulletFactory.requestCircularSlowProjetile(X, Y, (float)((0 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile(X, Y, (float)((1 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile(X, Y, (float)((2 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile(X, Y, (float)((3 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile(X, Y, (float)((4 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile(X, Y, (float)((5 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile(X, Y, (float)((6 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile(X, Y, (float)((7 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile(X, Y, (float)((8 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile(X, Y, (float)((9 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile(X, Y, (float)((10 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile(X, Y, (float)((11 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile(X, Y, (float)((12 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile(X, Y, (float)((13 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile(X, Y, (float)((14 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile(X, Y, (float)((15 * 2 * Math.PI) / 16), 0.005f * sign);
        }

        public void attackB2(int sign)
        {
            SoundEffects.Instance()._bullet.Play(0.7f, -0.2f, 0);

            BulletFactory bulletFactory = BulletFactory.Instance();

            bulletFactory.requestCircularSlowProjetile2(X, Y, (float)((0 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile2(X, Y, (float)((1 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile2(X, Y, (float)((2 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile2(X, Y, (float)((3 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile2(X, Y, (float)((4 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile2(X, Y, (float)((5 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile2(X, Y, (float)((6 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile2(X, Y, (float)((7 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile2(X, Y, (float)((8 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile2(X, Y, (float)((9 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile2(X, Y, (float)((10 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile2(X, Y, (float)((11 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile2(X, Y, (float)((12 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile2(X, Y, (float)((13 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile2(X, Y, (float)((14 * 2 * Math.PI) / 16), 0.005f * sign);
            bulletFactory.requestCircularSlowProjetile2(X, Y, (float)((15 * 2 * Math.PI) / 16), 0.005f * sign);
        }

        public void attackC1()
        {
            SoundEffects.Instance()._bullet.Play(0.7f, -0.6f, 0);

            BulletFactory bulletFactory = BulletFactory.Instance();

            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((0 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((1 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((2 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((3 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((4 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((5 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((6 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((7 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((8 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((9 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((10 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((11 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((12 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((13 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((14 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((15 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((16 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((17 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((18 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((19 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((20 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((21 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((22 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((23 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((24 * 2 * Math.PI) / 26));
            bulletFactory.requestMultiplyingProjectile(X, Y, (float)((25 * 2 * Math.PI) / 26));
        }

        public void attackD1()
        {
            SoundEffects.Instance()._bullet.Play(0.7f, -0.2f, 0);

            Random rand = new Random();

            BulletFactory bulletFactory = BulletFactory.Instance();

            bulletFactory.requestLaserProjetile(rand.Next(-100, 110) + X, rand.Next(-100, 110) + Y, (float)(rand.Next(0, 64) * 2 * Math.PI) / 64, 1);
            bulletFactory.requestLaserProjetile(rand.Next(-100, 110) + X, rand.Next(-100, 110) + Y, (float)(rand.Next(0, 64) * 2 * Math.PI) / 64, 1);
            bulletFactory.requestLaserProjetile(rand.Next(-100, 110) + X, rand.Next(-100, 110) + Y, (float)(rand.Next(0, 64) * 2 * Math.PI) / 64, 1);
            bulletFactory.requestLaserProjetile(rand.Next(-100, 110) + X, rand.Next(-100, 110) + Y, (float)(rand.Next(0, 64) * 2 * Math.PI) / 64, 1);
            bulletFactory.requestLaserProjetile(rand.Next(-100, 110) + X, rand.Next(-100, 110) + Y, (float)(rand.Next(0, 64) * 2 * Math.PI) / 64, 1);
            bulletFactory.requestLaserProjetile(rand.Next(-100, 110) + X, rand.Next(-100, 110) + Y, (float)(rand.Next(0, 64) * 2 * Math.PI) / 64, 1);
            bulletFactory.requestLaserProjetile(rand.Next(-100, 110) + X, rand.Next(-100, 110) + Y, (float)(rand.Next(0, 64) * 2 * Math.PI) / 64, 1);
            bulletFactory.requestLaserProjetile(rand.Next(-100, 110) + X, rand.Next(-100, 110) + Y, (float)(rand.Next(0, 64) * 2 * Math.PI) / 64, 1);
        }

        public override void update(GameTime time)
        {
            if(index == 0)
                SoundEffects.Instance().changeTracks = true;

            updateSpawners(time);
            
            if (movementQueue == null || movementQueue.Count == 0)
            {
                _player.isfinalBossDie = true;
                toDelete = true; return;//Don't know how to move? Then you must be finished and ready to move on.
            }

            movementQueue.Peek().Move(this, time);

            if (movementQueue.Peek().finished)//Current Movement finished? Go to the next.
                movementQueue.Dequeue();

            // Boss attacks
            while(_bullets.Any() && _bullets.Peek().Item1 == index)
            {
                switch (_bullets.Peek().Item2)
                {
                    case 'A': attackA1(); break;
                    case 'B': attackA2(); break;
                    case 'C': attackA3(); break;
                    case 'D': attackB1(1); break;
                    case 'E': attackB1(-1); break;
                    case 'F': attackB2(1); break;
                    case 'G': attackB2(-1); break;
                    case 'H': attackC1(); break;
                    case 'I': attackD1(); break;
                    default: break;
                    }

                _bullets.Dequeue();
            }

            index++;

            //switch (index)//Boss Attack indexes
            //{
            //    case 50: attackA1(); break;
            //    case 160: attackA1(); break;
            //    case 300: attackB1(1); break;
            //    case 320: attackB1(-1); break;
            //    case 470: attackB1(-1); break;
            //    case 600: attackC1(); break;
            //    case 720: attackC1(); break;
            //    case 860: attackB2(-1); break;
            //    case 1010: attackB2(1); attackA2(); break;
            //    case 1400: attackD1(); attackA3(); break;
            //}

            //// Stage 1 - 0 to 20 seconds.
            //if (_index < 380)
            //{
            //    Move up / Down / Diagnol, firing spiral attacks at player and outward shots like mid level boss.
            //    if (_index < 40) { spritePosition.Y += 5; }

            //    if (_index == 50)
            //        attackA1();
            //    if (_index > 90 && _index < 140)
            //    {
            //        spritePosition.Y += 4;
            //        spritePosition.X += 4;
            //    }
            //    if (_index == 160)
            //        attackA1();
            //    if (_index > 210 && _index < 280)
            //        spritePosition.X -= 6;
            //    if (_index == 300)
            //        attackB1(1);
            //    if (_index == 320)
            //        attackB1(-1);
            //}

            ////Stage 2 - 21 to 40 seconds.
            //else if (_index > 380 && _index < 700)
            //{
            //    if (_index < 460)
            //        spritePosition.X += 4;
            //    if (_index == 470)
            //        attackB1(-1);
            //    if (_index > 490 && _index < 570)
            //        spritePosition.X -= 4;
            //    if (_index == 600)
            //        attackC1();
            //    if (_index > 620 && _index < 700)
            //        spritePosition.X += 4;
            //    if (_index == 720)
            //        attackC1();
            //}
            //// Stage 3 - 41 to 60 seconds.
            //else if (_index > 740)
            //{
            //    if (_index > 740 && _index < 840)
            //        spritePosition.X -= 4;

            //    if (_index == 860)
            //        attackB2(-1);

            //    if (_index > 880 && _index < 920)
            //    {
            //        spritePosition.X += 4;
            //        spritePosition.Y += 4;
            //    }
            //    if (_index > 920 && _index < 970)
            //        spritePosition.X += 5;
            //    if (_index > 990 && _index < 1060)
            //        spritePosition.Y -= 5;
            //    if (_index == 1100)
            //    {
            //        attackB2(1);
            //        attackA2();
            //    }
            //    if (_index > 1140 && _index < 1170)
            //    {
            //        spritePosition.X -= 5;
            //        spritePosition.Y += 5;
            //    }
            //    if (_index > 1170 && _index < 1200)
            //        spritePosition.X -= 5;
            //    if (_index == 1400)
            //    {
            //        attackD1();
            //        attackA3();
            //    }
            //    if (_index > 1500)
            //        spritePosition.Y -= 5;
            //}
            //// End of 4 stages, Final boss exits from screen..
            //else if (_index > 1500)
            //{
            //    spritePosition.Y -= 5;
            //}
        }
    }
}

