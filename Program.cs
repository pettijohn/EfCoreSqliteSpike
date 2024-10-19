using System.Linq;
using Microsoft.EntityFrameworkCore;

using var db = new BloggingContext();

// Note: This sample requires the database to be created before running.
Console.WriteLine($"Database path: {db.DbPath}.");

// Create
Console.WriteLine("Inserting a new blog");
db.Add(new Blog { Url = "UtcNow", Created = DateTimeOffset.UtcNow });
db.SaveChanges();

// Read
// Console.WriteLine("Querying for a blog");
// var blogID = new Guid("A79EC554E5AEE04F938026D20F54A316");
// var result = db.Blogs.Single(b => b.BlogId == blogID);
// // var result = db.Blogs
// //     .OrderBy(b => b.BlogId)
// //     .First();
// Console.WriteLine(result.Url);


var blog = db.Blogs.ToList().First();
Console.WriteLine(blog.BlogId);
Console.WriteLine(blog.Created!.Value.ToLocalTime());

try
{
    // Try to insert a Post that violates FK constraint
    var bytes = Guid.NewGuid().ToByteArray();
    var blogs = db.Blogs
        .FromSqlInterpolated($@"INSERT INTO POSTS (PostId, Title, Content, BlogId)
    VALUES ({bytes}, 'foo', 'bar', {bytes})")
        .ToList();
}
catch (Microsoft.Data.Sqlite.SqliteException ex)
{
    if (ex.Message.Contains("FOREIGN KEY"))
        Console.WriteLine("FK Constraint successful");
    else
        throw;
}
// var result = db.Blogs.Single(b => b.BlogId == blog.BlogId);

// Console.WriteLine(new Guid(Convert.FromHexString("A79EC554E5AEE04F938026D20F54A316")));