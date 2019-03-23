﻿using System;

namespace WebAssembly.Instruction
{
    internal class I64truncF32u : Instruction
    {
        public override Instruction Run(Store store)
        {
            store.Stack.Push((UInt64)Math.Truncate((float)store.Stack.PopF32()));

            return this.Next;
        }

        public I64truncF32u(Parser parser) : base(parser, true)
        {
        }
    }
}