REM Build framework-dependant for all distributions.
set target=win-x64
set exe=.exe
set sc=false
set sc-text=
goto build
:win-x64

set target=win-x86
set exe=.exe
set sc=false
set sc-text=
goto build
:win-x86

set target=win-arm
set exe=.exe
set sc=false
set sc-text=
goto build
:win-arm

set target=win-arm64
set exe=.exe
set sc=false
set sc-text=
goto build
:win-arm64

set target=linux-x64
set exe=
set sc=false
set sc-text=
goto build
:linux-x64

set target=linux-arm
set exe=
set sc=false
set sc-text=
goto build
:linux-arm

set target=linux-arm64
set exe=
set sc=false
set sc-text=
goto build
:linux-arm64

set target=osx-x64
set exe=
set sc=false
set sc-text=
goto build
:osx-x64

REM Build framework-independant for all distributions.
set target=win-x64
set exe=.exe
set sc=true
set sc-text=-self-contained
goto build
:win-x64-self-contained

set target=win-x86
set exe=.exe
set sc=true
set sc-text=-self-contained
goto build
:win-x86-self-contained

set target=win-arm
set exe=.exe
set sc=true
set sc-text=-self-contained
goto build
:win-arm-self-contained

set target=win-arm64
set exe=.exe
set sc=true
set sc-text=-self-contained
goto build
:win-arm64-self-contained

set target=linux-x64
set exe=
set sc=true
set sc-text=-self-contained
goto build
:linux-x64-self-contained

set target=linux-arm
set exe=
set sc=true
set sc-text=-self-contained
goto build
:linux-arm-self-contained

set target=linux-arm64
set exe=
set sc=true
set sc-text=-self-contained
goto build
:linux-arm64-self-contained

set target=osx-x64
set exe=
set sc=true
set sc-text=-self-contained
goto build
:osx-x64-self-contained

REM Skip the build function.
goto end

REM Build function.
:build
dotnet publish --configuration Release --runtime %target% --output bin/zipped/%target% --self-contained %sc% -p:DebugType=None -p:PublishSingleFile=true
del bin\zipped\%target%%sc-text%.zip
7z a bin/zipped/%target%%sc-text%.zip ./bin/zipped/%target%/*
rmdir bin\zipped\%target% /s /q
goto %target%%sc-text%

REM Show screen after finishing.
:end
pause