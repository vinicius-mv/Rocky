using Rocky.Models;

namespace Rocky.DataAccess.Repository.Interfaces
{
    public interface IApplicationTypeRepository : IRepository<ApplicationType>
    {
        void Update(ApplicationType applicationType);
    }
}