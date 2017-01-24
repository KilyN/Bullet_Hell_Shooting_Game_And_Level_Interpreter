using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _487Game
{
    class RectangularHitBox : Hitbox
    {
        private int _height;
        private int _width;
        private float _angle;

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        public float Angle
        {
            get
            {
                return _angle;
            }
            set
            {
                _angle = value;
            }
        }

        public RectangularHitBox(int height, int width, float angle)
        {
            Height = height;
            Width = width;
            _angle = angle;
        }

        public Tuple<float, float>[] GetCorners(float X, float Y)
        {
            Tuple<float, float>[] corners = new Tuple<float, float>[4];
            float newX = (float)Math.Cos(_angle) * _width;
            float newY = (float)Math.Sin(_angle) * _width;
            float Xdiff = (float)Math.Cos(_angle + Math.PI / 4) * _height;
            float Ydiff = (float)Math.Sin(_angle + Math.PI / 4) * _height;

            corners[0] = new Tuple<float, float>(X - Xdiff, Y + Ydiff);
            corners[1] = new Tuple<float, float>(X + Xdiff, Y - Ydiff);
            corners[2] = new Tuple<float, float>(X + newX - Xdiff, Y + newY + Ydiff);
            corners[3] = new Tuple<float, float>(X + newX + Xdiff, Y + newY - Ydiff);

            return corners;
        }
    }
}
