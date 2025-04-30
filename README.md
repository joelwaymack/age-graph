# age-graph

## Overview

This is a simple API using Apache AGE PostgreSQL extension to create a graph data structure with a C# API. A simple Web App written with [Svelte](https://svelte.dev/) displays the graph using [force-graph](https://github.com/vasturiano/force-graph).

## Tooling

- [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Node.js](https://nodejs.org/en)
- Postrgres Database with the [Apache AGE extension](https://age.apache.org/) enabled
  - How to [enable extensions in Azure Postgres](https://learn.microsoft.com/en-us/azure/postgresql/extensions/how-to-allow-extensions?tabs=allow-extensions-portal)
  - Ensure you have the AGE extension enabled in both the `azure.extensions` and the `shared_preload_libraries` settings for Azure database for PostgreSQL flexible server
- An AI inferencing endpoint from Azure AI Foundry

## Running

- Database

  1. Create a new graph with the desired name in the postgres database using the following query:
     ```sql
     CREATE EXTENSION IF NOT EXISTS age CASCADE;
     SET search_path = ag_catalog, "$user", public;
     SELECT create_graph('[graph_name]');
     ```

- API

  1. Create a new file at `/Graph.Api/appsettings.Development.json` to store local environment variables.
  2. Add the following to define the connection string for the Postgres Database and the name you want to use for the graph

     ```json
     {
       "Postgres": {
         "ConnectionString": "[connection_string]",
         "GraphName": "[graph_name]"
       },
       "AiConfig": {
         "DeploymentName": "[deployment_name]",
         "ApiKey": "[api_key]",
         "Endpoint": "[endpont_uri]"
       }
     }
     ```

  3. In a terminal, start the API in the `/Graph.Api` directory by running `dotnet run`
  4. Navigate to <http://localhost:5054/swagger> to exercise the API.

- Web App
  1. In a terminal, install the dependencies in the `/Graph.WebApp` directory by running `npm i`
  2. In the same directory, run `npm start` with the API running

## Explanation

The graph database works off the [Cypher query language](https://neo4j.com/docs/getting-started/cypher/) created by neo4j for their graph databases. Labels are the primary means of differentiating types (think tables) in the database.

There are 2 routes for building out Vertices in the API:

- Explicit types: Your best approach is to create a specific type to represent each vertex label and to add specific edge labels to the `\Graph.Api\Models\EdgeType.cs` enum.
  - The `\Graph.Api\Models\Asset.cs` type, with its corresponding controller, is an example of explicit typing for a vertex.
- Vertex type: The Vertex type is a generic type that can represent a vertex with a label and properties. The Vertex type is returned from many API routes to handle the generic nature of the underlying graph.

Edges are defined by the `\Graph.Api\Models\Edge.cs` type. The label used for edges is defined in the `\Graph.Api\Models\EdgeType.cs` enum. Add or remove enums based on the edge types you want to support.

The `\Graph.Api\DataAccess\GraphDatabase.cs` type is the heart of the API. It translates actions from the Controllers to Cypher queries for accessing the graph. Add additional methods to this class to support additional graph queries.

## Credit

This uses code from the [npgsql-age](https://github.com/konnektr-io/npgsql-age) repository. Since the repository marks many of the classes as internal, the nuget package could not be used.
