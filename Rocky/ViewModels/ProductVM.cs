using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky.Models;
using System.Collections.Generic;

namespace Rocky.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }

        public IEnumerable<SelectListItem> CategorySelectList { get; set; }

        public IEnumerable<SelectListItem> ApplicationTypeSelectList { get; set; }
    }
}