﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    internal class F32trunc : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            f.PushF32((float)Math.Truncate((float)f.PopF32()));

            return Next;
        }


        public F32trunc(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}