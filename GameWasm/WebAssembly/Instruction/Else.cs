﻿namespace GameWasm.Webassembly.Instruction
{
    internal class Else : Instruction
    {
        public Instruction end;
        public int endPos = 0;

        protected override Instruction Run(Stack.Frame f)
        {
            return end;
        }

        public override void End(Instruction end, int pos = 0)
        {
            this.end = end;
            this.endPos = pos;
        }

        public Else(Parser parser, Function f) : base(parser, f, true)
        {
        }
    }
}