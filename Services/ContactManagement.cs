using System.Data;
using Contoso.Data;
using Contoso.Mail.Commands;
using Contoso.Mail.Models;
using Dapper;
using System.Net.Mail;
namespace Contoso.Mail.Services;

public interface IContactManagement
{
  CommandResult SignUp(Contact contact);
  CommandResult OptIn(Contact contact);
  CommandResult OptOut(string key);
  CommandResult Delete(Contact contact);
  Contact? Get(string email);
}

public class ContactManagement : IContactManagement
{
  private readonly IDbConnection _conn;

  public ContactManagement(IDbConnection conn)
  {
    _conn = conn;
  }

  public CommandResult SignUp(Contact contact)
  {
    //make sure the email is valid
    try
    {
      var emailAddress = new MailAddress(contact.Email);
    }
    catch (FormatException)
    {
      return new CommandResult
      {
        Data = new
        {
          Success = false,
          Message = "Invalid email format"
        }
      };

    }
    contact.Subscribed = true; //assuming they signed up for a reason
    var command = new ContactSignupCommand(contact);
    return command.Execute(_conn);
  }
  public CommandResult Delete(Contact contact)
  {
    var command = new ContactDeleteCommand(contact);
    return command.Execute(_conn);
  }
  public Contact? Get(string email)
  {
    var result = _conn.GetList<Contact>(new { email = email }).FirstOrDefault();
    return result;
  }

  public CommandResult OptIn(Contact contact)
  {
    var command = new ContactOptinCommand(contact);
    return command.Execute(_conn);
  }

  public CommandResult OptOut(string key)
  {
    var command = new ContactOptOutCommand(key);
    return command.Execute(_conn);
  }

  public CommandResult TagContacts(IEnumerable<string> emails, string tag)
  {
    var result = new ContactTagCommand
    {
      Tag = tag,
      Emails = emails
    };
    return result.Execute(_conn);
  }
}
