---
layout: post
title: "I'm out of Range? You're out of Range!"
date: 2014-10-26 00:00:00 -0500
---
In IIS there are several different options allowing you to control the behavior of an application.  With all of these settings Microsoft attempts to validate the entered values are within the accepted entries.  Unfortunately, if you are updating multiple settings at once, it's not very clear on which entry causes the issue, and it seems that Windows key stores can become corrupt preventing valid updates to the username and password.

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/BrokenEncryption/IISModifySettings.jpg" alt="Modify Settings" height="45%" width="45%" align="left" style="padding:2px 2px 2px 2px"/>

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/BrokenEncryption/IISError.png" alt="Alert Message" height="50%" width="50%" />

<br clear="all"/>

While updating the username and password in the configuration section, IIS responds with the value out of range error.

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/BrokenEncryption/SetUserAndPassword.jpg" alt="Set Credentials" />

The system allows the credentials to be set to any of the built in accounts, but it wouldn't set it to a user requiring a password.  

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/BrokenEncryption/DefaultUsers.jpg" alt="Default password" />

IIS stores its configuration data in an xml file located in the System32/Inetsrv/Config file allowing you to see what values it actually stores after an update.  This is important, because it shows which entered values are transformed, encrypted, etc. and provides insight into what might be happening: 


```
C:\Windows\System32\inetsrv\config\ApplicationHost.config
```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/BrokenEncryption/RunningTasks.jpg" alt="Running Tasks" align="right" style="padding:2px 2px 2px 2px" />

To view it, you'll need administrative access to the folder, and if you are running a 64-bit operating system, you'll need a 64-bit application to open the file.  64-bit Windows systems have the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa384187(v=vs.85).aspx" title="File System Redirector" target="_blank">File System Redirector</a> which will silently redirect any 32-bit application trying to access the System32 directory to the <strong>SysWOW64</strong>, as the System32 directory is reserved for 64-bit application use.  This means that if a 32-bit text editor (which unfortunately most are) tries to access the **ApplicationHost.config** used by IIS, it will be redirected to a different config file, and any changes made to it won't be reflected in IIS (unless you've specifically installed the 32-bit version of IIS).  Notepad is 64-bit, so it can view and modify the file. 

If IIS has any application pools running specific users, the application host file with have a node looking similar to this: 


```

<processModel identityType="SpecificUser" userName="IIS_User" password="[enc:IISWASOnlyAesProvider:wI5whfVYdG8CDd+HEpQ4EdjgdQSTp6RWGRhRNrNLs1NWfEyJeukGRB33KtflqTG5pwBdpFCeEY1j8wigviESNw==:enc]"/>

```


It shows the username and password the application pool uses, and the encryption used to store the password securely.  By default, IIS uses <strong>IISWASOnlyAesProvider</strong> to encrypt the application user passwords, and ultimately encrypting the password can cause the <strong>Value does not fall within expected range error</strong> when the key store becomes corrupt.

Microsoft has a tool in the **C:\Windows\System32\inetsrv** directory which can quickly help test if this is the problem.  Running the following command will display all the information concerning application pools <strong>including the usernames and decrypted passwords</strong>.


```
C:\Windows\System32\inetsrv>appcmd list apppool /text:*
```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/BrokenEncryption/appcmdUserNameAndPassword.jpg" alt="app cmd username and password" />

If IIS has a problem with encryption/decryption, then it will show up like this: 

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/BrokenEncryption/AppCmdBadPassword.jpg" alt="No Password" />

<div style="border-color:#000099;border-style:solid;border-width:2px; padding: 2px 2px 2px 2px">
<div style="text-align:center; font-weight:600">A side note about security</div>
What you have just seen is a very easy way for someone to gain usernames and passwords to potentially very high level accounts.  If you have a compromised IIS server and the account is an Active Directory account, the attacker now has access to a domain account to do further damage.  If that account has elevated privileges on other machines or even worse is a domain administrator, the attacker now has access to that as well.  (Remember SQL Server allows the use of domain accounts to designate access).

Furthermore, the attacker doesn't need access to the server if the ApplicationHost.Config is available. IIS allows multiple servers to use a <a href="http://www.iis.net/learn/manage/managing-your-configuration-settings/shared-configuration_264" title="Shared Configuration" target="_blank">Shared Configuration</a>.  If that share housing the file is available on the network with relaxed security, then all an attacker needs to do is steal the file and use another machine with the AppCmd tool to get the contents. 
</div>

Although not recommended, modifying the <strong>ApplicationHost.config</strong> file directly and not encrypting the password works:

```

<processModel identityType="SpecificUser" userName="IIS_User" password="MyPassword"/>

```

This doesn't correct the issue, as the encryption is still broken, but it does allow that application instance to run unhindered, and fixing it is relatively simple.  It is possible to recreate the AES encryption keys, but it's much easier to import a copy from an existing machine.  Microsoft provides the Aspnet_Regiis tool with the .NET framework located here: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\.
The following command exports the keys to the temp directory in a file named **AESKeys.xml**.

```
aspnet_regiis -px "iisWasKey"  "C:\temp\AESKeys.xml"
```


The <strong>-px</strong> nominates the key container to export.  By default it is: <strong>iisWasKey</strong>, but in case it has been changed, IIS designates the key container it uses in the **ApplicationHost.Config** here: 


```

<add name="IISWASOnlyRsaProvider" type="" description="Uses RsaCryptoServiceProvider to encrypt and decrypt" <strong>keyContainerName="iisWasKey"</strong> cspProviderName="" useMachineContainer="true" useOAEP="false" />

```


To import it, move the file to the machine in question and run the same command with the <strong>-pi</strong> switch instead:


```
aspnet_regiis -pi "iisWasKey"  "C:\temp\test.xml"
```


If IIS is configured with the defaults, this should fix the issue with minimal fuss.  If it has been configured with different key stores etc.  it may be slightly more difficult to update, but the process should be relatively the same.