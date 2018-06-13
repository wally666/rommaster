namespace RomMaster.FileSystemWatcher
{
    using System;
    using System.IO;

    public class Watcher
    {
        FileSystemWatcher watcher;

        public bool Enabled
        {
            get
            {
                return watcher.EnableRaisingEvents;
            }
            set
            {
                watcher.EnableRaisingEvents = value;
            }
        }

        public Watcher(string path)
        {
            watcher = new FileSystemWatcher(path, "*.*");
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.Changed += new FileSystemEventHandler(OnChanged);
            //watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.Error += new ErrorEventHandler(OnError);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"FileSystemWatcher.OnError: {e.GetException()}");
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"FileSystemWatcher.OnRenamed: {e.ChangeType}, {e.FullPath}, {e.Name}, {e.OldFullPath}, {e.OldName}");
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"FileSystemWatcher.OnChanged: {e.ChangeType}, {e.FullPath}, {e.Name}");
        }
    }
}
