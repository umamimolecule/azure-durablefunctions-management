# azure-durablefunctions-management
Adds HTTP endpoints to your Azure Durable Functions project to allow querying status of orchestration instances.

## NuGet Package

https://www.nuget.org/packages/Umamimolecule.AzureDurableFunctions.Management/

## Installation

Select your Durable Functions project and run the following command in the Package Manager console:

`install-package Umamimolecule.AzureDurableFunctions.Management`

## Motivation

Provide an out-of-the-box set of management functions without having to write them for each Durable Functions project.

When you add this package to your Azure Functions project, you'll automatically get the following endpoints to allow orchestration instance management:

![Functions Console](docs/functionsconsole.png)

The endpoints are located at `/{routeRoute}/orchestration/instances` (this will be `/api/orchestration/instances` unless you have overriden the default route prefix).

## Documentation

[See here](docs/documentation.md) for documentation of all the endpoints.

## Tutorial

Let's create a durable function project and add the management package so we can see how it works.

What you'll need:
 - Visual Studio (I'm using VS 2019)
 - Postman or a similar tool to issue GET and POST requests. ![Here is a Postman collection](tools/DurableFunctions Instance Management.postman_collection) you may find useful.

### Create new Azure Functions project

### Add Durable Functions extension

### Add management package

### Create a HTTP-triggered durable function

### Trigger the function

### Use the management endpoints

Query the running instances:

POST 
