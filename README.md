# EF Core Sqlite 

https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/types

## Storing Guid as BLOB 

See `SqliteDatatypesCommandInterceptor.ManipulateCommand`; it modifies `SqliteParameter.SqliteType = SqliteType.Blob;`. This is a supported datatype for `Microsoft.Data.Sqlite` so everything else magically works. 

Note there is questionable value in doing this. Compared to storing Guid as a String, insert and select performance are effectively identical, while size on disk is double for Strings. The biggest downside to storing as blob is that it is difficult to get human-readable Guids back out of the database, especially on the command line `sqlite3` and other tools. `SELECT quote(guidAsBlob)` returns a hex string of the byte array, but the sequence of bytes is incorrect (different endianness). 

```
substr(hex(blogid), 7, 2) || substr(hex(blogid), 5, 2) || substr(hex(blogid), 3, 2) || substr(hex(blogid), 1, 2) || '-' || substr(hex(blogid), 11, 2) || substr(hex(blogid), 9, 2) || '-' || substr(hex(blogid), 15, 2) || substr(hex(blogid), 13, 2) || '-' || substr(hex(blogid), 17, 4) || '-' || substr(hex(blogid), 21, 12)
```

## Storing DateTimeOffset as INT (Long, Unix Epoch) 

See `DateTimeOffsetEpochConverter`. This is not a built-in `Microsoft.Data.Sqlite` supported type (it stores as TEXT or REAL Julian date), so we convert it to a `long`. This value converter avoids the pattern of having a DateTimeOffset property that EF ignores and a Long parameter mapped to the underlying table. 

## Enforcing Foreign Keys

Connection string must include `";foreign keys=true"`. Sqlite's foreign key support is enforced at the connection level, not at the database level.

https://www.sqlite.org/foreignkeys.html