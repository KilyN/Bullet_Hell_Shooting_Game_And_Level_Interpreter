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
using System.Diagnostics;

namespace _487Game
{

    /// <summary>
    /// This class creates the batches of enemies for the entire game
    /// We can eventually read in custom enemy configurations when we construct this object to accept user-made levels 
    /// </summary>
    class EnemySpawner
    {
        // Tuple list, where item 1 is associated timing, item 2 is grouptype, item 3 is number of enemies, item 4 is starting vector
        // When items 3 and 4 are 0 (or null) they will act as defaults
        public Queue<Tuple<int, List<Enemy>>> _groups;
        // List of enemies
        public List<Enemy> _enemies;
        public Stopwatch splashScreentime;
        

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="player">player instance</param>
        /// <param name="settings">settings instance</param>
        /// <param name="batch">sprite batch instance</param>
        /// <param name="content">content manager instance</param>
        public EnemySpawner()
        {
            // assign to fields
            _groups = new Queue<Tuple<int, List<Enemy>>>();
            _enemies = new List<Enemy>();
            splashScreentime = new Stopwatch();
            splashScreentime.Start();
            // CreateAllGroups();
            
        }

        /// <summary>
        /// Executes all group actions that are within the active timeframe
        /// </summary>
        /// <param name="time">game time</param>
        public void SpawnAndMoveEnemies(GameTime time)
        {
            // Check it game time is after activation time (might want to implement "death time" later)
            if (_groups.Any() && (time.TotalGameTime.Seconds + time.TotalGameTime.Minutes * 60) - (splashScreentime.Elapsed.Seconds) >= _groups.First().Item1)
            {
                var group = _groups.Dequeue();
                _enemies.AddRange(group.Item2);
            }

            foreach (var enemy in _enemies)
            {
                if(enemy.Delay == 0)
                {
                    enemy.update(time);
                }
                else
                {
                    enemy.Delay--;
                }
            }

            _enemies.RemoveAll(en => en.toDelete);
        }

        /// <summary>
        /// Creates whole gameflow by creating all groups of enemies
        /// </summary>
        //private void CreateAllGroups()
        //{
        //    // Hardcode in enemy groups here
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(3, Group.DownAndBack, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(12, Group.ArcRight, 4, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(14, Group.ArcLeft, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(16, Group.DownAndLeft, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(18, Group.ArcLeft, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(19, Group.ArcRight, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(21, Group.DownAndRight, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(23, Group.DownAndBack, 10, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(26, Group.ArcLeft, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(27, Group.ArcRight, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(27, Group.ArcLeft, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(30, Group.ArcRight, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(30, Group.ArcLeft, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(32, Group.ArcRight, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(35, Group.DownAndLeft, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(36, Group.ArcRight, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(37, Group.ArcRight, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(38, Group.DownAndBack, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(39, Group.DownAndBack, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(43, Group.MidLevelBoss, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(62, Group.DownAndLeft, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(62, Group.DownAndRight, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(68, Group.StraightDown, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(71, Group.DownAndLeft, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(71, Group.DownAndRight, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(72, Group.ArcLeft, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(74, Group.DownAndBack, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(74, Group.DownAndBack, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(74, Group.DownAndBack, 0, new Vector2()));
        //    _groups.Enqueue(new Tuple<int, Group, int, Vector2>(80, Group.FinalBoss, 0, new Vector2()));
        //}

        // Load up level from list of enemies
        public void LoadLevel(List<Tuple<int, List<Enemy>>> groups)
        {
            groups.ForEach(x => _groups.Enqueue(x));
        }
     
        public void DrawEnemies(SpriteBatch spriteBatch)
        {
            foreach (var enemy in _enemies)
            {
                if(enemy.Delay == 0)
                {
                    enemy.draw(spriteBatch);
                }
            }
        }
    }
}
