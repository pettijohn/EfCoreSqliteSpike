using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class BloggingContext  : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    public string DbPath { get; }

    public BloggingContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "spike.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath};foreign keys=true")
            .AddInterceptors(new SqliteDatatypesCommandInterceptor());

    // TODO how do I prepend each (each connection) with PRAGMA foreign_keys = ON;
    // https://stackoverflow.com/a/6419268/435368

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetEpochConverter>();
    }
}


// https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions
public class DateTimeOffsetEpochConverter : ValueConverter<DateTimeOffset, long>
{
    public DateTimeOffsetEpochConverter()
        : base(
            v => v.ToUnixTimeMilliseconds(),
            v => DateTimeOffset.FromUnixTimeMilliseconds(v))
    {
    }
}

// https://github.com/dotnet/EntityFramework.Docs/blob/main/samples/core/Miscellaneous/CommandInterception/TaggedQueryCommandInterceptor.cs
public class SqliteDatatypesCommandInterceptor : DbCommandInterceptor
{
    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        ManipulateCommand(command);

        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        ManipulateCommand(command);

        return new ValueTask<InterceptionResult<DbDataReader>>(result);
    }

    private static void ManipulateCommand(DbCommand command)
    {
        var dbparams = command.Parameters as SqliteParameterCollection;
        if (dbparams == null) throw new Exception("Expected Sqlite database");
        foreach(var param in dbparams)
        {
            var sqliteParam = (Microsoft.Data.Sqlite.SqliteParameter)param;
            if(sqliteParam.Value is Guid)
            {
                sqliteParam.SqliteType = SqliteType.Blob;
                //dbparams[dbparams.IndexOf(param)].SqliteType = SqliteType.Blob;
            }
        }
    }
}

public class Blog
{
    public Guid BlogId { get; set; }
    public string Url { get; set; }

    public List<Post> Posts { get; } = new();

    public DateTimeOffset? Created { get; set; }
}

public class Post
{
    public Guid PostId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }

    public Guid BlogId { get; set; }
    public Blog? Blog { get; set; }
}