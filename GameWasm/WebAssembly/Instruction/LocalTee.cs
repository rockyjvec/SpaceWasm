﻿using System;
using System.Linq;

namespace GameWasm.Webassembly.Instruction
{
    class LocalTee : Instruction
    {
        public int index;

        protected override Instruction Run(Stack.Frame f)
        {
            var a = f.PopValue();
            f.Push(a);
            if (index >= f.Locals.Count())
                throw new Exception("Invalid local variable");
            f.Locals[index] = a;
            return Next;
        }

        public LocalTee(Parser parser, Function f) : base(parser, f, true)
        {
            index = (int)parser.GetUInt32();
        }

        public override string ToString()
        {
            return "tee_local $var" + index;
        }
    }
}