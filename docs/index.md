Saule is a Json Api (version 1.0) library for ASP.Net Web API 2. Its goal is to
get in your way as little as possible, so you can use your existing knowledge
and code in Web Api.

## Getting started

First install Saule using nuget:

```
Install-Package saule
```

To get started with Saule, add the following line to your `Startup.cs`'s
`Register` method:

```csharp
public static void Register(HttpConfiguration config)
{
    // ...

    config.ConfigureJsonApi();

    // ...
}
```

[Learn to customize the serialization process](Customizing-the-serialization-process)

---
> **Note**: if you are using an older (< 1.4) version of Saule, add the following lines instead:
>
> 

```csharp
public static void Register(HttpConfiguration config)
{
    // ...

     config.Formatters.Clear();
     config.Formatters.Add(new JsonApiMediaTypeFormatter());

    // ...
}
 ```

Saule requires you to define models for the resources you want to return from
your api.  This allows for you to customize the data your api returns without
cluttering your business logic with custom attributes. To define a model,
create a class that derives from `Saule.ApiResource`:

```csharp
public class PersonResource : ApiResource
{
    public PersonResource()
    {
        Attribute("Name");
        Attribute("Age");

        BelongsTo<CompanyResource>("job");
    }
}

public class CompanyResource : ApiResource
{
    public CompanyResource()
    {
        Attribute("Name");
        Attribute("Location")
    }
}
```
[Learn more about defining resource models](Resource-models)

To tell Saule that a particular controller method returns a specific resource,
add a `ReturnResourceAttribute`. If you put this attribute on the controller,
it will apply to all action methods within.

```csharp
public class PeopleController : ApiController
{
    [ReturnsResource(typeof(PersonResource))]
    public IQueryable<Person> Get()
    {
        return People.FindAll()
    }
}
```

That's it! You can now start using Web API as you normally would, while sending
and receiving Json Api!

__For more information, see links below.__

[Learn more about defining resource models](Resource-models)

[Learn to customize the serialization process](Customizing-the-serialization-process)

[Generating links](Generating-links)

[Pagination](Pagination)

[Queryable endpoints](Queryable-endpoints)

[Including (or not including) resources](Including-(or-not-including)-resources)
