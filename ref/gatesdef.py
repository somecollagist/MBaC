_a = list(a[::-1])
_b = list(b[::-1])
c  = "0"

r = []
fc = ""

for x in range(len(_a)):
    A = Convert(_a[x])
    B = gates["NOT"].Walk(Convert(_b[x]))
    C = Convert(c)

    N = gates["SUM"].Walk(A,B)
    X = gates["CON"].Walk(A,B)
    Y = gates["CON"].Walk(C,N)
    S = gates["SUM"].Walk(N,C)
    Cf= gates["ANY"].Walk(X,Y)

    if(debug):
        print(f"A:{Revert(A)}, B:{Revert(B)}, Ci:{Revert(C)}")
        print(f"N:{Revert(N)}, X:{Revert(X)}, Y:{Revert(Y)}")
        print(f"S:{Revert(S)}, Cf:{Revert(Cf)}\n")

    c = Revert(Cf)
    fc = c
    r.append(Revert(S))

r.reverse()
print(f"a: {a}, b:{b}")
print(f"r: {''.join(r)}, fc:{fc}\n")