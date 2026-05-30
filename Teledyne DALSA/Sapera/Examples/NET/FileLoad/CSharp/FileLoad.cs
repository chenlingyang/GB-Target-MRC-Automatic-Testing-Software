using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using DALSA.SaperaLT.SapClassBasic;


namespace DALSA.SaperaLT.Examples.NET.CSharp.FileLoad
{
    class FileLoad
    {
         static string[] FileList = {       
         "mono8.bmp",
         "mono16.tif",
         "rgb565.bmp",
         "rgb888.bmp",
         "rgb8888.bmp",
         "rgb101010.bmp",
         "yuy2.crc",};

        static void Main(string[] args)
        {

           SapBuffer Buffers = null;
           SapView View = null; 
           string filename = "";

           Console.WriteLine("Sapera Console File Load Example (C# version)");

           // Call GetOptions to determine which file to load
           if (!GetOptions(args,ref filename))
           {
              Console.WriteLine("Press any key to terminate.");
              Console.ReadKey(); 
              return;
           }

           // Allocate buffer with parameters compatible to file (does not load it)
           Buffers = new SapBuffer(filename, SapBuffer.MemoryType.Default);
           // Allocate view with buffer
           View = new SapView(Buffers); 

           // Create buffer object
           if (!Buffers.Create())
           {
               DestroysObjects(Buffers,View);
               return;
           }

           // Create view object
           if (!View.Create())
           {
               DestroysObjects(Buffers, View);
               return;
           }

           // Load file
           if(!Buffers.Load(filename, -1))
           {
               DestroysObjects(Buffers, View);
               return;
           }

           // Display buffer
           View.Show();


           Console.WriteLine("Press any key to terminate.");
           Console.ReadKey();
        }

        static bool GetOptions( string[] args,ref  string filename)
        {
           // Check the command line for user commands
           if(args.Length > 1)
           {
            if (string.Compare(args[1], "/?", StringComparison.Ordinal) == 0 || (string.Compare(args[1], "-?", StringComparison.Ordinal) == 0))
            {
              // print help
              Console.WriteLine("Usage:\n");
              Console.WriteLine("FileLoadCPP [<image filename>]");
                  return false;
            }

            // Check if user specified filename
            if(args.Length == 2)
            {
                 // Verify that the specified file exists
                 if (!File.Exists(args[1]))
                 {
                     Console.WriteLine("Specified image file {0} is invalid!\n", args[1]);
                    return false;
                 }
                 filename = args[1];              
                 return true;
             }
           
             Console.WriteLine("Invalid command line!\n");
             return false;
           }

           /// Ask user to select image file ///

           Console.WriteLine("Choose one image file: ");
           for (int i=0; i < FileList.Length; i++)
              Console.WriteLine("{0}. {1}", i + 1, FileList[i]);

           
           ConsoleKeyInfo info = Console.ReadKey(true);
           char key = info.KeyChar;
           int numkey = key - '0';

           if (numkey < 1 || numkey > FileList.Length)
           {
              Console.WriteLine("Invalid selection!\n");
              return false;
           }
             
           filename = Environment.GetEnvironmentVariable("SAPERADIR");
	       if (filename == null)
              return false;
           filename += "\\Images\\Display\\" + FileList[numkey - 1];
           return true;
        }

        static void DestroysObjects(SapBuffer Buffers,SapView View)
        {
          
            if (View != null)
            {
                View.Destroy();
                View.Dispose();
            }
            if (Buffers != null)
            {
                Buffers.Destroy();
                Buffers.Dispose();
            }

            Console.WriteLine("Press any key to terminate.");
            Console.ReadKey();
               
        }
     
    }
}
