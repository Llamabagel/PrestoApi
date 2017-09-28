using System.Collections.Generic;

namespace PrestoApi.Models.Presto
{
    /// <summary>
    /// A transit pass.
    /// </summary>
    public class Pass
    {
        /// <summary>
        /// The name of the transit pass
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The range of dates in which the pass is valid
        /// </summary>
        public string DateRange { get; set; }
        
        /// <summary>
        /// The price of the pass
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// The Product id for this pass.
        /// </summary>
        public string Id { get; set; }
    }

    /// <summary>
    /// A response container for pass lists.
    /// </summary>
    public class PassResponse
    {
        /// <summary>
        /// Any errors to report
        /// </summary>
        public string Error { get; set; }
        
        /// <summary>
        /// The list of passes in the response
        /// </summary>
        public IList<Pass> Passes { get; set; }
    }
}