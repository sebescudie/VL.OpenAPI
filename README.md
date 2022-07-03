# VL.OpenAPI

_Automagically create nodes from an OpenAPI description._

## Qu'est-ce que c'est

This plugin reads an OpenAPI description and generates nodes implementing all queries it contains.

The test setup uses a Directus instance for convenience, but don't hesitate to test it against whatever you have at hand.



### Current status

This is a super-early version of the plugin that was only tested with a dummy Directus instance that uses a super-simple data model. Focus was more to get the OpenAPI parser and node generation correctly working.

As of today, the factory correctly generates the nodes and makes queries to the instance. We just output the result as a raw string (which, in the case of Directus, is JSON).

The only supported auth mechanism is via an API key as a query parameter (the name of that parameter and its value are retrieved from the OpenAPI doc and the config file and added under the hood to all queries). If an API proposes several auth mechanism, we should develop an editor extension that allows to set this up properly (note for later).

If you test this plugin with something else than Directus or with a more complex Directus instance, please shout in the issues here on in the forum!

## Installation

- Build the solution located in `/src/VL.OpenAPI`

- Install the Directus instance located in the `/cms` folder (you will need nodeJS LTS)
  
  - `cd cms`
  
  - `npm install`
  
  - `npm update`
  
  - `npx directus database migrate:latest`

- Start the directus instance with `npx directus start`

- You can login with the following credentials : 
  
  - Mail : `foo@bar.com`
  
  - Password : `cms`

- Start vvvv and install the following dependencies
  
  - `Microsoft.OpenApi 1.2.3`
  
  - `Microsoft.OpenApi.Readers 1.2.3`
  
  - `RestSharp 108.0.1`

## Usage

#### Using the help patch

The test Directus instance is bundled with this repo. Make sure it's started and either start vvvv via the VS solution or open vvvv with the `--package-repositories` arg and look for OpenAPI in the help browser.

The help patch contains nothing special, but it already has the required config file next to it.

#### By hand

Next to your patch, create an `openAPI` folder. In that folder, create an `openapi.txt` document in which you'll list all the OpenAPI things you wanna generate nodes from. This file should be structured as follows :

```
ProjectName, http://url.to.the.openapi.thing, APIKey
```

- `ProjectName` is an arbitrary string used for you to identify the project. This will be used as a category in the node browser

- "URL to the OpenAPI thing" is an endpoint that points to the OpenAPI JSON spec

- `APIKey` is the API key you're using to interact with the resource. For now, the plugin only supports query parameter based authentication
