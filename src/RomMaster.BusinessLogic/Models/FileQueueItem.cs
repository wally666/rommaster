namespace RomMaster.BusinessLogic.Models
{
    public class FileQueueItem
    {
        public string File { get; set; }

        public override string ToString()
        {
            return File;
        }
    }
}
