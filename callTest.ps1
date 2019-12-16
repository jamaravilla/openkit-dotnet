param([string]$option = "Release")

$workspace = (Get-Location).toString()

if ("$env:APPVEYOR" -ieq "true") {
    $format = "AppVeyor"
    $outputDir = $workspace
    $nunit3_console = "nunit3-console"
} else {
    $format = "nunit3"
    $outputDir = "$env:OUTPUTDIR"
    $nunit3_console = "$env:USERPROFILE\.nuget\packages\nunit.consolerunner\3.8.0\tools\nunit3-console.exe"
}

if ($option -eq "Release") {
    $testAssemblies = Get-ChildItem -Recurse -Include openkit-dotnetfull-*Tests.dll, openkit-dotnetstandard-*Tests.dll, openkit-dotnetpcl-*Tests.dll | ? { $_.FullName -match "\\bin\\Release\\" } | % FullName
    & $nunit3_console --result="$outputDir\myresults.xml;format=$format" $testAssemblies

    # Run .NET Core tests
    $testProjects = Get-ChildItem -Recurse -Include openkit-dotnetcore-*Tests.csproj | % FullName
    foreach ($project in $testProjects) {
        & dotnet.exe test -c Release $project --results-directory $outputDir --logger trx --no-build
    }
} elseif ($option -eq "Coverage") {
    $openCoverConsole = "$env:USERPROFILE\.nuget\packages\opencover\4.6.519\tools\OpenCover.Console.exe"
    # Run .NET test assemblies, excluding .NET Core
    $testAssemblies = Get-ChildItem -Recurse -Include openkit-dotnetfull-*Tests.dll, openkit-dotnetstandard-*Tests.dll, openkit-dotnetpcl-*Tests.dll | ? { $_.FullName -match "\\bin\\Coverage\\" } | % FullName 
    & $openCoverConsole "-target:$nunit3_console" "-targetargs:--result=$outputDir\myresults.xml;format=$format $testAssemblies" -register:user -filter:"+[*]* -[*.Tests]*" -output:$outputDir\coverage.xml

    # Run .NET Core tests
    $testProjects = Get-ChildItem -Recurse -Include openkit-dotnetcore-*Tests.csproj | % FullName
    foreach ($project in $testProjects) {
        & $openCoverConsole "-target:dotnet.exe" -targetargs:"test -c Coverage $project --no-build" -register:user  -filter:"+[*]* -[*.Tests]*" -oldstyle -output:$outputDir\coverage.xml -mergeoutput
    }
}


