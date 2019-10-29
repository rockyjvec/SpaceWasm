﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    class I64store32 : Instruction
    {
        UInt32 align, offset;

        protected override Instruction Run(Stack.Frame f)
        {
            var v = f.PopI64();
            var index = f.PopI32();
            f.Function.Module.Memory[0].SetI32((UInt64)offset + (UInt64)index, (UInt32)v);
            return Next;
        }

        public I64store32(Parser parser, Function f) : base(parser, f, true)
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