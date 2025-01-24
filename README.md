# SantanderTest.HackerAPI

SantanderTest.HackerAPI is a .NET 8 web API that provides access to top stories from a hacker news-like service. The API uses caching to improve performance and reduce the number of requests to the external service.

## Features

- Fetch top stories
- Fetch story details by ID
- Caching to improve performance
- Unit tests with NUnit and Moq

## Installation

1. Clone the repository:

```
git clone https://github.com/yourusername/SantanderTest.HackerAPI.git
cd SantanderTest.HackerAPI
```
   
2. Restore the NuGet packages:

```
dotnet restore
```
    
3. Build the solution:

```
dotnet build
```

## Usage

1. Run the application:

```
dotnet run --project SantanderTest.HackerAPI
```

2. The API will be available at `https://localhost:5001`.

### Endpoints

- `GET /HackerAPI?count={count}`: Fetch the top `{count}` stories.

## Configuration

The API base URI and endpoints are configured in the `ApiUris` class located in the `SantanderTest.HackerAPI.Services.Constants` namespace.

## Testing

The project includes unit tests using NUnit and Moq. To run the tests, use the following command:

```
dotnet test
```

### Test Data

Test data is provided by the `TestDataProvider` class located in the `SantanderTest.HackerAPI.Tests` namespace.

## Project Structure

- `SantanderTest.HackerAPI`: Main project containing the API implementation.
- `SantanderTest.HackerAPI.Model`: Contains the data models.
- `SantanderTest.HackerAPI.Services`: Contains the service classes and constants.
- `SantanderTest.HackerAPI.Tests`: Contains the unit tests.

## Dependencies

- .NET 8
- Microsoft.Extensions.Caching.Memory
- Microsoft.Extensions.Logging
- Moq
- NUnit
- FluentAssertions

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the MIT License.
