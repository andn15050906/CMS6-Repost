using Microsoft.EntityFrameworkCore;
using CMSnet6.Models.EntityModels;

namespace CMSnet6.Helpers
{
    public class AppStart
    {
        public static void ExecuteWarmQuery(string connectionString)
        {
            //Log.Information("__Preparing warm up query.");

            using var context = new Context(new DbContextOptionsBuilder<Context>().UseSqlServer(connectionString).Options);
            context.Posts.FirstOrDefault();

            //Log.Information("__Warm up query done.");
        }
    }
}
