<!--- HTML Links --->
[GHW]: http://windows.github.com/ "GitHub for Windows"
[DOC]: http://docs.touchpointsoftware.com "TouchPointSoftware User Documentation"
[IDE]: http://www.visualstudio.com/en-us/downloads/
[SQL]: http://msdn.microsoft.com/en-us/evalcenter/hh230763.aspx
[WEB]: http://visualstudiogallery.msdn.microsoft.com/56633663-6799-41d7-9df7-0f2a504ca361
[RWM]: http://www.microsoft.com/en-us/download/details.aspx?id=7435
[libtidy32]: http://wemakeapps.net/downloads/TidyManaged/libtidy.dll.Win32.zip
[libtidy64]: http://wemakeapps.net/downloads/TidyManaged/libtidy.dll.Win64.zip

BVCMS Developer Quick Start Guide
---

Copyright (c) 2008-2015 Bellevue Baptist Church 
Licensed under the GNU General Public License (GPL v2)
you may not use this code except in compliance with the License.
see LICENSE file in this repository master branch

BVCMS is maintained by TouchPointSoftware, LLC.

### Install Development Tools and required libraries

1. **[Visual Studio Express 2013 or Visual Studio Community][IDE]**
1. **[SQL Server Express 2012 SP1][SQL]**
1. **[GitHub for Windows][GHW]**
1. **[Microsoft URL Rewrite Module 2.0 for IIS 7][RWM]** (if not already installed)
1. **libtidy** ([x86][libtidy32] or [x64][libtidy64] depending on your host) (optional, see [additional notes](#additional-notes))
	* `libtidy.dll` will need to either be in your application bin directory or under your PATH (such as under `c:\windows\system32`)

### Clone the Source Code Repository

1. Run **GitHub for Windows** and follow the setup instructions

	Note: You will need to create an account on GitHub to properly use all of the features

1. Go to the repository in a browser - **[https://github.com/bvcms/bvcms]()**

1. Click **"Clone in Windows"** button in the upper left part of the page

	It should request that GitHub use the link, allow it to continue

1. **GitHub for Windows** should launch and clone the repository to the default location

	The default location for **GitHub for Windows** is **"My Documents\\GitHub"**

### Open the Project

1. Start **Visual Studio 2013**

1. Open **CmsWeb.sln** solution in the root of the repository

	Note: If your file extensions are hidden, you will not see the ".sln"

1. Start the **Package Manager Console**

	Tools > Library Package Manager > Package Manager Console

1. At the top of the **Package Manager Console** you will see a notification telling you that some packages are missing, click **Restore** to begin downloading them

1. It should show a progress bar and then disappear when done

1. Edit **Web.config** under **CmsWeb** root directory

	- In the appSettings section, configure the **dbserver** and **host** values to point to your server and the name of your database (without the CMS_). These two values will become part of your connection string.
	- In the mailSettings, configure the pickupDirectoryLocation to a directory on your development machine (for testing purposes).

1. Right-click on **CMSWeb** and select **Set as StartUp Project**

1. Right-click on **CMSWeb** and select **Rebuild**. The project should compile successfully

	There will be some warnings, you can ignore them

1. The SQL Server database should be running

1. Click the **Play** button in the main toolbar to launch BVCMS

1. The first time you start up the system, it will create and populate a starter database

1. Once at the login screen, enter the default username and password and click **Log On**.

	Default Username: **admin**
	
	Default Password: **bvcms**

1. After successfully logging in, you should set your admin password immediately because the bvcms password is a one-time use password and will no longer work.

### Using BVCMS

For addition information on how to use BVCMS, please see the **[TouchPointSoftware Documentation][DOC]**.

### Additional Notes

* We use libtidy to format HTML, such as when viewing the HTML source of emails. If you're running the system on a 64-bit system using IIS Express and libtidy together, you'll want to enable IIS Express to run in 64-bit mode. To do this, you can open Visual Studio and to to Tools > Options > Projects and Solutions > Web Projects and then check the "Use the 64 bit version of IIS Express."