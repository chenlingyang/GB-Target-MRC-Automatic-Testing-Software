// -----------------------------------------------------------------------------------------
// Sapera++ console color split example  
// 
//    This program shows how to split and merge color images into single monochrome components.
//    An RGB image is loaded, split into three monochrome components then a simple processing is
//    applied to the three components before they are merged back to RGB as output.
//
// -----------------------------------------------------------------------------------------

#include "stdio.h"
#include "conio.h"
#include "SapClassBasic.h"

// File name definitions
#define INPUT_FILE_NAME "rgb8888.bmp"
#define OUTPUT_FILE_NAME "rgb8888_inverted.bmp"

// Main program
int main()
{
   SapBuffer inBuf, outBuf, compBuf;
   SapView inView(NULL, SapHwndAutomatic);
   SapView outView(NULL, SapHwndAutomatic);

   printf("Sapera Color Split Example\n");

   // Get image location from environment variable
   char inputFileName[MAX_PATH];
   inputFileName[0] = '\0';
   if (!GetEnvironmentVariable("SAPERADIR", inputFileName, MAX_PATH))
   {
      printf("Error: cannot obtain image location, environment variable not present.\n");
      return 0;
   }

   inputFileName[sizeof(inputFileName) - 1] = '\0';
   CorStrncat(inputFileName, "\\Images\\Display\\", sizeof(inputFileName) - strlen(inputFileName));
   CorStrncat(inputFileName, INPUT_FILE_NAME, sizeof(inputFileName) - strlen(inputFileName));

   // Allocate input buffer with parameters compatible to file (does not load it)
   inBuf.SetParametersFromFile(inputFileName, SapDefBufferType);
   if (!inBuf.Create())
      return 0;

   // Allocate output buffer with parameters compatible to input buffer
   outBuf.SetParameters(inBuf.GetCount(), inBuf.GetWidth(), inBuf.GetHeight(), inBuf.GetFormat(), inBuf.GetType());
   if (!outBuf.Create())
      return 0;

   // Associate view objects to input and output buffers
   inView.SetBuffer(&inBuf);
   if (!inView.Create())
      return 0;

   outView.SetBuffer(&outBuf);
   if (!outView.Create())
      return 0;

   inView.SetWindowTitle("Input Image");
   outView.SetWindowTitle("Output Image");

   // Load input image
   if (!inBuf.Load(inputFileName))
      return 0;

   // Allocate the three component buffers
   compBuf.SetParameters(3, inBuf.GetWidth(), inBuf.GetHeight(), SapFormatMono8, inBuf.GetType());
   if (!compBuf.Create())
      return 0;

   // Split input image in three monochrome components
   compBuf.SplitComponents(&inBuf);

   // Apply simple processing in the monochrome components (pixel inversion)
   PUINT8 pData;
   int numPix = compBuf.GetWidth() * compBuf.GetHeight();
   for (int compIndex = 0; compIndex < 3; compIndex++)
   {
      // Retrieve the data address of the current component
      compBuf.GetAddress(compIndex, (void**)&pData);

      // Process the component
      for (int pixIndex = 0; pixIndex < numPix; pixIndex++)
      {
         *pData = 255 - *pData;
         pData++;
      }
   }

   // Merge the three monochrome components into output image
   outBuf.MergeComponents(&compBuf);

   // Display input and output buffers
   inView.Show();
   outView.Show();

   // Terminate program
   printf("Press any key to terminate.\n");
   CorGetch();

   return 0;
}


