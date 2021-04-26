using System;
using System.Collections.Generic;
using System.Text;

namespace MultiBase
{
    public static class Ternary
    {
        public class Trit : Component.Digit
        {
            public const int FALSE = -1;
            public const int UNDEF =  0;
            public const int TRUE  =  1;

            public new Dictionary<char, int> CharToInt = new Dictionary<char, int>
            {
                {'-', FALSE },
                {'0', UNDEF },
                {'+', TRUE  }
            };
            public new Dictionary<int, char> IntToChar = new Dictionary<int, char>
            {
                {FALSE, '-' },
                {UNDEF, '0' },
                {TRUE,  '+' }
            };

            public Trit(int v = FALSE) : base(v) { }
            public Trit(char t = '-') : base(t) { }
            public Trit() : base() { }
        }

        public class TU : Component.Word<Trit>
        {
            public TU(Trit[] ts) : base(ts) { }
            public TU(string t) : base(t) { }
            public TU() : base() { }
        }

        public class Register : Component.Register<TU, Trit>
        {
            public Register(int _size, string _alias, TU _address) : base(_size, _alias, _address)
            {
                Value = new TU();
            }
        }

        public class Minstrel : Component.Minstrel<TU, Trit>
        {
            public Minstrel(TU _opcode, TU _data) : base(_opcode, _data) { }
        }
    }
}
