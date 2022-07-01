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

Next to your patch, create an `openAPI` folder. In that folder, create an `openapi.txt` document in which you'll list all the OpenAPI things you wanna generate nodes from. This file should be structured as follows :

```
ProjectName, http://url.to.the.openapi.thing, APIKey
```

- `ProjectName` is an arbitrary string used for you to identify the project. This will be used as a category in the node browser

- URL to the OpenAPI thing is an endpoint that points to the OpenAPI JSON spec

- `APIKey` is the API key you're using to interact with the resource. For now, the plugin only supports query parameter based authentication

## Stuff to do

- Sanitize node and pin names (proper casing)

- Make queries for real just outputting a JSON response for now

- Better to create blocking nodes and let ppl wrap them in AsyncTask if necesssary. Later we could generate both with the factory
