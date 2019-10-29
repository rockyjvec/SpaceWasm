﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    internal class F64trunc : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            f.PushF64((double)Math.Truncate((double)f.PopF64()));

            return Next;
        }

        public F64trunc(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}