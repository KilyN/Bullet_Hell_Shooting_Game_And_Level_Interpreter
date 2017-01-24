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
  /// Just a class that we can use to encapsulate our sound effects, cleans things up a bit
  /// </summary>
  public class SoundEffects
  {
    //public SoundEffect _song;
    public SoundEffect _bullet;
        public SoundEffect _bossSong;
        public SoundEffectInstance theBossSong;
        public bool changeTracks = false;

        private static SoundEffects soundEffectObject;


    private SoundEffects(ContentManager content)
    {
      _bullet = content.Load<SoundEffect>("Bullet");
            _bossSong = content.Load<SoundEffect>("Because Princess Inada is Scolding Me (SDM Remix)");
    }

        public static SoundEffects Instance()
        {
            if (soundEffectObject != null)
                return soundEffectObject;
            else
                return null;
        }

        public static SoundEffects Instance(ContentManager content)
        {
            if(soundEffectObject == null)
                soundEffectObject = new SoundEffects(content);
            return soundEffectObject;
        }

        public void update()
        {
            if (changeTracks && theBossSong != null)
            {
                try {
                    theBossSong.Volume -= 0.01f;
                }
                catch (ArgumentOutOfRangeException)
                {
                    theBossSong.Stop();
                    theBossSong = _bossSong.CreateInstance();
                    theBossSong.IsLooped = true;
                    theBossSong.Pitch = 0.3f;
                    theBossSong.Play();
                    changeTracks = false;
                }
            }
        }

  }
}
