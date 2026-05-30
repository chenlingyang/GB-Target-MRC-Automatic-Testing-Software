using System;
using System.IO;

using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.Examples.NET.Utils;

namespace DALSA.SaperaLT.Examples.NET.CSharp.GrabConsole3D
{
    class FileLoadConsole3D
    {
        // -----------------------------------------------------------------------------------------
        // Sapera.Net console file load 3D example  
        // 
        //    This program shows how to load an image file into a buffer in the host computer's memory,
        //    using Sapera++ Buffer object.  Also, a View object is used to display the Buffer.
        //
        // -----------------------------------------------------------------------------------------

         static string[] FileList = {
          "Range_C16.crc ",      
          "Profile_C16.crc",
          "Range_C16.tif",
          "Profile_C16.tif",
          "Range_AC16.crc",
          "Profile_AC16.crc",
          "Range_ACRW16.crc",
          "Profile_ACRW16.crc"
        };

        //enums
        enum FILE_FORMAT {tif,crc};

        //Parameters 
        static int Invalid_Flag_C_Parameter;         // Flag (0 or else) expressing if invalid values must be taking into account
        static double Invalid_Value_C_Parameter;     // The Z value flagged as invalid
        static double Scale_A_Parameter;             // Scale from Pixel to World according to the A (X) component
        static double Offset_A_Parameter;            // Offset from Pixel to World according to the A (X) component
        static double Scale_C_Parameter;             // Scale from Pixel to World according to the C (Z) component
        static double Offset_C_Parameter;            // Offset from Pixel to World according to the C (Z) component
        static SapBuffer.Val Output_Mode_Parameter;  // Output Mode of the device

        //File Attributes
        static SapFormat Pixel_Format_Attribute;  //AC, C, CR or ACRW
        static ExampleUtils.BUFFER_LAYOUT Buffer_Layout_Parameter;  //Range or Profile
        static FILE_FORMAT File_Format_Attribute;  //Range or Profile

        static int Index_DisplayedComponent = 1;  //Index in the split buffer to display


        static void Main(string[] args)
        {
           string filename_load = null;
           string filename_save = null;

           Console.WriteLine("Sapera Console File Load 3D Example (C# version)");

           // Call GetFileOptions to determine which file to load
           if(!GetOptions(args, ref filename_load))
           {
              Console.WriteLine("Press any key to terminate");
              Console.ReadKey(true);
              return;
           }

           if (filename_load.Contains(".crc"))
              File_Format_Attribute = FILE_FORMAT.crc;
           else
               File_Format_Attribute = FILE_FORMAT.tif;

           // Generate file name to save from file name to load
           filename_save = filename_load;
           filename_save = filename_save.Replace(".crc", "_crc.tif");

           // Allocate buffer with parameters compatible to file (does not load it)
            SapBuffer Buffers = new SapBuffer(filename_load, SapBuffer.MemoryType.ScatterGather);
            //Array of 4 buffers, each one holding a Component : A(X), C(Z), R(Reflectance), W(unused)
            SapBuffer[] Buffer_Components_Split = new SapBuffer[4];
            //The Buffer linked with the Viewing tool. Changing it will affect the display
            SapBuffer ViewBuffer = new SapBuffer();
            //Viewing tool
            SapView View = new SapView(ViewBuffer);

           // Create buffer object
           if (!Buffers.Create())
           {
               DestroysObjects(Buffers, ViewBuffer, View, Buffer_Components_Split);
               return;
           }

           Pixel_Format_Attribute = Buffers.Format;

           // Required for TIFF which cannot hold a 3D format value (uses Mono16 instead) 
           if (Pixel_Format_Attribute == SapFormat.Mono16)
               Pixel_Format_Attribute = SapFormat.Coord3D_C16;

           if (Buffers.Height == 1)
            Buffer_Layout_Parameter = ExampleUtils.BUFFER_LAYOUT.Profile;
           else
               Buffer_Layout_Parameter = ExampleUtils.BUFFER_LAYOUT.Range;

           if (File_Format_Attribute == FILE_FORMAT.crc)  //Only .crc files support the saving of parameters. .tiff use the defaults parameters
          {
              //Exctract the parameters values from a .crc : Invalid Value, Invalid Flag, Scale and Offset.
              if (!ExampleUtils.GetParametersFrom3DBuffer(Buffers, ref Invalid_Flag_C_Parameter, ref Invalid_Value_C_Parameter,
                  ref Scale_A_Parameter, ref Offset_A_Parameter, ref Scale_C_Parameter, ref Offset_C_Parameter, 
                  ref Buffer_Layout_Parameter, ref Output_Mode_Parameter))
              {
                 DestroysObjects(Buffers, ViewBuffer, View, Buffer_Components_Split);
                 return;
              }
          }

           //Contains 1 buffer for each possible component
           //  Buffer 0 = A (X)
           //  Buffer 1 = C (Z)
           //  Buffer 2 = R (Reflectance)
           //  Buffer 3 = W (unused)
           for (int bufIndex = 0; bufIndex < 4; bufIndex++)
           {
              Buffer_Components_Split[bufIndex] = new SapBuffer();
              //Setting the buffer general parameters (Count,Width,Height,Format,Type) according to the loaded file. We have to split into Mono Format Buffers.
              SetParameters(Buffer_Components_Split[bufIndex], Buffers.Count, Buffers.Width, Buffers.Height, SapFormat.Mono16, SapBuffer.MemoryType.Default);
              if (!Buffer_Components_Split[bufIndex].Create())
              {
                  DestroysObjects(Buffers, ViewBuffer, View, Buffer_Components_Split); 
                  return;
              }
           }

           SetParameters(ViewBuffer, Buffers.Count, Buffers.Width, ExampleUtils.PROFILE_WINDOW_HEIGHT, SapFormat.Mono16, (SapBuffer.MemoryType)(-1));//0 (unused)

           if (!Buffers.Load(filename_load, -1))
           {
               DestroysObjects(Buffers, ViewBuffer, View, Buffer_Components_Split);
               return;
           }

           if (!ViewBuffer.Create())
           {
               DestroysObjects(Buffers, ViewBuffer, View, Buffer_Components_Split);
               return;
           }

           if (!View.Create())
           {
               DestroysObjects(Buffers, ViewBuffer, View, Buffer_Components_Split);
               return;
           }

           switch (Pixel_Format_Attribute)  //Split the components, in the corresponding container,depending on the format
           {
           case SapFormat.Coord3D_C16:
               Buffer_Components_Split[1].Copy(Buffers);
               break;
           case SapFormat.Coord3D_AC16:
               Buffers.SplitComponents(Buffer_Components_Split[0], Buffer_Components_Split[1], null);
               break;
           case SapFormat.Coord3D_ACRW16:
               Buffers.SplitComponents(Buffer_Components_Split[0], Buffer_Components_Split[1], Buffer_Components_Split[2]);
               break;
           }

            bool Displaying = true;

            while (Displaying)
            {
                //Display the buffer according to his layout. 
                if (Buffer_Layout_Parameter == ExampleUtils.BUFFER_LAYOUT.Range)
                    ExampleUtils.DisplayRange(View, Buffer_Components_Split[Index_DisplayedComponent]);
              else
                    ExampleUtilsUnsafe.DisplayProfile(View, Buffer_Components_Split[Index_DisplayedComponent]);

                //Toogle the displayed component according to user input
               switch (Pixel_Format_Attribute)
               {
                case SapFormat.Coord3D_C16:
                  Console.WriteLine();
                  Console.WriteLine("Any Key to stop Displaying.");
                  Console.ReadKey(true);
                  Displaying = false;
                  break;
                case SapFormat.Coord3D_AC16:
                   Displaying = ExampleUtils.GetSelectionFromConsoleAC(ref Index_DisplayedComponent);
                   break;
                case SapFormat.Coord3D_ACRW16:
                   Displaying = ExampleUtils.GetSelectionFromConsoleACRW(ref Index_DisplayedComponent);
                   break;
               }
            }

            // Once Displayed Over, Process the image Pixel Mean and World Mean of the C component, according to parameters (defaults value if .tiff)
            int Pixel_Mean = 0;
            int World_Mean = 0;

            if (Buffer_Layout_Parameter == ExampleUtils.BUFFER_LAYOUT.Range)
            {
              ExampleUtilsUnsafe.ProcessingRange(Buffer_Components_Split[1],Invalid_Flag_C_Parameter, Invalid_Value_C_Parameter,
                   Scale_C_Parameter, Offset_C_Parameter, ref Pixel_Mean, ref World_Mean);
            }
           else
           {
               ExampleUtilsUnsafe.ProcessingProfile(Buffer_Components_Split[1], Invalid_Flag_C_Parameter, Invalid_Value_C_Parameter,
                   Scale_C_Parameter, Offset_C_Parameter, ref Pixel_Mean, ref World_Mean);
           }

            Console.WriteLine("\n");
            Console.WriteLine("Pixel_Mean = {0}, World_Mean = {1}", Pixel_Mean, World_Mean);
            Console.WriteLine();

           if (Pixel_Format_Attribute == SapFormat.Coord3D_C16)
            //Save C16 format as a tif. Parameters Lost.
            Buffers.Save(filename_save, "-format tif");
           else
            //Save MONO format as a tif. Parameters Lost.
            Buffer_Components_Split[Index_DisplayedComponent].Save(filename_save, "-format tif");

           DestroysObjects(Buffers, ViewBuffer, View, Buffer_Components_Split);
        }

        static bool GetOptions(string[] args, ref  string filename)
        {
            // Check the command line for user commands
            if (args.Length > 1)
            {
                if (string.Compare(args[1], "/?", StringComparison.Ordinal) == 0 || (string.Compare(args[1], "-?", StringComparison.Ordinal) == 0))
                {
                    // print help
                    Console.WriteLine("Usage:\n");
                    Console.WriteLine("FileLoadCPP [<image filename>]");
                    return false;
                }

                // Check if user specified filename
                if (args.Length == 2)
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
            for (int i = 0; i < FileList.Length; i++)
                Console.WriteLine("{0}. {1}", i, FileList[i]);


            ConsoleKeyInfo info = Console.ReadKey(true);
            char key = info.KeyChar;
            int numkey = key - '0';

            if (numkey > FileList.Length)
            {
                Console.WriteLine("Invalid selection!\n");
                return false;
            }

            filename = Environment.GetEnvironmentVariable("SAPERADIR");
            if (filename == null)
                return false;
            filename += "\\Images\\Display_3D\\" + FileList[numkey];
            return true;
        }

        static void SetParameters(SapBuffer buffer, int count, int width, int height, SapFormat format, SapBuffer.MemoryType type)
        {
            buffer.Width = width;
            buffer.Height = height;
            buffer.Format = format;
            buffer.Type = type;
        }

        static void DestroysObjects(SapBuffer Buffers, SapBuffer ViewBuffer, SapView View, SapBuffer[] Buffer_Components_Split)
        {
            View.Destroy();
            View.Dispose();

            ViewBuffer.Destroy();
            ViewBuffer.Dispose();

            Buffers.Destroy();
            Buffers.Dispose();

            for (int bufIndex = 0; bufIndex < 4; bufIndex++)
            {
                Buffer_Components_Split[bufIndex].Destroy();
                Buffer_Components_Split[bufIndex].Dispose();
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to terminate");
            Console.ReadKey(true);
        }
    }
}
