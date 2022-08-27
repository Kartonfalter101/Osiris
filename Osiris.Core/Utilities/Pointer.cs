using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osiris.Core.Utilities
{
    /// <summary>
    /// Used to store data for pattern scanning purposes.
    /// </summary>
    public struct Pointer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Pointer"/> struct.
        /// </summary>
        /// <param name="process">The process in which the pattern is to be searched.</param>
        /// <param name="address">The address to read from.</param>
        /// <param name="pointerPattern">The pattern with which to find the pointer in the specified module.</param>
        /// <param name="offsets">The offsets to the corresponding pointer.</param>
        public Pointer(Process process, long address = default, long[] pointerPattern = default, long[] offsets = default)
        {
            Memory mem = new Memory(process);

            Process = process;
            PointerPattern = pointerPattern;
            Offsets = offsets;
            Address = mem.ReadPointer(address, offsets, getValue: false);
            Value = mem.ReadPointer(address, offsets, getValue: true);
        }

        /// <summary>
        /// Gets the specified module in which the pattern is to be searched.
        /// </summary>
        public Process Process { get; private set; }

        /// <summary>
        /// Gets the Address of the pointer.
        /// </summary>
        public long Address { get; private set; }

        /// <summary>
        /// Gets the value of the pointer.
        /// </summary>
        public long Value { get; private set; }

        /// <summary>
        /// Gets the pattern with which to find the pointer in the specified module.
        /// </summary>
        public long[] PointerPattern { get; private set; }

        /// <summary>
        /// Gets the offsets to the corresponding pointer.
        /// </summary>
        public long[] Offsets { get; private set; }
    }
}
