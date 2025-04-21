using Dapper;
namespace Contoso.Mail.Models;

public class SignUpRequest
{
  public string Name { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
}
[Table("contacts")]
public class Contact
{
  public string Name { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public bool Subscribed { get; set; }
  public string Key { get; set; } = Guid.NewGuid().ToString();
  public int? ID { get; set; }
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

  public Contact()
  {

  }
  public Contact(string name, string email)
  {
    Name = name;
    Email = email;
  }
}