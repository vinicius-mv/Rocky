using Rocky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rocky.DataAccess.Repository.Interfaces
{
    public class ApplicationTypeRepository : Repository<ApplicationType>, IApplicationTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(ApplicationType appType)
        {
            var appTypeFromDb = _context.ApplicationTypes.FirstOrDefault(x => x.Id == appType.Id);
            if (appTypeFromDb != null)
            {
                appTypeFromDb.Name = appType.Name;
            }
        }
    }
}
