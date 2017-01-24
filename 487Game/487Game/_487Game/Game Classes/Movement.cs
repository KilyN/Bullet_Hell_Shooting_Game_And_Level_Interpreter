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
    /// The class that each GameObject posesses which tells it how to move.
    /// </summary>
    public class Movement
    {
        //Member Variables
        /// <summary>
        /// TurningAngle should be anywhere from 0 to about 0.05. Zero = straight. Postive = arc right. Negitive = arc left.
        /// Represents by how many radians the object should turn it's trajectory each tick.
        /// </summary>
        public float turningAngle;
        /// <summary>
        /// The current speed of the object, stored seperatly from the trajectory even though speed is used to determine the magintiude of the trajectory.
        /// </summary>
        public float speed;
        public float turningAngleAccleration;
        public Vector2 initalLaunchTrajectory;

        private int index = 0;
        private int duration;
        public bool finished = false;
        //Properties


        //Constructors
        /// <summary>
        /// Constructor that takes a launch angle rather than a trajectory vector as one of it's parameters.
        /// </summary>
        /// <param name="launchAngle">The angle at which to move at.</param>
        /// <param name="turningAngle">By how much the trajectory should turn each tick, in radians.
        /// Should be anywhere from 0 to about 0.05. Zero = straight. Postive = arc right. Negitive = arc left. Defaults to zero.</param>
        /// <param name="speed">How fast the object is moving. Defaults to 100.</param>
        public Movement(float launchAngle = float.NaN, int duration = -1, float turningAngle = 0, float speed = 100f, float turningAngleAccleration = 1f)
        {
            this.speed = speed;
            this.turningAngle = turningAngle;
            this.duration = duration;
            this.turningAngleAccleration = turningAngleAccleration;
            
            if(launchAngle == float.NaN)
            {
                initalLaunchTrajectory.X = float.NaN;
                initalLaunchTrajectory.Y = float.NaN;
            }
            else
            {
                initalLaunchTrajectory.X = (float)Math.Cos(launchAngle);
                initalLaunchTrajectory.Y = (float)Math.Sin(launchAngle);
                initalLaunchTrajectory *= speed;
            }
        }

        /// <summary>
        /// Constructor that takes a Vector for the trajctory instead of a angle.
        /// </summary>
        /// <param name="inputTrajectory">The trajectory at which to move at.</param>
        /// <param name="turningAngle">By how much the trajectory should turn each tick, in radians.
        /// Should be anywhere from 0 to about 0.05. Zero = straight. Postive = arc right. Negitive = arc left. Defaults to zero.</param>
        /// <param name="speed">How fast the object is moving. Defaults to 100.</param>
        public Movement(Vector2? inputTrajectory = null, int duration = -1, float turningAngle = 0, float speed = 100f, float turningAngleAccleration = 1f)
        {
            this.speed = speed;
            this.turningAngle = turningAngle;
            this.duration = duration;
            this.turningAngleAccleration = turningAngleAccleration;

            if(inputTrajectory == null)
            {
                initalLaunchTrajectory.X = float.NaN;
                initalLaunchTrajectory.Y = float.NaN;
            }
            else
            {
                Vector2 temp = new Vector2();
                temp.X = inputTrajectory.Value.X;
                temp.Y = inputTrajectory.Value.Y;
                temp.Normalize();
                this.initalLaunchTrajectory = temp * speed;
            }
        }

        //Methods
        /// <summary>
        /// The function which moves the given object by using it's internal parameters.
        /// </summary>
        /// <param name="obj">Object to move.</param>
        public void Move(GameObject obj, GameTime gameTime)
        {
            if (!obj.toDelete && !finished)//Don't update the sprite if it needs to be deleted. (This should never need to be used.)
            {
                if (index == 0 && !float.IsNaN(initalLaunchTrajectory.X))
                    obj.trajectory = this.initalLaunchTrajectory;
                else if(index == 0)
                {
                    obj.trajectory.Normalize();
                    obj.trajectory *= speed;
                }

                turningAngle *= turningAngleAccleration;

                //This line rotates a vector. I have no idea how it works and I wrote it myself, but it somehow does...
                obj.trajectory = Vector2.Transform(obj.trajectory, Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, turningAngle)));

                if (obj.trajectory.Y > 0)
                    obj.RotationAngle = (float)Math.Acos(Vector2.Dot(obj.trajectory, Vector2.UnitX) / (obj.trajectory.Length() * Vector2.UnitX.Length()));
                else //Updates the trajectory the projectile is traveling in so the sprite can point that direction as well.
                    obj.RotationAngle = (float)(Math.PI * 2 - Math.Acos(Vector2.Dot(obj.trajectory, Vector2.UnitX) / (obj.trajectory.Length() * Vector2.UnitX.Length())));

                obj.spritePosition += obj.trajectory * (float)gameTime.ElapsedGameTime.TotalSeconds;//Move object

                index++;
                if (duration <= index && duration != -1)
                    finished = true;
            }
        }
    }
}