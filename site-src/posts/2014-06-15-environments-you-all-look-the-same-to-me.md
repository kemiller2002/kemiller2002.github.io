---
layout: post
title: "Environments, You All Look the Same to Me"
date: 2014-06-15
---

Several years ago, I was working on moving data from the reporting server to the development environment so I could test structural changes.  Thinking I was in the development environment, and not realizing my level of access was such that it would work in production, I ran <strong>TRUNCATE TABLE Users</strong> and didn't bother to verify what environment I was actually running commands in. It was shortly after my panicked call to the DBA requesting help, that I found out the default access to the reporting server was **Administrator**.  (Clearly there needed to be a discussion about permissions given to production servers, but that's a different story).  Fortunately, he easily fixed it, and I learned that the only way to be sure this wasn't going to happen again was to never to have two different environments open in the same SQL Serve Management Studio (SSMS).

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/SSMSEnvironmentColor/Registered%20Server%20Menu.jpg" alt="Registered Server Menu" align="right"/>
One problem I've always had with SSMS, is the fact that you <strong>can</strong> have multiple servers open at the same time with no differentiation.  It's nice to be able to open them simultaneously, but it is also dangerous from a data integrity standpoint.  Did you run that update on the production database server or the development one?  It's easy to say, "Well keep track of what you are doing!" That's true, you should be cognizant, but people are people and sometimes you get distracted.  It's too easy to make a mistake, and there should be something that says, "HEY YOU'RE ON A PRODUCTION SERVER BE CAREFUL WHAT YOU ARE DOING!"  After several years of bemoaning this issue I found a solution to my problem.  You can change the properties on a registered server to show a different color on the open connections to it. Simply open the <emphasis>Registered Server Properties</emphasis>.

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/SSMSEnvironmentColor/RegisteredServerProperties.jpg" alt="Registered Server Properties" />

Select the <strong>Connection Properties</strong> tab.

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/SSMSEnvironmentColor/SelectColor.jpg" alt="Connection Properties" />

Check the <strong>Use Custom Color</strong>, press the <strong>Select</strong> button, and choose the color. 

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/SSMSEnvironmentColor/CustomColorSelected.jpg" alt="Selected Color" />

Now when you open a new query window from that server, the bottom of the window is the selected color.

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/SSMSEnvironmentColor/RedQueryWindow.jpg" alt="Query Window" />