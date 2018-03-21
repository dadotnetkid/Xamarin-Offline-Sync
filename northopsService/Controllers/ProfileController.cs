using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using northopsService.Models;

namespace northopsService.Controllers
{
    [Authorize]
    public class ProfileController : TableController<User>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            NorthopsContext context = new NorthopsContext();
            DomainManager = new EntityDomainManager<User>(context, Request);
        }

        // GET tables/Profile
        public IQueryable<User> GetAllUser()
        {
            return Query().ToList().AsQueryable();
        }

        // GET tables/Profile/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<User> GetUser(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Profile/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<User> PatchUser(string id, Delta<User> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Profile
        public async Task<IHttpActionResult> PostUser(User item)
        {
            User current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Profile/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteUser(string id)
        {
            return DeleteAsync(id);
        }
    }
}
