using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rocky.DataAccess.Repository.Interfaces
{
    public class InquiryDetailRepository : Repository<InquiryDetail>, IInquiryDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public InquiryDetailRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(InquiryDetail inquiryDetail)
        {
            var inquiryDetailFromDb = _context.InquiryDetails.AsNoTracking().FirstOrDefault(x => x.Id == inquiryDetail.Id);

            if(inquiryDetailFromDb != null)
            {
                // Update all the properties
                _context.InquiryDetails.Update(inquiryDetail);
            }
        }
    }
}
