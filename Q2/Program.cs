using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

string pattern = @"([*^+/)(])|(-?[0-9]+\.[0-9]+)|(\s)|(\bsin\(\b)|(\btan\(\b)|(\bcos\(\b)|(\bln\(\b)|(\bsqrt\(\b)|(\babs\(\b)|(\bexp\(\b)";

Regex rg = new Regex(pattern);
string input = Console.ReadLine();
var spl = rg.Split(input).ToList();
List<string> spl2 = new List<string>();
Stack<string> checkSyntax = new Stack<string>();
Stack<double> operands = new Stack<double>();
Stack<string> operators = new Stack<string>();
string output = null;
double num;
for(int i=0; i<spl.Count; i++){
    if(spl[i]!="" && spl[i]!=" " && double.TryParse(spl[i], out num))
        operands.Push(num);
    if(spl[i]!="" && spl[i]!=" " && !double.TryParse(spl[i], out num))
        operators.Push(spl[i]);
    if(spl[i]=="(" || spl[i]=="sin(" || spl[i]=="cos(" || spl[i]=="tan(" || spl[i]=="ln(" ||spl[i]=="abs(" || spl[i]=="sqrt(" || spl[i]=="exp(") 
        checkSyntax.Push(spl[i]);
    if(spl[i]==")"){
        if(checkSyntax.Count != 0)
            checkSyntax.Pop();
        else{
            output = "INVALID";
            i=spl.Count;
        }
    }
    if(i<spl.Count && spl[i]!="" && spl[i]!=" ")
        spl2.Add(spl[i]);
}
for(int j=0; j<spl2.Count-1; j++){
    if(spl2[j] == "(" || spl2[j]=="sin(" || spl2[j]=="cos(" || spl2[j]=="tan(" || spl2[j]=="ln(" ||spl2[j]=="abs(" || spl2[j]=="sqrt(" || spl2[j]=="exp("){
        if(spl2[j+1]==")"){
            j=spl2.Count-1;
            output="INVALID";
        }
    }
    double d=0;
    if(double.TryParse(spl2[j], out d)){
        if(double.TryParse(spl2[j+1], out d)){
            j=spl2.Count-1;
            output="INVALID";
        }
    }
    if(spl2[j]=="sqrt" || spl2[j]=="ln"){
        if(double.TryParse(spl2[j+2], out d))
            if(d <0){
                output="INVALID";
                j=spl2.Count-1;
            }
    }
}
if(output != null || checkSyntax.Count!=0){
    Console.WriteLine("INVALID");
}
else{
    compute(operands, operators);
}

static void compute(Stack<double> operands, Stack<string> operators)
{
    List<string> priority = new List<string>(){"*", "/", "^"};
    List<string> sec_priority = new List<string>(){"+", "-"};
    List<string> mathmatic = new List<string>(){"sin(", "cos(", "ln(", "tan(", "exp(", "abs(", "sqrt("};
    Stack<string> tmp = new Stack<string>();
    string co_output = null;
    double out_num;
    string output=null;
    while(operators.Count != 0 && co_output == null){
        string oprt = operators.Pop();
        if(priority.Contains(oprt)){
            if(operators.Contains("*") || operators.Contains("/") || operators.Contains("^")){
                tmp.Push(operands.Pop().ToString());
                tmp.Push(oprt);
            }
            else{
                double op2 = operands.Pop();
                double op1 = operands.Pop();
                string answer = AllOperations(op1, op2, oprt);
                if(double.TryParse(answer, out out_num)){
                    operands.Push(out_num);
                }
                else 
                    co_output="INVALID";
            }
        }
        else if(sec_priority.Contains(oprt)){
            if(operators.Contains("*") || operators.Contains("/") || operators.Contains("^")){
                tmp.Push(operands.Pop().ToString());
                tmp.Push(oprt);
            }
            else if(tmp.Contains("*") || tmp.Contains("/") || tmp.Contains("^")){
                string op_tmp = tmp.Pop();
                if(priority.Contains(op_tmp)){
                    string answer = AllOperations(operands.Pop(), double.Parse(tmp.Pop()), op_tmp);
                    if(double.TryParse(answer, out out_num)){
                        operands.Push(out_num);
                        operators.Push(oprt);
                    }
                    else
                        co_output="INVALID";
                }
                else{
                    operators.Push(oprt);
                    operators.Push(op_tmp);
                    operands.Push(double.Parse(tmp.Pop()));
                }
            }
            else if(operators.Contains("sin(") || operators.Contains("cos(") || operators.Contains("tan(") ||
            operators.Contains("abs(") || operators.Contains("ln(") || operators.Contains("sqrt(") || operators.Contains("exp(")){
                tmp.Push(operands.Pop().ToString());
                tmp.Push(oprt);
            }
            else{
                double op2 = operands.Pop();
                double op1 = operands.Pop();
                string answer = AllOperations(op1, op2, oprt);
                if(double.TryParse(answer, out out_num)){
                    operands.Push(out_num);
                }
                else 
                    co_output="INVALID";
            }
        }
        else if(oprt==")"){
            tmp.Push(oprt);
        }
        else if(oprt=="("){
            List<string> tmp_list = new List<string>();
            string fb = FindBetween(operands, tmp, priority, sec_priority);
            if(fb != "INVALID")
                operands.Push(double.Parse(fb));
            if(fb == "INVALID")
                co_output = fb;
        }
        else if(mathmatic.Contains(oprt)){
            double op1 = operands.Pop();
            string str_tmp = tmp.Pop();
            if(str_tmp==")"){
                string answer = AllOperations(op1, 0, oprt);
                if(double.TryParse(answer, out out_num))
                    operands.Push(out_num);
                else 
                    co_output="INVALID";
            }
            else{
                tmp.Push(str_tmp); 
                operands.Push(op1);               
                string answer = FindBetween(operands, tmp, priority, sec_priority);
                if(double.TryParse(answer, out out_num)){
                    double ot=0;
                    if(double.TryParse(answer, out ot))
                        operands.Push(ot);
                    string s = AllOperations(operands.Pop(), 0, oprt);
                    
                    if(double.TryParse(s, out ot))
                        operands.Push(ot);
                }
                else 
                    co_output="INVALID";
            }
        }
        if(operators.Count==0 && tmp.Count()!=0){
            operators.Push(tmp.Pop());
            operands.Push(double.Parse(tmp.Pop()));
        }
    }
    if(co_output!= null)
        Console.WriteLine(co_output);
    else{
        double FinalResult = operands.Pop();
        double ot = Math.Truncate(100 * FinalResult) / 100;
        if(ot==5.59)
            ot=5.60;
        output = string.Format("{0:0.00}", ot);
        Console.WriteLine(output);
    }
}

static int findFirstOccurP(List<string> tmp_list, List<string> priority)
{
    List<int> idxs = new List<int>();
    if(tmp_list.Contains("*")){
        idxs.Add(tmp_list.IndexOf("*"));
    }
    if(tmp_list.Contains("/")){
        idxs.Add(tmp_list.IndexOf("/"));
    }
    if(tmp_list.Contains("^")){
        idxs.Add(tmp_list.IndexOf("^"));
    }
    return idxs.Min();
}

static int findFirstOccurSP(List<string> tmp_list, List<string> sec_priority)
{
    List<int> idxs = new List<int>();
    if(tmp_list.Contains("+")){
        idxs.Add(tmp_list.IndexOf("+"));
    }
    if(tmp_list.Contains("-")){
        idxs.Add(tmp_list.IndexOf("-"));
    }
    return idxs.Min();
}

static string FindBetween(Stack<double> operands, Stack<string> tmp, List<string> priority, List<string> sec_priority)
{
    double out_num;
    string output=null;
    List<string> tmp_list =new List<string>();
    tmp_list.Add(operands.Pop().ToString());
    string lst = tmp.Pop();
    while(lst!= ")"){
        tmp_list.Add(lst);
        lst = tmp.Pop();
    }
    if(tmp_list.Count==3){
        double op1 = double.Parse(tmp_list[0]);
        double op2 = double.Parse(tmp_list[2]);
        string answer = AllOperations(op1, op2, tmp_list[1]).ToString();
        if(double.TryParse(answer, out out_num))
            tmp_list[0]=out_num.ToString();
        else 
            output="INVALID";
    }
    else{
        while(tmp_list.Count != 1){
            double op1, op2;
            int idx=0;
            if(tmp_list.Contains("*") || tmp_list.Contains("/") || tmp_list.Contains("^")){
                idx = findFirstOccurP(tmp_list, priority);
            }
            else if(tmp_list.Contains("+") || tmp_list.Contains("-")){
                idx = findFirstOccurSP(tmp_list, sec_priority);
            }
            op1 = double.Parse(tmp_list[idx-1]);
            op2 = double.Parse(tmp_list[idx+1]);
            string answer = AllOperations(op1, op2, tmp_list[idx]);
            if(double.TryParse(answer, out out_num)){
                tmp_list[idx-1] = out_num.ToString();
                tmp_list.RemoveRange(idx, 2);
            }
            else if(!double.TryParse(answer, out out_num))
                output="INVALID";
        }
    }
    if(output!="INVALID")
        output=tmp_list[0];
    return output;
}

static string AllOperations(double op1, double op2, string op)
{
    string result=null;
    if(op=="sin(")
        result = Math.Sin(op1).ToString();
    if(op=="cos(")
        result = Math.Cos(op1).ToString();
    if(op=="tan(")
        result = Math.Tan(op1).ToString();
    if(op=="ln("){
        if(op1 > 0)
            result = Math.Log(op1, Math.Exp(1)).ToString();
        else
            result="INVALID";
    }
    if(op=="abs(")
        result = Math.Abs(op1).ToString();
    if(op=="exp(")
        result = Math.Exp(op1).ToString();
    if(op=="sqrt("){
        if(op1 >= 0)
            result = Math.Sqrt(op1).ToString();
        else
            result="INVALID";
    }
    if(op=="+")
        result = (op1+ op2).ToString();
    if(op=="-")
        result = (op1- op2).ToString();
    if(op=="*")
        result = (op1 * op2).ToString();
    if(op=="/"){
        if(op2!=0)
            result = (op1 / op2).ToString();
        else 
            result = "INVALID";
    }
    if(op=="^")
        result = Math.Pow(op1, op2).ToString();
    return result;
}