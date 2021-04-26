using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using jLib;

#pragma warning disable IDE0044
#pragma warning disable IDE0052

namespace MultiBase
{
    public static class TTernary
    {
        /// <summary>
        /// Trit (base 3, ternary) data type.
        /// </summary>
        /// <remarks>
        /// Uses balanced ternary, -1=False, 0=Undef, 1=True
        /// </remarks>
        public class Trit
        {
            /// <summary>
            /// Trit value: -1=False, 0=Undef, 1=True
            /// </summary>
            public int Value { get; set; }

            public const int False = -1;    //So we know what false is
            public const int Undef = 0;     //So we know what unknown is
            public const int True = 1;      //So we know what true is

            public Trit(int v = -1)         //Base constructor, default value for a trit is false (-)
            {
                this.Value = v;
            }

            public Trit(string i = "-")
            {
                this.Value = i switch
                {
                    "0" => 0,
                    "+" => 1,
                    _ => -1
                };
            }

            //Don't even ask me why Trit.Value returns anything from 43 to 46, I've no idea. Use this if you wanna know alt.
            public string GetValue()
            {
                Console.WriteLine("CP1");
                Console.WriteLine(this.Value);
                Console.WriteLine("CP2");
                return this.Value switch
                {
                    45 => "-",
                    48 => "0",
                    43 => "+",
                    _ => this.Value.ToString()
                };
            }
        }

        public class TU5
        {
            public Trit[] Trits { get; set; } = new Trit[5];    //Size of a word, analagous to a byte

            public TU5(Trit[] tr)       //If constructed by a Trit[], set the trits to the first 5 trits of the array arg
            {
                this.Trits = tr[0..4];
            }

            public TU5(string t)        //If constructed by a string, map the std representation to trits to be passed to the array prop
            {
                Trit[] tr = new Trit[5];
                for(int x = 0; x < 5; x++)
                {
                    tr[x] = new Trit(t[x]);
                }
            }

            public TU5()                //If no argument is passed, create a null tryte (----- = -121)
            {
                this.Trits = new Trit[]
                {
                    new Trit(-1),
                    new Trit(-1),
                    new Trit(-1),
                    new Trit(-1),
                    new Trit(-1),
                };
            }

            public int Resolve()    //Returns the integer representation of a tryte
            {
                int o = 0;
                for(int x = 4; x >= 0; x--)
                {
                    switch(this.Trits[x].Value)
                    {
                        case -1:
                            o -= (int)Math.Pow(3, x);
                            break;
                        case 1:
                            o += (int)Math.Pow(3, x);
                            break;
                        default:
                            break;
                    }
                }
                return o;
            }
        }

        public class Register
        {
            public int Size { get; private set; }
            public string Alias { get; private set; }
            public TU5 Address { get; private set; }
            public TU5 Value { get; private set; }

            public Register(string _alias, TU5 _address)
            {
                this.Size = 5;
                this.Alias = _alias;
                this.Address = _address;
                this.Value = new TU5();
            }

            public void SetSize(int _s)
            {
                this.Size = _s;
            }

            public void SetValue(TU5 _in)
            {
                this.Value = _in;
            }
        }

        /// <summary>
        /// Pair of TU5s that describe a machine code instruction
        /// </summary>
        public class Minstrel
        {
            public TU5 Opcode { get; private set; }
            public TU5 Data { get; private set; }

            public Minstrel(TU5 _opcode, TU5 _data)
            {
                this.Opcode = _opcode;
                this.Data = _data;
            }
        }

        public class Prole2100
        {
            public string ArchitectureStyle { get; } = "TU5";

            public class ALU
            {
                
                public readonly Dictionary<TU5, Trit[]> opcodes = new Dictionary<TU5, Trit[]>()
                {
                    { new TU5("-0---"), new Trit[] { new Trit(Trit.Undef), new Trit(Trit.False)} }, // 0-, AND
                    { new TU5("-0--0"), new Trit[] { new Trit(Trit.Undef), new Trit(Trit.Undef)} }, // 00, OR
                    { new TU5("-0--+"), new Trit[] { new Trit(Trit.Undef), new Trit(Trit.True) } }, // 0+, XOR
                    { new TU5("-0-0-"), new Trit[] { new Trit(Trit.True),  new Trit(Trit.False)} }, // +-, ADD
                    { new TU5("-0-00"), new Trit[] { new Trit(Trit.True),  new Trit(Trit.Undef)} }, // +0, SUB
                };

                private class Gate
                {
                    //If you need to use this, a goes down, b goes across.
                    public string Alias { get; private set; }
                    public bool Is2D { get; private set; } = true;
                    private Trit[] Truth1D { get; set; } = new Trit[3];
                    private Trit[,] Truth2D { get; set; } = new Trit[3, 3];

                    public Gate(string _alias, bool _is2D, string table)
                    {
                        this.Alias = _alias;
                        this.Is2D = _is2D;
                        BuildTable(table);
                    }

                    public void BuildTable(string table)
                    {
                        if (this.Is2D && table.Length == 9)
                        {
                            for (int x = 0; x <= 2; x++)
                            {
                                for (int y = 0; y <= 2; y++) this.Truth2D[x, y] = new Trit(table[x * 3 + y]);
                            }
                        }
                        else if (!this.Is2D && table.Length == 3)
                        {
                            for (int x = 0; x <= 2; x++) this.Truth1D[x] = new Trit(table[x]);
                        }
                        else throw new MBaCException("Truth table is unserializable with the size of the table.");
                    }

                    #pragma warning disable CS1998  //These method's run sync, but I can't figure out how to make the ALU work async without this, so sorry :/
                    public async Task<Trit> Walk(Trit a)
                    {
                        if (this.Is2D) throw new MBaCException("Another argument must be specifed for this truth table");
                        return this.Truth1D[a.Value + 1];
                    }

                    public async Task<Trit> Walk(Trit a, Trit b)
                    {
                        if (!this.Is2D) throw new MBaCException("Two arguments cannot be marshalled into a 1-Dimensional truth table");
                        return this.Truth2D[a.Value + 1, b.Value + 1];
                    }
                    #pragma warning restore CS1998

                    public void GetTable()
                    {
                        for (int x = 0; x <= 2; x++)
                        {
                            for (int y = 0; y <= 2; y++)
                            {
                                Console.WriteLine($"{x - 1} by {y - 1} :: {this.Truth2D[x, y].GetValue()}");
                            }
                        }
                    }
                }

                private Dictionary<String, Gate> Gates = new Dictionary<string, Gate>()
                {
                    { "BUF", new Gate("BUF", false, "-0+")},        //Does nothing to a
                    { "NOT", new Gate("NOT", false, "+0-")},        //Returns the NOT of a
                    { "NTI", new Gate("NTI", false, "+--")},        //Negative Threshold Inverter (see Dr Jones' logic)
                    { "PTI", new Gate("PTI", false, "++-")},        //Positive Threshold Inverter (see Dr Jones' logic)
                    { "INC", new Gate("INC", false, "0+-")},        //Increment (doesn't do carry over)
                    { "DEC", new Gate("DEC", false, "+-0")},        //Decrement (doesn't do carry over)
                    { "ISF", new Gate("ISF", false, "+--")},        //Is False 
                    { "ISU", new Gate("ISU", false, "-+-")},        //Is Undefined
                    { "IST", new Gate("IST", false, "--+")},        //Is True
                    { "MIN", new Gate("MIN", false, "-00")},        //Diadic clamp minimum (see Dr Jones' logic)
                    { "MAX", new Gate("MAX", false, "00+")},        //Diadic clamp maximum (see Dr Jones' logic)

                    { "AND", new Gate("AND", true, "----00-0+")},   //Returns a  AND b
                    { "OR",  new Gate("OR",  true, "-0+00++++")},   //Returns a   OR b
                    { "NAND",new Gate("NAND",true, "++++00+0-")},   //Returns a NAND b
                    { "NOR", new Gate("NOR", true, "+0-00----")},   //Returns a  NOR b
                    { "XOR", new Gate("XOR", true, "-0+000+0-")},   //Returns a  XOR b
                    { "EQ",  new Gate("EQ",  true, "+---+---+")},   //Returns a   EQ b (composite of Identity gates ISx)
                    { "SUM", new Gate("SUM", true, "+-0-0+0+-")},   //Returns a  SUM b (doesn't do carry over, but is needed in ADD and SUB modules)
                    { "CON", new Gate("CON", true, "-0000000+")},   //Returns consensus of a and b
                    { "ANY", new Gate("ANY", true, "--0-0+0++")},   //Returns any of a and b (see Dr Jones' logic)
                    { "VAL", new Gate("VAL", true, "-0-000+0+")},   //Returns a is b isn't undefined (like really crap multiplication)

                };

                private Trit _a, _b, ci, and, or, xor, add, sub, cfa, cfs;
                private Trit[] addsubx;

                private async Task<Trit[]> Add(Trit a, Trit b, Trit cin)
                {
                    Trit[] returns = new Trit[2];   //First trit is for s, second is for cout

                    Trit n = await Gates["SUM"].Walk(a, b);
                    Trit x = await Gates["CON"].Walk(a, b);
                    Trit y = await Gates["CON"].Walk(n, cin);

                    returns[0] = await Gates["SUM"].Walk(n, cin);   //s return trit
                    returns[1] = await Gates["ANY"].Walk(x, y);     //count return trit

                    return returns;
                }

                private async Task<Trit[]> Min(Trit a, Trit b, Trit cin)
                {
                    return await Add(a, await Gates["NOT"].Walk(b), cin);
                }

                // Oh lawd, he comin'
                public async void Walk(Trit CTRL_A, Trit CTRL_B)
                {
                    Trit[] stream = new Trit[5];

                    ci = CT.Value.Trits[4]; //Access the last trit because that determines the carry in.
                    for (int x = 0; x < 5; x++)
                    {
                        _a = OPA.Value.Trits[x];
                        _b = OPB.Value.Trits[x];

                        and = await Gates["AND"].Walk(_a, _b);
                        or  = await Gates["OR" ].Walk(_a, _b);
                        xor = await Gates["XOR"].Walk(_a, _b);

                        addsubx = await Add(_a, _b, ci);
                        add = addsubx[0];
                        cfa = addsubx[1];
                        addsubx = await Min(_a, _b, ci);
                        sub = addsubx[0];
                        cfs = addsubx[1];

                        switch($"{CTRL_A.GetValue()}{CTRL_B.GetValue()}")
                        {
                            case "0-":  //AND
                                stream[x] = and;
                                break;

                            case "00":  //OR
                                stream[x] = or;
                                break;

                            case "0+":  //XOR
                                stream[x] = xor;
                                break;

                            case "+-":  //ADD
                                stream[x] = add;
                                ci = cfa;
                                break;

                            case "+0":  //SUB
                                stream[x] = sub;
                                ci = cfs;
                                break;
                        }
                    }
                    Registers[15].SetValue(new TU5(stream));
                    CT.SetValue(new TU5($"0000{ci.GetValue()}"));
                }
            }

            //We're just translating triador.cpp on the reference repos
            protected readonly string[] opcodes_str = new string[] //Operation mnemonics (this is basically assembly) each command takes a 5-trit arguement
            {
                /* ----- -121 */ "JMP", // Unconditional jump to memory address SR*27 + argument
                /* ----0 -120 */ "SKP", // Conditonal skip of next command depending on the sign of the register, see implementation for more detail
                /* ---0- -118 */ "NEG", // Negates the argument register
                /* ---00 -117 */ "DEC", // Increments argument register
                /* ---0+ -116 */ "INC", // Increments argument register
                /* ---+- -115 */ "COP", // Copies argument register to clipboard register CP
                /* ---+0 -114 */ "PST", // Pastes the value in CP to the argument register
                /* -0---  -94 */ "AND", // Sets R16 to OPA AND OPB
                /* -0--0  -93 */ "ORR", // Sets R16 to OPA OR  OPB
                /* -0--+  -92 */ "XOR", // Sets R16 to OPA XOR OPB
                /* -0-0-  -91 */ "ADD", // Sets R16 to OPA  +  OPB
                /* -0-00  -90 */ "MIN", // Sets R16 to OPA  -  OPB
                /* -+---  -67 */ "R01", // Write argument to register 01
                /* -+--0  -66 */ "R02", // Write argument to register 02
                /* -+--+  -65 */ "R03", // Write argument to register 03
                /* -+-0-  -64 */ "R04", // Write argument to register 04
                /* -+-00  -63 */ "RO6", // Write argument to register 05
                /* -+-0+  -62 */ "RO5", // Write argument to register 06
                /* -+-+-  -61 */ "RO7", // Write argument to register 07
                /* -+-+0  -60 */ "R08", // Write argument to register 08
                /* -+-++  -59 */ "R09", // Write argument to register 09
                /* -+0--  -58 */ "R10", // Write argument to register 10
                /* -+0-0  -57 */ "R11", // Write argument to register 11
                /* -+0-+  -56 */ "R12", // Write argument to register 12
                /* -+00-  -55 */ "R13", // Write argument to register 13
                /* -+000  -54 */ "R14", // Write argument to register 14
                /* -+00+  -53 */ "R15", // Write argument to register 15
                /* -+0+-  -52 */ "R16", // Write argument to register 16
            };

            private static Register[] Registers = new Register[]{
                new Register("R01", new TU5("+----")),
                new Register("R02", new TU5("+---0")),
                new Register("R03", new TU5("+---+")),
                new Register("R04", new TU5("+--0-")),
                new Register("R05", new TU5("+--00")),
                new Register("R06", new TU5("+--0+")),
                new Register("R07", new TU5("+--+-")),
                new Register("R08", new TU5("+--+0")),
                new Register("R09", new TU5("+--++")),
                new Register("R10", new TU5("+-0--")),
                new Register("R11", new TU5("+-0-0")),
                new Register("R12", new TU5("+-0-+")),
                new Register("R13", new TU5("+-00-")),
                new Register("R14", new TU5("+-000")),
                new Register("R15", new TU5("+-00+")),
                new Register("R16", new TU5("+-0+-")), //This is where the result of an ALU command is stored
            };

            private static Register OPA = new Register("OPA", new TU5("+-0+0"));    //Operand A register
            private static Register OPB = new Register("OPB", new TU5("+-0++"));    //Operand B register

            private static Register SR = new Register("SR", new TU5("+-+--"));      //Sector register, the first tryte of a memory address, covers 243 individual addresses
            private static Register CP = new Register("CP", new TU5("+-+-0"));      //Clipboard register, used to copy or paste data between registers
            private static Register CT = new Register("CT", new TU5("+-+-+"));      //Carry Trit register, used to store the left over trit from an ALU command, N.B. ONLY THE LAST TRIT IS DETERMINISTIC

            private static Register P1I = new Register("P1I", new TU5("+-+0-"));    //Stores the instruction for the executing command
            private static Register P1A = new Register("P1A", new TU5("+-+00"));    //Stores the arguement for the executing command
            private static Register P2I = new Register("P2I", new TU5("+-+0+"));    //Stores the instruction for the executing command
            private static Register P2A = new Register("P2A", new TU5("+-++-"));    //Stores the arguement for the executing command

            private static Register CSR = new Register("CSR", new TU5("+-++0"));    //Sector Register Counter, keeps track of the current instruction's sector
            private static Register CIR = new Register("CIR", new TU5("+-+++"));    //Instruction Register Counter, keeps track of the current instruction's specific address

            private static List<Minstrel> program = new List<Minstrel>();           //List of commands

            public void Boot()
            {
                try
                {
                    CT.SetValue(new TU5("00000"));
                    ALU alu = new ALU();

                    OPA.SetValue(new TU5("000+-"));
                    OPB.SetValue(new TU5("0000+"));
                    alu.Walk(new Trit(Trit.True), new Trit(Trit.False));    //Addition opcode

                    Console.Write(">>> ");
                    //foreach(Trit t in Registers[15].Value.Trits) Console.Write(t.GetValue());
                    Console.WriteLine();
                    Console.WriteLine($" {CT.Value.Trits[4]}");
                }
                catch(Exception e)
                {
                    Console.WriteLine("We're here lads");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }

            public void LoadProgram(StreamReader sr)
            {
                CT.SetSize(1);                                                                                                          //This is a void method and I can't run it outside of a method

                string stream = new Regex(@"[^\+\-0]").Replace(sr.ReadToEnd(), "");                                                     //Gets the characters of the .mbac3 file with all non -0+ chars removed
                string[] instructions = Enumerable.Range(0, stream.Length / 10).Select(i => stream.Substring(i * 10, 10)).ToArray();    //Splits the stream into chunks of 10 trits (2 TU5s, 1 Minstrel)
                foreach (string inst in instructions)                                                                                   //Generates a minstrel for each of the minstrels in the instruction array
                {
                    program.Add(new Minstrel(new TU5(inst.Substring(0, 5)), new TU5(inst.Substring(4, 5))));
                }
            }

            private void Cycle()
            {
            }
        }
    }
}
