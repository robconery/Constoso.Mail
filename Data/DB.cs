using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Reflection;
using System.IO;

namespace Contoso.Data
{
  public class CustomResolver : SimpleCRUD.IColumnNameResolver
  {
    public string ResolveColumnName(PropertyInfo propertyInfo)
    {
      return propertyInfo.Name.ToSnakeCase();
    }
  }

  public interface IDb
  {
    IDbConnection Connect();
  }

  public class DB : IDb
  {
    private static readonly string LocalDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "contoso.db");

    public IDbConnection Connect()
    {
      return DB.Sqlite();
    }

    public static IDbConnection Sqlite()
    {

      // Ensure the directory exists
      Directory.CreateDirectory(Path.GetDirectoryName(LocalDbPath));
      var connectionString = $"Data Source={LocalDbPath}";

      Dapper.SimpleCRUD.SetDialect(Dapper.SimpleCRUD.Dialect.SQLite);
      var resolver = new CustomResolver();
      SimpleCRUD.SetColumnNameResolver(resolver);
      var conn = new SqliteConnection(connectionString);
      conn.Open();

      // Initialize the database if it's a new connection
      InitializeDatabase(conn);

      return conn;
    }

    public static IDbConnection InMemorySqlite()
    {
      Dapper.SimpleCRUD.SetDialect(Dapper.SimpleCRUD.Dialect.SQLite);
      var resolver = new CustomResolver();
      SimpleCRUD.SetColumnNameResolver(resolver);

      // Create an in-memory connection for testing
      var conn = new SqliteConnection("Data Source=:memory:");
      conn.Open();

      // Initialize the database schema
      InitializeDatabase(conn);

      return conn;
    }

    private static string FindSqlFile()
    {
      var sqlFile = "Data/db_sqlite.sql";
      var execDirectory = Directory.GetCurrentDirectory();
      //do we have an Data directory here?
      var filePath = Path.Combine(execDirectory, sqlFile);
      if (File.Exists(filePath)) return filePath;

      //project root
      string projectDirectory = Directory.GetParent(execDirectory).Parent.Parent.FullName;
      filePath = Path.Combine(projectDirectory, sqlFile);
      if (File.Exists(filePath)) return filePath;

      return "";
    }

    private static void InitializeDatabase(IDbConnection connection)
    {
      // Read and execute the SQL schema from the project root

      var sqlFilePath = FindSqlFile();

      if (File.Exists(sqlFilePath))
      {
        string sql = File.ReadAllText(sqlFilePath);
        var statements = sql.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var statement in statements)
        {
          if (!string.IsNullOrWhiteSpace(statement))
          {
            connection.Execute(statement);
          }
        }
      }
      else
      {
        throw new InvalidOperationException($"Can't find sql file:  {sqlFilePath}");
      }
    }
  }
}