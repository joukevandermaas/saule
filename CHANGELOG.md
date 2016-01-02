## Version 1.4

- Filtering of attributes through user queries
  - You can specify the expression that will be executed for specific types, allowing
    e.g. case-insensitive filtering, and much more.
- Custom properties to specify the Id of a resource (using `WithId`)
- New way to set up Saule: use the extension method `ConfigureJsonApi` 
  instead of manually adding the `JsonApiMediaTypeFormatter`.
