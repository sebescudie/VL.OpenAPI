# VL.OpenAPI

_Automagically create nodes from an OpenAPI description._

## Installation

- Build the solution located in `/src/VL.OpenAPI`

- Install the Directus instance located in the `/cms` folder
  
  - `cd cms`
  
  - `npm install`
  
  - `npm update`
  
  - `npx directus database migrate:latest`

- Start the directus instance with `npx directus start`

- Start vvvv and install the following dependencies
  
  - `Microsoft.OpenApi 1.2.3`
  
  - `Microsoft.OpenApi.Readers 1.2.3`
  
  - `RestSharp 108.0.1`

## Usage

Next to your patch, create an `openAPI` . In that folder, create individual text files for each openAPI thing you want to create nodes from. In that text file, just paste the URL pointing to the openAPI schema. An example of that can be found in the help folder.

Later we should properly parse one endpoint per line, or even something like

```
http://url.to.the.openapi.thing CollectionName
```

Because in the case of Directus, we don't have any information about the project spec in the OpenAPI schema. Like that we could create propper categories per project.

## Stuff to do

- Sanitize node and pin names (proper casing)

- Make queries for real just outputting a JSON response for now

- Better to create blocking nodes and let ppl wrap them in AsyncTask if necesssary. Later we could generate both with the factory
