# PmsSmi
Project is developed with visual studio community dotnet core 3.0

It uses docker version Microsoft SQL Server 2017 (RTM-CU11) (KB4462262) - 14.0.3038.14 (X64)   Sep 14 2018 13:53:44   Copyright (C) 2017 Microsoft Corporation  Developer Edition (64-bit) on Linux (Ubuntu 16.04.5 LTS)

Steps to run

1. Configure mssql
2. patch appsettings.json  with correct connection string
3. dotnet build && dotnet-ef migrations add InitialState && dotnet-ef database update && dotnet run
4. controller have three endpoint. 
* http://localhost:5000/api/projects
* http://localhost:5000/api/tasks
* http://localhost:5000/api/reports 
   (use GET with Accept = application/vnd.openxmlformats-officedocument.spreadsheetml.sheet)

Known issues: 
1. report parameters at controller are not implemented
2. xslx file renderer is broken (due I completely forgot xlsx file format). So for now it only fit with row quantity :(
