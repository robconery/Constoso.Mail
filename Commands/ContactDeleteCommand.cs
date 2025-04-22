using System.Data;
using Contoso.Data;
using Contoso.Mail.Models;
using Dapper;

namespace Contoso.Mail.Commands;

public class ContactDeleteCommand
{
  public Contact Contact { get; set; }
  public ContactDeleteCommand(Contact contact)
  {
    Contact = contact;
  }
  public CommandResult Execute(IDbConnection conn)
  {
    var tx = conn.BeginTransaction();

    try
    {
      conn.Execute("delete from tagged where contact_id=@id", new { id = Contact.ID }, tx);
      conn.Execute("delete from activity where contact_id=@id", new { id = Contact.ID }, tx);
      conn.Execute("delete from subscriptions where contact_id=@id", new { id = Contact.ID }, tx);
      conn.Delete<Contact>(Contact, tx);
      tx.Commit();
      return new CommandResult
      {
        Deleted = 1,
        Data = new
        {
          Success = true,
        }
      };
    }
    catch (Exception e)
    {
      tx.Rollback();
      return new CommandResult
      {
        Data = new
        {
          Success = false,
          Message = e.Message
        }
      };
    }
  }
}