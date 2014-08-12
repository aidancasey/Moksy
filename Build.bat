@ECHO OFF
msbuild Build\BumpVersion.vproj /t:VersionArtifacts
if ERRORLEVEL 1 GOTO :E

msbuild Build\VersionAssemblies.vproj /t:VersionArtifacts
if ERRORLEVEL 1 GOTO :E

REM Run this under a VS2012 Command Prompt to build.
msbuild.exe Moksy.sln /t:Rebuild /p:Configuration=Release
if ERRORLEVEL 1 GOTO :E

IF EXIST "NugetStage" (
    DEL NugetStage /S /Q
)

MKDIR NugetStage
MKDIR NugetStage\content
MKDIR NugetStage\tools
MKDIR NugetStage\lib
MKDIR NugetStage\lib\net40
COPY Moksy.Host\bin\Release\Moksy*.dll NugetStage\lib\net40\
COPY Moksy.Host\bin\Release\Moksy*.exe NugetStage\lib\net40\
COPY Moksy.Host\bin\Release\Moksy*.exe.config NugetStage\content\
COPY Install.ps1 NugetStage\tools

COPY Package.nuspec NugetStage\Moksy.nuspec
COPY License.txt NugetStage\License.txt

CD NugetStage

..\nuget pack Moksy.nuspec

GOTO :DONE

:E
ECHO ----------------------------------------------------------------
ECHO You must run this batch file under the Developer Command Prompt. 
ECHO ----------------------------------------------------------------

:DONE
