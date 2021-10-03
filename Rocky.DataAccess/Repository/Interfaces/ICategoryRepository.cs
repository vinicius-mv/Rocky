﻿using Rocky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Rocky.DataAccess.Repository.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);
    }
}