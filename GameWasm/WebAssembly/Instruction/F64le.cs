﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    internal class F64le : Instruction
    {
        public override Instruction Run(Stack.Frame f)
        {
            var b = f.PopF64();
            var a = f.PopF64();

            if (a <= b)
            {
                f.Push((UInt32)1);
            }
            else
            {
                f.Push((UInt32)0);
            }

            return Next;
        }

        public F64le(Parser parser) : base(parser, true)
        {
        }
    }
}