using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky.Models;
using System.Collections.Generic;
using System.Linq;

namespace Rocky.DataAccess.Repository.Interfaces
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var productFromDb = _context.Products.AsNoTracking().FirstOrDefault(x => x.Id == product.Id);

            if (productFromDb != null)
            {
                // Update all the properties
                _context.Products.Update(product);
            }
        }

        public IEnumerable<SelectListItem> GetAllCategoriesDropDownList()
        {
            return _context.Categories.Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
        }

        public IEnumerable<SelectListItem> GetAllApplicationTypesDropDownList()
        {
            return _context.ApplicationTypes.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });
        }
    }
}