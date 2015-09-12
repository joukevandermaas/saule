# Saule
Saule is an Json Api (version 1.0) library for ASP.Net Web API 2.

To use Saule, you must define models that contain the information
about your domain:
```c#
public class PersonModel : ApiModel 
{
  public PersonModel()
  {
    Attribute("FirstName");
    Attribute("LastName");
    Attribute("Age");

    BelongsTo("Job", typeof(CompanyModel));
    HasMany("Friends", typeof(PersonModel));
  }
}
public class CompanyModel : ApiModel
{
  public CompanyModel()
  {
    Attribute("Name");
    Attribute("NumberOfEmployees");
  }
}
```

You can then use these to serialize any class into json api:
```c#
public class PersonController : ApiController
{
  [HttpGet, Route("people/{id}")]
  public ApiResponse<Person> GetPerson(string id)
  {
    return new Person().ToApiResponse(typeof(PersonModel));
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
