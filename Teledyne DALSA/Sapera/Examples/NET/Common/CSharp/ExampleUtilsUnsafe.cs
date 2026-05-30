using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using DALSA.SaperaLT.SapClassBasic;


namespace DALSA.SaperaLT.Examples.NET.Utils
{
    public class ExampleUtilsUnsafe
    {
        static public unsafe void DisplayProfile(SapView View, SapBuffer Buffer)
        {
            SapBuffer Buffer_View = View.Buffer;
            IntPtr pData_viewbuf;
            IntPtr pData_buf;

            Buffer_View.Clear();
            Buffer_View.GetAddress(out pData_viewbuf);
            Buffer.GetAddress(out pData_buf);

            ushort* begin_viewbuf = (ushort*)pData_viewbuf.ToPointer();
            ushort* begin_buf = (ushort*)pData_buf.ToPointer();

            for (int i = 0; i < Buffer_View.Width; ++i)
            {
                int value = *(begin_buf + i) / ExampleUtils.CLUSTERS_DIVISOR; //Convert [0-MAX_USHORT[ to  [0, WINDOWS_HEIGHT[
                *(begin_viewbuf + (Buffer_View.Width * (ExampleUtils.PROFILE_WINDOW_HEIGHT - value - 1) + i)) = ushort.MaxValue;
            }
            View.Show();
        }

         static public unsafe void ReadPoint(SapBuffer[] Buffers, int xIndex, int yIndex, SapFormat Pixel_Format,
             double Scale_A, double Offset_A, double Scale_C, double Offset_C,
             SapBuffer.Val Output_Mode, ref int X, ref int Z, ref int R)
         {
            SapDataMono value = new SapDataMono(0);
            switch (Pixel_Format)
            {
               case SapFormat.Coord3D_C16:
               {
                  Buffers[1].ReadElement(xIndex, yIndex, value);   // Native Z
                  Z = value.Mono * (int)Scale_C + (int)Offset_C;   // Real Z
                  if (Output_Mode == SapBuffer.Val.SCAN3D_OUTPUT_MODE_RECTIFIED_C)
                  {
                     // Real X (can be calculated from X index only in Rectified output mode)
                     X = xIndex * (int)Scale_A + (int)Offset_A;
                  }
               }
               break;
               case SapFormat.Coord3D_AC16:
               {
                  Buffers[0].ReadElement(xIndex, yIndex, value);   // Native X
                  X = value.Mono * (int)Scale_A + (int)Offset_A;   // Real X
                  Buffers[1].ReadElement(xIndex, yIndex, value);   // Native Z
                  Z = value.Mono * (int)Scale_C + (int)Offset_C;   // Real Z
               }
               break;
               case SapFormat.Coord3D_ACRW16:
               {
                  Buffers[0].ReadElement(xIndex, yIndex, value);   // Native X
                  X = value.Mono * (int)Scale_A + (int)Offset_A;   // Real X
                  Buffers[1].ReadElement(xIndex, yIndex, value);   // Native Z
                  Z = value.Mono * (int)Scale_C + (int)Offset_C;   // Real Z
                  Buffers[2].ReadElement(xIndex, yIndex, value);   // Native R
                  R = value.Mono;                                  // Real R = Native R
               }
               break;
         }
      }

        //Process (Mean) the C component of each line and does a mean of mean. Get World and Pixel Mean
        static public unsafe void ProcessingRange(SapBuffer Buffer, int Invalid_Flag_C, double Invalid_Value_C,
           double Scale_C, double Offset_C, ref int Pixel_Mean, ref int World_Mean)
        {
            int Nb_Pixels = 0;
            int Pixel_Mean_Line = 0;
            IntPtr pData_buf;

            Buffer.GetAddress(out pData_buf);
            ushort* begin = (ushort*)pData_buf;

            if (Invalid_Flag_C != 0) //Check for Invalid Values only if the flag is true.
            {
                for (int i = 0; i < Buffer.Width * Buffer.Height; ++i)
                {
                    if (*(begin + i) != Invalid_Value_C)
                    {
                        Pixel_Mean_Line += *(begin + i);
                        Nb_Pixels++;
                    }
                    if (i % Buffer.Width == 0)
                    {
                        if (Nb_Pixels != 0)
                            Pixel_Mean += Pixel_Mean_Line / Nb_Pixels;
                        Pixel_Mean_Line = 0;
                        Nb_Pixels = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Buffer.Width * Buffer.Height; ++i)
                {
                    Pixel_Mean_Line += *(begin + i);
                    if (i % Buffer.Width == 0)
                    {
                        Pixel_Mean += Pixel_Mean_Line / Buffer.Width;
                        Pixel_Mean_Line = 0;
                    }
                }
                Nb_Pixels = Buffer.Width * Buffer.Height;
            }

            Pixel_Mean = Pixel_Mean / Buffer.Height;

            // Pixel to World : (Position Pixel X * Scale X) + Offset X = Position Pixel World
            World_Mean = (Pixel_Mean * (int)Scale_C) + (int)Offset_C;
        }

        //Process (Mean) the C component of a single line. Get World and Pixel Mean
        static public unsafe void ProcessingProfile(SapBuffer Buffer, int Invalid_Flag_C, double Invalid_Value_C,
           double Scale_C, double Offset_C, ref int Pixel_Mean, ref int World_Mean)
        {
            int Nb_Pixels = 0;
            IntPtr pData_buf;

            Buffer.GetAddress(out pData_buf);
            ushort* begin = (ushort*)pData_buf;

            if (Invalid_Flag_C != 0) //Check for Invalid Values only if the flag is true.
            {
                for (int i = 0; i < Buffer.Width; ++i)
                {
                    if (*(begin + i) != Invalid_Value_C)
                    {
                        Pixel_Mean += *(begin + i);
                        Nb_Pixels++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Buffer.Width; ++i)
                {
                    Pixel_Mean += *(begin + i);
                }
                Nb_Pixels = Buffer.Width;
            }
            if (Nb_Pixels != 0)
                Pixel_Mean = Pixel_Mean / Nb_Pixels;

            // Pixel to World : (Position Pixel X * Scale X) + Offset X = Position Pixel World
            World_Mean = (Pixel_Mean * (int)Scale_C) + (int)Offset_C;
        }
    }
}
