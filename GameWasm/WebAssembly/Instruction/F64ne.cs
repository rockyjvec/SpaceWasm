﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    internal class F64ne : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            if (f.PopF64() != f.PopF64())
            {
                f.Push((UInt32)1);
            }
            else
            {
                f.Push((UInt32)0);
            }

            return Next;
        }

        public F64ne(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}