## Generating links

[Back to home](index)

----

> **Note**: You need at least Saule 1.2 for this to work.

Saule lets you customize the links that are generated during serialization. It
uses the `IUrlPathBuilder` interface to generate all urls. You can provide an 
implementation of this interface to the `ConfigureJsonApi` extension method on `HttpConfiguration`:

```csharp
config.ConfigureJsonApi(new JsonApiConfiguration {
    UrlPathBuilder = new DefaultUrlPathBuilder()
});
```

If you want to add a prefix to all urls, you can do so in the constructor:

```csharp
var prefixedUrls = new DefaultUrlPathBuilder("/api");
```

Saule comes with two implementations of `IUrlPathBuilder`:

Link type|`DefaultUrlPathBuilder`|`CanonicalUrlPathBuilder`
---|---|---
Collection of resources|`/people/`|`/people/`
Individual resource|`/people/123/`|`/people/123/`
Related resource|`/people/123/employer/`|`/companies/456/`
Related resource's self link|`/people/123/relationships/employer/`|`/people/123/relationships/employer/`

If you want to customize generated links beyond this, you can do so by extending
one of the above implementations or by implementing the interface from scratch.
The `IUrlPathBuilder` interface consists of four methods, which correspond to the
link types in the table above:

```csharp
public interface IUrlPathBuilder
{
    // collection of resources
    string BuildCanonicalPath(ApiResource resource);

    // individual resource
    string BuildCanonicalPath(ApiResource resource, string id);

    // related resource
    string BuildRelationshipPath(ApiResource resource, string id, ResourceRelationship relationship);

    // related resource self link
    string BuildRelationshipPath(ApiResource resource, string id, ResourceRelationship relationship, string relatedResourceId);
}
```

The included implementations use these methods internally as well; if you override
`BuildCanonicalPath(ApiResource)`, it will affect the result of all other methods. 