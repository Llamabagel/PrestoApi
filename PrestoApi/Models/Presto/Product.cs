namespace PrestoApi.Models.Presto
{
    /// <summary>
    /// A product that is available for purchase through PRESTO.
    /// 
    /// E.g. "Add Money" or a transit pass.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// The Id for this product type.
        /// 
        /// E.g. 5637144811 is "Add Money".
        /// 
        /// Each product has a different ID. Each type of pass for each transit provider has a different ID.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// The name of the product.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The listed price of the product
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// Any concessions associated with the product (Adult, Student, etc..)
        /// </summary>
        public string Concession { get; set; }
        
        /// <summary>
        /// The quanitity of this product in the user's cart.
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// The unique ID of this item (product) in the user's unique shopping cart.
        /// </summary>
        public string LineItemId { get; set; }
    }
}