using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using DALSA.SaperaLT.SapClassBasic;


namespace DALSA.SaperaLT.Examples.NET.CSharp.ColorSplit
{
    class ColorSplit
    {
        
        const String INPUT_FILE_NAME = "rgb8888.bmp";
        const String  OUTPUT_FILE_NAME = "rgb8888_inverted.bmp";
        
        static void Main(string[] args)
        {
            SapBuffer inBuf = new SapBuffer();
            SapBuffer outBuf = new SapBuffer();
            SapBuffer compBuf = new SapBuffer();
            SapView inView = new SapView(inBuf);
            SapView outView = new SapView(outBuf);
       
            Console.WriteLine("Sapera Color Split Example (C# version)\n");

            // Get image location from environment variable
            string inputFileName = Environment.GetEnvironmentVariable("SAPERADIR");
            if (inputFileName == null)
            {
                 Console.WriteLine("Error: cannot obtain image location, environment variable not present.");
                 return;
            }
            inputFileName += "\\Images\\Display\\" + INPUT_FILE_NAME;
            
             // Allocate input buffer with parameters compatible to file (does not load it)
            inBuf.SetParametersFromFile(inputFileName,SapBuffer.MemoryType.Default);
            if (!inBuf.Create())
            {
                DestroysObjects(inBuf, outBuf, compBuf, inView, outView);
                return;
            }
          
            // Allocate output buffer with parameters compatible to input buffer
            outBuf.Count = inBuf.Count;
            outBuf.Width = inBuf.Width;
            outBuf.Height = inBuf.Height;
            outBuf.Format = inBuf.Format;
            outBuf.Type = inBuf.Type;

            if (!outBuf.Create())
            {
                DestroysObjects(inBuf, outBuf, compBuf, inView, outView);
                return;
            }

            // Create view objects     
            if (!inView.Create())
            {
                DestroysObjects(inBuf, outBuf, compBuf, inView, outView);
                return;
            }

            if (!outView.Create())
            {
                DestroysObjects(inBuf, outBuf, compBuf, inView, outView);
                return;
            }

            inView.WindowTitle =  "Input Image";
            outView.WindowTitle = "Output Image";

            // Load input image
            if (!inBuf.Load(inputFileName, -1))
            {
                DestroysObjects(inBuf, outBuf, compBuf, inView, outView);
                return;
            }

            // Allocate the three component buffers
            compBuf.Count = 3;
            compBuf.Width = inBuf.Width;
            compBuf.Height = inBuf.Height;
            compBuf.Format = SapFormat.Mono8;
            compBuf.Type = inBuf.Type;

            if (!compBuf.Create())
            {
                DestroysObjects(inBuf, outBuf, compBuf, inView, outView);
                return;
            }
         
            //// Apply simple processing in the monochrome components (pixel inversion)    
            Console.WriteLine("Start Color Split Processing.. Wait\n");
            // Split input image in three monochrome components
            compBuf.SplitComponents(inBuf);

            // Apply simple processing in the monochrome components (pixel inversion)
            int numPix = compBuf.Width * compBuf.Height;
            byte[] dataBuf = new byte[numPix];

            GCHandle dataBufHandle = GCHandle.Alloc(dataBuf, GCHandleType.Pinned);
            IntPtr dataBufAddress = dataBufHandle.AddrOfPinnedObject();

            // Process the first two components using a temporary array
            for (int compIndex = 0; compIndex < 2; compIndex++)
            {
                compBuf.Read(compIndex, 0, numPix, dataBufAddress);
                for (int pixIndex = 0; pixIndex < numPix; pixIndex++)
                {
                    dataBuf[pixIndex] = (byte)(255 - dataBuf[pixIndex]);
                }
                compBuf.Write(compIndex, 0, numPix, dataBufAddress);
            }

            dataBufHandle.Free();

            // Process the third component use direct buffer data access.
            unsafe
            {
                // Retrieve the data address of the component
                compBuf.GetAddress(2, out dataBufAddress);
                byte* pData = (byte*)dataBufAddress.ToPointer();

                for (int pixIndex = 0; pixIndex < numPix; pixIndex++)
                {
                    pData[pixIndex] = (byte)(255 - pData[pixIndex]);
                }
            }

            //Merge the three monochrome components into output image
            outBuf.MergeComponents(compBuf);
            Console.WriteLine("End Processing.");
            // Display input and output buffers
            inView.Show();
            outView.Show();

            Console.WriteLine("Press any key to terminate.");
            Console.ReadKey();

            DestroysObjects(inBuf, outBuf, compBuf, inView, outView);

        }

        static void DestroysObjects(SapBuffer inBuffer, SapBuffer outBuffer, SapBuffer compBuffer, SapView inView, SapView outView)
        {
            if (inView != null)
            {
                inView.Destroy();
                inView.Dispose();
            }

            if (outView != null)
            {
                outView.Destroy();
                outView.Dispose();
            }

            if (inBuffer != null)
            {
                inBuffer.Destroy();
                inBuffer.Dispose();
            }

            if (outBuffer != null)
            {
                outBuffer.Destroy();
                outBuffer.Dispose();
            }

            if (compBuffer != null)
            {
                compBuffer.Destroy();
                compBuffer.Dispose();
            }
        }
    }
}
