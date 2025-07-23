---
layout: post
title: "Cut the Header Off and the Body will Die (Well not really)"
date: 2014-04-13
---

Recently, I had to work on an audit item concerning removing the "X-Powered-By" header from the IIS websites.  
<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/HttpHeader/Images/DefaultResponseHeader.jpg" alt="http header" />

You can right click on the header and select remove, but this causes two problems.  One if you have a lot of sites (I had about 30), it becomes rather time consuming.  Two you'll notice that the **Entry Type** of it is set to Inherited.  With HTTP headers there are two entry types in IIS: Local and Inherited.  Local means the current site or web application is the point of origin.  If you delete it, the header will be removed from the current node and all it's children.  Inherited means just the opposite.  It is not the point of origin and removing it does not permanently delete it from IIS.  All web sites list this response header as inherited.  

You can click on the machine which hosts the sites in IIS and remove it: 

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/HttpHeader/Images/DeleteFromComputer.jpg" alt="Delete From Computer" />


The problem with this being that someone has to manually delete it from each server.  This is both time consuming and error prone (any manual task is), and with PowerShell there is a much easier way: 

```
Remove-WebConfigurationProperty -PSPath MACHINE/WEBROOT/APPHOST `
-Name . -Filter system.webServer/httpProtocol/customHeaders `
-AtElement @{name = "X-Powered-By"}
```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/HttpHeader/Images/HttpHeaderAfterScript.jpg" alt="Http Header Removed" />

Consequently, if you want to check to see if it exists before running the delete command, you can add a few lines to check.  By default the <a href="http://technet.microsoft.com/en-us/library/ee790570.aspx" title="web configuration property link" target="_blank">Remove-WebConfigurationProperty</a> will output a warning if you try to delete something which doesn't exist.  


```

$foundExpoweredCount = Get-WebConfigurationProperty -PSPath MACHINE/WEBROOT/APPHOST `
-Filter system.webServer/httpProtocol/customHeaders -Name .  `
| WHERE {  $_.Collection | WHERE { $_.name -eq  "X-Powered-By" }  } | measure

if($foundExpoweredCount.Count -ne 0) {
    Write-Host "Removing X-Powered-By"
    Remove-WebConfigurationProperty -PSPath MACHINE/WEBROOT/APPHOST -Name . -Filter system.webServer/httpProtocol/customHeaders -AtElement @{name = "X-Powered-By"}
}


```


For any IIS Administration to work, you have to run PowerShell in Administrator mode.  With a <a href="http://technet.microsoft.com/en-us/security/jj720323.aspx" title="Hardened Server" target="_blank">hardened server</a> you'll have to supply your credentials to do so. 

PowerShell has the ability to add headers as well.  Just use the <a href="http://technet.microsoft.com/en-us/library/ee790572.aspx" title="Add Web Configuration" target="_blank">Add-WebConfigurationProperty</a>.


```
Add-WebConfigurationProperty -PSPath MACHINE/WEBROOT/APPHOST ` 
# or "MACHINE/WEBROOT/APPHOST/Your Web Site"
-Name . -Filter system.webServer/httpProtocol/customHeaders `
-AtElement @{name = "X-Powered-By" ; value='ASP.NET'}

```