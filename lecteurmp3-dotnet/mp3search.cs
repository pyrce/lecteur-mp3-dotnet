using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace lecteurmp3_dotnet
{
    // source : https://docs.microsoft.com/fr-fr/dotnet/csharp/programming-guide/file-system/how-to-iterate-through-a-directory-tree
    public class RecursiveFileSearch
    {
        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();
        public static List<Fichier> fileList = new List<Fichier>();

        public List<Fichier> Main()
        {
            string[] drives = System.Environment.GetLogicalDrives();
            loadLocalDrives(drives);
            return fileList;
        }


        static void loadLocalDrives(String[] drives)
        {
            foreach (string dr in drives)
            {
                System.IO.DriveInfo di = new System.IO.DriveInfo(dr);
                if (!di.IsReady)
                {
                    Console.WriteLine("The drive {0} could not be read", di.Name);
                    continue;
                }
                System.IO.DirectoryInfo rootDir = di.RootDirectory;
                WalkDirectoryTree(rootDir);
            }

            Console.WriteLine("Files with restricted access:");
            foreach (string s in log)
            {
                Console.WriteLine(s);
            }
        }

        static void getFichiers(System.IO.DirectoryInfo root, System.IO.FileInfo[] files, System.IO.DirectoryInfo[] subDirs)
        {
            foreach (System.IO.FileInfo fi in files)
            {
                Fichier File = new Fichier();
                File.Name = fi.Name;
                File.Path = fi.DirectoryName;
                //File.getProperties(fi.Name, fi.FullName);
                fileList.Add(File);
            }

            subDirs = root.GetDirectories();
            foreach (System.IO.DirectoryInfo dirInfo in subDirs)
            {
                if (dirInfo.FullName.Contains("Users") && !dirInfo.FullName.Contains("gao") && !dirInfo.FullName.Contains("AppData"))
                {
                    WalkDirectoryTree(dirInfo);
                }
            }
        }

        static void WalkDirectoryTree(System.IO.DirectoryInfo root)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            try
            {
                files = root.GetFiles("*.mp3");
            }

            catch (UnauthorizedAccessException e)
            {
                log.Add(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                getFichiers(root, files, subDirs);
            }
        }
    }
}