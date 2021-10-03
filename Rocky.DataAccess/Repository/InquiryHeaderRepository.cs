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
    public class InquiryHeaderRepository : Repository<InquiryHeader>, IInquiryHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public InquiryHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(InquiryHeader inquiryHeader)
        {
            var inquiryHeaderFromDb = _context.InquiryHeaders.AsNoTracking().FirstOrDefault(x => x.Id == inquiryHeader.Id);

            if(inquiryHeaderFromDb != null)
            {
                // Update all the properties
                _context.InquiryHeaders.Update(inquiryHeader);
            }
        }
    }
}
