using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky.Models;
using System.Collections.Generic;

namespace Rocky.DataAccess.Repository.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);

        IEnumerable<SelectListItem> GetAllCategoriesDropDownList();

        IEnumerable<SelectListItem> GetAllApplicationTypesDropDownList();
    }
}