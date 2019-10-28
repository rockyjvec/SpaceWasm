﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    class MemoryGrow : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            var size = f.PopI32();

            f.Push((UInt32)f.Function.Module.Memory[0].Grow(size));

            return Next;
        }

        public MemoryGrow(Parser parser) : base(parser, true)
        {
            UInt32 zero = parser.GetUInt32(); // May be used in future version of WebAssembly to address additional memories

            if(zero != 0x00)
            {
                Console.WriteLine("WARNING: memory.grow called with non-zero: 0x" + zero.ToString("X"));
            }
        }
    }
}
