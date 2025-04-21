namespace Contoso.Mail.Services;
using System.Net.Mail;
using System.Net;
using Contoso.Mail.Models;
using Contoso.Data;
using Dapper;


public interface IEmailSender
{
  public Task<Message> Send(Message mssg);
  public Task<int> SendBulk(IEnumerable<Message> mssgs);
}

public class InMemoryEmailSender : IEmailSender
{
  private readonly List<Message> _sent = new List<Message>();
  public IEnumerable<Message> Sent => _sent;

  public Task<Message> Send(Message mssg)
  {
    mssg.Sent();
    _sent.Add(mssg);
    return Task.FromResult(mssg);
  }
  public Task<int> SendBulk(IEnumerable<Message> mssgs)
  {
    foreach (var mssg in mssgs)
    {
      mssg.Sent();
      _sent.Add(mssg);
    }
    return Task.FromResult(mssgs.Count());
  }
}
public class MailHogSender : IEmailSender
{
  private SmtpClient _client;
  public MailHogSender()
  {
    _client = new SmtpClient("localhost", 1025);
  }
  public async Task<Message> Send(Message mssg)
  {
    var sendMessage = new MailMessage
    {
      IsBodyHtml = true,
      Subject = mssg.Subject,
      Body = mssg.Html,
      From = new MailAddress(mssg.SendFrom),
    };
    sendMessage.To.Add(new MailAddress(mssg.SendTo));
    await _client.SendMailAsync(sendMessage);
    mssg.Sent();
    return mssg;
  }

  public async Task<int> SendBulk(IEnumerable<Message> mssgs)
  {
    var count = 0;
    await Parallel.ForEachAsync(mssgs, async (mssg, ct) =>
    {
      //the SMTP client is not reusable and can only send
      //one email at a time, which is OK cause we're going it in parallel.
      //Also: the db calls will get whacked out unless we explicitly open a new connection
      using var client = new SmtpClient("localhost", 1025);
      using var conn = new DB().Connect();
      var sendMessage = new MailMessage
      {
        IsBodyHtml = true,
        Subject = mssg.Subject,
        Body = mssg.Html,
        From = new MailAddress(mssg.SendFrom),
      };
      sendMessage.To.Add(new MailAddress(mssg.SendTo));
      await client.SendMailAsync(sendMessage);
      mssg.Sent();
      await conn.UpdateAsync(mssg);
      Interlocked.Increment(ref count);
    });
    return count;
  }
}
public class SmtpEmailSender : IEmailSender
{
  private SmtpClient _client;
  public SmtpEmailSender()
  {
    var config = Viper.Config();
    var host = config.Get("SMTP_HOST");
    var user = config.Get("SMTP_USER");
    var pw = config.Get("SMTP_PASSWORD");
    var port = 465;
    _client = new SmtpClient(host, port);
    _client.Credentials = new NetworkCredential(user, pw);
    _client.UseDefaultCredentials = false;
    _client.EnableSsl = true;
  }

  public async Task<Message> Send(Message mssg)
  {
    var sendMessage = new MailMessage
    {
      IsBodyHtml = true,
      Subject = mssg.Subject,
      Body = mssg.Html,
      From = new MailAddress(mssg.SendFrom),
    };
    sendMessage.To.Add(new MailAddress(mssg.SendTo));
    await _client.SendMailAsync(sendMessage);
    mssg.Sent();
    return mssg;
  }

  public async Task<int> SendBulk(IEnumerable<Message> mssgs)
  {
    var count = 0;
    await Parallel.ForEachAsync(mssgs, async (mssg, ct) =>
    {
      //the SMTP client is not reusable and can only send
      //one email at a time, which is OK cause we're going it in parallel
      using var client = new SmtpClient("localhost", 1025);
      using var conn = new DB().Connect();
      var sendMessage = new MailMessage
      {
        IsBodyHtml = true,
        Subject = mssg.Subject,
        Body = mssg.Html,
        From = new MailAddress(mssg.SendFrom),
      };
      sendMessage.To.Add(new MailAddress(mssg.SendTo));
      await client.SendMailAsync(sendMessage);
      mssg.Sent();
      await conn.UpdateAsync(mssg);
      Interlocked.Increment(ref count);
    });
    return count;
  }
}
