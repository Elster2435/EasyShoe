using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyShoeDemo.Lib.Entities
{
    [Table("pickup_points")]
    public class PickupPoint
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("address")]
        public string Address { get; set; } = null!;
        public List<Order> Orders { get; set; } = null!;
    }
}
