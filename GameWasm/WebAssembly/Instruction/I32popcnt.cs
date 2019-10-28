﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    internal class I32popcnt : Instruction
    {
        public override Instruction Run(Stack.Frame f)
        {
            var a = f.PopI32();

            UInt32 bits = 0;
            UInt32 compare = 1;
            while (true)
            {
                if ((compare & a) != 0)
                {
                    bits++;
                }
                if (compare == 0x80000000) break;
                compare <<= 1;
            }

            f.Push(bits);

            return Next;
        }

        public I32popcnt(Parser parser) : base(parser, true)
        {
        }
    }
}