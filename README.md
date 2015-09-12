# Saule
Saule is an Json Api (version 1.0) library for ASP.Net Web API 2.

To use Saule, you must define resources that contain the information
about your domain:
```c#
public class PersonResource : ApiResource 
{
  public PersonResource()
  {
    WithAttribute("FirstName");
    WithAttribute("LastName");
    WithAttribute("Age");

    BelongsTo("Job", typeof(CompanyResource));
    HasMany("Friends", typeof(PersonResource));
  }
}
public class CompanyResource : ApiResource
{
  public CompanyResource()
  {
    WithAttribute("Name");
    WithAttribute("NumberOfEmployees");
  }
}
```

You can then use these to serialize any class into json api
(as long as your class has properties with the same names as
in your model):
```c#
public class PersonController : ApiController
{
  [HttpGet] 
  [ApiResource(typeof(PersonResource)]
  [Route("people/{id}")]
  public JohnSmith GetPerson(string id)
  {
    return new JohnSmith();
  }
}
```

```json
GET people/123

{
  "data": {
    "type": "person",
    "id": "123",
    "attributes": {
      "first-name": "John",
      "last-name": "Smith",
      "age": 45
    },
    "relationships": {
      "job": {
        "links": {
          "self": "people/123/relationships/job",
          "related": "people/123/job"
        },
        "data" {
          "type": "company",
          "id": "345"
        }
      },
      "friends": {
        "links": {
          "self": "people/123/relationships/friends",
          "related": "people/123/friends"
        }
      }
    }
  },
  "included": [
    {
      "type": "company",
      "id": "345",
      "attributes": {
        "name": "Awesome Company",
        "number-of-employees": 33
      }
    }
  ]
}
```

Deserialization works just like in normal Web Api; you don't need
to do anything special to make this work.
