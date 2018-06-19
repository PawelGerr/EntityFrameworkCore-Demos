# Demos for blog posts abount Entity Framework Core

## N+1 Queries Problem
See the method `ExecuteDemoDbQueries` in `Program.cs`

Blog posts:
* [Entity Framework Core Performance: Beware of N+1 Queries](http://weblogs.thinktecture.com/pawel/2018/04/entity-framework-core-performance-beware-of-n1-queries.html)
* [Entity Framework Core 2.1 Performance: Beware of N+1 Queries (Revisited)](http://weblogs.thinktecture.com/pawel/2018/05/entity-framework-core-21-performance-beware-of-n1-queries.html)

## Inheritance

Blog posts:
* [Entity Framework Core: Inheritance - Table-per-Type (TPT) is not supported, is it? (Part 1 - Code First)](http://weblogs.thinktecture.com/pawel/2018/05/entity-framework-core-inheritance-tpt-is-not-supported-is-it-part-1-code-first.html)
* [Entity Framework Core: Inheritance - Table-per-Type (TPT) is not supported, is it? (Part 2 - Database First)](http://weblogs.thinktecture.com/pawel/2018/05/entity-framework-core-inheritance-table-per-type-tpt-is-not-supported-is-it-part-2-database-first.html)

### Table-Per-Hierarchy (TPH)
**Code First**: see the method `ExecuteTphQueries` in `Program.cs` and the file in `/src/EntityFramework.Demo/TphModel/CodeFirst`

### Table-Per-Type (TPT)
**Code First**: see the method `ExecuteTptQueries` in `Program.cs` and the file in `/src/EntityFramework.Demo/TptModel/CodeFirst`

## Changing Schema at Runtime
See the method `ExecuteSchemaChangeQueries` in `Program.cs`

Blog posts:
* [Entity Framework Core: Changing Database Schema at Runtime](http://weblogs.thinktecture.com/pawel/2018/06/entity-framework-core-changing-database-schema-at-runtime.html )
* [Entity Framework Core: Changing DB Migration Schema at Runtime](http://weblogs.thinktecture.com/pawel/2018/06/entity-framework-core-changing-db-migration-schema-at-runtime.html)

## Issues with TransactionScopes
See the method `ExecuteTransactionScopeDemosAsync` in `Program.cs`

Blog posts: [Entity Framework Core: Use TransactionScope with Caution!](http://weblogs.thinktecture.com/pawel/2018/06/entity-framework-core-use-transactionscope-with-caution.html)
