﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    internal class I64rotr : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            var b = f.PopI64();
            var a = f.PopI64();

            f.PushI64((UInt64)((a >> (int)b) | (a << (64 - (int)b))));
            return Next;
        }

        public I64rotr(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}