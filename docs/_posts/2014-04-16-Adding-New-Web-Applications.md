---
layout: post
title: "Adding New Web Applications"
date: 2014-04-16 00:00:00 -0500
---
Using PowerShell to create new web applications with the <a href="http://www.iis.net/downloads/microsoft/powershell" title="PowerShell Snap-In" target="_blank">IIS PowerShell Snap-In</a> is incredibly easy.  Just use <a href="http://technet.microsoft.com/en-us/library/ee807831.aspx" title="New Web Application Link" target="_blank">New-WebApplication</a> and specify
<ul>
    <li>Site : Site to put it under</li>
    <li>Physical Path : The directory path to the folder where the application will reside</li>
    <li>Application Pool : The application pool running the web application</li>
    <li>Name : The name of the application (what appears in the URL)</li>
</ul>


```

New-WebApplication -Site "Default Web Site" `  
-PhysicalPath "C:\webApplications\" ` 
-ApplicationPool "DefaultAppPool" `
-Name "NewWebApplication" 

```


The physical path, application pool and site must be in place before running the cmdlet.  It won't create them automatically and will fail the install if they aren't present.  Conveniently, there are cmdlets to handle creating these as well:   

Creating a new directory: 

```
new-item "C:\webApplications" -ItemType directory
#or
mkdir C:\webApplications
```



Creating an application pool: 

```
New-WebAppPool -Name "My New Application Pool"
```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/CreatingWebApplications/Images/NewAppPoolPowerShell.jpg" alt="Ne App Pool PowerShell" />

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/CreatingWebApplications/Images/NewAppPoolIIS.jpg" alt="App Pool IIS" />

Creating a new site:

```
New-Website -Name "My Web Site" `
-id 22 `
-Port 8080 `
-HostHeader "kmmiller8" `
-PhysicalPath C:\temp 
```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/CreatingWebApplications/Images/CreatingAWebsite.jpg" alt="powershell created site" />

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/CreatingWebApplications/Images/IIS_WebSite.jpg" alt="IIS Site" />

PowerShell doesn't let you customize much during the default install (Restart schedules, application pool user, etc.), but it is easy to modify them after creation.  Use the <a href="http://technet.microsoft.com/en-us/library/hh849844.aspx" title="Set-ItemProperty" target="_blank">Set-ItemProperty</a> (Alternatively, you can use <a href="http://www.iis.net/learn/manage/powershell/powershell-snap-in-making-simple-configuration-changes-to-web-sites-and-application-pools" title="Get and Set Item Property" target="_blank">Get-ItemProperty</a> to look at the configuration values in IIS).
  

```
Set-ItemProperty "IIS:\AppPools\ApplicationPoolName" -Name Recycling.periodicRestart.schedule -Value @{value="00:00"}

```


Unless you specify the <strong>Force</strong> parameter, old web applications won't be overwritten by new ones.  In order to test them before making the update specify the conditional using <a href="http://technet.microsoft.com/en-us/library/ff730955.aspx" title="Test Path" target="_blank">Test-Path</a>: 

```
if(-not (Test-Path "IIS:\Sites\Default Web Site\NewWebApplication")){
.....#place web application update here. 
}

```


Removing a web application is just as easy as creating one too.  Just provide the name of the web application and the site it runs under: 

```
Remove-WebApplication "NewWebApplication" -Site "Default Web Site"
```