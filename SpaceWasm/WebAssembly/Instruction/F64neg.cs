﻿namespace WebAssembly.Instruction
{
    internal class F64neg : Instruction
    {
        public override Instruction Run(Store store)
        {
            var a = store.Stack.PopF64();
            store.Stack.Push(-a);
            return this.Next;
        }

        public F64neg(Parser parser) : base(parser, true)
        {
        }
    }
}