using Microsoft.EntityFrameworkCore;

namespace transitFlow.api.Data
{
    public class TransitFlowDbContext : DbContext
    {
        public TransitFlowDbContext(DbContextOptions<TransitFlowDbContext> options) : base(options)
        {
        }

    }
}
