# WowGuildAPI Documentation

WowGuildAPI provides access to various World of Warcraft guild and character data, including profiles, raid progressions, rankings, members, and Mythic+ scores.

## Endpoints

### 1. Get Guild Information

**URL**: `/api/guild/profile`  
**Method**: `GET`  
**Description**: Retrieves basic details about a guild, including name, region, realm, faction, and profile URL.

#### Query Parameters:
- `region` (required): The region where the guild is located. Allowed values: `us`, `eu`, `tw`, `kr`, `cn`.
- `realm` (required): The realm in which the guild exists (e.g., `Draenor`, `Stormrage`).
- `name` (required): The name of the guild.

### 2. Get Raid Progressions for a Guild

**URL**: `/api/guild/raid-progressions`  
**Method**: `GET`  
**Description**: Retrieves the raid progressions for a specified guild, showing the furthest boss defeated at each difficulty level.

#### Query Parameters:
- `region` (required): The region where the guild is located. Allowed values: `us`, `eu`, `tw`, `kr`, `cn`.
- `realm` (required): The realm in which the guild exists (e.g., `Draenor`, `Stormrage`).
- `name` (required): The name of the guild.

### 3. Get Raid Rankings for a Guild

**URL**: `/api/guild/raid-rankings`  
**Method**: `GET`  
**Description**: Retrieves the raid rankings for a specified guild, including rankings at normal, heroic, and mythic levels.

#### Query Parameters:
- `region` (required): The region where the guild is located. Allowed values: `us`, `eu`, `tw`, `kr`, `cn`.
- `realm` (required): The realm in which the guild exists (e.g., `Draenor`, `Stormrage`).
- `name` (required): The name of the guild.

### 4. Get Guild Members

**URL**: `/api/guild/members`  
**Method**: `GET`  
**Description**: Retrieves the guild members who hold specific ranks: Guild Master (0), and the next two ranks (1, 2).

#### Query Parameters:
- `region` (required): The region where the guild is located. Allowed values: `us`, `eu`, `tw`, `kr`, `cn`.
- `realm` (required): The realm in which the guild exists (e.g., `Draenor`, `Stormrage`).
- `name` (required): The name of the guild.

### 5. Get Character Information

**URL**: `/api/character/profile`  
**Method**: `GET`  
**Description**: Retrieves basic details about a character.

#### Query Parameters:
- `region` (required): The region where the character is located. Allowed values: `us`, `eu`, `tw`, `kr`, `cn`.
- `realm` (required): The realm in which the character exists (e.g., `Draenor`, `Stormrage`).
- `name` (required): The name of the character.

### 6. Get Mythic+ Scores for a Character

**URL**: `/api/character/mythic-plus-scores`  
**Method**: `GET`  
**Description**: Retrieves the Mythic+ scores for a character.

#### Query Parameters:
- `region` (required): The region where the character is located. Allowed values: `us`, `eu`, `tw`, `kr`, `cn`.
- `realm` (required): The realm in which the character exists (e.g., `Draenor`, `Stormrage`).
- `name` (required): The name of the character.

### 7. Get Mythic+ Best Runs for a Character

**URL**: `/api/character/mythic-plus-best-runs`  
**Method**: `GET`  
**Description**: Retrieves the Mythic+ best runs for a character.

#### Query Parameters:
- `region` (required): The region where the character is located. Allowed values: `us`, `eu`, `tw`, `kr`, `cn`.
- `realm` (required): The realm in which the character exists (e.g., `Draenor`, `Stormrage`).
- `name` (required): The name of the character.

## Caching

The API uses caching to optimize performance and reduce load. The cache key is generated based on the following format:
- For guilds: `guild:{region}:{realm}:{name}`
- For raid progressions: `raid-progressions:{region}:{realm}:{name}`
- For raid rankings: `raid-rankings:{region}:{realm}:{name}`
- For members: `members:{region}:{realm}:{name}`
- For characters: `character:{region}:{realm}:{name}`
- For Mythic+ scores: `mythic-plus-scores:{region}:{realm}:{name}`
- For Mythic+ best runs: `mythic-plus-best-runs:{region}:{realm}:{name}`

If the data is not found in the cache, the API will retrieve it from the database or external sources.

## Error Handling

The API returns appropriate error responses for invalid or missing data. Common error responses include:
- `400 Bad Request`: Invalid input parameters.
- `404 Not Found`: Data not found in the database or external sources.

## API Configuration and Setup

The WowGuildAPI is built using ASP.NET Core, and the following configuration steps are used to set up the application.

### 1. Caching Configuration

To optimize the performance, response caching is enabled, and Redis is used for caching.

- **Response Caching**: The API response caching is enabled to reduce the load on the server.
- **Redis Cache**: Redis is configured to store the cache data, with the connection string provided in the application settings.

### 2. Dependency Injection (DI) and Service Configuration

The application uses dependency injection to manage services and repositories:

- **Services**:
  - `IRaiderIoGuildProfileService` and `IRaiderIoCharacterProfileService` are registered for retrieving data from Raider.io.
  - `ICacheService` is registered to handle cache management.

- **Repositories**:
  - `IGuildRepository`, `IRaidProgressionsRepository`, `IRaidRankingsRepository`, `IMembersRepository` for guild-related data.
  - `ICharacterRepository`, `IMythicPlusScoresRepository`, and `IMythicPlusBestRunsRepository` for character-related data.

- **MongoDB Client**: The application uses MongoDB for storage, with a client instance created based on the connection string from the settings.

### 3. HTTP Client Configuration

The `HttpClient` is added to allow external API calls to be made within the application.

### 4. AutoMapper Setup

AutoMapper is used to map data between models. The `GuildMapper` is configured for this purpose.

### 5. Swagger and OpenAPI

Swagger is configured to document the API and generate interactive API documentation.

- **Swagger Configuration**: The Swagger UI is enabled in the development environment, and an OpenAPI description is provided.
- **Region Schema Operation Filter**: A custom operation filter `RegionSchemaOperationFilter` is added to handle region-specific operations.

### 6. CORS Policy

Cross-Origin Resource Sharing (CORS) is configured to allow any method and header from the specified origins.

### 7. Application Middleware

The following middleware is configured:

- **Swagger UI**: Displayed in development mode for testing the API.
- **HTTPS Redirection**: Ensures all requests are redirected to HTTPS.
- **CORS**: The configured CORS policy is applied.
- **Routing**: Controllers are mapped to handle incoming requests.

### 8. Running the Application

Finally, the application is started and ready to handle incoming requests.
