using System;
using System.Collections.Generic;
using System.Text;

using jLib;

namespace MultiBase
{
    public static class Ternary
    {
        /// <summary>
        /// Trit (base 3, ternary) data type.
        /// </summary>
        /// <remarks>
        /// Uses balanced ternary, -1=False, 0=Undef, 1=True
        /// </remarks>
        public class Trit : Data
        {
            /// <summary>
            /// Trit value: -1=False, 0=Undef, 1=True
            /// </summary>
            public int Value { get; set; }

            public const int False = -1;
            public const int Undef = 0;
            public const int True = 1;

            public Trit(int v = -1)
            {
                this.Value = v;
            }
        }

        /// <summary>
        /// Class of all ternary gates in polish notation.
        /// </summary>
        /// <remarks>
        /// We're using Kleene-Priest ternary logic here folks.
        /// </remarks>
        /// <see cref="https://en.wikipedia.org/wiki/Three-valued_logic#Kleene_and_Priest_logics"/>
        public class TernaryGates : IGates
        {
            public TernaryGates()
            {
            }

            public Data NOT(Data x)
            {
                return x.Value switch
                {
                    Trit.False => new Trit(Trit.True),
                    Trit.Undef => new Trit(Trit.Undef),
                    _ => new Trit(Trit.False), //Trit.True
                };
            }

            public Data OR(Data x, Data y)
            {
                switch ($"{x.Value}{y.Value}")
                {
                    case "-1-1":
                        return new Trit(Trit.False);
                    case "0-1":
                    case "-10":
                    case "00":
                        return new Trit(Trit.Undef);
                    default:
                        return new Trit(Trit.True);
                }
            }
            public Data AND(Data x, Data y)
            {
                switch ($"{x.Value}{y.Value}")
                {
                    case "11":
                        return new Trit(Trit.True);
                    case "00":
                    case "01":
                    case "10":
                        return new Trit(Trit.Undef);
                    default:
                        return new Trit(Trit.False);
                }
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

        public class Register : IRegister
        {
            public Data[] Digits { get; set; }
            public Type DataType { get; set; } = typeof(Trit);

            public void INC()
            {

            }
        }
    }
}
