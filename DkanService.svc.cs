using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;

namespace dkanapp
{

    public class DkanService : DataService<dkandevEntities>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
            // Examples:
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.SetServiceOperationAccessRule("GetPopularPosts", ServiceOperationRights.AllRead);
           //Set a reasonable paging site
           config.SetEntitySetPageSize("*", 25);
            // config.SetEntitySetAccessRule("MyEntityset", EntitySetRights.AllRead);
            // config.SetServiceOperationAccessRule("MyServiceOperation", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
        }

         protected override void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            base.OnStartProcessingRequest(args);
            //Cache for a minute based on querystring
            HttpContext context = HttpContext.Current;
            HttpCachePolicy c = HttpContext.Current.Response.Cache;
            c.SetCacheability(HttpCacheability.ServerAndPrivate);
            c.SetExpires(HttpContext.Current.Timestamp.AddSeconds(60));
            c.VaryByHeaders["Accept"] = true;
            c.VaryByHeaders["Accept-Charset"] = true;
            c.VaryByHeaders["Accept-Encoding"] = true;
            c.VaryByParams["*"] = true;
        }
 
        [WebGet]
        public IQueryable<node> GetPopularPosts()
        {
            var popularPosts = (from p in this.CurrentDataSource.nodes
                               orderby p.ViewCount
                               select p).Take(20);
 
            return popularPosts;
        }
    }
}
