﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    internal class I64sub : Instruction
    {
        public override Instruction Run(Stack.Frame f)
        {
            var b = f.PopI64();
            var a = f.PopI64();
            
            f.Push(a - b);
            return Next;
        }

        public I64sub(Parser parser) : base(parser, true)
        {
        }
    }
}