using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Reflection;

namespace Contoso.Mail.Data
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
      var dbPath = FindSqlFile().Replace("db.sql", "contoso.db");
      var connectionString = $"Data Source={dbPath}";

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
      var sqlFile = "Data/db.sql";
      var execDirectory = Directory.GetCurrentDirectory();

      // Check current directory
      var filePath = Path.Combine(execDirectory, sqlFile);
      if (File.Exists(filePath))
        return filePath;

      // Safely navigate up to project root
      var directory = new DirectoryInfo(execDirectory);
      for (int i = 0; i < 3 && directory != null; i++)
      {
        directory = directory.Parent;
      }

      if (directory != null)
      {
        filePath = Path.Combine(directory.FullName, sqlFile);
        if (File.Exists(filePath))
          return filePath;
      }

      // If we're here, we couldn't find the file
      throw new FileNotFoundException($"Could not locate SQL file: {sqlFile}");
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