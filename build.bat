@echo off
call .nuget\nuget.exe restore TruRating.TruModule.sln
call %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe TruRating.TruModule.sln /p:Configuration=Release /p:Platform="Any CPU"
ECHO ====================================================
ECHO Built to TruRating.TruModule.ConsoleRunner\bin\Release\
ECHO ====================================================
TruRating.TruModule.ConsoleRunner\bin\Release\TruRating.TruModule.ConsoleRunner.exe
pause