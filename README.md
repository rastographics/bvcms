<!--- HTML Links --->
[DOC]: https://docs.touchpointsoftware.com "TouchPointSoftware User Documentation"
[IDE]: https://www.visualstudio.com/downloads/
[SQL]: https://www.microsoft.com/en-us/download/details.aspx?id=54284
[WEB]: http://visualstudiogallery.msdn.microsoft.com/56633663-6799-41d7-9df7-0f2a504ca361
[RWM]: http://www.microsoft.com/en-us/download/details.aspx?id=7435
[GIT]: https://github.com/bvcms/bvcms.git
[PR]: PullRequestGuidelines.md

BVCMS Developer Quick Start Guide
---

Copyright (c) 2008-2018 Bellevue Baptist Church 
Licensed under the GNU General Public License (GPL v2)
you may not use this code except in compliance with the License.
see LICENSE file in this repository master branch

BVCMS is maintained by TouchPoint Software, Inc.

### Install Development Tools and required libraries

1. **[Visual Studio Community 2017][IDE]**
    - Be sure to install the ASP.NET and web development Workload option.
    - Install the Python development option if you want to debug Python scripts.
    
1. **[SQL Server Express 2016][SQL]**

### Get the cloning URL for the GitHub BVCMS Source Code Repository

1. Go to the repository in a browser - **[https://github.com/bvcms/bvcms]()**

1. Click green **"Clone or Download"** button in the upper right part of the page

1. Copy the link URL shown

### Load the Project

1. Start **Visual Studio**

1. On the right side, click the tab *Team Explorer* 

1. Under *Local Git Repositories*, click *Clone*

1. Paste the URL you copied in above (should be https://github.com/bvcms/bvcms.git), then click *Clone*

1. Back under the *Solution Explorer* tab, double click the CmsWeb.sln item

1. Find and edit the web.config file in the **CMSWeb** folder

    - In the `appSettings` section, configure the `dbserver` and `host` values to point to your server and the name you would like to call your database (without the CMS_). These two values will become part of your connection string.
    - for `dbserver` if you installed a full version of SQL Server (not Express) use `(local)` with the parentheses. 
      If you installed SQL Server Express, use `.\SQLEXPRESS`
    - In the `mailSettings` section, configure the pickupDirectoryLocation to a directory on your development machine (for testing purposes).
    - If you want to create a database with 150 test records in it, just use ``testdb`` for the host value

1. The SQL Server database should be running

1. Click the **Play** button in the main toolbar to launch BVCMS

1. The first time you start up the system, it will create and populate a starter database

1. Once at the login screen, enter the default username and password and click **Log On**.

	Default Username: **admin**
	
	Default Password: **bvcms**

1. After successfully logging in, you should set your admin password immediately because the bvcms password is a one-time use password and will no longer work.

### Using BVCMS

For additional information on how to use BVCMS, please see the **[TouchPointSoftware Documentation][DOC]**.

## Pull Requests

See [Pull Request Guidelines][PR]
