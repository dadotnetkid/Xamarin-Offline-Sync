using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;
using Newtonsoft.Json.Linq;
using northopsService.App_Start;
using northopsService.Models;
using UnitOfWorkExtension;

namespace northopsService.Controllers
{
    [Authorize]
    public class UsersController : TableController<User>
    {
        private NorthopsContext context;

        ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            context = new NorthopsContext();
            context.Configuration.LazyLoadingEnabled = false;
            context.Configuration.ProxyCreationEnabled = false;
            DomainManager = new EntityDomainManager<User>(new NorthopsContext(), Request);

        }

        // GET tables/User
        public IQueryable<User> GetAllUser()
        {
            return Query().ToList().AsQueryable();
        }

        public SingleResult<User> GetUser(string id)
        {
            var res = context.Users.Where(x => x.Id == id);
            return SingleResult.Create<User>(TableUtils.ApplyDeletedFilter<User>(res, true));
            //Lookup(id);
        }

        // PATCH tables/User/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<User> PatchUser(string id, Delta<User> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/User
        //   [AllowAnonymous]
        public async Task<IHttpActionResult> PostUser(User item)
        {

#if DEBUG
            item.Id = Guid.NewGuid().ToString();
            item.Email = "test@gmail.com";
            item.UserName = "test@gmail.com";
            item.Password = "123321";
            item.CreatedAt = DateTime.Now;
            item.UpdatedAt = DateTime.Now;
#endif
            var user = await userManager.CreateAsync(item, item.Password);
            return CreatedAtRoute("Tables", new { id = Guid.NewGuid().ToByteArray() }, user);
        }

        // DELETE tables/User/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteUser(string id)
        {
            return DeleteAsync(id);
        }
        [Route("api/CreateUser")]
        [HttpGet]
        public IHttpActionResult CreateUser(User user, string Password)
        {

            return Ok(new JObject() { ["message"] = "hello world" });
        }
        [Route("api/users")]
        [HttpGet]
        public IQueryable<User> Users()
        {
            //context = new northopsContext();
            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.LazyLoadingEnabled = false;
            return context.Users.ToList().AsQueryable();
        }
    }
}
