using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _487Game
{
  // This class encapsulates our game settings
  public class GameSettings
  {
    // Settings
    // resolution
    int _width;
    int _height;
    bool _mute;

    // Do control mapping here later with keystrokes

    // Getters/Setters
    public int Width
    {
      get { return _width; }
      set { _width = value; }
    }

    public int Height
    {
      get { return _height; }
      set { _height = value; }
    }

    public void ToggleMute()
    {
      _mute = !_mute;
    }

    // Default Settings
    public GameSettings()
    {
      _width = 600;
      _height = 800;
      _mute = false;
    }
  }
}
