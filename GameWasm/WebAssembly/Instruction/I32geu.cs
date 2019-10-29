﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    class I32geu : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            var b = (UInt32)f.PopI32();
            var a = (UInt32)f.PopI32();

            if (a >= b)
            {
                f.PushI32((UInt32)1);
            }
            else
            {
                f.PushI32((UInt32)0);
            }

            return Next;
        }

        public I32geu(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}
