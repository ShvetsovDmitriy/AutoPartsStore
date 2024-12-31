using System.Text.Json.Serialization;
using AutoPartsStore.Model.Order;

public class Customer
{
    [JsonIgnore]
    public int CustomerId { get; set; }        
    public string? FullName { get; set; }       
    public string? PhoneNumber { get; set; }
    [JsonIgnore]
    public  ICollection<Orders>? Orders { get; set; }
}


