# Saule
[![Build status](https://ci.appveyor.com/api/projects/status/uj3ddt85jaebjuh9/branch/master?svg=true)](https://ci.appveyor.com/project/JoukevanderMaas/saule/branch/master)


Saule is a JSON API (version 1.0) library for ASP.Net Web API 2. Install Saule using NuGet:

```
Install-Package saule
```

### [Visit the documentation website](http://joukevandermaas.github.io/saule)

To use Saule, you must define resources that contain the information
about your domain:
```c#
public class PersonResource : ApiResource
{
    public PersonResource()
    {
        Attribute("FirstName");
        Attribute("LastName");
        Attribute("Age");

        BelongsTo<CompanyResource>("Job");
        HasMany<PersonResource>("Friends");
    }
}
public class CompanyResource : ApiResource
{
    public CompanyResource()
    {
        Attribute("Name");
        Attribute("NumberOfEmployees");
    }
}
```

You can then use these to serialize any class into Json Api
(as long as your class has properties with the same names as
in your model):
```c#
public class PersonController : ApiController
{
    [HttpGet]
    [ReturnsResource(typeof(PersonResource))]
    [Route("people/{id}")]
    public JohnSmith GetPerson(string id)
    {
        return new JohnSmith();
    }
}
```

```json
GET http://example.com/people/123

{
  "data": {
    "type": "person",
    "id": "123",
    "attributes": {
      "first-name": "John",
      "last-name": "Smith",
      "age": 34
    },
    "relationships": {
      "job": {
        "links": {
          "self": "http://example.com/people/123/relationships/job/",
          "related": "http://example.com/people/123/job/"
        },
        "data": {
          "type": "company",
          "id": "456"
        }
      },
      "friends": {
        "links": {
          "self": "http://example.com/people/123/relationships/friends/",
          "related": "http://example.com/people/123/friends/"
        },
        "data": [
          {
            "type": "person",
            "id": "789"
          }
        ]
      }
    }
  },
  "included": [
    {
      "type": "company",
      "id": "456",
      "attributes": {
        "name": "Awesome, Inc.",
        "number-of-employees": 24
      }
    },
    {
      "type": "person",
      "id": "789",
      "attributes": {
        "first-name": "Sara",
        "last-name": "Jones",
        "age": 38
      }
    }
  ],
  "links": {
    "self": "http://example.com/people/123"
  }
}
```

Deserialization works just like in normal Web API; you don't need
to do anything special to make this work.

### Creating a new release

Follow the steps below to create a new release:

1. Create a branch called `release-v<version>` (e.g. `release-v1.5`)
2. Increase the version number in `appveyor.yml` in `master`
3. Push both changes and wait for the build
4. Copy the release notes into the release description on Github
5. Publish the new release