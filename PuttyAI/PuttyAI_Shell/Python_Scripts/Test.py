import sys

# Python3 program to convert a list 
# of integers into a single integer 
def convert(list): 
      
    # Converting integer list to string list 
    # and joining the list using join() 
    res = str("".join(map(str, list))) 
      
    return res 

def Simple():
    hello_text = "Hello from Python" + "\n"
    Call_Dir_Text = "Call Dir(): "
    Call_Dir_Text += convert(dir())
    Call_Dir_Text += "\n";
    SysPath = "Print the Path: " 
    SysPath += convert(sys.path)
    SysPath += "\n";

    return hello_text + Call_Dir_Text + SysPath;