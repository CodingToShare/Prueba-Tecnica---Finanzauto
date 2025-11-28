using System.Collections.Generic;

namespace ProductCatalog.Domain.Entities
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        // Se usa string para soportar la URL simulada solicitada, aunque en Northwind original suele ser byte[]
        public string? Picture { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
