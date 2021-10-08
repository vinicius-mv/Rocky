using Rocky.Models;

namespace Rocky.DataAccess.Repository.Interfaces
{
    public interface IInquiryHeaderRepository : IRepository<InquiryHeader>
    {
        void Update(InquiryHeader inquiryHeader);
    }
}