PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
) WITHOUT ROWID, STRICT;

BEGIN TRANSACTION;

CREATE TABLE "Blogs" (
    "BlogId" BLOB NOT NULL CONSTRAINT "PK_Blogs" PRIMARY KEY,
    "Url" TEXT NOT NULL,
    "Created" INT NOT NULL
) WITHOUT ROWID, STRICT;

CREATE TABLE "Posts" (
    "PostId" BLOB NOT NULL CONSTRAINT "PK_Posts" PRIMARY KEY,
    "Title" TEXT NOT NULL,
    "Content" TEXT NOT NULL,
    "BlogId" BLOB NOT NULL,
    CONSTRAINT "FK_Posts_Blogs_BlogId" FOREIGN KEY ("BlogId") REFERENCES "Blogs" ("BlogId") ON DELETE CASCADE
) WITHOUT ROWID, STRICT;

CREATE INDEX "IX_Posts_BlogId" ON "Posts" ("BlogId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241013215938_001_InitialTables', '8.0.10');

COMMIT;