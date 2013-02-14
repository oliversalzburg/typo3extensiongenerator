@echo off
if "%PROCESSOR_ARCHITECTURE%" == "x86" (
	call "C:\Program Files\Microsoft Visual Studio 11.0\VC\vcvarsall.bat"
) else (
	call "C:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\vcvarsall.bat"
)
MsBuild %~dp0\Typo3ExtensionGenerator.sln /t:Rebuild /p:Configuration=Release /p:Platform="Any CPU"