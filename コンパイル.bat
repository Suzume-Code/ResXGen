@ECHO OFF

SET OBJ=ResXGen.exe

TASKKILL /F /IM %OBJ%

SET EXE="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
SET REF="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF"

SET TYP=/t:exe
SET ICO=-win32icon:app.ico

SET DLL=/r:%REF%\PresentationCore.dll
SET DLL=%DLL% /r:%REF%\PresentationFramework.dll
SET DLL=%DLL% /r:%REF%\WindowsBase.dll

SET OUT=/out:%OBJ%

SET RES=

%EXE% %ICO% %TYP% %OUT% %DLL% %RES% AssemblyInfo.cs ResxGen.cs

IF ERRORLEVEL 1 (
  ECHO  ************************
  ECHO  *** コンパイルエラー ***
  ECHO  ************************
  PAUSE
)
