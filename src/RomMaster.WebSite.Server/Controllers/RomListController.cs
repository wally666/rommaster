﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RomMaster.Common.Database;
using RomMaster.Domain.Models;

namespace RomMaster.WebSite.App.Controllers
{
    [Route("api/[controller]")]
    public class RomListController : Controller
    {
        private readonly ILogger<RomListController> logger;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public RomListController(ILogger<RomListController> logger, IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.logger = logger;
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public IEnumerable<Rom> GetList()
        {
            using (var uow = unitOfWorkFactory.Create())
            {
                var repo = uow.GetRepository<Rom>();
                return repo.GetAll().Take(100);
            }
        }
    }
}
