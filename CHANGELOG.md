## Version 1.4

- [**FEATURE**] Filtering of attributes through user queries
  - You can specify the expression that will be executed for specific types, allowing
    e.g. case-insensitive filtering, and much more.
- [**FEATURE**] Custom properties to specify the Id of a resource (using `WithId`)
- [**FEATURE**] New way to set up Saule: use the extension method `ConfigureJsonApi` 
  instead of manually adding the `JsonApiMediaTypeFormatter`.
- [**FEATURE**] Better response code handling; Saule will now always send a 4xx or 5xx when an exception occurs
  (requires the new setup)
- [**BUGFIX**] Saule now supports recursive object graphs
- [**BUGFIX**] Saule can now be installed in .NET 4.5 projects
- [**BUGFIX**] Iconsistency between top-level `self` link and generated urls. If you don't specify an
  url path builder, the path namespace is now automatically guessed for you.
