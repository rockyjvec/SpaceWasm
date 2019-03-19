﻿namespace WebAssembly.Instruction
{
    internal class I32remu : Instruction
    {
        public override Instruction Run(Store store)
        {
            var b = store.Stack.PopI32();
            var a = store.Stack.PopI32();

            store.Stack.Push(a % b);

            return this.Next;
        }

        public I32remu(Parser parser) : base(parser, true)
        {
        }
    }
}