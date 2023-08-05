# TLA Second Project
In this repository I have 3 different projects for TLA(Theory of Language and Automata course) that are implemented in C#.  

In the first question (Q1 file), we try to get a grammer as input and a string to check. Then by applying grammer rules try to find if the string is acceptable or not by the grammar. If the input string is accepted, then print "Accpected", and if not, print "Rejected".  
Input format is as follows:  
1. In the first line, we get an integer of n that determines the number of variables in the grammar.  
2. In the next n lines, in each line, we get a rule of that grammar. Variables are wrapped by <>, # stands for nullablle transition and different rules separated by |.  
3. In the last line, we get a string to check.  

One pair od input and output:
Input:  
3  
&lt;S&gt; -> a&lt;S&gt;b | a&lt;A&gt; | b&lt;B&gt;  
&lt;A&gt; -> a&lt;A&gt; | #  
&lt;B&gt; -> b&lt;B&gt; | #   
aaab  
Output:  
Accepted  