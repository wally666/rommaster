using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RomMaster.Client.Database.Models;
using RomMaster.Common.Database;

namespace RomMaster.WebSite.App.Controllers
{
    [Route("api/[controller]")]
    public class DatController : Controller
    {
        private readonly ILogger<DatController> logger;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public DatController(ILogger<DatController> logger, IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.logger = logger;
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public IEnumerable<Dat> GetDats()
        {
            using (var uow = unitOfWorkFactory.Create())
            {
                var repo = uow.GetRepository<Dat>();
                return repo.GetAll();
            }
        }
    }
}
