
Authorization:Bearer Token

1)WebApi => Set as Startup Project
2)Add reference: Microsoft.EntityFrameworkCore.Design
3)Startup.cs => Add AddDbContext and AddIdentity
4)Add ApplicationUser model

***************************************
*  ApplicationDbContext is DbContext  *               
*  -o is path migrations              *
***************************************

Add-Migration InitialDbMigration -c UserDbContext -o Data/Migrations
Update-Database -Context UserDbContext

---------
Remove-Migration
Drop-Database
-----------------------------------------------

-----------------------------------------------------------------------------
Entity type |	Description
-----------------------------------------------------------------------------
User	    |   Represents the user.
Role	    |   Represents a role.
UserClaim	|   Represents a claim that a user possesses.
UserToken	|   Represents an authentication token for a user.
UserLogin	|   Associates a user with a login.
RoleClaim	|   Represents a claim that's granted to all users within a role.
UserRole	|   A join entity that associates users and roles.
-----------------------------------------------------------------------------

*****************************************************************************
NuGet
*****************************************************************************
Identity.WebApi:         | Amazon.Lambda.AspNetCoreServer
                         | Microsoft.EntityFrameworkCore
                         | Microsoft.EntityFrameworkCore.Design
                         | Microsoft.EntityFrameworkCore.SqlServer
-----------------------------------------------------------------------------
Identity.Domain:         | Microsoft.AspNetCore.Identity.EntityFrameworkCore
-----------------------------------------------------------------------------
Identity.Infrastructure: | Microsoft.AspNetCore.Identity.EntityFrameworkCore
                         | Microsoft.EntityFrameworkCore
                         | Microsoft.EntityFrameworkCore.SqlServer
                         | Microsoft.EntityFrameworkCore.Tools
                     