﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    class F64load : Instruction
    {
        public UInt32 align, offset;

        public F64load(Parser parser) : base(parser, true)
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
