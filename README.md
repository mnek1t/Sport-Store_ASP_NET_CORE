# Sports Store Application

## Description

The `SportsStore*` application is the classic approach taken by online stores everywhere.

## Application Features

As a result of completing this task, you will receive an application that implements the following features:
- an online product catalog that customers can browse by category and page;
- a shopping cart where users can add and remove products;
- a checkout where customers can enter delivery details;
- an administration area that includes create, read, update, and delete (CRUD) tools for manage directory;

## .NET Platform
The task uses the .NET Core CLI command line tool and references applications that target .NET 6.

Visual Studio 2022 is the most convenient tool to get the task done, however, if your work machine is not configured to run this IDE, development can be done with Visual Studio Code and the .NET Core SDK (or ather IDE).

## Branching

The task consists of four steps. The description of the each step of the task are in the corresponding `md`-file in the corresponding branch.

| Step | Step Description | Feature Branch Name |
| ------ | ------ | ------ |
| 0. | Building the basic infrastructure for the SportsStore application. | sports-store-application-0 |
| 1. | Definition of simple domain model with a product repository supported by SQL Server and Entity Framework Core. Development the HomeController controller that can create paginated product lists. Setting clean and friendly URL schemes. Styling of the content. |sports-store-application-1 |
| 2. | Development the navigate by category. Development the basic building blocks in place for adding items to a shopping cart.|sports-store-application-2 |
| 3. | Complete shopping cart development with a simple checkout process. | sports-store-application-3 |
| 4. | Implementation of CRUD operations that allow the administrator to create, read, update and delete products from repository and mark orders as shipped. | sports-store-application-4 |

After completing all steps you will get the final version of the application in the `main` branch.

_*This task was developed based on an example from the book by [Adam Freeman Pro ASP.NET Core 6: Develop Cloud-Ready Web Applications Using MVC, Blazor, and Razor Pages 9th ed. Edition](https://www.amazon.com/Pro-ASP-NET-Core-Cloud-Ready-Applications/dp/1484279565). If you are having difficulty completing the task, contact the source._
