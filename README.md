# Dropbox-Lister

Demo project to List Contents of your DropBox account. 
<br />
It's still in development. To try it out, you'll need a Dropbox Account and Dropbox app -Which can be created at App Console in http://developers.dropbox.com 


### Main Workflow:

 - Open browser to open the page for getting the app code from Dropbox's WebSite
 - Authenticate user with given app code
 - List root folder initially, then sub-folders if wanted
 - Send SIGINT (Ctrl-C) to close the application.



### To Do

 - This can evolve to a standalone API, need some planing for this, in case it's a serious possibility
 - Containerazation?
 - More extensive unit testing, and needs integration testing.
 - Possibly need some refactor here and there, extensive planning needed if scalibility needed.
