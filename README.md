# Ethereum Block Explorer
### Building a block explorer reporting tool (with Nethereum)

### Purpose

This solution's current purpose is to report on Blockchain data within either a specified range, or within a certain number of blocks from the most recent block at time of request.

There are two solutions provided for ease of use:

1. A WebAPI containing a single controller exposing the two main calls. This comes with a Swagger UI endpoint as a way of interacting. Note: depending on the volume of data requested, this could be really slow - a Postman or Curl request may be better. This API's purpose is really only to to provide the data for consumers to use.
2. A Blazor Application with a UI allowing viewers to see a tabbed report broken down into three sections
   1. Summary
   2. Externally Owned Account data
   3. Contract Data
3. The default startup project in Visual Studio is The BlockExplorer.Blazor

### Prerequisites

1. A compatible IDE to run, test and debug or
2. .Net Core 3.1
3. A Terminal if using the CLI (e.g. PowerShell or VS Code Terminal (or what ever you prefer))
4. Knowledge of any of 1-3

### Building

1. Open the solution file (in `/src/BlockExplorer.sln` ) in Visual Studio, Build (Ctrl+Shift+B)
2. If using CLI 
   1. Navigate to `/src/`
   2. Type in `dotnet build`

### Testing

1. Open the solution file (in `/src/BlockExplorer.sln` ) in Visual Studio
   1. Build (Ctrl+Shift+B)
   2. Right click on the Test project and select `Run tests` - alternatively the Test Explorer may provide options as well
2. If using CLI 
   1. Navigate to `/src/`
   2. Type in `dotnet test`

A note on testing - the testing at present is very rudimentary and will be improved to:

1. Cover all missing areas
2. Service level tests (running in memory with a mocked BlockChain)
3. Integration tests (checking some transactions on mainnet)

### Known issues and missing implementations

1. Filter to show either addresses with `received > 0` , `sent > 0` or both
2. Extensive value validation on the API endpoints (e.g. always a number for blocks)
3. Blazor double hit for the prerender needs fixing
4. Service tests
5. Integration tests
6. More descriptive UI content and usage explanation
7. Proper exception handling
8. Blockchain client circuit breakers  in case of repeated issues
9. Performance performance and performance adjustments
10. Rate limited endpoints may be hit too quickly for all the block data and could error
11. An alternate way may be needed to work out if it is a contract without having to look it up
12. Caching options?

### Running

1. For either the Blazor solution or the API one
   1. Navigate to the respective project folder
   2. Change the `RpcAddress` value in the `appsettings.json` file to your Node address (this could be mainnet, testnet or something like Ganache)
2. Open the solution file (in `/src/BlockExplorer.sln` ) in Visual Studio
   1. Build (Ctrl+Shift+B) 
   2. Press F5 to run (you could skip point 1, but I prefer to build)
3. If using CLI 
   1. Make sure nothing is running on port 5000 (http) /5001 (https) as the services bind to these
   2. Navigate to `/src/BlockExplorer.Blazor` or `/src/BlockExplorer.Api`
   3. Type in `dotnet run`
   4. Open a browser and got to:
      1. [https://localhost:5001/swagger/index.html](https://localhost:5001/swagger/index.html) for the WebAPI Swagger
      2. [https://localhost:5001/](https://localhost:5001/)  for the Blazor Application

### Solution structure

1. `BlockExplorer.Api` WebAPI with controller to return data
2. `BlockExplorer.Api.Models` Models returned by the API that could be turned into a Nuget package for consumers
3. `BlockExplorer.Blazor` Blazor project UI
4. `BlockExplorer.Clients` Blockchain clients implementing the Domain specified contract - this currently is just Nethereum
5. `BlockExplorer.Domain` Internal domain models, configuration  and service definitions
6. `BlockExplorer.Handlers` Handler implementations for retrieving and mapping the domain data for the API/Blazor 
7. `BlockExplorer.Tests.UnitTests` Basic unit tests

![image](https://user-images.githubusercontent.com/3957173/145733925-070a929f-3162-4e3e-9b2a-51c1e4d744b3.png)

![image](https://user-images.githubusercontent.com/3957173/145733903-16da2b50-c18f-4b39-a0ff-5eddb0a88842.png)

![image](https://user-images.githubusercontent.com/3957173/145733969-43d8b0e1-805d-4ccf-a22d-e88bf253668d.png)


