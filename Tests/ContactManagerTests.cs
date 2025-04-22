using Xunit;
using Contoso.Mail.Models;
using Contoso.Mail.Services;

namespace Contoso.Mail.Tests;
public class ContactManagerTests : TestBase
{
  private readonly IContactManagement _contactManager;

  public ContactManagerTests()
  {
    _contactManager = new ContactManagement(Conn);
  }

  [Fact]
  public void AddContact_ValidData_ShouldSucceed()
  {
    // Arrange
    var contact = new Contact { Email = "test@example.com", Name = "Test User" };

    // Act
    var result = _contactManager.SignUp(contact);

    // Assert
    Assert.True(result.Data.Success);
  }

  [Fact]
  public void AddContact_InvalidData_ShouldFail()
  {
    // Arrange
    var contact = new Contact { Email = "invalid-email", Name = "" };

    // Act
    var result = _contactManager.SignUp(contact);


    // Assert
    Assert.False(result.Data.Success);
  }

  [Fact]
  public void RemoveContact_ExistingContact_ShouldSucceed()
  {
    // Arrange
    var contact = new Contact { Email = "testdeleter@example.com", Name = "Test User" };
    _contactManager.SignUp(contact);
    contact = _contactManager.Get(contact.Email);
    // Act
    var result = _contactManager.Delete(contact);

    // Assert
    Assert.True(result.Data.Success);
  }

  [Fact]
  public void GetContact_ExistingContact_ShouldReturnContact()
  {
    // Arrange
    var contact = new Contact { Email = "testget@example.com", Name = "Test User" };
    _contactManager.SignUp(contact);

    // Act
    var result = _contactManager.Get("testget@example.com");

    // Assert
    Assert.NotNull(result);
  }

  [Fact]
  public void Contact_IsAutoSubbed()
  {
    var contact = new Contact { Email = "testsub@example.com", Name = "Test User" };
    _contactManager.SignUp(contact);

    contact = _contactManager.Get(contact.Email);
    Assert.NotNull(contact);
    Assert.True(contact.Subscribed);
  }
  public void Contact_CanUnsub()
  {
    var contact = new Contact { Email = "testunsub@example.com", Name = "Test User" };
    _contactManager.SignUp(contact);
    contact = _contactManager.Get(contact.Email);
    _contactManager.OptOut(contact.Key);
    contact = _contactManager.Get(contact.Email);

    Assert.False(contact.Subscribed);
  }
}
