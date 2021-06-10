![Alt text](./github%20images/Project%20logo.png?raw=true "Project logo")

My own implementation of a kanban board made just for fun and nothing more. Backend part.

For the webclient please see **kabanwebapp** repo.

**Used technologies / features:**
- ASP.NET Core 3.1
- Entity Framework Core
- Segregation into API, Domain and Database-related projects
- xUnit.net for Integration Tests  

**Possibilities:**
- User Registration / Authorization
- Creating, Updating, Removal User Boards
- Creating, Updating, Removal Board Lists, Cards, Card Comments
- Copying whole Board Lists
- Reordering Board Lists and Cards

**Build and Run**

To get API up and running, navigate to src/Kaban.API folder and execute
```powershell
dotnet run
```  

