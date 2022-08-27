using System;

namespace Osiris.Core.Geometry
{
    /// <summary>
    /// This component is used to define the position of a three-dimensonal or two-dimensional object.
    /// </summary>
    public struct Vector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> struct.
        /// </summary>
        /// <param name="x">The x component of the three-dimensional Vector.</param>
        /// <param name="y">The y component of the three-dimensional Vector.</param>
        /// <param name="z">The z component of the three-dimensional Vector.</param>
        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> struct.
        /// </summary>
        /// <param name="x">The x component of the two-dimensional Vector.</param>
        /// <param name="z">The z component of the two-dimensional Vector.</param>
        public Vector(double x, double z)
        {
            X = x;
            Y = 0;
            Z = z;
        }

        /// <summary>
        /// Gets a zero vector.
        /// </summary>
        public static Vector Zero
        {
            get
            {
                return new Vector(0, 0, 0);
            }
        }

        /// <summary>
        /// Gets the X component of the Vector.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the Y component of the Vector.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Gets the Z component of the Vector.
        /// </summary>
        public double Z { get; }

        public static Vector operator +(Vector origin, Vector target)
        {
            return new Vector(origin.X + target.X, origin.Y + target.Y, origin.Z + target.Z);
        }

        public static Vector operator -(Vector origin, Vector target)
        {
            return new Vector(origin.X - target.X, origin.Y - target.Y, origin.Z - target.Z);
        }

        public static Vector operator -(Vector origin)
        {
            return new Vector(-origin.X, -origin.Y, -origin.Z);
        }

        public static Vector operator *(Vector origin, double value)
        {
            return new Vector(origin.X * value, origin.Y * value, origin.Z * value);
        }

        public static Vector operator *(double value, Vector origin)
        {
            return new Vector(origin.X * value, origin.Y * value, origin.Z * value);
        }

        public static Vector operator /(Vector origin, double value)
        {
            return new Vector(origin.X / value, origin.Y / value, origin.Z / value);
        }

        public static bool operator !=(Vector origin, Vector target)
        {
            return !(origin == target);
        }

        public static bool operator ==(Vector origin, Vector target)
        {
            double diffX = origin.X - target.X;
            double diffY = origin.Y - target.Y;
            double diffZ = origin.Z - target.Z;
            if (diffX == 0 && diffY == 0 && diffZ == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates the distance between two two-dimensional Vectors.
        /// </summary>
        /// <param name="origin">The origin point to subtract from.</param>
        /// <param name="target">The target point to be subtracted.</param>
        /// <returns>Returns the distance between two two-dimensional Vectors.</returns>
        public static double Distance(Vector origin, Vector target)
        {
            var xDistance = origin.X - target.X;
            var zDistance = origin.Z - target.Z;

            return Math.Sqrt(Math.Pow(xDistance, 2.00) + Math.Pow(zDistance, 2.00));
        }

        /// <summary>
        /// Calculates a hash code that can be used to insert and identify an object in a hash-based collection.
        /// </summary>
        /// <returns>Returns a hash code.</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ (Y.GetHashCode() << 2) ^ (Z.GetHashCode() >> 2);
        }

        /// <summary>
        /// Determines whether two object instances are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>Return <see cref="true"/> if the object are considered equal.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Vector))
            {
                return false;
            }

            return Equals((Vector)obj);
        }
    }
}
