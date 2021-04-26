using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using static MultiBase.Component;

#pragma warning disable IDE1006

namespace MultiBase
{
    /// <summary>
    /// Interface class for a CPU Architecture
    /// </summary>
    /// <typeparam name="T">The word type of the Architecture</typeparam>
    /// <typeparam name="U">The unit type of the Architecture</typeparam>
    /// <typeparam name="V">The ALU class used by the Architecture</typeparam>
    public interface ISys<T, U, V> where T : Word<U> where U : Digit where V : ALU<T, U>
    {
        /// <summary>
        /// The Word Length used by the CPU
        /// </summary>
        public static int WordLength { get; protected set; }
        /// <summary>
        /// A method to be run which defines various properties of the CPU
        /// </summary>
        public void Setup();

        /// <summary>
        /// A reference to the ALU design of the CPU
        /// </summary>
        public V MathBox { get; }

        /// <summary>
        /// An array of valid Opcodes in the CPU's assembly language
        /// </summary>
        /// <remarks>This is used only as reference to the programmer and is not used in the running of the CPU</remarks>
        public string[] Opcodes { get; }

        /// <summary>
        /// An array of non-specialised registers used by the CPU, each containing a word.
        /// </summary>
        public Register<T, U>[] PrimaryRegisters { get; set; }

        /// <summary>
        /// Operand A used in ALU calculations
        /// </summary>
        public Register<T, U> OPA { get; set; }
        /// <summary>
        /// Operand B used in ALU calculations
        /// </summary>
        public Register<T, U> OPB { get; set; }
        /// <summary>
        /// Register containing the result of an ALU calculation
        /// </summary>
        public Register<T, U> OUT { get; set; }

        /// <summary>
        /// Sector register of the currently executing instruction
        /// </summary>
        public Register<T, U> SR { get; set; }
        /// <summary>
        /// Fine Address register of the currently executing instruction
        /// </summary>
        public Register<T, U> AR { get; set; }
        /// <summary>
        /// Sector register for an instruction to be jumped to
        /// </summary>
        public Register<T, U> JS { get; set; }
        /// <summary>
        /// Fine Address register for an instruction to be jumped to
        /// </summary>
        public Register<T, U> JA { get; set; }
        /// <summary>
        /// Clipboard register, used to copy values from one register to another
        /// </summary>
        public Register<T, U> CP { get; set; }
        /// <summary>
        /// Carry register, one Digit long, used for overflow in ALU calculations
        /// </summary>
        public Register<T, U> CR { get; set; }
    }

    [Serializable]
    public class MBaCException : Exception
    {
        public MBaCException() : base() { }
        public MBaCException(string message) : base(message) { }
        public MBaCException(string message, Exception inner) : base(message, inner) { }

        protected MBaCException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class Component
    {
        public class Digit
        {
            public int Value { get; set; }

            public static Dictionary<char, int> CharToInt { get; }
            public static Dictionary<int, char> IntToChar { get; }

            public Digit(int v) => Value = v;
            public Digit(char t) => Value = CharToInt[t];
            public Digit() => Value = 0;
        }

        public class Word<T> where T : Digit
        {
            public static int WordSize { get; set; }

            public T[] Digits { get; set; } = new T[WordSize];

            public Word(T[] ts) => Digits = ts[0..WordSize];
            public Word(string t) => Digits = (T[])Array.ConvertAll(t.ToCharArray()[0..WordSize], u => new Digit(u));
            public Word() => Digits = Digits.Select(i => (T)new Digit()).ToArray();
        }

        public class Register<T, U> where T : Word<U> where U : Digit
        {
            public int Size { get; protected set; }
            public string Alias { get; protected set; }
            public T Address { get; protected set; }
            public T Value { get; set; }

            /// <summary>
            /// Constructs a Register
            /// </summary>
            /// <param name="_size">Number of digits the Register will contain</param>
            /// <param name="_alias">A string name which can be used to identify the Register</param>
            /// <param name="_address">The digital address of the Register</param>
            public Register(int _size, string _alias, T _address)
            {
                Size = _size;
                Alias = _alias;
                Address = _address;
                Value = (T)new Word<U>();
            }
        }

        public class Minstrel<T, U> where T : Word<U> where U : Digit
        {
            public T Opcode { get; }
            public T Data { get; }

            public Minstrel(T _opcode, T _data)
            {
                Opcode = _opcode;
                Data = _data;
            }
        }

        public class Gate<U> where U : Digit
        {
            public string Alias { get; private set; }
            public bool Is2D { get; private set; } = true;
            public U[] Truth1D { get; private set; }
            public U[,] Truth2D { get; private set; }

            public Gate(string _alias, bool _is2D, string _table, int _maxValue)
            {
                this.Alias = _alias;
                this.Is2D = _is2D;
                BuildTable(_table, _maxValue);
            }

            private void BuildTable(string _table, int _maxValue)
            {
                if (Is2D && _table.Length == Math.Pow(_maxValue, 2))
                {
                    for (int x = 0; x < _maxValue; x++)
                    {
                        for (int y = 0; y < _maxValue; y++)
                        {
                            Truth2D[x, y] = (U)new Digit(_table[x * 3 + y]);
                        }
                    }
                }
                else if (!Is2D && _table.Length == _maxValue)
                {
                    for (int x = 0; x < _maxValue; x++)
                    {
                        Truth1D[x] = (U)new Digit(_table[x]);
                    }
                }
                else throw new MBaCException("Truth table cannot be serialised with the size specified.");
            }

            public virtual U Walk(U a)
            {
                if (Is2D) throw new MBaCException("One arguement is insufficient to walk a 2-Dimensional Gate.");
                return Truth1D[a.Value];
            }

            public virtual U Walk(U a, U b)
            {
                if (!Is2D) throw new MBaCException("Two arguements cannot be resolved to walk a 1-Dimensional Gate.");
                return Truth2D[a.Value, b.Value];
            }
        }

        public interface ALU<T, U> where T : Word<U> where U : Digit
        {
            public Dictionary<T, U[]> OpCodes { get; }

        }
    }
}
