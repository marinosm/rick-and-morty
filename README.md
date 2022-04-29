This is an API built to consume [Rick and Morty REST API](https://rickandmortyapi.com/documentation/#rest) for the purposes of an interview exercise.

According to requirements the following two endpoints are exposed:
1. Based on a character name (example: `GET /CharactersAndCoresidents?name=Alan Rails`) fetch the character information together with information regarding other characters residing in the same location.
2. Based on a character name (example: `GET /CharactersAndFirstEpisodeInfo?name=Rick`) fetch the character information together with information in which episode the character appeared first and the list of other characters who first appeared in the same episode.

A [SwaggerDoc](https://swagger.io/docs/) is also exposed at the base url.
