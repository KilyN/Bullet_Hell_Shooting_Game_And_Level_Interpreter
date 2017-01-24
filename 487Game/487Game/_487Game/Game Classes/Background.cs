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
    class Background
    {
        private GameSettings settings;
        private Texture2D backgroundtexture;
        private int height;
        private Player player;

        public Background(Texture2D texture, GameSettings settings, Player player)
        {
            this.settings = settings;
            backgroundtexture = texture;
            this.player = player;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundtexture, new Vector2(0,height), Color.White);
            spriteBatch.Draw(backgroundtexture, new Vector2(0, height- backgroundtexture.Height), Color.White);
            if (!(player.lives <= 0 || player.isfinalBossDie))
            {
                height++;
                if (height > backgroundtexture.Height)
                    height = 0;
            }
        }
    }
}
