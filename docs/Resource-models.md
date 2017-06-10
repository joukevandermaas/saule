## Resource Models

[Back to home](index)

## Attributes
Saule uses resource models to serialize any object into a
a set of predefined resources. To define such a model, create
a class that derives from `Saule.ApiResource`:

```csharp
public class CompanyResource
{
    public CompanyResource()
    {
        Attribute("Name");
        Attribute("Location");
    }
}
```
When serializing a type, Saule will try to find properties with
the same name as the attributes, and add them to the response
if they exist. It will never add properties that don't have an
attribute or relationship definition.

## Relationships

Saule allows you to define two different kinds of relationships:
to-one and to-many. These relationships will be serialized with
a `links` property, so your clients can find the resource based
on the response. To define relationships, use the `BelongsTo` or
`HasMany` methods in your resource's constructor:
```csharp
public class CompanyResource
{
    public CompanyResource()
    {
        BelongsTo<CountryResource>("Country"); // /companies/123/country
        HasMany<PersonResource>("Employees");  // /companies/123/employees
    }
}
```

If you want, you can customize the url that Saule generates for these
relationships by providing a second parameter:
```csharp
public class CompanyResource
{
    public CompanyResource()
    {
        BelongsTo<CountryResource>("Country", "/origin");  // /companies/123/origin
        HasMany<PersonResource>("Employees", "/workers");  // /companies/123/workers
    }
}
```


## Type name & custom Id properties

When using the resource from within a `ReturnsResourceAttribute`, Saule will create
Json Api with the type `company` (or whatever the name of your resource class is). 
To customize this, use the `OfType` method in your constructor:

```csharp
public class CompanyResource
{
    public CompanyResource()
    {
        OfType("Coorporation");

        Attribute("Name");
        Attribute("Location");
    }
}
```
```
{
  "data": {
    "type": "coorporation",
    ...
  }
}
```

You can also customize the property Saule uses to determine the id for
a resource. By default, it will use a property named `Id`. To customize
this behavior, use the `WithId` function in your constructor:

```csharp
public class CompanyResource
{
    public CompanyResource()
    {
        WithId("CompanyId");

        // ...
    }
}
```

## Customizing self links

If you want to influence the self links that are generated for each
resource, you can use the overload that lets you specify the path of
the url for a resource:

```csharp
public class CompanyResource
{
    public CompanyResource()
    {
        OfType("Coorporation", "/companies");

        Attribute("Name");
        Attribute("Location");
    }
}
```
```
{
  "data": {
    "type": "coorporation",
    "links": {
      "self": "http://example.com/api/companies"
    }
    ...
  }
}
```

If you want to customize the generated links objects further, see 
[Generating links](Generating-links).