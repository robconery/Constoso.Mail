using System.Data;
using Contoso.Mail.Data;

public abstract class TestBase : IDisposable
{
  public IDbConnection Conn { get; set; }
  protected TestBase()
  {
    Viper.Test();
    Conn = DB.InMemorySqlite();
  }

  public void Dispose()
  {
    // Do "global" teardown here; Called after every test method.
    Conn.Close();
    Conn.Dispose();
  }
}