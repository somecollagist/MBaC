 using System;
using System.Collections.Generic;
using System.Text;

using jLib;

namespace MultiBase
{
    public static class Binary
    {
        /// <summary>
        /// Bit (base 2, binary) data type.
        /// </summary>
        public class Bit : Data
        {
            /// <summary>
            /// Bit value; 0 = False, 1 = True
            /// </summary>
            public int Value { get; set; }

            public const int False = 0;
            public const int True = 1;

            public Bit(int v = 0)
            {
                this.Value = v;
            }
        }

        /// <summary>
        /// Class of all binary gates in polish notation.
        /// </summary>
        public class BinaryGates : IGates
        {
            public BinaryGates()
            {
            }

            public int ConvertToInt(Data[] data)
            {
                int ret = 0;
                int len = data.Length - 1;

                foreach (Bit bit in data)
                {
                    ret += (bit.Value) * (int)(Math.Pow(2, len));
                    len--;
                }
                return ret;
            }
            public Data[] ConvertToData(int num)
            {
                if (num == 0) return new Bit[] { new Bit(0) };

                string binary = XMath.Functions.DecimalToBase(num, 2);
                int len = binary.Length;
                Bit[] bits = new Bit[len];

                for (int x = 0; x < len; x++) bits[x] = new Bit(Convert.ToInt16(binary[x].ToString()));
                return bits;
            }

            public Data NOT(Data x)
            {
                return x.Value switch
                {
                    Bit.False => new Bit(Bit.True),
                    _ => new Bit(Bit.False), //Bit.True
                };
            }

            public Data OR(Data x, Data y)
            {
                if (x.Value == 1 || y.Value == 1) return new Bit(Bit.True);
                return new Bit(Bit.False);
            }
            public Data AND(Data x, Data y)
            {
                if (x.Value == 1 && y.Value == 1) return new Bit(Bit.True);
                return new Bit(Bit.False);
            }

            public Data NOR(Data x, Data y)
            {
                return NOT(OR(x, y));
            }
            public Data NAND(Data x, Data y)
            {
                return NOT(AND(x, y));
            }
        }
    }
}
