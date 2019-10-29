﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    internal class I64rems : Instruction
    {
        protected override Instruction Run(Stack.Frame f)
        {
            var b = (Int64)f.PopI64();
            var a = (Int64)f.PopI64();

            if (b == 0) throw new Trap("integer divide by zero");

            try
            {
                if ((UInt64)a == 0x8000000000000000 && (UInt64)b == 0xFFFFFFFFFFFFFFFF)
                {
                    f.Push((UInt64)0);
                }
                else
                {
                    f.Push((UInt64)(a % b));
                }
            }
            catch (System.OverflowException e)
            {
                throw new Trap("integer overflow");
            }

            return Next;
        }

        public I64rems(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}