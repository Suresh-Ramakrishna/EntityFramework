//Configuring EF//


* Create a new Project (Optional, we can add the ef model in any of the existing project. To keep data model seperated from our projects, we are creating a new project)
* Right Click on a project ->Add->New Item-> In Data Tab, select ADO.NET Entity Data Model
* Give a name for the Data Model, In our case, we are naming "SchoolModel"
* Click on Add,
* Select EF Designer from Database and click on Next
* Setup database connection.
* In Save connection settings in App.Config as, give the name for the DBContext class. In our case "SchoolEntities"
* Click Next
* Select Entity Framework 6.x and click on Next
* Select object to include and Uncheck Pluralize or Singularize object names to retain table name from db
* Click on Finish
* If data model is created in a new project, add this projects reference to other projects which needs db access.


Note: You also need to install "EntityFramework" nuget package to the project where data model is being used, if data model is created as a seperate project
