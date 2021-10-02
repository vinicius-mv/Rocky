using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rocky.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public void Add(T entity)
        {
           _dbSet.Add(entity);
        }

        public T Find(int id)
        {
            return _dbSet.Find(id);
        }

        public T FirstOrDefault(System.Linq.Expressions.Expression<Func<T, bool>> filter = null, string includeProperties = null, bool isTracking = true)
        {
            IQueryable<T> query = _dbSet;

            query = IncludeFilter(query, filter);

            query = IncludeNavigationProperties(query, includeProperties);

            query = IncludeAsNoTracking(query, isTracking);

            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(System.Linq.Expressions.Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null, bool isTracking = true)
        {
            IQueryable<T> query = _dbSet;

            query = IncludeFilter(query, filter);

            query = IncludeNavigationProperties(query, includeProperties);

            query = IncludeOrderBy(query, orderBy);

            query = IncludeAsNoTracking(query, isTracking);

            return query.ToList();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private static IQueryable<T> IncludeAsNoTracking(IQueryable<T> query, bool isTracking)
        {
            if (!isTracking)
                query = query.AsNoTracking();

            return query;
        }

        private static IQueryable<T> IncludeOrderBy(IQueryable<T> query, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
        {
            if (orderBy != null)
                query = orderBy(query);
            return query;
        }

        private static IQueryable<T> IncludeNavigationProperties(IQueryable<T> query, string includeProperties)
        {
            if (includeProperties != null)
            {
                foreach (var incluceProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluceProp);
                }
            }

            return query;
        }

        private static IQueryable<T> IncludeFilter(IQueryable<T> query, Expression<Func<T, bool>> filter)
        {
            if (filter != null)
                query = query.Where(filter);
            return query;
        }
    }
}
