using System;
using System.Collections.Generic;
using System.Text;

public class Element : IComparable<Element>
{
    public Element(double aVal, double aFunc)
    {
        val = aVal;
        func = aFunc;
    }

    public double Value
    {
        get { return val; }
        set { val = value; }
    }

    public double MembershipFunction
    {
        get { return func; }
        set { func = value; }
    }

    public int CompareTo(Element E)
    {
        return this.Value.CompareTo(E.Value);
    }

    public int CompareTo(double aVal)
    {
        return this.Value.CompareTo(aVal);
    }

    public override string ToString()
    {
        return "<" + Value + ", " + MembershipFunction.ToString("F2") + ">";
    }

    double val;
    double func;
}

public class FuzzySet
{
    public FuzzySet()
    {
        Set = new List<Element>();
    }

    public int Count
    {
        get { return Set.Count; }
    }

    public bool Contains(double aVal)
    {
        foreach (Element E in Set)
        {
            if (E.CompareTo(aVal) == 0)
                return true;
        }

        return false;
    }

    public bool Contains(Element aElem)
    {
        foreach (Element E in Set)
        {
            if (E.CompareTo(aElem) == 0)
                return true;
        }

        return false;
    }

    public int Find(Element aElem)
    {
        for (int i = 0; i < Count; i++)
        {
            if (Set[i].CompareTo(aElem) == 0)
                return i;
        }

        return -1;
    }

    public void Sort()
    {
        Set.Sort();
    }

    public void AddElement(Element E)
    {
        if (!Contains(E))
            Set.Add(new Element(E.Value, E.MembershipFunction));
    }

    public void AddElement(double aVal, double aFunc)
    {
        if (!Contains(aVal))
            Set.Add(new Element(aVal, aFunc));
    }

    public Element this[int index]
    {
        get
        {
            return Set[index];
        }
        set
        {
            Set[index] = value;
        }
    }

    public static FuzzySet operator &(FuzzySet lhs, FuzzySet rhs)
    {
        FuzzySet S = new FuzzySet();

        for (int i = 0, k; i < rhs.Count; i++)
        {
            if ((k = lhs.Find(rhs[i])) != -1)
            {
                S.AddElement(new Element(lhs[k].Value, Math.Min(lhs[k].MembershipFunction, rhs[i].MembershipFunction)));
            }
        }

        return S;
    }

    public static FuzzySet operator |(FuzzySet lhs, FuzzySet rhs)
    {
        FuzzySet S = new FuzzySet();

        for (int i = 0; i < lhs.Count; i++)
        {
            S.AddElement(lhs[i].Value, lhs[i].MembershipFunction);
        }

        for (int i = 0, k; i < rhs.Count; i++)
        {
            if ((k = S.Find(rhs[i])) != -1)
            {
                S[k].MembershipFunction = Math.Max(S[k].MembershipFunction, rhs[i].MembershipFunction);
            }
            else
            {
                S.AddElement(rhs[i]);
            }
        }

        return S;
    }

    public static FuzzySet operator !(FuzzySet rhs)
    {
        FuzzySet S = new FuzzySet();

        for (int i = 0; i < rhs.Count; i++)
        {
            S.AddElement(new Element(rhs[i].Value, 1 - rhs[i].MembershipFunction));
        }

        return S;
    }

    public void Copy(FuzzySet S)
    {
        if (Set != null)
            Set.Clear();
        else
            Set = new List<Element>();

        for (int i = 0; i < S.Count; i++)
            AddElement(S[i]);
    }

    public static FuzzySet Very(FuzzySet A)
    {
        FuzzySet S = new FuzzySet();

        for (int i = 0; i < A.Count; i++)
        {
            S.AddElement(A[i].Value, Math.Pow(A[i].MembershipFunction, 2));
        }

        return S;
    }

    public static FuzzySet Slightly(FuzzySet A)
    {
        FuzzySet S = new FuzzySet();

        for (int i = 0; i < A.Count; i++)
        {
            S.AddElement(A[i].Value, Math.Pow(A[i].MembershipFunction, 0.5));
        }

        return S;
    }

    public override string ToString()
    {
        string s = "{";
        foreach (Element E in Set)
        {
            s += E + " ";
        }
        return s + "}";
    }

    List<Element> Set;
}

public class FuzzyVariable
{
    public FuzzyVariable()
    {
        A = new FuzzySet();
    }

    public FuzzyVariable(string aName, double aMin, double aMax)
    {
        name = aName;
        min = aMin;
        max = aMax;
        A = new FuzzySet();
    }

    public FuzzyVariable(string aName, FuzzySet S, double aMin, double aMax)
    {
        name = aName;
        min = aMin;
        max = aMax;
        A = new FuzzySet();
        A.Copy(S);
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public double Min
    {
        get { return min; }
        set { min = value; }
    }

    public double Max
    {
        get { return max; }
        set { max = value; }
    }

    public FuzzySet Set
    {
        get { return A; }
        set { Set.Copy(value); }
    }

    public static FuzzyVariable operator &(FuzzyVariable lhs, FuzzyVariable rhs)
    {
        FuzzyVariable V = new FuzzyVariable();
        V.Name = lhs.Name + "_and_" + rhs.Name;
        V.Min = Math.Max(lhs.Min, rhs.Min);
        V.Max = Math.Min(lhs.Max, rhs.Max);
        V.Set = lhs.Set & rhs.Set;

        return V;
    }

    public static FuzzyVariable operator |(FuzzyVariable lhs, FuzzyVariable rhs)
    {
        FuzzyVariable V = new FuzzyVariable();
        V.Name = lhs.Name + "_or_" + rhs.Name;
        V.Min = Math.Min(lhs.Min, rhs.Min);
        V.Max = Math.Max(lhs.Max, rhs.Max);
        V.Set = lhs.Set | rhs.Set;

        return V;
    }

    public static FuzzyVariable operator !(FuzzyVariable rhs)
    {
        FuzzyVariable V = new FuzzyVariable();
        V.Name = "not_" + rhs.Name;
        V.Min = rhs.Min;
        V.Max = rhs.Max;
        V.Set = !rhs.Set;

        return V;
    }

    public static FuzzyVariable Very(FuzzyVariable A)
    {
        FuzzyVariable V = new FuzzyVariable();
        V.Name = "very_" + A.Name;
        V.Min = A.Min;
        V.Max = A.Max;
        V.InitSet(FuzzySet.Very(A.Set));

        return V;
    }

    public static FuzzyVariable Slightly(FuzzyVariable A)
    {
        FuzzyVariable V = new FuzzyVariable();
        V.Name = "slightly_" + A.Name;
        V.Min = A.Min;
        V.Max = A.Max;
        V.InitSet(FuzzySet.Slightly(A.Set));

        return V;
    }

    public void Sort()
    {
        A.Sort();
    }

    public override string ToString()
    {
        return Name + " " + Set;
    }

    public void InitSet(FuzzySet S)
    {
        A.Copy(S);
    }

    string name;
    FuzzySet A;
    double min;
    double max;
}

public class LinguisticVariable
{
    public LinguisticVariable()
    {
        T = new List<FuzzyVariable>();
    }

    public LinguisticVariable(string aName)
    {
        name = aName;
        T = new List<FuzzyVariable>();
    }

    public LinguisticVariable(string aName, double aMin, double aMax)
    {
        name = aName;
        min = aMin;
        max = aMax;
        T = new List<FuzzyVariable>();
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public List<FuzzyVariable> Term
    {
        get { return T; }
    }

    public double Min
    {
        get { return min; }
        set { min = value; }
    }

    public double Max
    {
        get { return max; }
        set { max = value; }
    }

    public FuzzyVariable Value
    {
        get { return Val; }
        set { Val = value; }
    }

    public void AddTerm()
    {
        if (Val == null)
            return;
        T.Add(Val);
    }

    public void AddTerm(FuzzyVariable V)
    {
        T.Add(V);
    }

    public void AddTerm(string name, FuzzySet A, double min, double max)
    {
        T.Add(new FuzzyVariable(name, A, min, max));
    }

    public void InitTerms(string[] terms, bool man)
    {
        double x1, x2, i;

        if (man)
        {
            x1 = 150;
            x2 = 174;
        }
        else
        {
            x1 = 150;
            x2 = 160;
        }
        T.Clear();
        T.Add(new FuzzyVariable(terms[0], x1, x2));
        for (i = x1; i <= x2; i++)
            T[0].Set.AddElement(i, (x2 - i) / (x2 - x1));
        for (i = x2 + 1; i <= max; i++)
            T[0].Set.AddElement(i, 0);

        if (man)
        {
            x1 = 170;
            x2 = 180;
        }
        else
        {
            x1 = 159;
            x2 = 169;
        }
        T.Add(new FuzzyVariable(terms[1], x1, x2));
        for (i = min; i <= x1 - 1; i++)
            T[1].Set.AddElement(i, 0);
        for (i = x1; i <= x2; i++)
            T[1].Set.AddElement(i, (10- Math.Abs(2 * i - (x1 + x2))) / (10));
        for (i = x2 + 1; i <= max; i++)
            T[1].Set.AddElement(i, 0);

        if (man)
        {
            x1 = 177;
            x2 = 210;
        }
        else
        {
            x1 = 166;
            x2 = 210;
        }
        T.Add(new FuzzyVariable(terms[2], x1, x2));
        for (i = min; i <= x1 - 1; i++)
            T[2].Set.AddElement(i, 0);
        for (i = x1; i <= x2; i++)
            T[2].Set.AddElement(i, (i - x1) / (x2 - x1));

    }

    public int FindTerm(string name)
    {
        for (int i = 0; i < T.Count; i++)
            if (T[i].Name == name)
                return i;

        return -1;
    }

    public void Generate(string s)
    {
        FuzzyVariable V1 = null, V2;
        Stack<string> op = new Stack<string>();
        LinkedList<string> post = new LinkedList<string>();
        Stack<FuzzyVariable> var = new Stack<FuzzyVariable>();
        int k;
        string tmp = null;
        List<string> tokens = new List<string>();

        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] != ' ')
            {
                if (s[i] == '(')
                {
                    if (tmp != null)
                    {
                        tokens.Add(tmp);
                        tmp = null;
                    }
                    tokens.Add("(");
                }
                else
                    if (s[i] == ')')
                    {
                        if (tmp != null)
                        {
                            tokens.Add(tmp);
                            tmp = null;
                        }
                        tokens.Add(")");
                    }
                    else
                        tmp += s[i];
            }
            else
            {
                tokens.Add(tmp);
                tmp = null;
            }
        }
        if (tmp != null)
            tokens.Add(tmp);
        foreach (string str in tokens)
        {
            Console.WriteLine(str);
        }

        for (int i = 0; i < tokens.Count; i++)
        {
            if ((k = FindTerm(tokens[i])) != -1)
                post.AddLast(tokens[i]);
            else
            {
                switch (tokens[i])
                {
                    case "not":
                    case "very":
                    case "slightly":
                    case "(":
                        op.Push(tokens[i]);
                        break;
                    case ")":
                        while (true)
                        {
                            if (op.Count == 0)
                                break;
                            if (op.Peek() == "(")
                            {
                                op.Pop();
                                break;
                            }
                            post.AddLast(op.Pop());
                        }
                        if (op.Count != 0)
                            switch (op.Peek())
                            {
                                case "not":
                                case "very":
                                case "slightly":
                                case "and":
                                case "or":
                                    post.AddLast(op.Pop());
                                    break;
                            }
                        break;
                    case "and":
                    case "or":
                        op.Push(tokens[i]);
                        break;
                }
            }
        }
        while (op.Count != 0)
            post.AddLast(op.Pop());

        foreach (string str in post)
        {
            if ((k = FindTerm(str)) != -1)
            {
                var.Push(T[k]);
            }
            else
                if (str == "not" || str == "very" || str == "slightly")
                {
                    V1 = var.Pop();
                    switch (str)
                    {
                        case "not":
                            V1 = !V1;
                            break;
                        case "very":
                            V1 = FuzzyVariable.Very(V1);
                            break;
                        case "slightly":
                            V1 = FuzzyVariable.Slightly(V1);
                            break;
                    }
                    var.Push(V1);
                }
                else
                {
                    V1 = var.Pop();
                    V2 = var.Pop();
                    switch (str)
                    {
                        case "and":
                            V1 = V1 & V2;
                            break;
                        case "or":
                            V1 = V1 | V2;
                            break;
                    }
                    var.Push(V1);
                }
        }

        Val = var.Pop();
        Val.Sort();
    }

    string name;
    List<FuzzyVariable> T;
    FuzzyVariable Val;
    double min;
    double max;
}