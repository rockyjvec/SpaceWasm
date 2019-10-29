﻿using System;
using System.Linq;

namespace GameWasm.Webassembly.Instruction
{
    class GlobalGet : Instruction
    {
        Global global;

        protected override Instruction Run(Stack.Frame f)
        {
            f.Push(global.GetValue());
            return Next;
        }

        public GlobalGet(Parser parser, Function f) : base(parser, f, true)
        {
            var index = (int)parser.GetUInt32();
            if (index >= parser.Module.Globals.Count())
                throw new Exception("Invalid global variable");
            global = parser.Module.Globals[index];
        }

        public override string ToString()
        {
            return "get_global " + global.Name + " (" + Type.Pretify(global.GetValue()) + ")";
        }
    }
}