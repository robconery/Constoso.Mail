using Dapper;
namespace Contoso.Mail.Models;

[Table("broadcasts")]
public class Broadcast
{
  public int? ID { get; set; }
  public int? EmailId { get; set; }
  public string Status { get; set; } = "pending";
  public string Name { get; set; } = string.Empty;
  public string? Slug { get; set; }
  public string? ReplyTo { get; set; }
  public string SendToTag { get; set; } = "*";
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}