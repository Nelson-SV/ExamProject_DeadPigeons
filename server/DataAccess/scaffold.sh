 run #!/bin/bash

connectionString="HOST=localhost;DB=testdb;UID=testuser;PWD=testpass;PORT=5432;"
context="AppDbContext"

# Scaffold the database context and models
dotnet ef dbcontext scaffold \
  "$connectionString" \
  Npgsql.EntityFrameworkCore.PostgreSQL \
  --output-dir Models \
  --context-dir . \
  --context "$context" \
  --no-onconfiguring \
  --data-annotations \
  --force

# Generate partial AppDbContext file
pre="
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace DataAccess;
public partial class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
"
dbsets=$(grep DbSet "$context.cs" | grep -v AspNet)
post="}"

# Write the content to the context file
echo -e "$pre" "$dbsets" "$post" > "$context.cs"

# Format the generated code
dotnet format
