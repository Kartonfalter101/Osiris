using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Osiris.Core.Utilities
{
    /// <summary>
    /// This class provides method to read memory from any process.
    /// </summary>
    public class Memory
    {
        private float numberOfBytesRead = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Memory"/> class.
        /// </summary>
        /// <param name="gameProcess">Sets the gameprocess to read the memory from.</param>
        public Memory(Process gameProcess = default)
        {
            GameProcess = gameProcess;
        }

        private enum Protection
        {
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400,
        }

        /// <summary>
        /// Gets or sets the game process that shall be automated.
        /// </summary>
        public Process GameProcess { get; set; }

        /// <summary>
        /// Obtains the module based on the given module name.
        /// </summary>
        /// <param name="moduleName">The module name to be find.</param>
        /// <returns>Returns the correct modules based on the specified module name.</returns>
        public ProcessModule GetModuleByModuleName(string moduleName)
        {
            foreach (ProcessModule module in GameProcess.Modules)
            {
                if (module.ModuleName == moduleName)
                {
                    return module;
                }
            }

            return null;
        }

        /// <summary>
        /// Reads the pointer from an address.
        /// </summary>
        /// <param name="offset">The address to read from.</param>
        /// <returns></returns>
        public long ReadPointer(long offset)
        {
            var buffer = new byte[255];
            ReadProcessMemory(GameProcess.Handle, offset, buffer, buffer.Length, ref numberOfBytesRead);
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// Reads the pointer from an address with multiple offsets.
        /// </summary>
        /// <param name="baseAddress">The address to read from.</param>
        /// <param name="offsets">The offsets of the address.</param>
        /// <param name="getValue">Specifies whether the value or the address should be returned.</param>
        /// <returns>Returns the address or the value of the pointer.</returns>
        public long ReadPointer(long baseAddress, long[] offsets, bool getValue = true)
        {
            var value = ReadPointer(baseAddress);
            foreach (long offset in offsets)
            {
                if (!getValue && offset == offsets.Last())
                {
                    return value + offset;
                }

                value = ReadPointer(value + offset);
            }

            return value;
        }

        /// <summary>
        /// Reads an offset that points to a float value.
        /// </summary>
        /// <param name="offset">The offset to be read.</param>
        /// <returns>Returns the value from the given offset.</returns>
        public float ReadFloat(long offset)
        {
            var buffer = new byte[255];
            ReadProcessMemory(GameProcess.Handle, offset, buffer, buffer.Length, ref numberOfBytesRead);
            return BitConverter.ToSingle(buffer, 0);
        }

        /// <summary>
        /// Gets a pointer based on the given array of bytes. NEEDS TO BE REFACTORED.
        /// </summary>
        /// <param name="moduleName">The module name to get the pointer from.</param>
        /// <param name="bytesToFind">The array of bytes to find in the specified module.</param>
        /// <param name="offset">Adds an offset to the found pointer.</param>
        /// <returns>Returns the pointer of the array of bytes.</returns>
        public long GetPointerByAOB(string moduleName, byte[] bytesToFind, byte offset = default)
        {
            int currentOffset = 0;
            bool pointerFound = false;
            var moduleBaseAddress = (long)GetModuleByModuleName(moduleName).BaseAddress;
            var moduleMemorySize = GetModuleByModuleName(moduleName).ModuleMemorySize;
            byte[] bytesOfModule = new byte[moduleMemorySize];

            VirtualProtect(moduleBaseAddress, moduleMemorySize, Protection.PAGE_EXECUTE_READWRITE, out _);
            ReadProcessMemory(GameProcess.Handle, moduleBaseAddress, bytesOfModule, bytesOfModule.Length, ref numberOfBytesRead);

            for (; currentOffset < bytesOfModule.Length; currentOffset++)
            {
                int iterationCount = 0;
                foreach (byte byteToFind in bytesToFind)
                {
                    if (bytesOfModule[currentOffset] == byteToFind)
                    {
                        currentOffset++;
                        iterationCount++;
                        if (iterationCount == bytesToFind.Length)
                        {
                            pointerFound = true;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (pointerFound)
                {
                    break;
                }
            }

            return moduleBaseAddress + (currentOffset - bytesToFind.Length) + offset;
        }

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr handle, long baseAddress, byte[] buffer, int size, ref float numberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtect(long lpAddress, int dwSize, Protection flNewProtect, out Protection lpflOldProtect);
    }
}
