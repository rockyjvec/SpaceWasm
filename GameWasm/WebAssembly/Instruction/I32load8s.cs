﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    class I32load8s : Instruction
    {
        UInt32 align, offset;

        public override Instruction Run(Stack.Frame f)
        {
            f.Push(f.Function.Module.Memory[0].GetI328s((UInt64)offset + (UInt64)f.PopI32()));
            return Next;
        }

        public I32load8s(Parser parser) : base(parser, true)
        {
            align = (UInt32)parser.GetUInt32();
            offset = (UInt32)parser.GetUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + "(offset = " + offset + ")";
        }
    }
}
