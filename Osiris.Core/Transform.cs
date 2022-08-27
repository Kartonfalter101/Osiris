using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using Osiris.Core.Geometry;
using Osiris.Core.Utilities;

namespace Osiris.Core
{
    /// <summary>
    /// The base component used to store movement specific data of the player.
    /// </summary>
    public class Transform
    {
        private readonly Memory memory;

        private readonly long xPointer;
        private readonly long yPointer;
        private readonly long zPointer;
        private readonly long rPointer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> class.
        /// </summary>
        /// <param name="gameProcess">The game process to be automated.</param>
        /// <param name="x">The pointer to find the x coordinate in a 3d world.</param>
        /// <param name="y">The pointer to find the y coordinate in a 3d world.</param>
        /// <param name="z">The pointer to find the z coordinate in a 3d world.</param>
        /// <param name="r">The pointer to find the rotation in a 3d world.</param>
        public Transform(Process gameProcess, long x, long y, long z, long r)
        {
            GameProcess = gameProcess;

            memory = new Memory(GameProcess);

            xPointer = x;
            yPointer = y;
            zPointer = z;
            rPointer = r;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> class.
        /// </summary>
        /// <param name="gameProcess">The game process to be automated.</param>
        /// <param name="xpattern">The pattern to find the pointer that points to the x coordinate in a 3d world.</param>
        /// <param name="ypattern">The pattern to find the pointer that points to the y coordinate in a 3d world.</param>
        /// <param name="zpattern">The pattern to find the pointer that points to the z coordinate in a 3d world.</param>
        /// <param name="rpattern">The pattern to find the pointer that points to the rotation in a 3d world.</param>
        public Transform(Process gameProcess, Pointer xpattern, Pointer ypattern, Pointer zpattern, Pointer rpattern)
        {
            // var coordinatesVector = GetPointerByAOB("yourprocess.exe", CoordinatesPattern);
            // xPointer = coordinatesVector + 44;
            // yPointer = coordinatesVector + 48;
            // zPointer = coordinatesVector + 40;
            // rPointer = GetPointerByAOB("yourprocess.exe", RotationPattern, 22);
        }

        /// <summary>
        /// Gets the Memory Instance.
        /// </summary>
        public Memory MemoryInstance { get; }

        /// <summary>
        /// Gets the game process to be automated.
        /// </summary>
        public Process GameProcess { get; private set; }

        /// <summary>
        /// Gets the position of the player.
        /// </summary>
        public Vector Position
        {
            get
            {
                return new Vector(
                    memory.ReadFloat(xPointer),
                    memory.ReadFloat(yPointer),
                    memory.ReadFloat(zPointer));
            }
        }

        /// <summary>
        /// Gets the rotation of the player.
        /// </summary>
        public Euler Rotation
        {
            get
            {
                return new Euler(
                    0,
                    memory.ReadFloat(rPointer) * (float)(180 / Math.PI),
                    0);
            }
        }

        /// <summary>
        /// Looks at the given target position.
        /// </summary>
        /// <param name="targetPosition">The Vector to look at.</param>
        public void LookAt(Vector targetPosition)
        {
            bool rotatePlayer = true;
            
            while (rotatePlayer)
            {
                // Prevents from crashing in some game clients due to posting too much messages in a short period of time.
                Thread.Sleep(75);

                // Gets the current angle between player and target
                var lookAtRotation = Euler.LookAtRotation(Position, targetPosition, Rotation);
                if (Euler.TurnDirection(lookAtRotation) is Euler.Direction.Right)
                {
                    // Makes the player to turn to the right side.
                    Input.Emulate(GameProcess, Input.KeyState.WM_KEYDOWN, Keys.D);
                    while (lookAtRotation < 355.0d)
                    {
                        lookAtRotation = Euler.LookAtRotation(Position, targetPosition, Rotation);

                        if (Euler.TurnDirection(lookAtRotation) != Euler.Direction.Right)
                        {
                            LookAt(targetPosition);
                            return;
                        }
                    }

                    while (lookAtRotation < 357d)
                    {
                        lookAtRotation = Euler.LookAtRotation(Position, targetPosition, Rotation);

                        if (Euler.TurnDirection(lookAtRotation) != Euler.Direction.Right)
                        {
                            LookAt(targetPosition);
                            return;
                        }
                    }

                    // Stops the player from turning to the right side.
                    Input.Emulate(GameProcess, Input.KeyState.WM_KEYUP, Keys.D);
                    rotatePlayer = false;
                }
                else
                {
                    // Makes the player to turn to the left side.
                    Input.Emulate(GameProcess, Input.KeyState.WM_KEYDOWN, Keys.A);
                    while (lookAtRotation > 5.0d)
                    {

                        lookAtRotation = Euler.LookAtRotation(Position, targetPosition, Rotation);

                        if (Euler.TurnDirection(lookAtRotation) != Euler.Direction.Left)
                        {
                            LookAt(targetPosition);
                            return;
                        }
                    }

                    while (lookAtRotation > 3d)
                    {
                        lookAtRotation = Euler.LookAtRotation(Position, targetPosition, Rotation);

                        if (Euler.TurnDirection(lookAtRotation) != Euler.Direction.Left)
                        {
                            LookAt(targetPosition);
                            return;
                        }
                    }

                    // Stops the player from turning to the left side.
                    Input.Emulate(GameProcess, Input.KeyState.WM_KEYUP, Keys.A);
                    rotatePlayer = false;
                }
            }
        }

        /// <summary>
        /// Moves to the given target position.
        /// </summary>
        /// <param name="targetPosition">The Vector to move to.</param>
        public void MoveTo(Vector targetPosition, bool throttleSpeed = false)
        {
            // Gets the current angle between player and target.
            var lookAtRotation = Euler.LookAtRotation(Position, targetPosition, Rotation);

            // Makes the player move forward.
            Input.Emulate(GameProcess, Input.KeyState.WM_KEYDOWN, Keys.W);

            // Checks continuously the distance between player and target and adjusts the rotation.
            while (Vector.Distance(Position, targetPosition) > 10f)
            {
                LookAt(targetPosition);
            }

            // Stops the player from moving forward.
            Input.Emulate(GameProcess, Input.KeyState.WM_KEYUP, Keys.W);
        }
    }
}