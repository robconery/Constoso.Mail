using Dapper;
using Contoso.Data;

namespace Contoso.Mail.Models;

[Table("activity")]
public class Activity
{
  public int? ID { get; set; }
  public int? ContactId { get; set; }
  public string Key { get; set; } = Guid.NewGuid().ToString();
  public string? Description { get; set; }
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public Activity()
  {

  }

}