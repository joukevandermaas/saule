---
title: Getting started
resource: true
---

First install Saule using Nuget:

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

[Learn to customize the serialization process](4-customizing-serialization)

---

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
[Learn more about defining resource models](2-basics-resource-models)

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
and receiving JSON API!