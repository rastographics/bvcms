<!--- HTML Links --->
[DOC]: https://docs.touchpointsoftware.com "TouchPointSoftware User Documentation"
[IDE]: https://www.visualstudio.com/downloads/
[SQL]: https://www.microsoft.com/en-us/sql-server/sql-server-editions-express
[PR]: PullRequestGuidelines.md

BVCMS Developer Quick Start Guide
---

Copyright (c) 2008-2019 Bellevue Baptist Church 
Licensed under the GNU General Public License (GPL v2)
you may not use this code except in compliance with the License.
see [LICENSE](LICENSE) file in this repository master branch

BVCMS is maintained by TouchPoint Software, Inc.

### Install Development Tools and required libraries

1. **[Visual Studio Community 2017][IDE]**
    - Be sure to install the ASP.NET and web development Workload option.
    - Install the Python development option if you want to debug Python scripts.
    - Update NodeJS to the latest version by downloading the installer from [nodejs.org](https://nodejs.org/) Run the Node installer using all default options.
    
1. **[SQL Server Express 2017][SQL]**

### Get the cloning URL for the GitHub BVCMS Source Code Repository

1. Go to the repository in a browser - **[https://github.com/bvcms/bvcms](https://github.com/bvcms/bvcms)**.

1. Click green **"Clone or Download"** button in the upper right part of the page.

1. Copy the link URL shown.

### Load the Project

1. Start **Visual Studio**.

1. On the right side, click the tab *Team Explorer*.

1. Under *Local Git Repositories*, click *Clone*.

1. Paste the URL you copied in above (should be `https://github.com/bvcms/bvcms.git`), then click *Clone*.

1. Back under the *Solution Explorer* tab, double click the CmsWeb.sln item.

1. Find and edit the web.config file in the **CMSWeb** folder.

    - In the `appSettings` section, configure the `host` value to point to the name you would like to call your database (without the CMS_). This value will become part of your connection string.
    - In the `mailSettings` section, configure the pickupDirectoryLocation to a directory on your development machine (for testing purposes).
    - If you want to create a database with 150 test records in it, just use ``testdb`` for the host value.

1. Find and edit the ConnectionStrings.config file in the **CMSWeb** folder
    - Change the database server in the connection string named `CMS` if `(local)` or `localhost` is not the name of your SQL Server. For instance, if you installed SQL Express with the default options, you would change `(local)` to `.\SQLEXPRESS`
    - If SQL Server is not installed on your local machine, you may have to make further connection string changes.  See [connectionstrings.com](https://www.connectionstrings.com/sql-server/) for more help.

1. The SQL Server database should be running

1. Start SSMS (SQL Server Management Studio) and run the following script to allow installation of CLR extensions.

        sp_configure 'clr strict security', 0
        GO
        RECONFIGURE;

1. Click the **Play** button in the main toolbar to launch BVCMS.

1. The first time you start up the system, it will create and populate a starter database.

1. Once at the login screen, enter the default username and password and click **Log On**.

	Default Username: **admin**
	
	Default Password: **bvcms**

1. After successfully logging in, you should set your admin password immediately because the bvcms password is a one-time use password and will no longer work.

### Using BVCMS

For additional information on how to use BVCMS, please see the **[TouchPoint Software Documentation][DOC]**.

## Pull Requests

See [Pull Request Guidelines][PR].
