﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    class I64lts : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            var b = (Int64)f.PopI64();
            var a = (Int64)f.PopI64();

            if (a < b)
            {
                f.Push((UInt32)1);
            }
            else
            {
                f.Push((UInt32)0);
            }

            return Next;
        }

        public I64lts(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}
