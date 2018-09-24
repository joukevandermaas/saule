---
title: Customizing serialization
resource: true
---

To add serialization and deserialization to your Web Api project,
add the following lines to your `Startup.cs`:

```csharp
public static void Register(HttpConfiguration config)
{
    // ...

    config.ConfigureJsonApi();

    // ...
}
```

This will register Saule to serialize and deserialize every incoming
and outgoing request.

## Using camelCase for Attribute Names

Attribute names are serialized using dash/kebab notation.
Saule allows you to use camelCase as an alternative.

```csharp
public static void Register(HttpConfiguration config)
{
    // ...

    var jsonApiConfig = new JsonApiConfiguration
    {
        PropertyNameConverter = new CamelCasePropertyNameConverter()
    };
    config.ConfigureJsonApi(jsonApiConfig);
    
    // ...
}
```

## Return Json Api Responses

Saule follows the Json Api spec and only returns Json Api responses
when they are requested using the http request header:
`Accept: application/vnd.api+json`

This allows clients to opt-in to receiving responses in Json Api format.
You can configure Saule to send Json Api responses regardless of the
presence of the http request header.

There are few ways to accomplish this:

### Option 1

Configure Saule in your `Startup.cs` like this:

```csharp
public static void Register(HttpConfiguration config)
{
    // ...

    config.ConfigureJsonApi(new JsonApiConfiguration(), overwriteOtherFormatters: true);

    // ...
}
```

### Option 2

Add the `[JsonApi]` attribute to API controllers or methods.
This method allows you enable JsonApi responses for specific
end points.  For example, v1 responds with json or xml and v2
always responds with Json Api regardless of the
presence of the http request header.

```csharp
// Returns a JSON API response if http request includes header:
//      'Accept: application/vnd.api+json'
[Route("api/v1/companies")]
[Paginated(PerPage = 20)]
[AllowsQuery]
public IEnumerable<Company> GetV1()
{
    return GetCompanies();
}

// Adding [JsonApi] always returns a JSON API response
[JsonApi]
[Route("api/v2/companies")]
[Paginated(PerPage = 20)]        
[AllowsQuery]
public IEnumerable<Company> GetV2()
{
    return GetCompanies();
}
```

## Using `JsonConverter`

Saule allows you to specify any number of `JsonConverters`.
To e.g. serialize enums as strings, use:

```csharp
config.ConfigureJsonApi(new JsonApiConfiguration {
    JsonConverters = { new StringEnumConverter() }
});
```

## Creating an `ExceptionFilter`

When creating an exception filter, the normal formatter pipeline is skipped.
This means you'll need to manually add the `JsonApiMediaTypeFormatter` when
creating your response:
```csharp
sealed class MyExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(HttpActionExecutedContext context)
    {
        context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new ObjectContent(
                context.Exception.GetType(),
                context.Exception,
                new JsonApiMediaTypeFormatter())
        };
    }
}
```
The media type formatter will automatically serialize subclasses of `System.Exception`
or `System.Web.Http.HttpError` as a JSON API error response. No other types are supported
at this time.
