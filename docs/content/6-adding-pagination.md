---
title: Adding pagination
resource: true
---

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
