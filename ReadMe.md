<!--- HTML Links --->
[WPI]: http://www.microsoft.com/web/downloads/platform.aspx "Web Platform Installer"
[GHW]: http://windows.github.com/ "GitHub for Windows"
[DOC]: http://www.bvcms.com/doc "BVCMS Documentation"

BVCMS Developer Quick Start Guide
---

Software Install
---
1. Download and install **[Microsoft Web Platform Installer][WPI]**
1. Select the **Products** tab
1. Add the following to be installed:
	+ **Visual Studio 2012 for Web**
 	+ **SQL Server Express 2008 R2 SP2**
 	+ **SQL Server 2008 R2 Management Studio Express SP1**
1. Click **Install** and let the **Web Platform Installer** install all the selected packages

GitHub for Windows Install
---
1. Download **[GitHub for Windows][GHW]**
1. Install **GitHub for Windows**
1. Run **GitHub for Windows** and follow the setup instructions
	+ Note: You will need to create an account on GitHub to properly use all of the features

Clone the Repository
---
1. Go to the repository in a browser - **[https://github.com/bvcms/bvcms]()**
1. Click **"Clone in Windows"** button in the upper left part of the page
	+ It should request that GitHub use the link, allow it to continue
1. **GitHub for Windows** should launch and clone the repository to the default location
	+ The default location for **GitHub for Windows** is **"My Documents\\GitHub"**

Database Creation and Population
---
1. Open **SQL Management Studio**
	+ Start > All Programs > Microsoft SQL Server 2008 R2 > SQL Server Management Studio
1. In **Server Name** box, put **.\SQLEXPRESS** and hit Connect
1. Right-click on **Databases** and select **New Database**
1. Enter **CMS_bellevue** in the database name and click **OK**
1. Select **CMS_bellevue** in the database list
1. Open the main database schema file: **BuildCmsDb.sql**
	+ File > Open > File
	+ Navigate to the repository directory
	+ Open the **BuildCmsDb.sql** file
		+ Note: If your file extensions are hidden, you will not see the ".sql"
1. Verify that **CMS_bellevue** is listed next to the **Execute** button as the active database and then click **Execute**
	+ Note: Computers with 2Gb of memory or less may have trouble creating the database because of not enough memory
1. Right-click on **Databases** and select **New Database**
1. Enter **CMS\_bellevue\_img** in the database name and click **OK**
1. Select **CMS\_bellevue\_img** in the database list
1. Open the image database schema file: **BuildCmsImageDb_SQLSchema.sql**
	+ File > Open > File
	+ Navigate to the repository directory
	+ Open the **BuildCmsImageDb_SQLSchema.sql** file
		+ Note: If your file extensions are hidden, you will not see the ".sql"
1. Verify that **CMSi\_bellevue** is listed next to the **Execute** button as the active database and then click **Execute**

Open the Project
---
1. Start **VisualStudio 2012 for Web**
1. Open **CmsWeb.sln** solution in the root of the repository
	+ Note: If your file extensions are hidden, you will not see the ".sln"
	+ Note: CmsDataSqlClr won't open properly in the Express edition of Visual Studio.
	It is safe to remove this project from the list
1. Start the **Package Manager Console**
	+ Tools > Library Package Manager > Package Manager Console
1. At the top of the **Package Manager Console** you will see a notification telling you that some packages are missing, click **Restore** to begin downloading them
1. It should show a progress bar and then disappear when done
1. Edit **Web.config** under **CmsWeb** root directory
	+ Change the word "Windows" to "Forms" on line 33
1. Right-click on **CMSWeb** and select **Set as StartUp Project**
1. Right-click on **CMSWeb** and select **Rebuild**. The project should compile successfully
	+ There will be some warnings, you can ignore them
1. Click the **Play** button in the main toolbar to launch BVCMS
1. Once at the login screen, enter the default username and password and click **Log On**
	+ **Default Username: admin**
	+ **Default Password: bvcms**

Using BVCMS
---
For addition information on how to use BVCMS, please see the **[BVCMS Documentation][DOC]**.
