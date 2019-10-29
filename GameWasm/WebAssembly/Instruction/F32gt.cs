﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    class F32gt : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            var b = f.PopF32();
            var a = f.PopF32();

            if (a > b)
            {
                f.PushI32((UInt32)1);
            }
            else
            {
                f.PushI32((UInt32)0);
            }

            return Next;
        }

        public F32gt(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}