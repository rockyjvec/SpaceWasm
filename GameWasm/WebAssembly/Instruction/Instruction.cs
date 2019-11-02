﻿using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GameWasm.Webassembly.New;

namespace GameWasm.Webassembly.Instruction
{
    public class Instruction
    {
        public byte opCode = 0x00;
        public Instruction Next = null;
        public UInt32 Pointer = 0;
        public int Pos = 0;
        
        public Instruction(Parser parser, bool implemented = false)
        {
            if(!implemented)
            {
                throw new Exception("Instruction not implemmented: " + this);
            }
        }

        public virtual void End(Instruction end)
        {
           // throw new Exception("End not implementedin " + this);
        }

        public static Inst[] Consume(Parser parser, bool debug)
        {
            Stack<Instruction> controlFlowStack = new Stack<Instruction>();
            Instruction start = null;
            Instruction last = null;

            bool done = false;

            int pos = 0;
            while (!done)
            {
                Instruction current = null;
                UInt32 pointer = parser.GetPointer();
                byte code = parser.GetByte();
                
                switch (code)
                {
                    /* Control Instructions */

                    case 0x00: // unreachable
                        current = new Unreachable(parser);
                        break;
                    case 0x01: // nop
                        current = new Nop(parser);
                        break;
                    case 0x02: // block
                        current = new Block(parser);
                        controlFlowStack.Push(current);
                        break;
                    case 0x03: // loop
                        current = new Loop(parser);
                        controlFlowStack.Push(current);
                        break;
                    case 0x04: // if
                        current = new If(parser);
                        controlFlowStack.Push(current);
                        break;
                    case 0x05: // else
                        if (controlFlowStack.Count() == 0)
                        {
                            throw new Exception("Else with no matching if.");
                        }
                        else
                        {
                            current = new Else(parser);
                            var match = controlFlowStack.Pop();
                            match.End(current); // notify of else
                            controlFlowStack.Push(current); // add back to find end
                            break;
                        }
                    case 0x0B: // end
                        if(controlFlowStack.Count() == 0)
                        {
                            current = new End(parser);
                            done = true;
                            break;
                        }
                        else
                        {
                            current = new End(parser);
                            var match = controlFlowStack.Pop();

                            match.End(current); // notify of end
                            break;
                        }
                    case 0x0C: // br
                        current = new Br(parser); break;
                    case 0x0D: // br_if
                        current = new BrIf(parser); break;
                    case 0x0E: // br_table
                        current = new BrTable(parser); break;
                    case 0x0F: // return
                        current = new Return(parser); break;
                    case 0x10: // call
                        current = new Call(parser); break;
                    case 0x11: // call_indirect
                        current = new CallIndirect(parser); break;

                    /* Parametric Instructions */

                    case 0x1A: // drop
                        current = new Drop(parser); break;
                    case 0x1B: // select
                        current = new Select(parser); break;

                    /* Variable Instructions */

                    case 0x20: // local.get
                        current = new LocalGet(parser); break;
                    case 0x21: // local.set
                        current = new LocalSet(parser); break;
                    case 0x22: // local.tee
                        current = new LocalTee(parser); break;
                    case 0x23: // global.get
                        current = new GlobalGet(parser); break;
                    case 0x24: // global.set
                        current = new GlobalSet(parser); break;

                    /* Memory Instructions */

                    case 0x28: // i32.load
                        current = new I32load(parser); break;
                    case 0x29: // i64.load
                        current = new I64load(parser); break;
                    case 0x2A: // f32.load
                        current = new F32load(parser); break;
                    case 0x2B: // f64.load
                        current = new F64load(parser); break;
                    case 0x2C: // i32.load8_s
                        current = new I32load8s(parser); break;
                    case 0x2D: // i32.load8_u
                        current = new I32load8u(parser); break;
                    case 0x2E: // i32.load16_s
                        current = new I32load16s(parser); break;
                    case 0x2F: // i32.load16_u
                        current = new I32load16u(parser); break;
                    case 0x30: // i64.load8_s
                        current = new I64load8s(parser); break;
                    case 0x31: // i64.load8_u
                        current = new I64load8u(parser); break;
                    case 0x32: // i64.load16_s
                        current = new I64load16s(parser); break;
                    case 0x33: // i64.load16_u
                        current = new I64load16u(parser); break;
                    case 0x34: // i64.load32_s
                        current = new I64load32s(parser); break;
                    case 0x35: // i64.load32_u
                        current = new I64load32u(parser); break;
                    case 0x36: // i32.store
                        current = new I32store(parser); break;
                    case 0x37: // i64.store
                        current = new I64store(parser); break;
                    case 0x38: // f32.store
                        current = new F32store(parser); break;
                    case 0x39: // f64.store
                        current = new F64store(parser); break;
                    case 0x3A: // i32.store8
                        current = new I32store8(parser); break;
                    case 0x3B: // i32.store16
                        current = new I32store16(parser); break;
                    case 0x3C: // i64.store8
                        current = new I64store8(parser); break;
                    case 0x3D: // i64.store16
                        current = new I64store16(parser); break;
                    case 0x3E: // i64.store32
                        current = new I64store32(parser); break;
                    case 0x3F: // memory.size
                        current = new MemorySize(parser); break;
                    case 0x40: // memory.grow
                        current = new MemoryGrow(parser); break;

                    /* Numeric Instructions */

                    case 0x41: // i32.const
                        current = new I32const(parser); break;
                    case 0x42: // i64.const
                        current = new I64const(parser); break;
                    case 0x43: // f32.const
                        current = new F32const(parser); break;
                    case 0x44: // f64.const
                        current = new F64const(parser); break;

                    case 0x45: // i32.eqz
                        current = new I32eqz(parser); break;
                    case 0x46: // i32.eq
                        current = new I32eq(parser); break;
                    case 0x47: // i32.ne
                        current = new I32ne(parser); break;
                    case 0x48: // i32.lt_s
                        current = new I32lts(parser); break;
                    case 0x49: // i32.lt_u
                        current = new I32ltu(parser); break;
                    case 0x4A: // i32.gt_s
                        current = new I32gts(parser); break;
                    case 0x4B: // i32.gt_u
                        current = new I32gtu(parser); break;
                    case 0x4C: // i32.le_s
                        current = new I32les(parser); break;
                    case 0x4D: // i32.le_u
                        current = new I32leu(parser); break;
                    case 0x4E: // i32.ge_s
                        current = new I32ges(parser); break;
                    case 0x4F: // i32.ge_u
                        current = new I32geu(parser); break;

                    case 0x50: // i64.eqz
                        current = new I64eqz(parser); break;
                    case 0x51: // i64.eq
                        current = new I64eq(parser); break;
                    case 0x52: // i64.ne
                        current = new I64ne(parser); break;
                    case 0x53: // i64.lt_s
                        current = new I64lts(parser); break;
                    case 0x54: // i64.lt_u
                        current = new I64ltu(parser); break;
                    case 0x55: // i64.gt_s
                        current = new I64gts(parser); break;
                    case 0x56: // i64.gt_u
                        current = new I64gtu(parser); break;
                    case 0x57: // i64.le_s
                        current = new I64les(parser); break;
                    case 0x58: // i64.le_u
                        current = new I64leu(parser); break;
                    case 0x59: // i64.ge_s
                        current = new I64ges(parser); break;
                    case 0x5A: // i64.ge_u
                        current = new I64geu(parser); break;

                    case 0x5B: // f32.eq
                        current = new F32eq(parser); break;
                    case 0x5C: // f32.ne
                        current = new F32ne(parser); break;
                    case 0x5D: // f32.lt
                        current = new F32lt(parser); break;
                    case 0x5E: // f32.gt
                        current = new F32gt(parser); break;
                    case 0x5F: // f32.le
                        current = new F32le(parser); break;
                    case 0x60: // f32.ge
                        current = new F32ge(parser); break;

                    case 0x61: // f64.eq
                        current = new F64eq(parser); break;
                    case 0x62: // f64.ne
                        current = new F64ne(parser); break;
                    case 0x63: // f64.lt
                        current = new F64lt(parser); break;
                    case 0x64: // f64.gt
                        current = new F64gt(parser); break;
                    case 0x65: // f64.le
                        current = new F64le(parser); break;
                    case 0x66: // f64.ge
                        current = new F64ge(parser); break;

                    case 0x67: // i32.clz
                        current = new I32clz(parser); break;
                    case 0x68: // i32.ctz
                        current = new I32ctz(parser); break;
                    case 0x69: // i32.popcnt
                        current = new I32popcnt(parser); break;
                    case 0x6A: // i32.add
                        current = new I32add(parser); break;
                    case 0x6B: // i32.sub
                        current = new I32sub(parser); break;
                    case 0x6C: // i32.mul
                        current = new I32mul(parser); break;
                    case 0x6D: // i32.div_s
                        current = new I32divs(parser); break;
                    case 0x6E: // i32.div_u
                        current = new I32divu(parser); break;
                    case 0x6F: // i32.rem_s
                        current = new I32rems(parser); break;
                    case 0x70: // i32.rem_u
                        current = new I32remu(parser); break;
                    case 0x71: // i32.and
                        current = new I32and(parser); break;
                    case 0x72: // i32.or
                        current = new I32or(parser); break;
                    case 0x73: // i32.xor
                        current = new I32xor(parser); break;
                    case 0x74: // i32.shl
                        current = new I32shl(parser); break;
                    case 0x75: // i32.shr_s
                        current = new I32shrs(parser); break;
                    case 0x76: // i32.shr_u
                        current = new I32shru(parser); break;
                    case 0x77: // i32.rotl
                        current = new I32rotl(parser); break;
                    case 0x78: // i32.rotr
                        current = new I32rotr(parser); break;

                    case 0x79: // i64.clz
                        current = new I64clz(parser); break;
                    case 0x7A: // i64.ctz
                        current = new I64ctz(parser); break;
                    case 0x7B: // i64.popcnt
                        current = new I64popcnt(parser); break;
                    case 0x7C: // i64.add
                        current = new I64add(parser); break;
                    case 0x7D: // i64.sub
                        current = new I64sub(parser); break;
                    case 0x7E: // i64.mul
                        current = new I64mul(parser); break;
                    case 0x7F: // i64.div_s
                        current = new I64divs(parser); break;
                    case 0x80: // i64.div_u
                        current = new I64divu(parser); break;
                    case 0x81: // i64.rem_s
                        current = new I64rems(parser); break;
                    case 0x82: // i64.rem_u
                        current = new I64remu(parser); break;
                    case 0x83: // i64.and
                        current = new I64and(parser); break;
                    case 0x84: // i64.or
                        current = new I64or(parser); break;
                    case 0x85: // i64.xor
                        current = new I64xor(parser); break;
                    case 0x86: // i64.shl
                        current = new I64shl(parser); break;
                    case 0x87: // i64.shr_s
                        current = new I64shrs(parser); break;
                    case 0x88: // i64.shr_u
                        current = new I64shru(parser); break;
                    case 0x89: // i64.rotl
                        current = new I64rotl(parser); break;
                    case 0x8A: // i64.rotr
                        current = new I64rotr(parser); break;

                    case 0x8B: // f32.abs
                        current = new F32abs(parser); break;
                    case 0x8C: // f32.neg
                        current = new F32neg(parser); break;
                    case 0x8D: // f32.ceil
                        current = new F32ceil(parser); break;
                    case 0x8E: // f32.floor
                        current = new F32floor(parser); break;
                    case 0x8F: // f32.trunc
                        current = new F32trunc(parser); break;
                    case 0x90: // f32.nearest
                        current = new F32nearest(parser); break;
                    case 0x91: // f32.sqrt
                        current = new F32sqrt(parser); break;
                    case 0x92: // f32.add
                        current = new F32add(parser); break;
                    case 0x93: // f32.sub
                        current = new F32sub(parser); break;
                    case 0x94: // f32.mul
                        current = new F32mul(parser); break;
                    case 0x95: // f32.div
                        current = new F32div(parser); break;
                    case 0x96: // f32.min
                        current = new F32min(parser); break;
                    case 0x97: // f32.max
                        current = new F32max(parser); break;
                    case 0x98: // f32.copysign
                        current = new F32copysign(parser); break;

                    case 0x99: // f64.abs
                        current = new F64abs(parser); break;
                    case 0x9A: // f64.neg
                        current = new F64neg(parser); break;
                    case 0x9B: // f64.ceil
                        current = new F64ceil(parser); break;
                    case 0x9C: // f64.floor
                        current = new F64floor(parser); break;
                    case 0x9D: // f64.trunc
                        current = new F64trunc(parser); break;
                    case 0x9E: // f64.nearest
                        current = new F64nearest(parser); break;
                    case 0x9F: // f64.sqrt
                        current = new F64sqrt(parser); break;
                    case 0xA0: // f64.add
                        current = new F64add(parser); break;
                    case 0xA1: // f64.sub
                        current = new F64sub(parser); break;
                    case 0xA2: // f64.mul
                        current = new F64mul(parser); break;
                    case 0xA3: // f64.div
                        current = new F64div(parser); break;
                    case 0xA4: // f64.min
                        current = new F64min(parser); break;
                    case 0xA5: // f64.max
                        current = new F64max(parser); break;
                    case 0xA6: // f64.copysign
                        current = new F64copysign(parser); break;

                    case 0xA7: // i32.wrap_i64
                        current = new I32wrapI64(parser); break;
                    case 0xA8: // i32.trunc_f32_s
                        current = new I32truncF32s(parser); break;
                    case 0xA9: // i32.trunc_f32_u
                        current = new I32truncF32u(parser); break;
                    case 0xAA: // i32.trunc_f64_s
                        current = new I32truncF64s(parser); break;
                    case 0xAB: // i32.trunc_f64_u
                        current = new I32truncF64u(parser); break;
                    case 0xAC: // i64.extend_i32_s
                        current = new I64extendI32s(parser); break;
                    case 0xAD: // i64.extend_i32_u
                        current = new I64extendI32u(parser); break;
                    case 0xAE: // i64.trunc_f32_s
                        current = new I64truncF32s(parser); break;
                    case 0xAF: // i64.trunc_f32_u
                        current = new I64truncF32u(parser); break;
                    case 0xB0: // i64.trunc_f64_s
                        current = new I64truncF64s(parser); break;
                    case 0xB1: // i64.trunc_f64_u
                        current = new I64truncF64u(parser); break;
                    case 0xB2: // f32.convert_i32_s
                        current = new F32convertI32s(parser); break;
                    case 0xB3: // f32.convert_i32_u
                        current = new F32convertI32u(parser); break;
                    case 0xB4: // f32.convert_i64_s
                        current = new F32convertI64s(parser); break;
                    case 0xB5: // f32.convert_i64_u
                        current = new F32convertI64u(parser); break;
                    case 0xB6: // f32.demote_f64
                        current = new F32demoteF64(parser); break;
                    case 0xB7: // f64.convert_i32_s
                        current = new F64convertI32s(parser); break;
                    case 0xB8: // f64.convert_i32_u
                        current = new F64convertI32u(parser); break;
                    case 0xB9: // f64.convert_i64_s
                        current = new F64convertI64s(parser); break;
                    case 0xBA: // f64.convert_i64.u
                        current = new F64convertI64u(parser); break;
                    case 0xBB: // f64.promote_f32
                        current = new F64promoteF32(parser); break;
                    case 0xBC: // i32.reinterpret_f32
                        current = new I32reinterpretF32(parser); break;
                    case 0xBD: // i64.reinterpret_i32
                        current = new I64reinterpretI32(parser); break;
                    case 0xBE: // f32.reinterpret_i32
                        current = new F32reinterpretI32(parser); break;
                    case 0xBF: // f64.reinterpret_i64
                        current = new F64reinterpretI64(parser); break;
                    default:
                        throw new Exception("Opcode not implemented: 0x" + code.ToString("X"));
                }

                current.opCode = code;
                current.Pos = pos;
                pos++;
                if (start == null) start = current;
                if (current != null)
                {
                    current.Pointer = pointer;
                    if(debug)
                    {
                        Console.WriteLine(current);
                        Console.ReadKey();
                    }

                    if (last != null && !done && last.Next == null)
                    {
                        last.Next = current;
                    }

                    last = current;
                }
            }
            
            /*** INITIALIZE NEW PROGRAM ***/
            List<Inst> program = new List<Inst>();
            Inst i = new Inst();
            for (var inst = start; inst != null; inst = inst.Next)
            {
                i.opCode = inst.opCode;
                i.pointer = inst.Pointer;
                i.i = inst;
                
                switch (inst.opCode)
                {
                    case 0x00: // unreachable
                    case 0x01: // nop
                        program.Add(i);
                        break;
                    case 0x02: // block
                        i.i32 = (UInt32)(inst as Block).label.Pos;
                        program.Add(i);
                        break;
                    case 0x03: // loop
                        program.Add(i);
                        break;
                    case 0x04: // if
                        i.i32 = (UInt32) (inst as If).label.Pos;
                        program.Add(i);
                        break;
                    case 0x05: // else
                        i.i32 = (UInt32) (inst as Else).label.Pos;
                        program.Add(i);
                        break;
                    case 0x0B: // end
                        program.Add(i);
                        break;
                    case 0x0C: // br
                        i.i32 = (inst as Br).labelidx;
                        program.Add(i);
                        break;
                    case 0x0D: // br_if
                        i.i32 = (inst as BrIf).labelidx;
                        program.Add(i);
                        break;
                    case 0x0E: // br_table
                        i.i32 = (inst as BrTable).defaultLabelidx;
                        i.table = (inst as BrTable).table;
                        program.Add(i);
                        break;
                    case 0x0F: // return
                        program.Add(i);
                        break;
                    case 0x10: // call
                        i.i32 = (UInt32)(inst as Call).funcidx;
                        program.Add(i);
                        break;
                    case 0x11: // call_indirect
                        i.i32 = (UInt32)(inst as CallIndirect).tableidx;
                        program.Add(i);
                        break;

                    /* Parametric Instructions */

                    case 0x1A: // drop
                        program.Add(i);
                        break;
                    case 0x1B: // select
                        program.Add(i);
                        break;

                    /* Variable Instructions */

                    case 0x20: // local.get
                        i.i32 = (UInt32)(inst as LocalGet).index;
                        program.Add(i);
                        break;
                    case 0x21: // local.set
                        i.i32 = (UInt32)(inst as LocalSet).index;
                        program.Add(i);
                        break;
                    case 0x22: // local.tee
                        i.i32 = (UInt32)(inst as LocalTee).index;
                        program.Add(i);
                        break;
                    case 0x23: // global.get
                        i.i32 = (UInt32)(inst as GlobalGet).index;
                        program.Add(i);
                        break;
                    case 0x24: // global.set
                        i.i32 = (UInt32)(inst as GlobalSet).index;
                        program.Add(i);
                        break;

                    /* Memory Instructions */

                    case 0x28: // i32.load
                        i.i32 = (inst as I32load).offset;
                        program.Add(i);
                        break;
                    case 0x29: // i64.load
                        i.i32 = (inst as I64load).offset;
                        program.Add(i);
                        break;
                    case 0x2A: // f32.load
                        i.i32 = (inst as F32load).offset;
                        program.Add(i);
                        break;
                    case 0x2B: // f64.load
                        i.i32 = (inst as F64load).offset;
                        program.Add(i);
                        break;
                    case 0x2C: // i32.load8_s
                        i.i32 = (inst as I32load8s).offset;
                        program.Add(i);
                        break;
                    case 0x2D: // i32.load8_u
                        i.i32 = (inst as I32load8u).offset;
                        program.Add(i);
                        break;
                    case 0x2E: // i32.load16_s
                        i.i32 = (inst as I32load16s).offset;
                        program.Add(i);
                        break;
                    case 0x2F: // i32.load16_u
                        i.i32 = (inst as I32load16u).offset;
                        program.Add(i);
                        break;
                    case 0x30: // i64.load8_s
                        i.i32 = (inst as I64load8s).offset;
                        program.Add(i);
                        break;
                    case 0x31: // i64.load8_u
                        i.i32 = (inst as I64load8u).offset;
                        program.Add(i);
                        break;
                    case 0x32: // i64.load16_s
                        i.i32 = (inst as I64load16s).offset;
                        program.Add(i);
                        break;
                    case 0x33: // i64.load16_u
                        i.i32 = (inst as I64load16u).offset;
                        program.Add(i);
                        break;
                    case 0x34: // i64.load32_s
                        i.i32 = (inst as I64load32s).offset;
                        program.Add(i);
                        break;
                    case 0x35: // i64.load32_u
                        i.i32 = (inst as I64load32u).offset;
                        program.Add(i);
                        break;
                    case 0x36: // i32.store
                        i.i32 = (inst as I32store).offset;
                        program.Add(i);
                        break;
                    case 0x37: // i64.store
                        i.i32 = (inst as I64store).offset;
                        program.Add(i);
                        break;
                    case 0x38: // f32.store
                        i.i32 = (inst as F32store).offset;
                        program.Add(i);
                        break;
                    case 0x39: // f64.store
                        i.i32 = (inst as F64store).offset;
                        program.Add(i);
                        break;
                    case 0x3A: // i32.store8
                        i.i32 = (inst as I32store8).offset;
                        program.Add(i);
                        break;
                    case 0x3B: // i32.store16
                        i.i32 = (inst as I32store16).offset;
                        program.Add(i);
                        break;
                    case 0x3C: // i64.store8
                        i.i32 = (inst as I64store8).offset;
                        program.Add(i);
                        break;
                    case 0x3D: // i64.store16
                        i.i32 = (inst as I64store16).offset;
                        program.Add(i);
                        break;
                    case 0x3E: // i64.store32
                        i.i32 = (inst as I64store32).offset;
                        program.Add(i);
                        break;
                    case 0x3F: // memory.size
                        program.Add(i);
                        break;
                    case 0x40: // memory.grow
                        program.Add(i);
                        break;

                    /* Numeric Instructions */

                    case 0x41: // i32.const
                        i.i32 = (inst as I32const).value;
                        program.Add(i);
                        break;
                    case 0x42: // i64.const
                        i.i64 = (inst as I64const).value;
                        program.Add(i);
                        break;
                    case 0x43: // f32.const
                        i.f32 = (inst as F32const).value;
                        program.Add(i);
                        break;
                    case 0x44: // f64.const
                        i.f64 = (inst as F64const).value;
                        program.Add(i);
                        break;

                    case 0x45: // i32.eqz
                        program.Add(i);
                        break;
                    case 0x46: // i32.eq
                        program.Add(i);
                        break;
                    case 0x47: // i32.ne
                        program.Add(i);
                        break;
                    case 0x48: // i32.lt_s
                        program.Add(i);
                        break;
                    case 0x49: // i32.lt_u
                        program.Add(i);
                        break;
                    case 0x4A: // i32.gt_s
                        program.Add(i);
                        break;
                    case 0x4B: // i32.gt_u
                        program.Add(i);
                        break;
                    case 0x4C: // i32.le_s
                        program.Add(i);
                        break;
                    case 0x4D: // i32.le_u
                        program.Add(i);
                        break;
                    case 0x4E: // i32.ge_s
                        program.Add(i);
                        break;
                    case 0x4F: // i32.ge_u
                        program.Add(i);
                        break;

                    case 0x50: // i64.eqz
                        program.Add(i);
                        break;
                    case 0x51: // i64.eq
                        program.Add(i);
                        break;
                    case 0x52: // i64.ne
                        program.Add(i);
                        break;
                    case 0x53: // i64.lt_s
                        program.Add(i);
                        break;
                    case 0x54: // i64.lt_u
                        program.Add(i);
                        break;
                    case 0x55: // i64.gt_s
                        program.Add(i);
                        break;
                    case 0x56: // i64.gt_u
                        program.Add(i);
                        break;
                    case 0x57: // i64.le_s
                        program.Add(i);
                        break;
                    case 0x58: // i64.le_u
                        program.Add(i);
                        break;
                    case 0x59: // i64.ge_s
                        program.Add(i);
                        break;
                    case 0x5A: // i64.ge_u
                        program.Add(i);
                        break;

                    case 0x5B: // f32.eq
                        program.Add(i);
                        break;
                    case 0x5C: // f32.ne
                        program.Add(i);
                        break;
                    case 0x5D: // f32.lt
                        program.Add(i);
                        break;
                    case 0x5E: // f32.gt
                        program.Add(i);
                        break;
                    case 0x5F: // f32.le
                        program.Add(i);
                        break;
                    case 0x60: // f32.ge
                        program.Add(i);
                        break;

                    case 0x61: // f64.eq
                        program.Add(i);
                        break;
                    case 0x62: // f64.ne
                        program.Add(i);
                        break;
                    case 0x63: // f64.lt
                        program.Add(i);
                        break;
                    case 0x64: // f64.gt
                        program.Add(i);
                        break;
                    case 0x65: // f64.le
                        program.Add(i);
                        break;
                    case 0x66: // f64.ge
                        program.Add(i);
                        break;

                    case 0x67: // i32.clz
                        program.Add(i);
                        break;
                    case 0x68: // i32.ctz
                        program.Add(i);
                        break;
                    case 0x69: // i32.popcnt
                        program.Add(i);
                        break;
                    case 0x6A: // i32.add
                        program.Add(i);
                        break;
                    case 0x6B: // i32.sub
                        program.Add(i);
                        break;
                    case 0x6C: // i32.mul
                        program.Add(i);
                        break;
                    case 0x6D: // i32.div_s
                        program.Add(i);
                        break;
                    case 0x6E: // i32.div_u
                        program.Add(i);
                        break;
                    case 0x6F: // i32.rem_s
                        program.Add(i);
                        break;
                    case 0x70: // i32.rem_u
                        program.Add(i);
                        break;
                    case 0x71: // i32.and
                        program.Add(i);
                        break;
                    case 0x72: // i32.or
                        program.Add(i);
                        break;
                    case 0x73: // i32.xor
                        program.Add(i);
                        break;
                    case 0x74: // i32.shl
                        program.Add(i);
                        break;
                    case 0x75: // i32.shr_s
                        program.Add(i);
                        break;
                    case 0x76: // i32.shr_u
                        program.Add(i);
                        break;
                    case 0x77: // i32.rotl
                        program.Add(i);
                        break;
                    case 0x78: // i32.rotr
                        program.Add(i);
                        break;

                    case 0x79: // i64.clz
                        program.Add(i);
                        break;
                    case 0x7A: // i64.ctz
                        program.Add(i);
                        break;
                    case 0x7B: // i64.popcnt
                        program.Add(i);
                        break;
                    case 0x7C: // i64.add
                        program.Add(i);
                        break;
                    case 0x7D: // i64.sub
                        program.Add(i);
                        break;
                    case 0x7E: // i64.mul
                        program.Add(i);
                        break;
                    case 0x7F: // i64.div_s
                        program.Add(i);
                        break;
                    case 0x80: // i64.div_u
                        program.Add(i);
                        break;
                    case 0x81: // i64.rem_s
                        program.Add(i);
                        break;
                    case 0x82: // i64.rem_u
                        program.Add(i);
                        break;
                    case 0x83: // i64.and
                        program.Add(i);
                        break;
                    case 0x84: // i64.or
                        program.Add(i);
                        break;
                    case 0x85: // i64.xor
                        program.Add(i);
                        break;
                    case 0x86: // i64.shl
                        program.Add(i);
                        break;
                    case 0x87: // i64.shr_s
                        program.Add(i);
                        break;
                    case 0x88: // i64.shr_u
                        program.Add(i);
                        break;
                    case 0x89: // i64.rotl
                        program.Add(i);
                        break;
                    case 0x8A: // i64.rotr
                        program.Add(i);
                        break;

                    case 0x8B: // f32.abs
                        program.Add(i);
                        break;
                    case 0x8C: // f32.neg
                        program.Add(i);
                        break;
                    case 0x8D: // f32.ceil
                        program.Add(i);
                        break;
                    case 0x8E: // f32.floor
                        program.Add(i);
                        break;
                    case 0x8F: // f32.trunc
                        program.Add(i);
                        break;
                    case 0x90: // f32.nearest
                        program.Add(i);
                        break;
                    case 0x91: // f32.sqrt
                        program.Add(i);
                        break;
                    case 0x92: // f32.add
                        program.Add(i);
                        break;
                    case 0x93: // f32.sub
                        program.Add(i);
                        break;
                    case 0x94: // f32.mul
                        program.Add(i);
                        break;
                    case 0x95: // f32.div
                        program.Add(i);
                        break;
                    case 0x96: // f32.min
                        program.Add(i);
                        break;
                    case 0x97: // f32.max
                        program.Add(i);
                        break;
                    case 0x98: // f32.copysign
                        program.Add(i);
                        break;

                    case 0x99: // f64.abs
                        program.Add(i);
                        break;
                    case 0x9A: // f64.neg
                        program.Add(i);
                        break;
                    case 0x9B: // f64.ceil
                        program.Add(i);
                        break;
                    case 0x9C: // f64.floor
                        program.Add(i);
                        break;
                    case 0x9D: // f64.trunc
                        program.Add(i);
                        break;
                    case 0x9E: // f64.nearest
                        program.Add(i);
                        break;
                    case 0x9F: // f64.sqrt
                        program.Add(i);
                        break;
                    case 0xA0: // f64.add
                        program.Add(i);
                        break;
                    case 0xA1: // f64.sub
                        program.Add(i);
                        break;
                    case 0xA2: // f64.mul
                        program.Add(i);
                        break;
                    case 0xA3: // f64.div
                        program.Add(i);
                        break;
                    case 0xA4: // f64.min
                        program.Add(i);
                        break;
                    case 0xA5: // f64.max
                        program.Add(i);
                        break;
                    case 0xA6: // f64.copysign
                        program.Add(i);
                        break;

                    case 0xA7: // i32.wrap_i64
                        program.Add(i);
                        break;
                    case 0xA8: // i32.trunc_f32_s
                        program.Add(i);
                        break;
                    case 0xA9: // i32.trunc_f32_u
                        program.Add(i);
                        break;
                    case 0xAA: // i32.trunc_f64_s
                        program.Add(i);
                        break;
                    case 0xAB: // i32.trunc_f64_u
                        program.Add(i);
                        break;
                    case 0xAC: // i64.extend_i32_s
                        program.Add(i);
                        break;
                    case 0xAD: // i64.extend_i32_u
                        program.Add(i);
                        break;
                    case 0xAE: // i64.trunc_f32_s
                        program.Add(i);
                        break;
                    case 0xAF: // i64.trunc_f32_u
                        program.Add(i);
                        break;
                    case 0xB0: // i64.trunc_f64_s
                        program.Add(i);
                        break;
                    case 0xB1: // i64.trunc_f64_u
                        program.Add(i);
                        break;
                    case 0xB2: // f32.convert_i32_s
                        program.Add(i);
                        break;
                    case 0xB3: // f32.convert_i32_u
                        program.Add(i);
                        break;
                    case 0xB4: // f32.convert_i64_s
                        program.Add(i);
                        break;
                    case 0xB5: // f32.convert_i64_u
                        program.Add(i);
                        break;
                    case 0xB6: // f32.demote_f64
                        program.Add(i);
                        break;
                    case 0xB7: // f64.convert_i32_s
                        program.Add(i);
                        break;
                    case 0xB8: // f64.convert_i32_u
                        program.Add(i);
                        break;
                    case 0xB9: // f64.convert_i64_s
                        program.Add(i);
                        break;
                    case 0xBA: // f64.convert_i64.u
                        program.Add(i);
                        break;
                    case 0xBB: // f64.promote_f32
                        program.Add(i);
                        break;
                    case 0xBC: // i32.reinterpret_f32
                        program.Add(i);
                        break;
                    case 0xBD: // i64.reinterpret_i32
                        program.Add(i);
                        break;
                    case 0xBE: // f32.reinterpret_i32
                        program.Add(i);
                        break;
                    case 0xBF: // f64.reinterpret_i64
                        program.Add(i);
                        break;
                }
            }

            i.opCode = 0x0F;
            i.pointer = (UInt32)program.Count;
            i.i = new Return(null);
            
            program.Add(i);

            /******************************/


            return program.ToArray();
        }

        public static string Translate(byte opCode)
        {
            switch (opCode)
            {
                case 0x00: return "unreachable";
                case 0x01: return "nop";
                case 0x02: return "block";
                case 0x03: return "loop";
                case 0x04: return "if";
                case 0x05: return "else";
                case 0x0B: return "end";
                case 0x0C: return "br";
                case 0x0D: return "br_if";
                case 0x0E: return "br_table";
                case 0x0F: return "return";
                case 0x10: return "call";
                case 0x11: return "call_indirect";
                case 0x1A: return "drop";
                case 0x1B: return "select";
                case 0x20: return "local.get";
                case 0x21: return "local.set";
                case 0x22: return "local.tee";
                case 0x23: return "global.get";
                case 0x24: return "global.set";
                case 0x28: return "i32.load";
                case 0x29: return "i64.load";
                case 0x2A: return "f32.load";
                case 0x2B: return "f64.load";
                case 0x2C: return "i32.load8_s";
                case 0x2D: return "i32.load8_u";
                case 0x2E: return "i32.load16_s";
                case 0x2F: return "i32.load16_u";
                case 0x30: return "i64.load8_s";
                case 0x31: return "i64.load8_u";
                case 0x32: return "i64.load16_s";
                case 0x33: return "i64.load16_u";
                case 0x34: return "i64.load32_s";
                case 0x35: return "i64.load32_u";
                case 0x36: return "i32.store";
                case 0x37: return "i64.store";
                case 0x38: return "f32.store";
                case 0x39: return "f64.store";
                case 0x3A: return "i32.store8";
                case 0x3B: return "i32.store16";
                case 0x3C: return "i64.store8";
                case 0x3D: return "i64.store16";
                case 0x3E: return "i64.store32";
                case 0x3F: return "memory.size";
                case 0x40: return "memory.grow";
                case 0x41: return "i32.const";
                case 0x42: return "i64.const";
                case 0x43: return "f32.const";
                case 0x44: return "f64.const";
                case 0x45: return "i32.eqz";
                case 0x46: return "i32.eq";
                case 0x47: return "i32.ne";
                case 0x48: return "i32.lt_s";
                case 0x49: return "i32.lt_u";
                case 0x4A: return "i32.gt_s";
                case 0x4B: return "i32.gt_u";
                case 0x4C: return "i32.le_s";
                case 0x4D: return "i32.le_u";
                case 0x4E: return "i32.ge_s";
                case 0x4F: return "i32.ge_u";
                case 0x50: return "i64.eqz";
                case 0x51: return "i64.eq";
                case 0x52: return "i64.ne";
                case 0x53: return "i64.lt_s";
                case 0x54: return "i64.lt_u";
                case 0x55: return "i64.gt_s";
                case 0x56: return "i64.gt_u";
                case 0x57: return "i64.le_s";
                case 0x58: return "i64.le_u";
                case 0x59: return "i64.ge_s";
                case 0x5A: return "i64.ge_u";
                case 0x5B: return "f32.eq";
                case 0x5C: return "f32.ne";
                case 0x5D: return "f32.lt";
                case 0x5E: return "f32.gt";
                case 0x5F: return "f32.le";
                case 0x60: return "f32.ge";
                case 0x61: return "f64.eq";
                case 0x62: return "f64.ne";
                case 0x63: return "f64.lt";
                case 0x64: return "f64.gt";
                case 0x65: return "f64.le";
                case 0x66: return "f64.ge";
                case 0x67: return "i32.clz";
                case 0x68: return "i32.ctz";
                case 0x69: return "i32.popcnt";
                case 0x6A: return "i32.add";
                case 0x6B: return "i32.sub";
                case 0x6C: return "i32.mul";
                case 0x6D: return "i32.div_s";
                case 0x6E: return "i32.div_u";
                case 0x6F: return "i32.rem_s";
                case 0x70: return "i32.rem_u";
                case 0x71: return "i32.and";
                case 0x72: return "i32.or";
                case 0x73: return "i32.xor";
                case 0x74: return "i32.shl";
                case 0x75: return "i32.shr_s";
                case 0x76: return "i32.shr_u";
                case 0x77: return "i32.rotl";
                case 0x78: return "i32.rotr";
                case 0x79: return "i64.clz";
                case 0x7A: return "i64.ctz";
                case 0x7B: return "i64.popcnt";
                case 0x7C: return "i64.add";
                case 0x7D: return "i64.sub";
                case 0x7E: return "i64.mul";
                case 0x7F: return "i64.div_s";
                case 0x80: return "i64.div_u";
                case 0x81: return "i64.rem_s";
                case 0x82: return "i64.rem_u";
                case 0x83: return "i64.and";
                case 0x84: return "i64.or";
                case 0x85: return "i64.xor";
                case 0x86: return "i64.shl";
                case 0x87: return "i64.shr_s";
                case 0x88: return "i64.shr_u";
                case 0x89: return "i64.rotl";
                case 0x8A: return "i64.rotr";
                case 0x8B: return "f32.abs";
                case 0x8C: return "f32.neg";
                case 0x8D: return "f32.ceil";
                case 0x8E: return "f32.floor";
                case 0x8F: return "f32.trunc";
                case 0x90: return "f32.nearest";
                case 0x91: return "f32.sqrt";
                case 0x92: return "f32.add";
                case 0x93: return "f32.sub";
                case 0x94: return "f32.mul";
                case 0x95: return "f32.div";
                case 0x96: return "f32.min";
                case 0x97: return "f32.max";
                case 0x98: return "f32.copysign";
                case 0x99: return "f64.abs";
                case 0x9A: return "f64.neg";
                case 0x9B: return "f64.ceil";
                case 0x9C: return "f64.floor";
                case 0x9D: return "f64.trunc";
                case 0x9E: return "f64.nearest";
                case 0x9F: return "f64.sqrt";
                case 0xA0: return "f64.add";
                case 0xA1: return "f64.sub";
                case 0xA2: return "f64.mul";
                case 0xA3: return "f64.div";
                case 0xA4: return "f64.min";
                case 0xA5: return "f64.max";
                case 0xA6: return "f64.copysign";
                case 0xA7: return "i32.wrap_i64";
                case 0xA8: return "i32.trunc_f32_s";
                case 0xA9: return "i32.trunc_f32_u";
                case 0xAA: return "i32.trunc_f64_s";
                case 0xAB: return "i32.trunc_f64_u";
                case 0xAC: return "i64.extend_i32_s";
                case 0xAD: return "i64.extend_i32_u";
                case 0xAE: return "i64.trunc_f32_s";
                case 0xAF: return "i64.trunc_f32_u";
                case 0xB0: return "i64.trunc_f64_s";
                case 0xB1: return "i64.trunc_f64_u";
                case 0xB2: return "f32.convert_i32_s";
                case 0xB3: return "f32.convert_i32_u";
                case 0xB4: return "f32.convert_i64_s";
                case 0xB5: return "f32.convert_i64_u";
                case 0xB6: return "f32.demote_f64";
                case 0xB7: return "f64.convert_i32_s";
                case 0xB8: return "f64.convert_i32_u";
                case 0xB9: return "f64.convert_i64_s";
                case 0xBA: return "f64.convert_i64.u";
                case 0xBB: return "f64.promote_f32";
                case 0xBC: return "i32.reinterpret_f32";
                case 0xBD: return "i64.reinterpret_i32";
                case 0xBE: return "f32.reinterpret_i32";
                case 0xBF: return "f64.reinterpret_i64";
                default: return "unknown opcode";
            }            
        }

        // Finds common strings of instructions for the purpose of optimization
        static public void Analyze(Function[] program, int length)
        {
            Dictionary<string, int> stats = new Dictionary<string, int>();

            byte[] chunk = new byte[length];
            
            double total = 0;
            for (int f = 0; f < program.Length; f++)
            {
                if (program[f].program == null) continue;
                total += program[f].program.Length;
            }

            double complete = 0;
            for (int f = 0; f < program.Length; f++)
            {
                if (program[f].program == null) continue;
                for (int i = 0; i < program[f].program.Length - length; i++)
                {
                    complete++;
                    for (int o = 0; o < length; o++)
                    {
                        chunk[o] = program[f].program[i].opCode;

                        var s = Encoding.ASCII.GetString(chunk);
                        if (!stats.ContainsKey(s))
                        {
                            stats.Add(s, 0);
                        }

                        stats[s]++;
                    }
                }
                Console.WriteLine("Percent Complete: %" + (100*complete/total));
            }
            
            foreach (var keyValuePair in stats.OrderBy(x => x.Value))
            {
                byte[] result = Encoding.ASCII.GetBytes(keyValuePair.Key);
                for (int i = 0; i < length; i++)
                {
                    if(i == 0)
                        Console.Write(Translate(result[i]));
                    else
                        Console.Write(", " + Translate(result[i]));
                }
                Console.WriteLine(": " + keyValuePair.Value);
            }
        }
    }
}
