using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.Examples.NET.Utils;

namespace DALSA.SaperaLT.Examples.NET.CSharp.GrabLut
{
    class GrabLut
    {

        static void Xfer_XferNotify(object sender, SapXferNotifyEventArgs args)
        {
            SapView view = args.Context as SapView;
            view.Show();
        }

        static void Main(string[] args)
        {
            SapAcquisition acq = null;
            SapView view = null;
            SapLut lut = null;
            SapTransfer transfer = null;
            SapBuffer buffer = null;

            Console.WriteLine("Sapera Console Grab Lut Example (C# version)\n");

            MyAcquisitionParams acqParams = new MyAcquisitionParams();


            if (!GetOptions(args, acqParams))
            {
                Console.WriteLine("\nPress any key to terminate\n");
                Console.ReadKey();
                return;
            }

            ArrayList listLutFiles = new ArrayList();
            SapLocation location = new SapLocation(acqParams.ServerName, acqParams.ResourceIndex);
            // Allocate objects
            acq = new SapAcquisition(location,acqParams.ConfigFileName);
            buffer = new SapBuffer(1, acq, SapBuffer.MemoryType.ScatterGather);
            transfer = new SapAcqToBuf(acq, buffer);
            view = new SapView(buffer);

            // Event 
            transfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            transfer.XferNotify += new SapXferNotifyHandler(Xfer_XferNotify);
            transfer.XferNotifyContext = view;

            if (!acq.Create())
            {
                Console.WriteLine("Error during SapAcqDevice creation!\n");
                DestroyObjects(acq, buffer, transfer, view);
                return;
            }

            if (!buffer.Create())
            {
                Console.WriteLine("Error during SapBuffer creation!\n");
                DestroyObjects(acq, buffer, transfer, view);
                return;
            }

            if (!transfer.Create())
            {
                Console.WriteLine("Error during SapAcqDeviceToBuf creation!\n");
                DestroyObjects(acq, buffer, transfer, view);
                return;
            }

            if (!view.Create())
            {
                Console.WriteLine("Error during SapView creation!\n");
                DestroyObjects(acq, buffer, transfer, view);
                return;
            }

            // Create a LUT compatible with the acquisition
            if(buffer.Format != SapFormat.Uint8 && buffer.Format != SapFormat.Uint16)
            {
               Console.WriteLine("Input buffer format must be 8 or 16-bit monochrome.\n");
               DestroyObjects(acq, buffer, transfer, view);
               return; 
            }

            if (acq.Luts != null)
                lut = acq.Luts[0];
            else
            {
                Console.WriteLine("The selected acquisition device has no LUTs\n");
                DestroyObjects(acq, buffer, transfer, view);
                return;
            }
            
            Console.WriteLine("\nPress any key to perform LUT operation. Press 'q' to quit.\n");
            ConsoleKeyInfo info = Console.ReadKey(true);
            char key = info.KeyChar;
           
            while (key != 'q')
            {
                if (!ProcessLut(acq, buffer, lut, listLutFiles))
                {
                    DestroyObjects(acq, buffer, transfer, view);
                    return;
                }

                transfer.Grab();
                Console.WriteLine("\n\nGrab started, press a key to freeze");
                Console.ReadKey(true);
                transfer.Freeze();
                transfer.Wait(5000);
                Console.WriteLine("\nPress any key to perform LUT operation. Press 'q' to quit.\n");
                info = Console.ReadKey(true);
                key = info.KeyChar;
            }

            DestroyObjects(acq, buffer, transfer, view);
            location.Dispose();      
        }

        static bool ProcessLut(SapAcquisition acq, SapBuffer buf, SapLut lut, ArrayList plistLutFiles)
        {

            // Enable LUT
            acq.LutEnable = true;
          
            bool bLutLoaded = false;

            // Load one of the LUT previously saved in the demo
            bLutLoaded = ExampleUtils.GetLoadLUTFiles(lut, plistLutFiles);

            if (!bLutLoaded)
            {
                string acqLutName = ExampleUtils.GetLUTOptionsFromQuestions(buf, lut);
                Console.WriteLine("\n......................... ");
                Console.WriteLine("Showing " + acqLutName);
                Console.WriteLine(".........................\n");
            }

            // Apply the LUT you created
            acq.ApplyLut(true,0);

            return true;
        }

        static bool GetOptions(string[] args, MyAcquisitionParams acparams)
        {
            // Check if arguments were passed
            if (args.Length > 1)
                return ExampleUtils.GetOptionsFromCommandLine(args, acparams);
            else
                return ExampleUtils.GetCorAcquisitionOptionsFromQuestions(acparams);
        }



        static void DestroyObjects(SapAcquisition acq, SapBuffer buf, SapTransfer xfer, SapView view)
        {

            if (xfer != null)
            {
                xfer.Destroy();
                xfer.Dispose();
            }

            if (acq != null)
            {
                acq.Destroy();
                acq.Dispose();
            }

            if (buf != null)
            {
                buf.Destroy();
                buf.Dispose();
            }

            if (view != null)
            {
                view.Destroy();
                view.Dispose();
            }

            Console.WriteLine("\nPress any key to terminate\n");
            Console.ReadKey();
        }
    }
}
