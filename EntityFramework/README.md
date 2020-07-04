## Configuring Entity Framework
* Create a New Project
* Right Click on a Project -> Add -> New Item -> In Data Tab, Select ADO.NET Entity Data Model
* Give a Name for the Data Model, In our case, we are naming "SchoolModel". Click on Add
* Select "EF Designer from Database" and click on Next
* Setup DataBase connection.
* In "Save connection settings in App.Config as", give the name for the DBContext class. In our case "SchoolEntities". Click Next. 
* Select Entity Framework 6.x and click on Next
* Select Objects to include and Uncheck Pluralize or Singularize object names to retain Table Name as Property Name in context class. 
* Click on Finish.
* If Data Model is created in as a Nwew Project, Add this project's reference to other projects which needs DB Access.

>You also need to install "EntityFramework" nuget package to the project where data model is being used, if data model is created as a seperate project
#
<i>PS: We can add the EF Model in any of the existing project. To keep data model seperated from our projects, we are creating a new project</i>
