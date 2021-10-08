using Microsoft.EntityFrameworkCore;
using Rocky.Models;
using System.Linq;

namespace Rocky.DataAccess.Repository.Interfaces
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(ApplicationUser applicationUser)
        {
            var applicationUserFromDb = _context.ApplicationUsers.AsNoTracking().FirstOrDefault(x => x.Id == applicationUser.Id);

            if (applicationUserFromDb != null)
            {
                // Update all the properties
                _context.ApplicationUsers.Update(applicationUser);
            }
        }
    }
}