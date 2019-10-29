﻿namespace GameWasm.Webassembly.Instruction
{
    internal class I32and : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            var b = f.PopI32();
            var a = f.PopI32();

            f.PushI32(a & b);
            return Next;
        }

        public I32and(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}