@ECHO OFF
REM Quick package script
REM Oliver Salzburg 2013
SETLOCAL

REM Get folder name
FOR %%f in (%CD%) do set foldername=%%~nxf
FOR /F "delims=" %%a IN ('..\tool\tolower %foldername%') DO @SET foldername=%%a

REM Get date and time
FOR /F "tokens=1 delims= " %%a IN ('date /t') DO (set mydate=%%a)
FOR /F "tokens=1-2 delims=/:" %%a IN ('time /t') DO (set mytime=%%a%%b)

cd bin\Release
del *.pdb
..\..\..\tool\7za a -r -tzip "..\..\%foldername%-%mydate%-%mytime%.zip" *

IF EXIST build-post.cmd CALL build-post.cmd
ENDLOCAL