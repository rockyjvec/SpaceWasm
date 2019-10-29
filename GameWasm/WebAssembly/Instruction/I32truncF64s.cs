﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    internal class I32truncF64s : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            f.PushI32((UInt32)(Int32)Math.Truncate((double)f.PopF64()));

            return Next;
        }

        public I32truncF64s(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}