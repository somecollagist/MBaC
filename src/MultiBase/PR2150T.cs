using System;
using System.Collections.Generic;
using System.Text;

using static MultiBase.Ternary;
using static MultiBase.Component;

namespace MultiBase
{
    public class PR2150T : ISys<TU, Trit, PR2150T_ALU>
    {
        public static int WordLength { get; protected set; }

        public void Setup()
        {
            TU.WordSize = 5;
            WordLength = 5;
        }

        public PR2150T_ALU MathBox { get; } = new PR2150T_ALU();

        public string[] Opcodes { get; } = new string[]
        {
            /* ????? */ "JMP", // Unconditional jump to memory address defined by jump registers
            /* ????? */ "NEG", // Desc.
            /* ????? */ "DEC", // Desc.
            /* ????? */ "INC", // Desc.
            /* ????? */ "COP", // Desc.
            /* ????? */ "PST", // Desc.
            /* ????? */ "AND", // Desc.
            /* ????? */ "ORR", // Desc.
            /* ????? */ "XOR", // Desc.
            /* ????? */ "ADD", // Desc.
            /* ????? */ "SUB", // Desc.
        };

        //NB: all registers exist in the reserved sector -----

        public Register<TU, Trit>[] PrimaryRegisters { get; set; } = new Register<TU, Trit>[]
        {
            new Register<TU, Trit>(WordLength, "R01", new TU("+----")),
            new Register<TU, Trit>(WordLength, "R02", new TU("+---0")),
            new Register<TU, Trit>(WordLength, "R03", new TU("+---+")),
            new Register<TU, Trit>(WordLength, "R04", new TU("+--0-")),
            new Register<TU, Trit>(WordLength, "R05", new TU("+--00")),
            new Register<TU, Trit>(WordLength, "R06", new TU("+--0+")),
            new Register<TU, Trit>(WordLength, "R07", new TU("+--+-")),
            new Register<TU, Trit>(WordLength, "R08", new TU("+--+0")),
            new Register<TU, Trit>(WordLength, "R09", new TU("+--++")),
            new Register<TU, Trit>(WordLength, "R10", new TU("+-0--")),
            new Register<TU, Trit>(WordLength, "R11", new TU("+-0-0")),
            new Register<TU, Trit>(WordLength, "R12", new TU("+-0-+")),
            new Register<TU, Trit>(WordLength, "R13", new TU("+-00-")),
            new Register<TU, Trit>(WordLength, "R14", new TU("+-000")),
            new Register<TU, Trit>(WordLength, "R15", new TU("+-00+")),
            new Register<TU, Trit>(WordLength, "R16", new TU("+-0+-")),
            new Register<TU, Trit>(WordLength, "R17", new TU("+-0+0")),
            new Register<TU, Trit>(WordLength, "R18", new TU("+-0++")),
            new Register<TU, Trit>(WordLength, "R19", new TU("+-+--")),
            new Register<TU, Trit>(WordLength, "R20", new TU("+-+-0")),
            new Register<TU, Trit>(WordLength, "R21", new TU("+-+-+")),
            new Register<TU, Trit>(WordLength, "R22", new TU("+-+0-")),
            new Register<TU, Trit>(WordLength, "R23", new TU("+-+00")),
            new Register<TU, Trit>(WordLength, "R24", new TU("+-+0+")),
            new Register<TU, Trit>(WordLength, "R25", new TU("+-++-")),
            new Register<TU, Trit>(WordLength, "R26", new TU("+-++0")),
            new Register<TU, Trit>(WordLength, "R27", new TU("+-+++"))
        };

        public Register<TU, Trit> OPA { get; set; } = new Register<TU, Trit>(WordLength, "OPA", new TU("+00--"));
        public Register<TU, Trit> OPB { get; set; } = new Register<TU, Trit>(WordLength, "OPB", new TU("+00-0"));
        public Register<TU, Trit> OUT { get; set; } = new Register<TU, Trit>(WordLength, "OUT", new TU("+00-+"));
        public Register<TU, Trit> SR  { get; set; } = new Register<TU, Trit>(WordLength, "SR",  new TU("+000-"));
        public Register<TU, Trit> AR  { get; set; } = new Register<TU, Trit>(WordLength, "AR",  new TU("+0000"));
        public Register<TU, Trit> JS  { get; set; } = new Register<TU, Trit>(WordLength, "JS",  new TU("+000+"));
        public Register<TU, Trit> JA  { get; set; } = new Register<TU, Trit>(WordLength, "JA",  new TU("+00+-"));
        public Register<TU, Trit> CP  { get; set; } = new Register<TU, Trit>(WordLength, "CP",  new TU("+00+0"));
        public Register<TU, Trit> CR  { get; set; } = new Register<TU, Trit>(1,          "CR",  new TU("+00++"));
    }

    public class PR2150T_ALU : ALU<TU, Trit>
    {
        public Dictionary<TU, Trit[]> OpCodes { get; } = new Dictionary<TU, Trit[]>()
        {

        };
    }
}
