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
    ///  This class is an interpreter for all keyboard input. Hold a player reference to call relevent play methods
    /// </summary>
    class KeystrokeInterpreter
    {
        KeyboardState _oldState;
        Player _player;
        bool _controlToggle;
        bool _bombIsDeployed;

        public KeystrokeInterpreter(Player player)
        {
            _player = player;
            _oldState = Keyboard.GetState();
            _controlToggle = true;
            _bombIsDeployed = false;
        }

        public void PerformAction(KeyboardState newState)
        {

            // Keyboard Up
            if (newState.IsKeyDown(Keys.Up) || newState.IsKeyDown(Keys.W))
                _player.MovePlayer(Keys.Up);

            if (newState.IsKeyDown(Keys.Down) || newState.IsKeyDown(Keys.S))
                _player.MovePlayer(Keys.Down);

            if (newState.IsKeyDown(Keys.Left) || newState.IsKeyDown(Keys.A))
                _player.MovePlayer(Keys.Left);

            if (newState.IsKeyDown(Keys.Right) || newState.IsKeyDown(Keys.D))
                _player.MovePlayer(Keys.Right);

            if (newState.IsKeyDown(Keys.RightControl) && _controlToggle == true)
            {
                if (!_oldState.IsKeyDown(Keys.RightControl))
                {
                    _player.ToggleSpeed();
                    _controlToggle = false;
                }
            }

            if (newState.IsKeyDown(Keys.Y) && newState.IsKeyDown(Keys.U))
            {
                _player.triggerHealthCheatCode();
            }

            if (newState.IsKeyDown(Keys.P))
            {
                if (!_player.isBulletCheatOn) { _player.isBulletCheatOn = true; }
                else if (_player.isBulletCheatOn) { _player.isBulletCheatOn = false; }
            }

            if (newState.IsKeyUp(Keys.RightControl))
                _controlToggle = true;

            if (newState.IsKeyDown(Keys.Space))
                _player.Shoot();

            if (newState.IsKeyDown(Keys.NumPad5) || newState.IsKeyDown(Keys.D5))
            {
                if (!_oldState.IsKeyDown(Keys.NumPad5))
                    BulletFactory.Instance().requestTrollProjectile(_player.X,_player.Y,(float)(3* Math.PI) / 2f,0);
            }

            if (newState.IsKeyDown(Keys.NumPad7) || newState.IsKeyDown(Keys.D7))
            {
                if (!_oldState.IsKeyDown(Keys.NumPad7))
                    BulletFactory.Instance().requestTrollProjectile2(_player.X, _player.Y, -Vector2.UnitY, 0);
            }

            if (newState.IsKeyDown(Keys.NumPad9) || newState.IsKeyDown(Keys.D9))
            {
                if (!_oldState.IsKeyDown(Keys.NumPad9))
                    for(int i = 0; i < 720; i++)
                        BulletFactory.Instance().requestArcingProjectile(_player.X, _player.Y, (float)(2f * Math.PI) * (i / 720f), 0.04f, BulletFactory.Instance().textureContainer.greenProjectile, 300,true);
            }
            if (newState.IsKeyDown(Keys.B)&& _bombIsDeployed==false)         // shootBoom once
            {
                if (!_oldState.IsKeyDown(Keys.B))
                    _player.shootBoom();
                    _bombIsDeployed = true;
                    _player.bombIsDisplay = false;
                    
            }
            _oldState = newState;

        }
    }
}
