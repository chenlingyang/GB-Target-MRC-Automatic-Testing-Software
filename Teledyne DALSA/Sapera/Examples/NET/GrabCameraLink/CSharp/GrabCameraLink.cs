using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.Examples.NET.Utils;

namespace DALSA.SaperaLT.Examples.NET.CSharp.GrabCameraLink
{
    class GrabCameraLink
    {
        static void xfer_XferNotify(object sender, SapXferNotifyEventArgs args)
        {
            SapView View = args.Context as SapView;
            View.Show();
        }

        static void Main(string[] args)
        {
            SapAcquisition Acq = null;
            SapAcqDevice AcqDevice = null;
            SapFeature Feature = null;
            SapBuffer Buffers = null;
            SapTransfer Xfer = null;
            SapView View = null;
            Boolean isNotSupported = false, acquisitionCreated = true, acqDeviceCreated = true, status = false;

            Console.WriteLine("Sapera Console Grab Camera Link Example (C# version)\n");

            MyAcquisitionParams acqParams = new MyAcquisitionParams();

            // Call GetOptions to determine which acquisition device to use and which config
            // file (CCF) should be loaded to configure it.
            if (!ExampleUtils.GetCorAcquisitionOptionsFromQuestions(acqParams))
            {
                Console.WriteLine("\nPress any key to terminate\n");
                Console.ReadKey(true);
                return;
            }

            SapLocation loc = new SapLocation(acqParams.ServerName, acqParams.ResourceIndex);

            if (SapManager.GetResourceCount(acqParams.ServerName, SapManager.ResourceType.Acq) > 0)
            {
                Acq         = new SapAcquisition(loc, acqParams.ConfigFileName);
                Buffers     = new SapBuffer(1, Acq, SapBuffer.MemoryType.ScatterGather);
                Xfer        = new SapAcqToBuf(Acq, Buffers);
                View        = new SapView(Buffers);

               // Create acquisition object
                if (!Acq.Create())
                {
                    acquisitionCreated = false;
                }
            }

            //get the SapAcqDevice (camera)
            if (!ExampleUtils.GetCorAcqDeviceOptionsFromQuestions(acqParams, false))
            {
                Console.WriteLine("\nPress any key to terminate\n");
                Console.ReadKey(true);
                return;
            }
            
            SapLocation loc2 = new SapLocation(acqParams.ServerName, acqParams.ResourceIndex);
            AcqDevice   = new SapAcqDevice(loc2, false);
            Feature = new SapFeature(loc2);
            Feature.Create();
            
            // Create acquisition object
            if (!AcqDevice.Create())
            {
                acqDeviceCreated = false;
            }

            //if any of the frame grabber or camera failed to initialize, end program
            if (!acquisitionCreated || !acqDeviceCreated)
                DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer, View);

            //check to see if this is a DFNC camera. If it is not, end program
            //Program checks for both current and legacy naming
            string modelName;
            status = AcqDevice.IsFeatureAvailable("DeviceModelName");
            if (status)
            {
                AcqDevice.GetFeatureValue("DeviceModelName", out modelName);
                if (modelName.Contains("Genie"))
                    isNotSupported = true;
            }
            else
            {
                isNotSupported = true;
            }

            if (isNotSupported)
            {
                Console.WriteLine("Current camera model is not supported.\n");
                DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer, View);
                return;
            }

            // End of frame event
            Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            Xfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
            Xfer.XferNotifyContext = View;

            // Create buffer object
            if (!Buffers.Create())
            {
                Console.WriteLine("Error during SapBuffer creation!\n");
                DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer, View);
                return;
            }

            // Create buffer object
            if (!Xfer.Create())
            {
                Console.WriteLine("Error during SapTransfer creation!\n");
                DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer, View);
                return;
            }

            // Create buffer object
            if (!View.Create())
            {
                Console.WriteLine("Error during SapView creation!\n");
                DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer, View);
                return;
            }

            Xfer.Grab();

            Console.WriteLine("Press any key to stop grab.");
            Console.ReadKey(true);

            // Stop grab
            Xfer.Freeze();
            if (!Xfer.Wait(5000))
            {
                Console.WriteLine("Grab could not stop properly.\n");
                Xfer.Abort();
            }

            DestroysObjects(Acq, Feature, AcqDevice, Buffers, Xfer, View);
            loc.Dispose();
            loc2.Dispose();
        }

        static void DestroysObjects(SapAcquisition acq, SapFeature feature, SapAcqDevice camera, SapBuffer buf, SapTransfer xfer, SapView view)
        {

            if (view != null)
            {
                view.Destroy();
                view.Dispose();
            }

            if (xfer != null)
            {
                xfer.Destroy();
                xfer.Dispose();
            }

            if (buf != null)
            {
                buf.Destroy();
                buf.Dispose();
            }

            if (acq != null)
            {
                acq.Destroy();
                acq.Dispose();
            }

            if (feature != null)
            {
                feature.Destroy();
                feature.Dispose();
            }

            if (camera != null)
            {
                camera.Destroy();
                camera.Dispose();
            }

            Console.WriteLine("\nPress any key to terminate\n");
            Console.ReadKey(true);
        }
    }
}
