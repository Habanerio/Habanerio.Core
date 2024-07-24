using System.ComponentModel.DataAnnotations.Schema;
using Habanerio.Core.DBs.MongoDB.EFCore;
using MongoDB.EntityFrameworkCore;

namespace Habanerio.Samples.Customers.MongoEFCore.Entities;

[Table("customers")]
[Collection("customers")]
public class CustomerDbEntity : MongoDbEntity
{
    public string FirstName { get; set; } = "";

    public string LastName { get; set; } = "";

    public string Email { get; set; } = "";

    //public required ContactInfoDbEntity ContactInfo { get; set; }
}


public class ContactInfoDbEntity : MongoDbEntity
{
    public required AddressDbEntity ShippingAddress { get; set; }
    public AddressDbEntity? BillingAddress { get; set; }
    public required List<PhoneNumberDbEntity> Phone { get; set; } = new List<PhoneNumberDbEntity>();
}

public class PhoneNumberDbEntity : MongoDbEntity
{
    public required PhoneTypes Type { get; set; }
    public required int CountryCode { get; set; }
    public required string Number { get; set; }
}

public class AddressDbEntity : MongoDbEntity
{
    public required string Line1 { get; set; }
    public string? Line2 { get; set; }
    public string? Line3 { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public required string PostalCode { get; set; }
}

public enum PhoneTypes
{
    Home,
    Mobile,
    Work,
    Fax
}