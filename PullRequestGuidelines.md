We are happy to accept pull requests 
as long as we can see them operate and thoroughly test on a public server before we accept them. 
We are expecting no less from you than we expect of our developers.  
So it would be up to you to install a working version of the software with a test database 
(the code comes with a test database) on that server 
and provide credentials to us for testing. 
It is also helpful to create a video of your working code so we can understand what it does and the intent. 

We do not have the resources to help you with coding your feature. 
But we will test it and provide feedback. 
Your feature would have to be thoroughly proven not to break any existing functionality, 
and any UI elements follow our existing UI patterns and code. 

The pull request would have to be able to merge automatically 
and be up to date with our code (using rebase) to avoid messy merge commits of our work. 
Also, any third party libraries required must be available via NuGet, and no proprietary code will be accepted. 
Your code will need to be licensed with a compatible Open Source license with the GPL v2.
If the code is not modular, but rather integrated into the existing code, 
it should be given to the project and will be copyrighted by Bellevue Baptist Church 
(the current copyright holder of the project).

Once the feature passes functionality tests, 
we will review your code and provide any feedback on that. 
Once that passes muster, we will accept a pull request and make it part of the bvcms/bvcms repository. 
If the feature is of significant size, 
we would want to make it a "dark feature" (able to be made operational with a switch in Settings or as a user setting).  
This way, we can test it in production against our real databases, without it affecting existing customers. 
Once it passes that production test, we would document it and publish a blog post to introduce it. 
You would need to provide some documentation for us to use as a starting point.
