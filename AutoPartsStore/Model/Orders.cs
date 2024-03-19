using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AutoPartsStore.Model
{
    public class Orders
    {
        public int OrderId { get; set; }       
        public int CustomerId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime OrderDate { get; set; }
        
        public List<OrderItems>? OrderItems { get; set; }
        [JsonIgnore]
        public  Customer? Customer { get; set; }
    }

}
