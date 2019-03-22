using RomMaster.Common.Database;
using System.Collections.Generic;

namespace RomMaster.Domain.Models
{
    public class Game : IEntity
    {
        public int Id { get; private set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? Year { get; set; }

        public virtual ISet<Rom> Roms { get; set; }

        public Game()
        {
            this.Roms = new HashSet<Rom>();
        }
    }
}