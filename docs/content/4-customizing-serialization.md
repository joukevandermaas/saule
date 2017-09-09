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
