﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAssembly.Instruction
{
    class If : Instruction
    {
        Instruction endLabel;

        byte type;

        public override void End(Instruction end)
        {
            this.endLabel = end;
        }

        public override Instruction Run(Store store)
        {
            var v = store.Stack.PopI32();

            store.CurrentFrame.Labels.Push(this.endLabel);

            if (v > 0)
            {
                return this.Next;
            }
            else
            {
                if(this.endLabel as Else == null)
                {
                    store.CurrentFrame.Labels.Pop();
                }

                return this.endLabel.Next;
            }
        }

        public If(Parser parser) : base(parser, true)
        {
            this.type = parser.GetBlockType();
        }
    }
}
