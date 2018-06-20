namespace RomMaster.Client.Database.Models
{
    using System;
    using System.Collections.Generic;

    /*
    
        <name>fix_Sony - PlayStation Portable</name>
		<description>fix_</description>
		<category>FIXDATFILE</category>
		<version>06.20.2018 16:42:58</version>
		<date>06.20.2018</date>
		<author>RomVault</author>

    */

    public class Dat// : Entity
    {
        public int Id { get; private set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string Version { get; set; }

        public DateTime Date { get; set; }

        public string Author { get; set; }

        public virtual ISet<Game> Games { get; set; }

        public Dat()
        {
            this.Games = new HashSet<Game>();
        }
    }
}
