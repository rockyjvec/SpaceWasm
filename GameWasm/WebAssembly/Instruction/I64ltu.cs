﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    class I64ltu : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            var b = (UInt64)f.PopI64();
            var a = (UInt64)f.PopI64();

            if (a < b)
            {
                f.PushI32((UInt32)1);
            }
            else
            {
                f.PushI32((UInt32)0);
            }

            return Next;
        }

        public I64ltu(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}
