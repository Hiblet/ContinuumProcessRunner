# ContinuumProcessRunner

A Plug-in Tool for Alteryx to run processes in a shell with parameters (such as Python.exe with your Python script) and return the data and errors back to your Alteryx workflow.


## Intro

An "executable" is a program, and it usually is held in a file with a ".EXE" extension.  Many executables can be run from DOS or Windows.Start.Run, and you can usually pass in data to the executable by putting strings after the program filename, and these strings are called command line arguments.

This tool will take the path to an executable file (directory and filename), and any additional command line arguments, and run the exe in a shell.  The argument or parameters you pass will be the data in your selected Alteryx columns, in column order.  The names of the columns do not matter, only the relative order is important.

The data returned is whatever the executable writes to it's Standard Output stream, which is usually what the executable prints to the command line.

When any executable completes, it sends back a return code.  This is usually 0 for success, and other numbers as failure codes, and this return code is reported directly back to your workflow, to a new column.

Also, if the spawned process errors or throws an exception, the errors are trapped and directly reported to another new column.  This is achieved by monitoring what the executable writes to the Standard Error output stream.

A Diagnostics column is added to the results to show the actual command that was used to spawn the process.  This is because many executable processes are extremely inflexible in how they process command line variables, and you, the user, need to be able to see what data you are passing, including formatting.  

The tool has the option to "Auto Escape" your parameters.  For example, if a column has some a data string that contains a space, the auto escape function will put quotes around the string on the command line so that the string is passed as a single string.  If the Auto Escape function is turned off, the string will be passed exactly as you have it in the data field, with no quotes, and the command line executable will probably interpret the data as two distinct strings.  The Diagnostic field will show you what is being passed on the command line, to allow you to tailor your data to the exact requirements of the executable.

Each Alteryx row can refer to a different executable, so you can run either different versions of programs (as in different Python versions), or completely distinct, unrelated executables, from the same table data.  Each executable must complete before the next is called, so you can chain calls to a series of different executables together.


### Why Would I Use This Thing?

If you have existing Python, Ruby, Perl, DOS Batch or Powershell scripts, you can integrate these directly into your workflows.  If these scripts take command line parameters, there is effectively nothing you need to do to these scripts to adapt them.

If you have multiple different command line apps that are currently manually executed, you can build a flow to puppet them.  Additionally, you can build logic into your workflow and chain ProcessRunner tools together.  So you can do StepA (Python), check the output, then run StepB (Perl), C (Powershell) and D (Ruby), and then email the output of StepE back to someone.  Basically, you have all the awesome power of Alteryx, and seamless integration to your legacy command line utilities or interpreted language code.

Alteryx does have a command line adapter, but it often requires you to output data to file, then adapt your command line util to pick up the file.  Then your command line util has to output a file, and Alteryx has to pick up that output file.  It can be made to work, but it is brittle and inelegant.  You also do not get any errors in your utils coming back to the flow, or the return code, so if something goes wrong, debugging is hard.

This tool effectively gives you the Windows.Start.Run or DOS (CommandLine) functionality in your workflow.  Anything you can trigger from Windows.Start.Run, you can run from within Alteryx.  If you have your own custom code, or legacy utilities that do a particular job, you can integrate them directly into your workflows.

This is awesome.  Why would you not use this thing?




### History

Alteryx has a tool to shell out to DOS and run some command line stuff, but it is a little clunky, and getting data into and out of the shell can be problematic.

I was looking for a way to shell out and run Python scripts.  To do this, you run Python.Exe, and pass the executable the script name as the first parameter, and any command line arguments as subsequent parameters.  Once I got that working, it occurred to me that many other command line apps work exactly the same way, they have an exe file name, and they take command line arguments.  If I allowed the user to pass in the executable name, I could pass the Alteryx column data as the command line arguments, and I would have a generalised process runner, rather than just a Python adapter.

Theoretically, Windows does not really distinguish between DOS and Windows apps too much, so I am thinking that you could pass a Windows executable path, and if that Windows app ran on command line arguments, and completed without interaction, it would work the same way.  I have not tried this though...

The tool is written in C# using DotNet 4.6.1, targeting x64 platform.


### Tech Notes

Shelling out to another process is difficult.  Really difficult.  One of the main problems is that your calling program (Alteryx) must listen to both the output stream, and the error stream.  If your calling program listens synchronously, you are exposed to the risk of a deadlock, ie boom, tears.

This tool avoids the problem by using asynchronous events to listen to the output and error streams.  That means it will return both the output, and the errors, and the exit code.  The tool can also handle multiple outputs, where the spawned process prints multiple lines back to the console over time.

The input parameters are passed as command line arguments to the executable, in Alteryx column order, as Strings.  You can select the columns to pass to the command line, and all other columns are ignored and just pass through the tool.

The output, error and diagnostic columns are currently sized at 1073741823 characters (Int32.MaxValue bytes, 2 bytes per char), and the return code is a generous 256 characters, considering that the code should be a single numeric digit.



## Installation

These are placeholder notes;  You should be provided in the release with a DLL, and an INI file.


### Admin Installations (in C:\Program Files\Alteryx)

The DLL file should be copied to...

```
    C:\Program Files\Alteryx\bin\Plugins\ContinuumProcessRunner
```
    
The INI file should be copied to...

```
    C:\Program Files\Alteryx\Settings\AdditionalPlugins
```    

### Non-Admin Installations 

Wherever Alteryx is installed, it should have a "bin" folder beneath it, containing the AlteryxGui.exe file.
If you find this, you need to copy the DLL to a directory location like...

```
    Alteryx\bin\Plugins\ContinuumProcessRunner
```
    
The INI file should be copied to... 
    
```    
    Alteryx\Settings\AdditionalPlugins    
```


### All Installations: Check the INI file
    
Finally, and crucially, you must check and possibly edit the INI file (in a text editor like Notepad) and change the path to be correct for your installation.

If the DLL loads, but the INI file is incorrect, you should still be able to find the tool via the Search bar in Alteryx Designer.

If the DLL file is in the wrong place, you will not be able to find the tool in the Alteryx Designer Search bar.


### All Installations: Unblocking

A DLL file is considered dangerous by Windows, if it does not originate on your own machine.  This means Windows blocks the DLL from being used, even if it is in the right folder.

To manually unblock the DLL, find the DLL file using Windows FileExplorer, in the Alteryx\bin\Plugins\ContinuumProcessRunner folder that you copied it to in the step above.  Right-click the file and select Properties, and on the General tab (first tab) you should see, at the bottom, an Unblock option.  Click that to unblock the DLL.



## Usage

The tool requires you to select the text column that contains the path to the executable.  This should include the file extension.

This can be a relative path definition, but the path may be relative to where the plug-in is running (C:\Program Files\Alteryx\bin\Plugins\ContinuumProcessRunner) rather than where the workflow is saved.  To be safe, just use a full path, from the root of C: downwards (or D:, whatever).

Next is the SelectedCols box, and here you tick the box for every column that you want to pass to your command line executable.  The arguments are passed as strings, in the order of that the columns appear in the Alteryx row.  Data is by default automatically escaped, so if you have a column with the string...

```
    My Data
```

...this is passed on the command line in quotes like this...

```
    c:\users\user\myProgram.exe "My Data"
```
... because if it was passed literally, the receiving executable would process it as two distinct text parameters, "My" and "Data".  Unchecking the Auto Escape box turns off this feature, if required.


## Output

The tool has four output columns, "Std Out", "Return Code", "Exceptions" and "Diagnostics", which you can rename.

"Std Out" is the column that will receive the printed output from your spawned process.

"Return Code" is the column that will get whatever your executable returns to command line on exit.  Usually this is a zero for success, and a non-zero integer for failure.

"Exceptions" is the column that will receive whatever the application returns on the Standard Error stream, which is where most exceptions and errors will be sent.

"Diagnostics" is a column that shows how the spawn command line string looked when the executable was called.  If the executable did not function as you expected, you can examine the call and see if the data passed on the command line is what you expected.  If necessary, you can munge the data in Alteryx to shape it correctly, and if necessary turn off the Auto Escape feature, and take full control of what is applied to the command line.  If you are having great difficulty, you can pass a single column, turn off auto escaping, and shape a single string to be your command line parameters in Alteryx.  As usual with Alteryx, there are many ways to skin the cat.

Executables are called synchronously (one at a time) in row order, and must complete before the next row is called.  If the executable is bugged and loops forever, the workflow will stall at that executable indefinitely.

Columns that are not the executable path are sent to the command line as parameters, in column order.

I would suggest making the executable the first column.  If the exe runs a script, make the script path the second column, and then all subsequent columns can be your command line input data.  


## Diagnostics

There are two features that will help you get ProcessRunner working with your selected executables.

The first is Auto Escape, which is defaulted to true, checked.  This makes the tool escape your command line arguments by default.  However, you may want to have more control and turn off this feature, so that you pass exactly what you have in your flow data cells to your command line executable.  To turn off auto escaping, uncheck the Auto Escape checkbox, and then the tool will not put quotes around strings that have spaces.

The second is the Diagnostics column.  As mentioned above, this shows the string that the tool uses to spawn the executable.  You should be able to take the string, copy it, and manually call the executable from the DOS prompt, and get the exact behaviour that you see in ProcessRunner.


### Example A: Python Scripts

You can run the Python executable, with a script, and pass data as command line parameters.  Anything the script prints is passed back as data.  

Below are some example input data records:

```
---------------------------------------------------------------------
|ExecutablePath          |ScriptPath                |Name      |Age |
---------------------------------------------------------------------
|C:/Python37/python.exe  |C:/MyScripts/Test1.py     |Steve     |  50|
|C:/Python26/python.exe  |C:/OldScripts/Test2.py    |Alice     |  21|
|C:/Python37/python.exe  |C:/MyScripts/Test3.py     |Bob Smith |  32|
---------------------------------------------------------------------
```

Let us assume that the Test1, Test2 and Test3 scripts are all the same, and just report the passed in parameters, like this:

```
import sys
print ("This is the name of the script: ", sys.argv[0])
print ("Number of arguments: ", len(sys.argv))
print ("The arguments are: " , str(sys.argv))
```

The tool would be configured such that the Exe Path drop down box is pointed to column "ExecutablePath", and we will leave the output columns with their default names.

The result of running this data through the tool would be this:

```
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
|ExecutablePath          |ScriptPath                |Name      |Age |ProcessRunnerStdOut|ProcessRunnerReturnCode|ProcessRunnerException|ProcessRunnerDiagnostics                                   |
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
|C:/Python37/python.exe  |C:/MyScripts/Test1.py     |Steve     |  50|This is the name...|0                      |                      |C:/Python37/python.exe C:/MyScripts/Test1.py Steve 50      |
|C:/Python26/python.exe  |C:/OldScripts/Test2.py    |Alice     |  21|This is the name...|0                      |                      |C:/Python26/python.exe C:/OldScripts/Test2.py Alice 21     |
|C:/Python37/python.exe  |C:/MyScripts/Test3.py     |Bob Smith |  32|This is the name...|0                      |                      |C:/Python37/python.exe C:/MyScripts/Test3.py "Bob Smith" 32|
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
```

The ProcessRunnerStdOut column would have a red triangle tag indicating that the cell has embedded returns within it.

If we cut and paste the contents of a cell, we would see this:

```
This is the name of the script:  C:/MyScripts/Test1.py
Number of arguments:  3
The arguments are:  ['C:/MyScripts/Test1.py', 'Steve', '50']
```

This tells us that the Python executable successfully fired up the script as the first argument.  This worked, because the ScriptPath column was the first column in the record that was not the Executable.

The script ran and reported the passed in script name (argv[0]), and the number of variables received, and the actual variables received on the command line.
The three print statements in the script resulted in 3 separate lines in the output data cell.

For the third row, "Bob Smith" is the name, and this has a space in it.  The tool detects the space, and quotes the string, so that only 3 variables are passed; The script, and the two parameters.  
Note that if Auto Escape is turned off, the quotes would not be applied to the name, and the executable would be called with the script and three parameters [Bob,Smith,32].

If one of the scripts raises an exception, the ProcessRunnerReturnCode would be "1", and the ProcessRunnerException cell would contain the exception received.  ProcessRunnerStdOut would contain all data received up to the exception, so any print statements executed before failure would be passed through.



### Example B: Batch Files

You can create a DOS batch file, and run CMD.EXE to run it.  Data from your flow is passed as command line parameters, and anything the batch file echo's back is returned as data.

Below is an example input data record:

```
------------------------------------------------------------------------------------------------------
|ExecutablePath              |Parameter1 |Parameter2                                     |Parameter3 |
------------------------------------------------------------------------------------------------------
|C:\Windows\System32\cmd.exe |/c         |C:\Users\user\documents\batchfiles\mybatch.bat |Testing!   |
------------------------------------------------------------------------------------------------------
```

Let us assume that the batch file just echo's back the passed in parameter if one is passed, and just exits if not.  
You could of course use multiple parameters.  Here's the batch file...

```
@echo off
if %1z==z goto jumpExit
rem We have received a command line parameter...
echo Parameter 1:%1
:jumpExit
echo Finished.
```

The tool would be configured such that the Exe Path drop down box is pointed to column "ExecutablePath".  

The second column "/c" is the first parameter passed to CMD.EXE and tells CMD.EXE that the thing that follows is the command.  
The thing that follows "/c" is the name of the batch file to run, and the final parameter is the first parameter passed to the batch file.

The result of running this data through the tool would be this:

```
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
|ExecutablePath              |Parameter1 |Parameter2                                     |Parameter3 |ProcessRunnerStdOut  |ProcessRunnerReturnCode |ProcessRunnerException |ProcessRunnerDiagnostics                                                              |
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
|C:\Windows\System32\cmd.exe |/c         |C:\Users\user\documents\batchfiles\mybatch.bat |Testing!   |Parameter 1:Testing! |0                       |                       |C:\Windows\System32\cmd.exe /c C:\Users\user\documents\batchfiles\mybatch.bat Testing!|
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
```

The ProcessRunnerStdOut column would have a red triangle tag indicating that the cell has embedded returns within it.

If we cut and paste the contents of a cell, we would see this:

```
Parameter 1:Testing!
Finished.
```

Everything echo'ed back by the batch file is returned to the flow.  This tells us that the batch file ran, and received "Testing!" as it's first parameter.  The CMD.EXE exited gracefully and returned 0 (no error) and the Exception column is empty, because we had no errors.



### Example C: Powershell

PowerShell is like DOS, with added Windows and DotNet.  Your machine needs to be permissioned to run scripts, and once you can run scripts, you can automate them and pass them data from Alteryx.  Whatever the script writes back to the output is passed back to Alteryx.

Below is an example input data record:

```
-------------------------------------------------------------------------------------------------------------------------
|ExecutablePath                                            |Parameter1                                  |Param2 |Param3 |
-------------------------------------------------------------------------------------------------------------------------
|C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe |C:\users\user\documents\powershell\myPS.ps1 |Alpha  |Beta   |
-------------------------------------------------------------------------------------------------------------------------
```

Let us assume that the Powershell script just writes the parameters it receives back to the console.  Note that in Powershell, the output stream can be used to pipe out data to subsequent processes, and ProcessRunner will pick up both the stuff you write to the screen (Write-Host) and the stuff that is piped out.

You could of course use any number of parameters.  Here's the script file...

```
Write-Host "Hello World! From Powershell!"
Write-Host "Arguments:"$($args.count)
$args
```

The tool would be configured such that the Exe Path drop down box is pointed to column "ExecutablePath".  

The result of running this data through the tool would be this:

```
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
|ExecutablePath                                            |Parameter1                                  |Param2 |Param3 |ProcessRunnerStdOut           |ProcessRunnerReturnCode |ProcessRunnerException |ProcessRunnerDiagnostics                                                                                        |
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
|C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe |C:\users\user\documents\powershell\myPS.ps1 |Alpha  |Beta   |Hello World! From Powershell! |0                       |                       |C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe C:\users\user\documents\powershell\myPS.ps1 Alpha Beta|
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
```

The ProcessRunnerStdOut column would have a red triangle tag indicating that the cell has embedded returns within it.

If we cut and paste the contents of a cell, we would see this:

```
Hello World! From Powershell!
Arguments: 2
Alpha
Beta
```

This shows us that the script received two arguments, as expected.  The process exited gracefully, and there were no exceptions.


## Known Issues

### Macro Unfriendliness

There is currently a problem when the ProcessRunner tool is wrapped in a macro.  This is under active investigation.
The tool appears to run correctly, but a Browse tool downstream of ProcessRunner errors.


## Contact

Questions, feedback and abuse can be sent to steve at continuum dot je.


## Credit

Most of the serious code that spawns the process and ensures it does not deadlock was written by Roger Knapp, who also goes by the name "CSharpTest.Net".  I have effectively wrapped up his code and altered it as minimally as possible, to make it an Alteryx plug in.


## License

The code from Roger Knapp is provided under Apache License 2.0 .  This license allows the use of code for commercial purposes, provided the code makes no attempt to represent the Apache Foundation or Apache products.

This product is therefore the private commercial intellectual property of Continuum Jersey.


## Update History

### The tool should have a version code on the configuration screen, next to the name.  This section details the changes relative to each version.

#### v1.0.0 
Alpha Release

#### v1.0.1
Beta; Max field size for StdOut and Exception output fields increased from 65535 to 2Gb, allowing 1 billion 2-byte chars.

#### v1.0.2
Beta; Use StringBuilder to build output to improve high-load performance.  Prior to this change, 10000 records output took 33.7 secs, after this change, 8.9 secs.  Vrrroooom.

#### v1.0.3
Beta; Default working directory for the spawned process changed from this.WorkingDirectory to Environment.SpecialFolder.Personal .
The previous working directory would be wherever the DLL ran from, which could be Admin restricted.
The new working directory is the users MyDocuments folder, which the user should have full rights to.

#### v1.0.4
Beta; Use StringBuilder to build exceptions string, similar to change from v1.0.1 to v1.0.2

#### v1.0.5
Beta; Added secret options to turn Diagnostics on/off, and switch for autoEscape on/off, set from tool's XML config.

#### v1.0.6
Beta; Added SelectedCols control, to selectively pass columns to the command line.

#### v1.0.7
Beta; Record copy process updated to handle non-string types, such as Blob type.

#### v1.0.8
Beta; Auto Escape and Diagnostics features made accessible via configuration screen.

#### v1.0.9
Beta; Internal tidying of variable names.  Additional info returned in debug version.