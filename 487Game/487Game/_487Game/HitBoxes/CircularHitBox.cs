using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _487Game
{
    public class CircularHitBox : Hitbox
    {
        /// <summary>
        /// Member variable for the radius of the hitbox.
        /// </summary>
        private float _radius;
        
        /// <summary>
        /// Method to get the Radius value / Set the radius value.
        /// </summary>
        public float Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
            }
        }

        public CircularHitBox(float radius)
        {
            Radius = radius;
        }
        
    }
}
