﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    internal class F32abs : Instruction
    {
        public override Instruction Run(Stack.Frame f)
        {
            f.Push((float)Math.Abs(f.PopF32()));
            return Next;
        }
        public F32abs(Parser parser) : base(parser, true)
        {
        }
    }
}