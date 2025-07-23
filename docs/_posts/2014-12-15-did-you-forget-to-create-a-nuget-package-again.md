---
layout: post
title: "Did You Forget to Create a Nuget Package Again?"
date: 2014-12-15
---

The biggest problem with managing Nuget packages for internal applications is creating and publishing the correct versions for others to consume.  It's not hard, but it does require manual steps to update the Nuspec file, create the package, and publish it.  For Nuget packages which are only occasionally updated, this probably isn't a problem.  There is adequate time to develop and test it, but when using packages to keep multiple solutions in sync by using the same libraries, and all of them are under active development, it can be easy to miss one of several steps.

Automating the process is relatively easy.  The <a href="http://npe.codeplex.com/" title="Nuget Package Explorer" target="_blank">Nuget Package Explorer</a> is great for creating packages through a GUI, but it doesn't help with automating the process of creating packages after a build.  For that, there is the <a href="https://nuget.codeplex.com/releases/view/58939" title="Nuget Command Line" target="_blank">Nuget Command Line</a>.

When I install it, I put it in the following location: 
```
C:\Utilities\Nuget\
```
  The following scripts assume it will be located here.  If not, simply modify them to point to the correct location. 

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/AutomaticallyCreateNuget/PostBuildEvent.png" alt="Post Build Event" />

In the post build events text box under the **Build Events** tab in the project file place the following code.  It needs to be all on one line with no carriage returns.  


```
Powershell Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope CurrentUser; $(ProjectDir)CreateNugetPackage.ps1 "$(ProjectPath)" "$(ProjectExt)" "$(TargetPath)" "$(TargetName)" "$(ConfigurationName)"
```
  

Create the <a href="https://github.com/kemiller2002/StructuredSight/blob/master/AutomaticallyCreateNuget/CreateNugetExample/CreateNugetPackage.ps1" title="Create Nuget" target="_blank">CreateNugetPackage.ps1</a>:


```

Param(
    [string] $projectPath,
    [string] $projectFileExt,
    [string] $targetPath,
    [string] $targetName,
    [string] $configurationName
)

$nuspecFilePath = "$projectPath.nuspec"

$nuget = "C:\Utilities\Nuget\NuGet.exe"

[xml]$nuspecContent = switch ([System.IO.File]::Exists($nuspecFilePath)) 
{
  $false { 
           Write-Host "nuspec file not found at: $projectPath. Creating"
           & $nuget spec $projectPath | Write-Host 
           [xml]$file = Get-Content $nuspecFilePath

           $metaData = $file.package.metadata 
           $metaData.id = $targetName
              
           $metaData.RemoveChild($metaData.SelectSingleNode("licenseUrl")) | Out-Null
           $metaData.RemoveChild($metaData.SelectSingleNode("projectUrl")) | Out-Null 
           $metaData.RemoveChild($metaData.SelectSingleNode("iconUrl")) | Out-Null 

           $file
          }
    $true   {Get-Content $nuspecFilePath}
    
}

$assembly = [System.Reflection.Assembly]::LoadFile($targetPath)

$assemblyVersion = $assembly.GetName().Version.ToString()

Write-Host "Setting assembly version to: $assemblyVersion"

$nuspecContent.package.metadata.version = $assemblyVersion

$nuspecContent.Save($nuspecFilePath)

& $nuget pack $projectPath -IncludeReferencedProjects -Properties Configuration=$configurationName

```
 

Modify the <a href="http://msdn.microsoft.com/en-us/library/microsoft.visualbasic.applicationservices.assemblyinfo%28v=vs.110%29.aspx" title="Assembly Info" target="_blank">AssemblyInfo.cs</a> file and change the AssemblyVersion to have a <strong>*</strong> to increment the build number (the last zero in the series).  As future versions of the assembly are created in new branches, the other numbers should be changed to reflect the current version.  (2.8.1.* for Major version 2, Minor Version 8, Patch Version 1)

```
[assembly: AssemblyVersion("1.0.0.*")]
```


Now everytime Visual Studio makes a successful build, a new Nuget package will appear in the appropriate folder next to the newly created assembly: 

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/AutomaticallyCreateNuget/ShowSnagItFiles.png" alt="Nugets" />

A full example project with it can be found on <a href="https://github.com/kemiller2002/StructuredSight/tree/master/AutomaticallyCreateNuget" title="example" target="_blank">GitHub at this repo</a>.