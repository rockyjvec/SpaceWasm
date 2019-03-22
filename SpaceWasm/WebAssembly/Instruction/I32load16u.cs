﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAssembly.Instruction
{
    class I32load16u : Instruction
    {
        UInt32 align, offset;

        public override Instruction Run(Store store)
        {

            store.Stack.Push(store.CurrentFrame.Module.Memory[0].GetI3216u((UInt64)offset + (UInt64)store.Stack.PopI32()));
            return this.Next;
        }

        public I32load16u(Parser parser) : base(parser, true)
        {
            this.align = (UInt32)parser.GetUInt32();
            this.offset = (UInt32)parser.GetUInt32();
        }
    }
}