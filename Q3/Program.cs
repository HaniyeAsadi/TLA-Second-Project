using System;
using System.Linq;
using System.Collections.Generic;

var transitions = Console.ReadLine().Split(new string[] {"00"}, StringSplitOptions.None).ToList();
int num = int.Parse(Console.ReadLine());
List<List<string>> check_strings = new List<List<string>>();
for(int i=0; i<num; i++){
    var str = Console.ReadLine().Split(new char[]{'0', ' '}, StringSplitOptions.RemoveEmptyEntries).ToList();
    check_strings.Add(str);
}
List<string> states= new List<string>();
List<string> finalStates = new List<string>();
Dictionary<Tuple<string, string>, Tuple<string, string, string>> Machine = new Dictionary<Tuple<string, string>, Tuple<string, string, string>>();
foreach(var tr in transitions){
    var newTr = tr.Split(new char[]{'0'});
    if(!states.Contains(newTr[0]))
        states.Add(newTr[0]);
    if(!states.Contains(newTr[2]))
        states.Add(newTr[2]);
    Tuple<string, string> t1 = new Tuple<string, string>(newTr[0], newTr[1]);
    Tuple<string, string, string> t2 = new Tuple<string, string, string>(newTr[2], newTr[3], newTr[4]);
    Machine.Add(t1, t2);
}
finalStates.Add(new string('1', states.Count));
foreach(var ch_string in check_strings){
    Touring(states, finalStates, Machine, ch_string);
}      

static void Touring(List<string> states, List<string> finalStates, Dictionary<Tuple<string, string>, Tuple<string, string, string>> Machine, List<string> ch_string)
{
    string result = null;
    string currentState = "1";
    List<string> Reached = new List<string>();
    if(ch_string.Count==0){
        Reached.Add(currentState);
        Tuple<string, string> zero = new Tuple<string, string>(currentState, "1");
        while(Machine.ContainsKey(zero)){
            currentState = Machine[zero].Item1;
            Reached.Add(currentState);
            zero = new Tuple<string, string>(currentState, "1");
        }
        result="Rejected";
        foreach(var r in Reached){
            if(finalStates.Contains(r)){
                result = "Accepted";
                break;
            }
        }
    }
    else{
        int i=0;
        while(i<ch_string.Count){
            var ikey = new Tuple<string, string>(currentState, ch_string[i]);
            if(Machine.ContainsKey(ikey)){
                var ivalue = Machine[ikey];
                currentState = ivalue.Item1;
                ch_string[i] = ivalue.Item2;
                if(ivalue.Item3 == "1")
                    i-=2;
            }
            else if(!Machine.ContainsKey(ikey)){
                result = "Rejected";
                i=ch_string.Count;
            }
            if(i<0){
                for(int j=0; j<Math.Abs(i); j++){
                    ch_string.Insert(0, "1");
                }
                i=0;
            }
            i++;
        }
        Reached.Add(currentState);
        var rch = new Tuple<string, string>(currentState, "1");
        while(Machine.ContainsKey(rch) && result == null){
            currentState = Machine[rch].Item1;
            Reached.Add(currentState);
            rch = new Tuple<string, string>(currentState,"1");
        }
        result = "Rejected";
        foreach(var r in Reached){
            if(finalStates.Contains(r))
                result = "Accepted";
        }
    }
    Console.WriteLine(result);
}