using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyShoeDemo.Lib.Entities
{
    [Table("orders")]
    public class Order
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("client_id")]
        public int ClientId { get; set; }
        [Column("pickup_point_id")]
        public int PickupPointId { get; set; }
        [Column("order_date")]
        public DateOnly OrderDate { get; set; }
        [Column("delivery_date")]
        public DateOnly DeliveryDate { get; set; }
        [Column("status")]
        public string Status { get; set; } = null!;
        [Column("pickup_code")]
        public string PickupCode { get; set; } = null!;
        public User Client { get; set; } = null!;
        public PickupPoint PickupPoint { get; set; } = null!;
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
