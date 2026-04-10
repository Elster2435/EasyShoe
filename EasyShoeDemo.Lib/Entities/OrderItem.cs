using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyShoeDemo.Lib.Entities
{
    [Table("order_items")]
    public class OrderItem
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("product_article")]
        public string ProductArticle { get; set; } = null!;
        [Column("quantity")]
        public int Quantity { get; set; }
        public List<Order> Orders { get; set; } = new();
    }
}
