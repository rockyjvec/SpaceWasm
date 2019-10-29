﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    internal class F32max : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            var b = f.PopF32();
            var a = f.PopF32();

            f.Push((float)Math.Max(a, b));
            return Next;
        }

        public F32max(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}