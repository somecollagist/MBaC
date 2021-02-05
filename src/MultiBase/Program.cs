using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using jLib;

#pragma warning disable IDE1006

namespace MultiBase
{
    class Program
    {
        static void Main(string[] _args)
        {
            Ternary.Prole2100 prole2100 = new Ternary.Prole2100();                  //Load a system design
            StreamReader sr = new StreamReader("programs/prole2100/adder.mbac3");   //Create a stream to the program to run

            prole2100.Boot();
            prole2100.LoadProgram(sr);                                              //Let the system instance load the program
        }
    }

    public interface Data                       //Base class for bits, trits, etc.
    {
        int Value { get; set; }                 //The int value of the number
    }

    public interface IFunctions
    {
        public Data Add(Data x, Data y);
        public Data Sub(Data x, Data y);
        public Data Mul(Data x, Data y);
        public Data Div(Data x, Data y);
    }

    //This is where we start to define the cool bits of a system
    public interface Register
    {

    }

    /// <summary>
    /// How to make a system
    /// </summary>
    public interface ISys
    {
        public string ArchitectureStyle { get; }

    }

    [Serializable]
    public class MBaCException : Exception
    {
        public MBaCException() : base() { }
        public MBaCException(string message) : base(message) { }
        public MBaCException(string message, Exception inner) : base(message, inner) { }

        protected MBaCException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}