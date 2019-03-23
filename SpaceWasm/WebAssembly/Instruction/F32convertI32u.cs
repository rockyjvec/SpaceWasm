﻿namespace WebAssembly.Instruction
{
    internal class F32convertI32u : Instruction
    {
        public override Instruction Run(Store store)
        {
            store.Stack.Push((float)store.Stack.PopI32());
            return this.Next;
        }

        public F32convertI32u(Parser parser) : base(parser, true)
        {
        }
    }
}