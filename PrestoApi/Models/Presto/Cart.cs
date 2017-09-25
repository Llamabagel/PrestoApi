using System.Collections.Generic;

namespace PrestoApi.Models.Presto
{
    /// <summary>
    /// An object representation of the user's shopping cart for a specified card.
    /// </summary>
    public class Cart
    {
        public Cart()
        {
            Products = new List<Product>();
        }
        
        /// <summary>
        /// The unique ID for this shopping cart
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// The sub total of the user's current order
        /// </summary>
        public decimal SubTotal { get; set; }
        
        /// <summary>
        /// The number of the user's PRESTO card associated with this cart.
        /// </summary>
        public string CardNumber { get; set; }
        
        /// <summary>
        /// The list of <see cref="Product"/>s in the cart.
        /// </summary>
        public IList<Product> Products { get; set; }
        
        /// <summary>
        /// Any errors encountered while retrieving the cart.
        /// </summary>
        public string Error { get; set; }
    }
}