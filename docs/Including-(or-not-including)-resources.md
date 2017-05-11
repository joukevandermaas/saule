## Including (or not including) resources

[Back to home](index)

----

> **Note**: You need at least Saule 1.6 for this to work.

By default, Saule includes all related resources into the generated JSON. If you do not want this, you can add the `DisableDefaultIncludedAttribute` to your action method:

```cs
[DisableDefaultIncluded]
[ReturnsResource(typeof(PersonResource))]
[HttpGet]
public IEnumerable<Person> Get()
{
    return GetThePeople();
}
```

This makes it so all resources are only included when requested using the [`include` query parameter](http://jsonapi.org/format/#fetching-includes). Clients can now specify explicitly which relationships to include (whitelist).

If you specify the `AllowsQueryAttribute`, clients can still specify the `include` query parameter to control what to include:

```cs
[AllowsQuery]
[ReturnsResource(typeof(PersonResource))]
[HttpGet]
public IEnumerable<Person> Get()
{
    return GetThePeople();
}
```

A request to `/people?include=address,friends` will now *only* include the `address` and `friends` relationships, nothing else (such as `job`). However, if the client does not specify any `include` parameter, all related resources will still be included.