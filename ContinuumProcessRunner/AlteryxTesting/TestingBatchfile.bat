@echo off
if %1z==z goto jumpExitFail
if %2z==z goto jumpExitFail

rem We have two parameters, echo them to the screen
echo P1=%1, P2=%2
goto jumpExit

:jumpExitFail
echo This batch file requires two parameters

:jumpExit
echo Finished