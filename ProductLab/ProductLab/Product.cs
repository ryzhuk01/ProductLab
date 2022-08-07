using System.Runtime.Serialization;

namespace ProductLabLifeHack;

[DataContract]
public class Product
{
    [DataMember(Name = "name")]
    public string? Title { get; set; }

    [DataMember(Name = "brand")]
    public string? Brand { get; set; }

    [DataMember(Name = "feedbacks")]
    public int Feedbacks { get; set; }

    [DataMember(Name = "priceU")]
    public int Price { get; set; }
}