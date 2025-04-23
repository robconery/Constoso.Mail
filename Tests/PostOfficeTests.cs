using Contoso.Mail.Models;
using Contoso.Mail.Services;
using Microsoft.Data.Sqlite;
using System.Data;
using Xunit;
using Dapper;

namespace Contoso.Mail.Tests;

public class PostOfficeTests : TestBase
{

  private readonly PostOffice _po;

  public PostOfficeTests()
  {
    _po = new PostOffice(this.Conn);
  }

  [Fact]
  public void CreateBroadcastFromMarkdown_WithValidMarkdown_ReturnsBroadcast()
  {
    var markdown = @"---
Subject: Test Email
Slug: test-email
SendToTag: test-tag
---
# Test Content";

    var result = _po.CreateBroadcastFromMarkdown(markdown);

    Assert.Equal("Test Email", result.Name);
    Assert.Equal("test-email", result.Slug);
    Assert.Equal("test-tag", result.SendToTag);
  }

  [Fact]
  public void CreateBroadcastFromMarkdownEmail_WithValidDoc_ReturnsBroadcast()
  {
    var doc = new MarkdownEmail
    {
      Data = new
      {
        Subject = "Test Email",
        Slug = "test-email",
        SendToTag = "test-tag"
      }
    };

    var result = _po.CreateBroadcastFromMarkdownEmail(doc);

    Assert.Equal("Test Email", result.Name);
    Assert.Equal("test-email", result.Slug);
    Assert.Equal("test-tag", result.SendToTag);
  }

  [Fact]
  public void ContactCount_WithAllContacts_ReturnsCorrectCount()
  {
    // Setup test data
    this.Conn.Execute("INSERT INTO contacts (email, subscribed) VALUES (@email, @subscribed)",
        new[] {
                new { email = "test1@test.com", subscribed = true },
                new { email = "test2@test.com", subscribed = true },
                new { email = "test3@test.com", subscribed = false }
        });

    var result = _po.ContactCount("*");

    Assert.Equal(2, result);
  }

  [Fact]
  public void ContactCount_WithTaggedContacts_ReturnsCorrectCount()
  {
    // Setup test data
    this.Conn.Execute("INSERT INTO contacts (id, email, subscribed) VALUES (1, 'test1@test.com', true)");
    this.Conn.Execute("INSERT INTO tags (id, slug) VALUES (1, 'test-tag')");
    this.Conn.Execute("INSERT INTO tagged (contact_id, tag_id) VALUES (1, 1)");

    var result = _po.ContactCount("test-tag");

    Assert.Equal(1, result);
  }
}
