## Adding metadata

[Back to home](index)

----

> **Note**: You need at least Saule 1.7 for this to work.

Saule allows you to add metadata to responses that are returned from your API. To
enable this for a resource, you need to override the `GetMetadata` method on your
`ApiResource`:

```csharp
public class PersonResource : ApiResource
{
    public override object GetMetadata(object response, Type resourceType, bool isEnumerable)
    {
        if (resourceType != typeof(Person))
        {
            return null;
        }

        if (isEnumerable)
        {
            var people = (IEnumerable<Person>) response;

            return new CollectionMetadata
            {
                Total = people.Count()
            }
        }
        else
        {
            var person = (Person)response;

            return new PersonMetadata
            {
                NumberOfFriends = friends.Count(),
                NumberOfFamilyMembers = family.Count()
            };

        }
    }
}
```

`GetMetadata` takes three parameters:

- `response`: The response object you returned from your action method (after filtering and pagination have been applied).
- `resourceType`: The type of the response you returned from your action method. If you returned a collection, this is the type of the items in the collection.
- `isCollection`: `true` if you returned an enumerable (type that derives from `IEnumerable`) from your action method, otherwise `false`.

Any object you return from this method will be serialized and included in the root object of the JSON API response under the `meta` property.
If you want to serialize the object in a different way (e.g. `camelCased` properties), you can return a `JToken` from this method. The `JToken`
will not be changed, but directly included in the response.