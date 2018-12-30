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
    public class DatListController : Controller
    {
        private readonly ILogger<DatListController> logger;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public DatListController(ILogger<DatListController> logger, IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.logger = logger;
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public IEnumerable<Dat> GetList()
        {
            using (var uow = unitOfWorkFactory.Create())
            {
                var repo = uow.GetRepository<Dat>();
                return repo.GetAll().Take(100);
            }
        }
    }
}
