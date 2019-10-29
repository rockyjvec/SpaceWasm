﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    internal class F32reinterpretI32 : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            f.PushF32(BitConverter.ToSingle(BitConverter.GetBytes(f.PopI32()), 0));
            return Next;
        }

        public F32reinterpretI32(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}