﻿using System;
using System.Linq;

namespace GameWasm.Webassembly.Instruction
{
    class GlobalSet : Instruction
    {
        Global global;
        public int index;

        protected override Instruction Run(Stack.Frame f)
        {
            global.Set(f.Pop());

            return Next;
        }

        public GlobalSet(Parser parser, Function f) : base(parser, f, true)
        {
            index = (int)parser.GetUInt32();
            if (index >= parser.Module.Globals.Count())
                throw new Exception("Invalid global variable");
            global = parser.Module.Globals[index];
        }

        public override string ToString()
        {
            return "set_global " + global.Name + " (" + Type.Pretify(global.GetValue()) + ")";
        }
    }
}
