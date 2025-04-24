using Contoso.Data;
using Contoso.Mail.Models;
using Microsoft.Data.Sqlite;
using System.Data;
using Dapper;

namespace Contoso.Mail.Services;


public interface IPostOffice
{

}

/// <summary>
/// Handles broadcast creation and mailing operations for the list mailer application
/// </summary>
public class PostOffice
{
  private readonly IDbConnection _db;

  public PostOffice(IDbConnection db)
  {
    _db = db;
  }

  /// <summary>
  /// Creates a broadcast from a markdown email document
  /// </summary>
  public Broadcast CreateBroadcastFromMarkdownEmail(MarkdownEmail doc)
  {
    if (doc.Data == null)
    {
      throw new InvalidOperationException("Need frontmatter with Subject and Slug");
    }

    return new Broadcast
    {
      Name = doc.Data.Subject,
      Slug = doc.Data.Slug,
      SendToTag = doc.Data.SendToTag
    };
  }

  /// <summary>
  /// Creates a broadcast from raw markdown content
  /// </summary>
  public Broadcast CreateBroadcastFromMarkdown(string markdown)
  {
    var doc = MarkdownEmail.FromString(markdown);
    return CreateBroadcastFromMarkdownEmail(doc);
  }

  public long ContactCount(string SendToTag)
  {
    //do we have a tag?
    long contacts = 0;
    if (SendToTag == "*")
    {
      contacts = _db.ExecuteScalar<long>("select count(1) from contacts where subscribed = true");
    }
    else
    {
      var sql = @"
        select count(1) as count from contacts 
        inner join tagged on tagged.contact_id = contacts.id
        inner join tags on tags.id = tagged.tag_id
        where subscribed = true
        and tags.slug = @tagSlug
      ";
      contacts = _db.ExecuteScalar<long>(sql, new { tagSlug = SendToTag });
    }
    return contacts;
  }

}
