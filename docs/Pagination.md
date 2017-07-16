## Pagination

[Back to home](index)

----

Adding pagination to an action method's results is as easy as adding an attribute:

```csharp
public class PeopleController : ApiController
{
    [Paginate(PerPage = 25, PageSizeLimit = 100)]
    [ReturnsResource(typeof(PersonResource))]
    public IQueryable<Person> Get()
    {
        return People.FindAll()
    }
}
```

This will generate `next`, `prev` and `first` links in your responses, and interpret the page[number]
and page[size] query parameter when appropriate.

Saule will use LINQ to query the `IQueryable<T>` you return, so pagination is not done in memory.
If you return an `IEnumerable<T>`, the query is executed in memory instead. Note that Saule does
not support the non-generic versions of `IQueryable` and `IEnumerable`.

Default values for pagination can be provided with the `JsonApiConfiguration` for your application.  When the optional PerPage or PageSizeLimit values are omitted from the Paginate attribute, those settings are taken from the `JsonApiConfiguration` defaults:

```csharp
public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        // Web API routes
        config.MapHttpAttributeRoutes();

        config.ConfigureJsonApi(new JsonApiConfiguration
        {
            PaginationConfig = new PaginationConfig
            {
                DefaultPageSize = 25,
                DefaultPageSizeLimit = 100
            }
        });
    }
}
```

If `PaginationConfig` is not specified for the `JsonApiConfiguration`, the default page size is 10 and there is no default page size limit.
