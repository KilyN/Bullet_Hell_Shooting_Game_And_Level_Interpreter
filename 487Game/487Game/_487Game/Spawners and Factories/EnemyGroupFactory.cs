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
    /// Enemy Group class that creates groups of enemys and executes their movement methods
    /// </summary>
    class EnemyGroupFactory
    {
        // Member fields
        Player _player;
        GameSettings _settings;
        ContentManager _content;
        Random rand = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">type enum</param>
        /// <param name="player">player reference</param>
        /// <param name="settings">settings reference</param>
        /// <param name="content">content reference</param>
        /// <param name="numEnemies">number of enemies to create</param>
        public EnemyGroupFactory(Player player, GameSettings settings, ContentManager content)
        {
            _player = player;
            _settings = settings;
            _content = content;
        }

        public List<Enemy> CreateGroup(Queue<Movement>[] mvQueue, Queue<Tuple<Projectile, int>>[] bullets, Vector2 init, int number = 1, int spacing = 0, int xshift = 0, bool xrand = false, int health = 10)
        {
            var enemies = new List<Enemy>();
            
            Texture2D texture;
            switch (rand.Next(0, 4))
            {
                case 0: texture = _content.Load<Texture2D>("enemy1"); break;
                case 1: texture = _content.Load<Texture2D>("enemy2"); break;
                case 2: texture = _content.Load<Texture2D>("enemy3"); break;
                case 3: texture = _content.Load<Texture2D>("enemy4"); break;
                default: texture = _content.Load<Texture2D>("Ajax"); break;
            }

            for (int i = 0; i < number; i++)
            {
                if (xrand)
                {
                    Vector2 newVec = new Vector2(rand.Next() % _settings.Width, init.Y);
                    enemies.Add(new BasicEnemy(texture, newVec, _player, mvQueue[i], spacing * i, bullets != null ? bullets[i] : null, health));
                }
                else
                {
                    Vector2 newVec = new Vector2(init.X + xshift * i, init.Y);
                    enemies.Add(new BasicEnemy(texture, newVec, _player, mvQueue[i], spacing * i, bullets != null ? bullets[i] : null, health));
                }
            }

            return enemies;
        }


        /// <summary>
        /// Final Boss creation - comes down then goes back up
        /// </summary>
        public List<Enemy> CreateFinalBoss(Queue<Movement> mvQueue, Vector2 init, Queue<Tuple<int, char>> bullets)
        {
            return new List<Enemy>() { new FinalBoss(_content.Load<Texture2D>("boss"), init, _player, mvQueue,_settings, _content.Load<Texture2D>("GreenHealthBar"), bullets) };
        }
    }
}
