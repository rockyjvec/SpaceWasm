﻿using System;

namespace GameWasm.Webassembly.Instruction
{
    class Custom : Instruction
    {
        Action a;
        protected override Instruction Run(Stack.Frame f)
        {
            a();
            return Next;
        }

        public Custom(Action a) : base(null,null, true)
        {
            this.a = a;
        }
    }
}
