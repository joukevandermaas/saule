## Master (upcoming 1.8)

- [**BUGFIX**] Removing port from the URL if the port is a standard port (#206 by @sergey-litvinov-work)
- [**FEATURE**] Added support for searching of multiple fields via the filter query parameter (#205 by @phyberapex)
- [**FEATURE**] Added support for using the attributes (DisableDefaultIncludedAttribute, AllowsQueryAttribute, and PaginatedAttribute ) on class level (#204 by @dejarp)
- [**FEATURE**] Added new LinkType (LinkType.TopSelf) to only include a self link for the top level element (#202 by @sergey-litvinov-work)
- [**FEATURE**] Added sparse fieldset to the query parameter for queryable endpoints (#199 by @phyberapex)
- [**BUGFIX**] Support camel case serialization for relationship names

## Version 1.7.1

- [**BUGFIX**] Support camel case serialization for nested properties in attributes and meta hashes

## Version 1.7

- [**BUGFIX**] Serialise complex objects correctly (#149 by @laurence79)
- [**BUGFIX**] Do not require Accept header (#151 by @bjornharrtell)
- [**FEATURE**] Optional links support (#153 by @bjornharrtell)
- [**BUGFIX**] Fix regression missing 'data' key for relationships when using DisableDefaultIncluded (#155 by @bjornharrtell)
- [**FEATURE**] Add support for metadata (#158)
- [**BUGFIX**] Omit data for relationship objects not existing as property in the original model (#161 by @bjornharrtell)
- [**FEATURE**] Pagination page size (#165 by @erikhejl, #170 by @sergey-litvinov-work)
- [**FEATURE**] Configuration improvments (formatter can be only add at end of the collection, without deleting other formatters) (#168 by @tomasjurasek)
- [**FEATURE**] Functionality to return list of HttpErrors instead of just one error (#172 by @sergey-litvinov-work)
- [**FEATURE**] Added deserialization method to `JsonApiSerializer<T>` (#179 @madsphi)
- [**FEATURE**] Provide ability to handle JsonApi parameters in WebApi action itself manually (#181 by @sergey-litvinov-work)
- [**MAINTENANCE**] Upgrade xunit dependencies (#183 by @bjornharrtell)
- [**BUGFIX**] Fix regression with omitted data (#184 by @bjornharrtell)
- [**FEATURE**] Opt into JsonApi per webapi endpoint (#187, #189 by @barsh)
- [**FEATURE**] Option to serialize attributes in camelCase (#190 by @barsh)

## Version 1.6

- [**BUGFIX**] Support belongsTo relationships when data is null (#124 by @goo32)
- [**BUGFIX**] Use included resource when serialising its links (#130 by @laurence79)
- [**BUGFIX**] Serialize everything with kebab case by default (#132)
- [**BUGFIX**] Return reasonable errors for invalid content (#133)
- [**FEATURE**] Related resources (#137 by @bjornharrtell and @yohanmishkin)
- [**FEATURE**] Expose properties of resource (#142 by @bjornharrtell)
- [**MAINTENANCE**] Upgrade dependencies (#143 by @bjornharrtell)
- [**BUGFIX**] Prevent a resource being included in both the data and included sections (#147 by @laurence79)
- [**MAINTENANCE**] Move documentation to Github Pages (#148 by @adamalesandro)

## Version 1.5.1

- [**BUGFIX**] Convert case for relationship as well in ResourceDeserializer (#117 by @bxh)

## Version 1.5

- [**FEATURE**] Make configuration more convenient & flexible (#101)
- [**FEATURE**] Support recursively nested objects in the `included` array (#110 by @rhyek)
- [**BUGFIX**] Don't handle non JSON API requests (#118 by @nukefusion)

## Version 1.4.2

- [**BUGFIX**] GUID ids do not work for relationships (#96)

## Version 1.4.1

- [**REGRESSION**] `HttpError` not passed through; Saule specific error is serialized instead.

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
