using entity = Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Repositories.Implementation
{
    [ExcludeFromCodeCoverage]
    public class SailingRepository : Repository<entity.Route>, ISailingRepository
    {   
        private CommonDbContext sailingContext
        {
            get
            {
                return _dbContext as CommonDbContext;
            }
        }

        public SailingRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
