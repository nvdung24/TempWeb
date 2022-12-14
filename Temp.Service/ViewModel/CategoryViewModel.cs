using System.Collections.Generic;
using Temp.DataAccess.Data;

namespace Temp.Service.ViewModel
{
    /// <summary>
    /// category view model
    /// </summary>
    public class CategoryViewModel
    {
        /// <summary>
        /// id category
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// category name
        /// </summary>
        public string Name { get; set; }
        public ICollection<Product> Product { get; set; }
    }
}