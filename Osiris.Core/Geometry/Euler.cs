using System;

namespace Osiris.Core.Geometry
{
    /// <summary>
    /// This component is used to define the rotation of a three-dimensonal object. (this project only uses the y component)
    /// </summary>
    public struct Euler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Euler"/> struct.
        /// </summary>
        /// <param name="x">The x component of the euler.</param>
        /// <param name="y">The y component of the euler.</param>
        /// <param name="z">The z component of the euler.</param>
        public Euler(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Directions to look at.
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// Value for turning Left.
            /// </summary>
            Left = 0,

            /// <summary>
            /// Value for turning Right.
            /// </summary>
            Right = 1,
        }

        /// <summary>
        /// Gets the X component of the euler.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the Y component of the euler.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Gets the Z component of the euler.
        /// </summary>
        public double Z { get; }

        /// <summary>
        /// Calculates the appropriate turn direction from the look-at rotation.
        /// </summary>
        /// <param name="lookAtRotation">The look-at rotation of which the turn direction is calculated.</param>
        /// <returns>Returns the appropriate turn direction.</returns>
        public static Direction TurnDirection(double lookAtRotation)
        {
            if (lookAtRotation > 180)
            {
                return Direction.Right;
            }
            else
            {
                return Direction.Left;
            }
        }

        /// <summary>
        /// Redefines the pivot point of the player euler so when linearly aligned to it will always return 0/360 degrees.
        /// </summary>
        /// <param name="origin">The player position.</param>
        /// <param name="target">The target position.</param>
        /// <param name="playerRotation">The current player rotation.</param>
        /// <returns>Returns the player rotation based on the target position.</returns>
        public static double LookAtRotation(Vector origin, Vector target, Euler playerRotation)
        {
            var difference = LookAtAngle(origin, target) - playerRotation.Y;

            if (difference < 0)
            {
                difference += 360;
            }

            return difference;
        }

        private static double LookAtAngle(Vector origin, Vector target)
        {
            var degrees = Math.Atan2(target.X + (-origin.X), target.Z + (-origin.Z)) * (180 / Math.PI);

            if (degrees >= 0)
            {
                return degrees;
            }
            else
            {
                return 180 + (180 - (-degrees));
            }
        }
    }
}