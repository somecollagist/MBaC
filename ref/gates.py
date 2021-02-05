from os import system

class Gate():
    def MakeTable(self, table, a, b):
        tau = []
        for x in range(0, len(a)):
            tau.append([])
            for y in range(0, len(b)):
                tau[x].append(table[y*3+x])

        return tau

    def GetTable(self):
        print("\n".join(["\t".join([str(cell) for cell in row]) for row in self.table]))

    def __init__(self, alias, table, b="?"):
        self.alias = alias
        self.table = self.MakeTable(table, "-0+", b)

    def Walk(self, a, b="x"):
        if(b == "x"):
            return Convert(self.table[a][0])
        return Convert(self.table[b][a])

def ShowGates():
    for gate in gates:
        print(gates[gate].alias)

def GetGate(alias):
    try:
        return gates[alias].GetTable()
    except:
        return "Gate not found"

def Convert(n):
        if(n == "-"):
            return 0
        if(n == "0"):
            return 1
        if(n == "+"):
            return 2

def Revert(n):
    if(n == 0):
        return "-"
    if(n == 1):
        return "0"
    if(n == 2):
        return "+"

def RunGates(a,b,debug=False):
    with open("gatesdef.py") as f:
        exec("".join(f.readlines()))

gates = {
    "BUF" : Gate("BUF", "-0+"),
    "NOT" : Gate("NOT", "+0-"),
    "NTI" : Gate("NTI", "+--"),
    "PTI" : Gate("PTI", "++-"),
    "INC" : Gate("INC", "0+-"),
    "DEC" : Gate("DEC", "+-0"),
    "ISF" : Gate("ISF", "+--"),
    "ISU" : Gate("ISU", "-+-"),
    "IST" : Gate("IST", "--+"),
    "MIN" : Gate("MIN", "-00"),
    "MAX" : Gate("MAX", "00+"),

    "AND" : Gate("AND",  "----00-0+", "-0+"),
    "OR"  : Gate("OR",   "-0+00++++", "-0+"),
    "NAND": Gate("NAND", "++++00+0-", "-0+"),
    "NOR" : Gate("NOR",  "+0-00----", "-0+"),
    "XOR" : Gate("XOR",  "-0+000+0-", "-0+"),
    "EQ"  : Gate("EQ",   "+---+---+", "-0+"),

    "SUM" : Gate("SUM", "+-0-0+0+-", "-0+"),
    "SUB" : Gate("SUB", "0-++0--+0", "-0+"),

    "CON" : Gate("CON", "-0000000+", "-0+"),
    "ANY" : Gate("ANY", "--0-0+0++", "-0+"),

    "VAL" : Gate("VAL", "-0+000-0+", "-0+"),
}

while(True):
    cmd = input("Command: ")

    if(cmd == "help"):
        print("    help : shows this message")
        print("    show : shows all the gates")
        print("    get  : gets the table for a gate after prompting for its name")
        print("    cls  : clears the console")
        print("    run  : runs the specified gate with multiple program-defined arguments")
        print("    debug: runs the specified gate with a set of program-defined arguments in debug mode")
        print("Press Ctrl+C for a keyboard interrupt to exit")
    elif(cmd == "show"):
        ShowGates()
    elif(cmd == "get"):
        gate = input("Gate: ")
        ret = GetGate(gate)
        if(ret):
            print(ret)
    elif(cmd == "run"):
        RunGates("+0", "0+")
        RunGates("+0", "++")
        RunGates("+0", "0-")
        RunGates("+0", "00")
        RunGates("0-", "0+")
        RunGates("0-", "0-")
        RunGates("0-", "00")
        RunGates("00", "+-")
        RunGates("00", "-+")
        RunGates("00", "00")
    elif(cmd == "debug"):
        RunGates("0-", "0+", True)

    print()
    if(cmd == "cls"):
        system("cls")