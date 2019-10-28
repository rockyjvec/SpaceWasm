﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    internal class F64max : Instruction
    {
        public override Instruction Run(Stack.Frame f)
        {
            var b = f.PopF64();
            var a = f.PopF64();

            f.Push((double)Math.Max(a, b));
            return Next;
        }

        public F64max(Parser parser) : base(parser, true)
        {
        }
    }
}