using System;
using System.Collections.Generic;

namespace pdf6b1.Models
{
    public partial class Category
    {
        public Category()
        {
            Product = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? Status { get; set; }

        public ICollection<Product> Product { get; set; }
    }
}
