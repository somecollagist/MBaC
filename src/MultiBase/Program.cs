using System;
using System.Collections.Generic;
using System.Linq;

using jLib;

#pragma warning disable IDE1006

namespace MultiBase
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello world");
        }
    }

    public interface Data
    {
        int Value { get; set; }
    }

    public interface IGates
    {
        public Data NOT(Data x);

        public Data OR(Data x, Data y);
        public Data AND(Data x, Data y);

        public Data NOR(Data x, Data y);
        public Data NAND(Data x, Data y);
    }

    public interface IFunctions
    {
        public Data Add(Data x, Data y);
        public Data Sub(Data x, Data y);
        public Data Mul(Data x, Data y);
        public Data Div(Data x, Data y);
    }

    public interface IRegister
    {
        public Data[] Digits { get; set; }
        public Type DataType { get; set; }

        public void INC();
    }
}