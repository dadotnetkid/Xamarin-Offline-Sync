using northopsService.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Custom.Identity.Data.Repository
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        NorthopsContext  context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(NorthopsContext  context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
                //return orderBy(query);
            }
            else
            {
                return query.ToList();
                //return query;
            }
        }
        public virtual IEnumerable<object> Get(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           Expression<Func<TEntity,object>> Selector=null,
           string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (Selector != null)
                query.Select(Selector);

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
                //return orderBy(query);
            }
            else
            {
                return query.ToList();
                //return query;
            }
        }
        //public virtual IQueryable<TEntity> Fetch(string includeProperties = "")
        //{
        //    IQueryable<TEntity> query = dbSet;
        //    if (includeProperties != "")
        //        foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        //            query = query.Include(includeProperty);

        //    return query;
        //}
        public virtual IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;
            if (includeProperties != "")
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(includeProperty);
            if (filter != null)
                query = query.Where(filter);

            return query;
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }
        public virtual TEntity Find(Expression<Func<TEntity, bool>> filter, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;
            if (includeProperties != "")
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(includeProperty);

            return query.Where(filter).FirstOrDefault();
        }
        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filter)
        {

            return await dbSet.Where(filter).FirstOrDefaultAsync();
        }
        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }
        public virtual void InsertRange(IEnumerable<TEntity> entity)
        {
            dbSet.AddRange(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }
        public virtual async Task<int> UpdateAsync(TEntity entity)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
            return await context.SaveChangesAsync();
        }
        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter, string includeProperties = "")
        {
            var entity = await dbSet.Where(filter).FirstOrDefaultAsync();
            return entity;
        }
        public virtual async Task<int> InsertAsync(TEntity entity)
        {
            dbSet.Add(entity);
            return await context.SaveChangesAsync();
        }
        public virtual async Task<TEntity> CloneAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await dbSet.Where(filter).AsNoTracking().FirstOrDefaultAsync();
        }
        public virtual TEntity Clone(Expression<Func<TEntity, bool>> filter, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;
            if (includeProperties != "")
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(includeProperty);
            return query.Where(filter).AsNoTracking().FirstOrDefault();
        }
        public void Detach(TEntity entity)
        {
            dbSet.Remove(entity);
        }
    }

}