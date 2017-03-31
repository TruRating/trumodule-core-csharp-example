@echo off
call .nuget\nuget.exe restore TruRating.TruModule.V2xx.sln
call %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe TruRating.TruModule.V2xx.sln /p:Configuration=Release /p:Platform="Any CPU"
ECHO ====================================================
ECHO Built to TruRating.TruModule.V2xx.ConsoleRunner\bin\Release\
ECHO ====================================================
TruRating.TruModule.V2xx.ConsoleRunner\bin\Release\TruRating.TruModule.V2xx.ConsoleRunner.exe
pause