using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RomMaster.Common.Database;
using RomMaster.Domain.Models;

namespace RomMaster.WebSite.App.Controllers
{
    [Route("api/[controller]")]
    public class GameListController : Controller
    {
        private readonly ILogger<GameListController> logger;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public GameListController(ILogger<GameListController> logger, IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.logger = logger;
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public IEnumerable<Game> GetList()
        {
            using (var uow = unitOfWorkFactory.Create())
            {
                var repo = uow.GetRepository<Game>();
                return repo.GetAll().Take(100);
            }
        }
    }
}
