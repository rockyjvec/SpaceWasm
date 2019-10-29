﻿namespace GameWasm.Webassembly.Instruction
{
    internal class F64copysign : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            var b = f.PopF64();
            var a = f.PopF64();

            if (a >= 0 && b < 0)
            {
                a = -a;
            }

            if (a < 0 && b >= 0)
            {
                a = -a;
            }

            f.PushF64(a);
            return Next;
        }

        public F64copysign(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}