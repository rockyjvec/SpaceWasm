﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    class I32eqz : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            if (f.PopI32() == 0)
            {
                f.PushI32((UInt32)1);
            }
            else
            {
                f.PushI32((UInt32)0);
            }

            return Next;
        }

        public I32eqz(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}