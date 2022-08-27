using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Osiris.Core
{
    /// <summary>
    /// The Input component that provides methods to emulate keyboard actions.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// The available key states.
        /// </summary>
        public enum KeyState
        {
            /// <summary>
            /// The key down state.
            /// </summary>
            WM_KEYDOWN = 0x100,

            /// <summary>
            /// The key up state.
            /// </summary>
            WM_KEYUP = 0x101,
        }

        /// <summary>
        /// Emulates a single keystroke to the target process.
        /// </summary>
        /// <param name="process">The target process.</param>
        /// <param name="keyState">The key state.</param>
        /// <param name="key">The actual key to emulate.</param>
        public static void Emulate(Process process, KeyState keyState, Keys key)
        {
            PostMessage(process.MainWindowHandle, (uint)keyState, (int)key, 0);
        }

        /// <summary>
        /// Emulates whole strings by sending several keystrokes.
        /// </summary>
        /// <param name="process">The target process.</param>
        /// <param name="keyState">The key state.</param>
        /// <param name="value">The value to be emulated.</param>
        public static void Emulate(Process process, KeyState keyState, string value)
        {
            foreach (char key in value.ToCharArray())
            {
                PostMessage(process.MainWindowHandle, (uint)keyState, (int)ConvertCharToVirtualKey(key), 0);
            }
        }

        private static Keys ConvertCharToVirtualKey(char key)
        {
            int vkey = VkKeyScan(key);
            int modifiers = vkey >> 8;
            Keys retval = (Keys)(vkey & 0xff);

            if ((modifiers & 1) != 0)
            {
                retval |= Keys.Shift;
            }

            if ((modifiers & 2) != 0)
            {
                retval |= Keys.Control;
            }

            if ((modifiers & 4) != 0)
            {
                retval |= Keys.Alt;
            }

            if (key == '/')
            {
                retval = Keys.Divide;
            }

            return retval;
        }

        [DllImport("user32.dll")]
        private static extern int VkKeyScan(char key);

        [DllImport("user32.dll")]
        private static extern IntPtr PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
    }
}
