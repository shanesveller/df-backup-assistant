using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SubdirectoryBrowsing
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The following subdirectories exist:");

            foreach(string subDir in Directory.EnumerateDirectories("."))
                Console.WriteLine(subDir);

            Console.ReadLine();
        }
    }
}
