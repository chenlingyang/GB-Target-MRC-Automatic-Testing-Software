// -----------------------------------------------------------------------------------------
// Sapera++ console file load 3D example  
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
  "Range_C16.crc ",      
  "Profile_C16.crc",
  "Range_C16.tif",
  "Profile_C16.tif",
  "Range_AC16.crc",
  "Profile_AC16.crc",
  "Range_ACRW16.crc",
  "Profile_ACRW16.crc"
};

int FileListSize = sizeof(FileList) / sizeof(FileList[0]);  //Number of files for input choice

//enums
enum FILE_FORMAT {tif,crc};

//Parameters 
int Invalid_Flag_C_Parameter;         // Flag (0 or else) expressing if invalid values must be taking into account
double Invalid_Value_C_Parameter;     // The Z value flagged as invalid
double Scale_A_Parameter;             // Scale from Pixel to World according to the A (X) component
double Offset_A_Parameter;            // Offset from Pixel to World according to the X (Xw) component
double Scale_C_Parameter;             // Scale from Pixel to World according to the C (Z) component
double Offset_C_Parameter;            // Offset from Pixel to World according to the C (Z) component
int Output_Mode_Parameter;            // Output Mode of the device

//File Attributes
SapFormat Pixel_Format_Attribute;  //AC, C, CR or ACRW
BUFFER_LAYOUT Buffer_Layout_Parameter;  //Range or Profile
FILE_FORMAT File_Format_Attribute;  //Range or Profile

int Index_DisplayedComponent = 1;  //Index in the split buffer to display


int main(int argc, char* argv[])
{
   char filename_load[MAX_PATH];
   char filename_save[MAX_PATH];
   char *strPosition;

   printf("Sapera Console File Load 3D Example (C++ version)\n");

   // Call GetFileOptions to determine which file to load
   if(!GetFileOptions(argc, argv, "Images\\Display_3D\\", FileList, FileListSize, filename_load, sizeof(filename_load)))
   {
      printf("Press any key to terminate\n");
      CorGetch(); 
      return 0;
   }

   if (strstr(filename_load, ".crc") != NULL)
      File_Format_Attribute = crc;
   else
	   File_Format_Attribute = tif;

   // Generate file name to save from file name to load
   CorStrncpy(filename_save, filename_load, MAX_PATH);
   strPosition = strstr(filename_save, ".");
   *strPosition = '_';
   CorStrncat(filename_save, ".tif", MAX_PATH - strlen(filename_save));

   // Allocate buffer with parameters compatible to file (does not load it)
    SapBuffer Buffers(filename_load);
	//Array of 4 buffers, each one holding a Component : A(X), C(Z), R(Reflectance), W(unused)
	SapBuffer Buffer_Components_Split[4];
	//The Buffer linked with the Viewing tool. Changing it will affect the display
	SapBuffer ViewBuffer;
	//Viewing tool
	SapView View(&ViewBuffer, SapHwndAutomatic);

   // Create buffer object
   if (!Buffers.Create())
      goto FreeHandles;

   Pixel_Format_Attribute = Buffers.GetFormat();
   if (Pixel_Format_Attribute == SapFormatMono16)
      Pixel_Format_Attribute = SapFormatCoord3D_C16;

   if (Buffers.GetHeight() == 1)
   	Buffer_Layout_Parameter = Profile;
   else
   	Buffer_Layout_Parameter = Range;

  if (File_Format_Attribute == crc)  //Only .crc files support the saving of parameters. .tiff use the defaults parameters
  {
	  //Exctract the parameters values from a .crc : Invalid Value, Invalid Flag, Scale and Offset.
	  if (!GetParametersFrom3DBuffer(&Buffers, &Invalid_Flag_C_Parameter, &Invalid_Value_C_Parameter,
         &Scale_A_Parameter, &Offset_A_Parameter, &Scale_C_Parameter, &Offset_C_Parameter,
         &Buffer_Layout_Parameter, &Output_Mode_Parameter))
     {
		  goto FreeHandles;
     }
  }

   //Contains 1 buffer for each possible component
   //  Buffer 0 = A (X)
   //  Buffer 1 = C (Z)
   //  Buffer 2 = R (Reflectance)
   //  Buffer 3 = W (unused)
   for (int bufIndex = 0; bufIndex < 4; bufIndex++)
   {
      //Setting the buffer general parameters (Count,Width,Height,Format,Type) according to the loaded file. We have to split into Mono Format Buffers.
      Buffer_Components_Split[bufIndex].SetParameters(Buffers.GetCount(), Buffers.GetWidth(), Buffers.GetHeight(), SapFormatMono16, SapBuffer::TypeDefault);
      if (!Buffer_Components_Split[bufIndex].Create())
	      goto FreeHandles;
   }

   ViewBuffer.SetParameters(Buffers.GetCount(), Buffers.GetWidth(), PROFILE_WINDOW_HEIGHT, SapFormatMono16, -1);

   if (!Buffers.Load(filename_load, -1))
	   goto FreeHandles;

   if (!ViewBuffer.Create())
	   goto FreeHandles;

   if (!View.Create())
      goto FreeHandles; 

   switch (Pixel_Format_Attribute)  //Split the components, in the corresponding container,depending on the format
   {
   case SapFormatCoord3D_C16:
	   Buffer_Components_Split[1].Copy(&Buffers);
	   break;
   case SapFormatCoord3D_AC16:
	   Buffers.SplitComponents(&Buffer_Components_Split[0], &Buffer_Components_Split[1], NULL);
	   break;
   case SapFormatCoord3D_ACRW16:
	   Buffers.SplitComponents(&Buffer_Components_Split[0], &Buffer_Components_Split[1], &Buffer_Components_Split[2]);
	   break;
   }

	bool Displaying = true;

	while (Displaying)
	{
		//Display the buffer according to his layout. 
		if (Buffer_Layout_Parameter == Range)
		   DisplayRange(&View, &Buffer_Components_Split[Index_DisplayedComponent]);
      else
			DisplayProfile(&View, &Buffer_Components_Split[Index_DisplayedComponent]);

		//Toogle the displayed component according to user input
	   switch (Pixel_Format_Attribute)
	   {
      case SapFormatCoord3D_C16:
	      printf("\nAny Key to stop Displaying.\n");
	      CorGetch();
		   Displaying = false;
		   break;
      case SapFormatCoord3D_AC16:
		   Displaying = GetSelectionFromConsoleAC(&Index_DisplayedComponent);
		   break;
      case SapFormatCoord3D_ACRW16:
		   Displaying = GetSelectionFromConsoleACRW(&Index_DisplayedComponent);
		   break;
	   }
	}

	// Once Displayed Over, Process the image Pixel Mean and World Mean of the C component, according to parameters (defaults value if .tiff)
	int Pixel_Mean = 0;
	int World_Mean = 0;

	if (Buffer_Layout_Parameter == Range)
	{
      ProcessingRange(&Buffer_Components_Split[1],Invalid_Flag_C_Parameter, Invalid_Value_C_Parameter,
	   	   Scale_C_Parameter, Offset_C_Parameter, &Pixel_Mean, &World_Mean);
	}
   else
   {
   	ProcessingProfile(&Buffer_Components_Split[1],Invalid_Flag_C_Parameter, Invalid_Value_C_Parameter,
	   	   Scale_C_Parameter, Offset_C_Parameter, &Pixel_Mean, &World_Mean);
   }

	printf("\n\nPixel_Mean = %d, World_Mean = %d\n\n", Pixel_Mean, World_Mean);

   if (Pixel_Format_Attribute == SapFormatCoord3D_C16)
   	//Save C16 format as a tif. Parameters Lost.
   	Buffers.Save(filename_save, "-format tif");
   else
   	//Save MONO format as a tif. Parameters Lost.
   	Buffer_Components_Split[Index_DisplayedComponent].Save(filename_save, "-format tif");

FreeHandles:
   printf("Press any key to terminate.\n");
   CorGetch();

	// Destroy view object
	View.Destroy();

	// Destroy buffer object
	Buffers.Destroy();

	// Destroy Component A/C/R/W Buffer Objects
   for (int bufIndex = 0; bufIndex < 4; bufIndex++)
	   Buffer_Components_Split[bufIndex].Destroy();

	// Destroy View Buffer Object
	ViewBuffer.Destroy();

   return 0;
}
