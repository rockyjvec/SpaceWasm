﻿namespace GameWasm.Webassembly.Instruction
{
    internal class I64mul : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            var b = f.PopI64();
            var a = f.PopI64();

            f.PushI64(a * b);

            return Next;
        }

        public I64mul(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}