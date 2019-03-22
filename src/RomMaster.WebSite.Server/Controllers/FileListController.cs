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
    public class FileListController : Controller
    {
        private readonly ILogger<FileListController> logger;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public FileListController(ILogger<FileListController> logger, IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.logger = logger;
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public IEnumerable<File> GetList()
        {
            using (var uow = unitOfWorkFactory.Create())
            {
                var repo = uow.GetRepository<File>();
                return repo.GetAll().Take(100);
            }
        }
    }
}
