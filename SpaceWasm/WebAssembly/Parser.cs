﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAssembly;

namespace WebAssembly
{
    public class Parser
    {
        private byte[] bytes;
        private UInt32 index;
        public Module.Module Module;

        public Parser(byte[] bytes, Module.Module module)
        {
            this.bytes = bytes;
            this.index = 0;
            this.Module = module;
        }

        public UInt32 GetPointer()
        {
            return this.index;
        }

        public void SetPointer(UInt32 index)
        {
            this.index = index;
        }

        public bool Done()
        {
            return index >= bytes.Length;
        }

        public byte GetByte()
        {
            return this.bytes[this.index++];
        }

        public byte[] GetBytes(int offset, int length)
        {
            byte[] output = new byte[length];
            Buffer.BlockCopy(this.bytes, offset, output, 0, length);
            return output;
        }

        public byte PeekByte()
        {
            return this.bytes[this.index];
        }

        public Instruction.Instruction GetExpr(bool debug = false)
        {
            return Instruction.Instruction.Consume(this, debug);
        }

        public UInt32 GetIndex()
        {
            return this.GetUInt32();
        }

        public UInt32 GetUInt32()
        {
            UInt32 result = 0;
            byte shift = 0;
            while (true)
            {
                byte b = this.GetByte();
                result |= (UInt32)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                    break;
                shift += 7;
            }

            return result;
        }

        public Int32 GetInt32()
        {
            return this.GetSignedLEB128(32);
        }

        public Int32 GetInt64()
        {
            return this.GetSignedLEB128(64);
        }

        public UInt64 GetUInt64()
        {
            UInt64 result = 0;
            byte shift = 0;
            while (true)
            {
                byte b = this.GetByte();
                result |= (UInt64)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                    break;
                shift += 7;
            }

            return result;
        }

        public float GetF32()
        {
            float result = BitConverter.ToSingle(this.bytes, (int)this.index);
            this.index += 4;
            return result;
        }

        public double GetF64()
        {
            double result = BitConverter.ToDouble(this.bytes, (int)this.index);
            this.index += 8;
            return result;
        }

        public Table GetTableType()
        {
            byte elemType = this.GetElemType();
            UInt32 min, max;
            bool hasMax = this.GetLimits(out min, out max);

            return new Table(elemType, min, max);
        }

        public Memory GetMemType()
        {
            UInt32 min = 0, max = 0;
            bool hasMax = this.GetLimits(out min, out max);

            return new Memory(min, max);
        }

        public bool GetLimits(out UInt32 min, out UInt32 max)
        {
            if (this.GetBoolean())
            {
                min = this.GetUInt32();
                max = this.GetUInt32();
                return true;
            }
            else
            {
                min = this.GetUInt32();
                max = 0;
                return false;
            }
        }

        public byte GetElemType()
        {
            var type = this.GetByte();

            switch (type)
            {
                case 0x70: //funcref
                    break;
                default:
                    throw new Exception("Invalid element type: 0x" + type.ToString("X"));
            }

            return type;
        }

        public bool GetBoolean()
        {
            byte b = this.GetByte();

            switch (b)
            {
                case 0x00:
                    return false;
                case 0x01:
                    return true;
                default:
                    throw new Exception("Invalid boolean value: 0x" + b.ToString("X"));
            }
        }

        public void GetGlobalType(out byte type, out bool mutable)
        {
            type = this.GetValType();
            mutable = this.GetBoolean();
        }

        public byte GetBlockType()
        {
            if (this.bytes[this.index] == 0x40)
            {
                this.index++;
                return 0x40;
            }
            else
            {
                return this.GetValType();
            }
        }

        public byte GetValType()
        {
            byte valType = this.GetByte();

            switch (valType)
            {
                case 0x7F:
                case 0x7E:
                case 0x7D:
                case 0x7C:
                    break;
                default:
                    throw new Exception("Invalid value type: 0x" + valType.ToString("X"));
            }

            return valType;
        }

        public string GetName()
        {
            var length = this.GetUInt32();
            byte[] sub = new byte[length];
            Array.Copy(this.bytes, this.index, sub, 0, length);
            string result = System.Text.Encoding.UTF8.GetString(sub);
            this.index += length;

            return result;
        }

        public UInt32 GetVersion()
        {
            this.index += 4;
            return BitConverter.ToUInt32(this.bytes, 4);
        }

        public void Skip(UInt32 size)
        {
            this.index += size;
        }

        private Int32 GetSignedLEB128(byte size)
        {
            Int32 result = 0;
            byte shift = 0;
            byte b;
            do
            {
                b = this.bytes[this.index++];
                result |= ((0x7F & b) << shift);
                shift += 7;
            } while ((b & 0x80) != 0);

            /* sign bit of byte is second high order bit (0x40) */
            if ((shift < size) && ((b & 0x40) != 0))
                /* sign extend */
                result |= (~0 << shift);

            return result;
        }


    }
}