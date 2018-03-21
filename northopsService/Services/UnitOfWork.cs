using northopsService.Models;
using System;
using System.Threading.Tasks;

namespace Custom.Identity.Data.Repository
{
    public class UnitOfWork : IDisposable
    {
        private northopsService.Models.NorthopsContext  context = new NorthopsContext ();


        #region Login Management
        private GenericRepository<User> userRepository;
        public GenericRepository<User> UserRepository
        {
            get
            {

                if (this.userRepository == null)
                {
                    this.userRepository = new GenericRepository<User>(context);
                }
                return userRepository;
            }
        }
        private GenericRepository<UserRole> roleRepository;
        public GenericRepository<UserRole> RoleRepository
        {
            get
            {

                if (this.roleRepository == null)
                {
                    this.roleRepository = new GenericRepository<UserRole>(context);
                }
                return roleRepository;
            }
        }
        #endregion

        #region Subscripttions
     

        #endregion

       
        #region Services
       
        #endregion
 
        
       

        public void Save()
        {
            context.SaveChanges();
        }
        
        public async Task<int> SaveAsync()
        {
            return await context.SaveChangesAsync();
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}