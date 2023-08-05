using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

string pattern = @"(<[A-Z]>)|([a-z])";
Regex rg = new Regex(pattern);
List<string> Variables= new List<string>();
int number = int.Parse(Console.ReadLine());
Dictionary<string, List<List<string>>> Grammer = new Dictionary<string, List<List<string>>>();
for(int i = 0; i<number; i++){
    List<string> input = Console.ReadLine().Split(new string[]{"->"}, StringSplitOptions.RemoveEmptyEntries).ToList();
    Variables.Add(input[0].Trim());
    List<string> rightHand = input[1].Split(new char[]{'|', ' '}, StringSplitOptions.RemoveEmptyEntries).ToList();
    List<List<string>> listvalue = new List<List<string>>();
    foreach(var rh in rightHand){
        var val = rg.Split(rh).ToList();
        int j=0;
        while(j<val.Count){
            if(val[j]==""){
                val.Remove(val[j]);
            }
            else 
                j++;
        }
        listvalue.Add(val);
    }
    Grammer.Add(input[0].Trim(), listvalue);
}

string checkstring = Console.ReadLine();
Dictionary<string, List<List<string>>> NewGrammer = RemoveNullables(Grammer);
string startVariable = NewGrammer.ElementAt(0).Key;
List<List<string>> startingList = NewGrammer[startVariable];
Pumping(startingList, NewGrammer, Variables, checkstring);

static void Pumping(List<List<string>> startingList, Dictionary<string, List<List<string>>> newGrammer, List<string> variables, string checkstring)
{
    string result = "Rejected";
    List<List<string>> nextiteration = new List<List<string>>();
    nextiteration.AddRange(startingList);
    while(result == "Rejected" && nextiteration.Count !=0){
        List<List<string>> nextList = new List<List<string>>();
        foreach(var nxt in nextiteration){
            if(result == "Rejected"){
                List<List<string>> check = useProduction(nxt, newGrammer, variables);
                for(int i=0; i<check.Count; i++){
                    bool hasVar = check[i].Any(x => variables.Any(y => y==x));
                    if(!hasVar){
                        string str = String.Join("", check[i]);
                        if(str == checkstring){
                            result="Accepted";
                            i=check.Count;
                            break;
                        }
                    }
                    else if(hasVar){
                        if(check[i].Count <= checkstring.Length){
                            nextList.Add(check[i]);
                        }
                    }
                }
            }
        }
        nextiteration.Clear();
        nextiteration.AddRange(nextList);
    }
    Console.WriteLine(result);
}
static List<List<string>> useProduction(List<string> currList, Dictionary<string, List<List<string>>> newGrammer, List<string> variables)
{
    List<List<string>> afterUsePros = new List<List<string>>();
    int useVar=0, len=currList.Count;
    for(int i=0; i< len; i++){
        if(!variables.Contains(currList[i])){
            if(afterUsePros.Count ==0){
                List<string> m = new List<string>();
                m.Add(currList[i]);
                afterUsePros.Add(m);
            }
            else{
                foreach(var up in afterUsePros){
                    up.Add(currList[i]);
                }
            }
        }
        else if(useVar==0){
            useVar=1;
            if(afterUsePros.Count == 0){
                foreach (var cl in newGrammer[currList[i]]){
                    List<string> cl_tmp = new List<string>();
                    foreach(var clt in cl){
                        cl_tmp.Add(clt);
                    }
                    afterUsePros.Add(cl_tmp);
                }
            }
            else{
                List<string> tmp = afterUsePros[0];
                afterUsePros.Remove(tmp);
                foreach (var cl in newGrammer[currList[i]]){
                    List<string> addList = new List<string>();
                    addList.AddRange(tmp);
                    List<string> tmp2 = new List<string>();
                    tmp2.AddRange(cl);
                    addList.AddRange(tmp2);
                    afterUsePros.Add(addList);
                }
            }
        }
        else{
            foreach (var up in afterUsePros){
                List<string> up2 = new List<string>();
                up2.AddRange(up);
                up2.Add(currList[i]);
                up.Clear();
                up.AddRange(up2);
            }
        }
    }
    return afterUsePros;
}

static Dictionary<string, List<List<string>>> RemoveNullables(Dictionary<string, List<List<string>>> grammer)
{
    List<string> nullable = new List<string>(){"#"};
    int s1=0;
    while(s1 < grammer.Count){
        int s2=0;
        string y = grammer.ElementAt(s1).Key;
        while(s2< grammer.ElementAt(s1).Value.Count-1){
            List<List<string>> v = grammer[y];
            int s3=0;
            while(s3<v.Count){
                var set = new HashSet<string>(v[s3]);
                var equals = set.SetEquals(nullable);
                if(equals){
                    grammer[y].Remove(v[s3]);
                    foreach(var n in grammer){
                        int k=0;
                        while(k<n.Value.Count){
                            List<string> m = n.Value[k];
                            if(m.Contains(y)){
                                int idx =m.IndexOf(y);
                                List<string> tmp = new List<string>();
                                tmp.AddRange(m);
                                tmp.RemoveAt(idx);
                                n.Value.Add(tmp);
                            }
                            k++;
                        }
                    }
                }
                s3++;
            }
            s2++;
        }
        s1++;
    }
    return grammer;
}
