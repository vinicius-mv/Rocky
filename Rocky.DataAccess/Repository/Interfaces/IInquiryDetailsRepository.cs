using Rocky.Models;

namespace Rocky.DataAccess.Repository.Interfaces
{
    public interface IInquiryDetailRepository : IRepository<InquiryDetail>
    {
        void Update(InquiryDetail inquiryDetail);
    }
}