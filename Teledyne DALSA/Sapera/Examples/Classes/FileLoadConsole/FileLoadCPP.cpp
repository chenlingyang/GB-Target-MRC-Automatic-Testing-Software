// -----------------------------------------------------------------------------------------
// Sapera++ console file load example  
// 
//    This program shows how to load an image file into a buffer in the host computer's memory,
//    using Sapera++ Buffer object.  Also, a View object is used to display the Buffer.
//
// -----------------------------------------------------------------------------------------

#include "stdio.h"
#include "conio.h"
#include "sapclassbasic.h"
#include "ExampleUtils.h"

const char *FileList[] = {
   "mono8.bmp",
   "mono16.tif",
   "rgb565.bmp",
   "rgb888.bmp",
   "rgb8888.bmp",
   "rgb101010.bmp",
   "yuy2.crc",
};

int FileListSize = sizeof(FileList) / sizeof(FileList[0]);


int main(int argc, char* argv[])
{
   char        filename[MAX_PATH];

   printf("Sapera Console File Load Example (C++ version)\n");

   // Call GetFileOptions to determine which file to load
   if(!GetFileOptions(argc, argv, "Images\\Display\\", FileList, FileListSize, filename, sizeof(filename)))
   {
      printf("Press any key to terminate\n");
      CorGetch(); 
      return 0;
   }

   // Allocate buffer with parameters compatible to file (does not load it)
   SapBuffer Buffers(filename);

   // Allocate view object
   SapView View(&Buffers, SapHwndAutomatic);

   // Create buffer object
   if (!Buffers.Create())
      goto FreeHandles;

   // Create view object
   if (!View.Create())
      goto FreeHandles;

   // Load file
   if(!Buffers.Load(filename, -1))
      goto FreeHandles;

   // Display buffer
   View.Show();

FreeHandles:
   printf("Press any key to terminate.\n");
   CorGetch();

	// Destroy view object
	if (!View.Destroy()) return FALSE;

	// Destroy buffer object
	if (!Buffers.Destroy()) return FALSE;

   return 0;
}
