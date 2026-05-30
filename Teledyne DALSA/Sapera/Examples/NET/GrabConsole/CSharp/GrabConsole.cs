using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.Examples.NET.Utils;

namespace DALSA.SaperaLT.Examples.NET.CSharp.GrabConsole
{
   class GrabConsole
   {
      static float lastFrameRate = 0.0f;

      static void xfer_XferNotify(object sender, SapXferNotifyEventArgs args)
      {
         // refresh view
         SapView View = args.Context as SapView;
         View.Show();

         // refresh frame rate
         SapTransfer transfer = sender as SapTransfer;
         if (transfer.UpdateFrameRateStatistics())
         {
            SapXferFrameRateInfo stats = transfer.FrameRateStatistics;
            float framerate = 0.0f;

            if (stats.IsLiveFrameRateAvailable)
               framerate = stats.LiveFrameRate;

            // check if frame rate is stalled
            if (stats.IsLiveFrameRateStalled)
            {
               Console.WriteLine("Live Frame rate is stalled.");
            }

            // update FPS only if the value changed by +/- 0.1
            else if ((framerate > 0.0f) && (Math.Abs(lastFrameRate - framerate) > 0.1f))
            {
               Console.WriteLine("Grabbing at {0} frames/sec", framerate);
               lastFrameRate = framerate;
            }
         }
      }

      static void Main(string[] args)
      {
         SapAcquisition Acq = null;
         SapAcqDevice AcqDevice = null;
         SapBuffer Buffers = null;
         SapTransfer Xfer = null;
         SapView View = null;

         Console.WriteLine("Sapera Console Grab Example (C# version)\n");

         MyAcquisitionParams acqParams = new MyAcquisitionParams();

         // Call GetOptions to determine which acquisition device to use and which config
         // file (CCF) should be loaded to configure it.
         if (!GetOptions(args, acqParams))
         {
            Console.WriteLine("\nPress any key to terminate\n");
            Console.ReadKey(true);
            return;
         }

         SapLocation loc = new SapLocation(acqParams.ServerName, acqParams.ResourceIndex);

         if (SapManager.GetResourceCount(acqParams.ServerName, SapManager.ResourceType.Acq) > 0)
         {
            Acq = new SapAcquisition(loc, acqParams.ConfigFileName);
            Buffers = new SapBufferWithTrash(2, Acq, SapBuffer.MemoryType.ScatterGather);
            Xfer = new SapAcqToBuf(Acq, Buffers);

            // Create acquisition object
            if (!Acq.Create())
            {
               Console.WriteLine("Error during SapAcquisition creation!\n");
               DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
               return;
            }
            Acq.EnableEvent(SapAcquisition.AcqEventType.StartOfFrame);

         }

         else if (SapManager.GetResourceCount(acqParams.ServerName, SapManager.ResourceType.AcqDevice) > 0)
         {
            AcqDevice = new SapAcqDevice(loc, acqParams.ConfigFileName);
            Buffers = new SapBufferWithTrash(2, AcqDevice, SapBuffer.MemoryType.ScatterGather);
            Xfer = new SapAcqDeviceToBuf(AcqDevice, Buffers);

            // Create acquisition object
            if (!AcqDevice.Create())
            {
               Console.WriteLine("Error during SapAcqDevice creation!\n");
               DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
               return;
            }
         }

         View = new SapView(Buffers);

         // End of frame event
         Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
         Xfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
         Xfer.XferNotifyContext = View;

         // Create buffer object
         if (!Buffers.Create())
         {
            Console.WriteLine("Error during SapBuffer creation!\n");
            DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
            return;
         }

         // Create buffer object
         if (!Xfer.Create())
         {
            Console.WriteLine("Error during SapTransfer creation!\n");
            DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
            return;
         }

         // Create buffer object
         if (!View.Create())
         {
            Console.WriteLine("Error during SapView creation!\n");
            DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
            return;
         }


         Xfer.Grab();
         Console.WriteLine("\n\nGrab started, press a key to freeze");
         Console.ReadKey(true);
         Xfer.Freeze();
         Xfer.Wait(1000);

         DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
         loc.Dispose();
      }


      static bool GetOptions(string[] args, MyAcquisitionParams acqParams)
      {
         // Check if arguments were passed
         if (args.Length > 1)
            return ExampleUtils.GetOptionsFromCommandLine(args, acqParams);
         else
            return ExampleUtils.GetOptionsFromQuestions(acqParams);
      }


      static void DestroysObjects(SapAcquisition acq, SapAcqDevice camera, SapBuffer buf, SapTransfer xfer, SapView view)
      {

         if (xfer != null)
         {
            xfer.Destroy();
            xfer.Dispose();
         }

         if (camera != null)
         {
            camera.Destroy();
            camera.Dispose();
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
         Console.ReadKey(true);
      }
   }
}
