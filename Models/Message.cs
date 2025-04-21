using Dapper;

namespace Contoso.Mail.Models;

[Table("messages")]
public class Message
{
  [Key]
  public int? ID { get; set; }
  public string Source { get; set; } = "broadcast";
  public string Slug { get; set; }
  public string Status { get; set; } = "pending";
  public string SendTo { get; set; }
  public string SendFrom { get; set; } = "noreply@Contoso.dev";
  public string Subject { get; set; }
  public string Html { get; set; }
  //public DateTimeOffset SendAt { get; set; }
  public DateTimeOffset SendAt { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset SentAt { get; set; }
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public Message(string slug, string sendTo, string subject, string html)
  {
    SendTo = sendTo;
    Slug = slug;
    Subject = subject;
    Html = html;
  }
  public Message()
  {

  }
  public void Sent()
  {
    this.Status = "sent";
    this.SentAt = DateTimeOffset.UtcNow;
  }
  public bool ReadyToSend()
  {
    return this.Status == "pending" &&
      //this.SendAt <= DateTimeOffset.Now &&
      !String.IsNullOrEmpty(this.SendTo) &&
      !String.IsNullOrEmpty(this.SendFrom) &&
      !String.IsNullOrEmpty(this.Html) &&
      !String.IsNullOrEmpty(this.Subject);
  }
}