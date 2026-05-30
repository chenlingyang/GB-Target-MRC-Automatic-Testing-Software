/*
 * The following declarations are extracted from the <SAPERA_DEV>\externals\FileFormats\JPEGfiles folder
 * which is not redistributed along with the SaperaLT installation.
 */

#ifndef _JPEGDEF_H_
#define _JPEGDEF_H_


#define jpeg_common_fields \
struct jpeg_error_mgr * err;	/* Error handler module */\
struct jpeg_memory_mgr * mem;	/* Memory manager module */\
struct jpeg_progress_mgr * progress; /* Progress monitor, or NULL if none */\
   void * client_data;		/* Available for use by application */\
   boolean is_decompressor;	/* So common code can tell which is which */\
   int global_state		/* For checking call sequence validity */

#define JMETHOD(type,methodname,arglist)  type (*methodname) arglist

#define D_MAX_BLOCKS_IN_MCU   10    /* decompressor's limit on blocks per MCU */
#define MAX_COMPS_IN_SCAN     4     /* JPEG limit on # of components in one scan */
#define NUM_ARITH_TBLS        16    /* Arith-coding tables are numbered 0..15 */
#define NUM_HUFF_TBLS         4     /* Huffman tables are numbered 0..3 */
#define NUM_QUANT_TBLS        4     /* Quantization tables are numbered 0..3 */
#define DCTSIZE2              64    /* DCTSIZE squared; # of elements in a block */
#define FAR far

typedef unsigned char JOCTET;
typedef unsigned int JDIMENSION;
typedef struct jpeg_decompress_struct * j_decompress_ptr;
typedef char JSAMPLE;
typedef JSAMPLE FAR *JSAMPROW;   /* ptr to one image row of pixel samples. */
typedef JSAMPROW *JSAMPARRAY;    /* ptr to some rows (a 2-D sample array) */
typedef struct jpeg_marker_struct FAR * jpeg_saved_marker_ptr;

/* Data source object for decompression */
struct jpeg_source_mgr {
   const JOCTET * next_input_byte;   /* => next byte to read from buffer */
   size_t bytes_in_buffer;           /* # of bytes remaining in buffer */

   JMETHOD(void, init_source, (j_decompress_ptr cinfo));
   JMETHOD(boolean, fill_input_buffer, (j_decompress_ptr cinfo));
   JMETHOD(void, skip_input_data, (j_decompress_ptr cinfo, long num_bytes));
   JMETHOD(boolean, resync_to_restart, (j_decompress_ptr cinfo, int desired));
   JMETHOD(void, term_source, (j_decompress_ptr cinfo));
};


/* Known color spaces. */
typedef enum {
   JCS_UNKNOWN,		/* error/unspecified */
   JCS_GRAYSCALE,		/* monochrome */
   JCS_RGB,		/* red/green/blue */
   JCS_YCbCr,		/* Y/Cb/Cr (also known as YUV) */
   JCS_CMYK,		/* C/M/Y/K */
   JCS_YCCK		/* Y/Cb/Cr/K */
} J_COLOR_SPACE;

/* DCT/IDCT algorithm options. */
typedef enum {
   JDCT_ISLOW,		/* slow but accurate integer algorithm */
   JDCT_IFAST,		/* faster, less accurate integer method */
   JDCT_FLOAT		/* floating-point: accurate, fast on fast HW */
} J_DCT_METHOD;

/* Dithering options for decompression. */
typedef enum {
   JDITHER_NONE,		/* no dithering */
   JDITHER_ORDERED,	/* simple ordered dither */
   JDITHER_FS		/* Floyd-Steinberg error diffusion dither */
} J_DITHER_MODE;

/* DCT coefficient quantization tables. */
typedef struct {
   /* This array gives the coefficient quantizers in natural array order
   * (not the zigzag order in which they are stored in a JPEG DQT marker).
   * CAUTION: IJG versions prior to v6a kept this array in zigzag order.
   */
   UINT16 quantval[DCTSIZE2];	/* quantization step for each coefficient */
   /* This field is used only during compression.  It's initialized FALSE when
   * the table is created, and set TRUE when it's been output to the file.
   * You could suppress output of a table by setting this to TRUE.
   * (See jpeg_suppress_tables for an example.)
   */
   boolean sent_table;		/* TRUE when table has been output */
} JQUANT_TBL;

/* Huffman coding tables. */
typedef struct {
   /* These two fields directly represent the contents of a JPEG DHT marker */
   UINT8 bits[17];		/* bits[k] = # of symbols with codes of */
   /* length k bits; bits[0] is unused */
   UINT8 huffval[256];		/* The symbols, in order of incr code length */
   /* This field is used only during compression.  It's initialized FALSE when
   * the table is created, and set TRUE when it's been output to the file.
   * You could suppress output of a table by setting this to TRUE.
   * (See jpeg_suppress_tables for an example.)
   */
   boolean sent_table;		/* TRUE when table has been output */
} JHUFF_TBL;

/* Basic info about one component (color channel). */
typedef struct {
   /* These values are fixed over the whole image. */
   /* For compression, they must be supplied by parameter setup; */
   /* for decompression, they are read from the SOF marker. */
   int component_id;		/* identifier for this component (0..255) */
   int component_index;		/* its index in SOF or cinfo->comp_info[] */
   int h_samp_factor;		/* horizontal sampling factor (1..4) */
   int v_samp_factor;		/* vertical sampling factor (1..4) */
   int quant_tbl_no;		/* quantization table selector (0..3) */
   /* These values may vary between scans. */
   /* For compression, they must be supplied by parameter setup; */
   /* for decompression, they are read from the SOS marker. */
   /* The decompressor output side may not use these variables. */
   int dc_tbl_no;		/* DC entropy table selector (0..3) */
   int ac_tbl_no;		/* AC entropy table selector (0..3) */

   /* Remaining fields should be treated as private by applications. */

   /* These values are computed during compression or decompression startup: */
   /* Component's size in DCT blocks.
   * Any dummy blocks added to complete an MCU are not counted; therefore
   * these values do not depend on whether a scan is interleaved or not.
   */
   JDIMENSION width_in_blocks;
   JDIMENSION height_in_blocks;
   /* Size of a DCT block in samples.  Always DCTSIZE for compression.
   * For decompression this is the size of the output from one DCT block,
   * reflecting any scaling we choose to apply during the IDCT step.
   * Values of 1,2,4,8 are likely to be supported.  Note that different
   * components may receive different IDCT scalings.
   */
   int DCT_scaled_size;
   /* The downsampled dimensions are the component's actual, unpadded number
   * of samples at the main buffer (preprocessing/compression interface), thus
   * downsampled_width = ceil(image_width * Hi/Hmax)
   * and similarly for height.  For decompression, IDCT scaling is included, so
   * downsampled_width = ceil(image_width * Hi/Hmax * DCT_scaled_size/DCTSIZE)
   */
   JDIMENSION downsampled_width;	 /* actual width in samples */
   JDIMENSION downsampled_height; /* actual height in samples */
   /* This flag is used only for decompression.  In cases where some of the
   * components will be ignored (eg grayscale output from YCbCr image),
   * we can skip most computations for the unused components.
   */
   boolean component_needed;	/* do we need the value of this component? */

   /* These values are computed before starting a scan of the component. */
   /* The decompressor output side may not use these variables. */
   int MCU_width;		/* number of blocks per MCU, horizontally */
   int MCU_height;		/* number of blocks per MCU, vertically */
   int MCU_blocks;		/* MCU_width * MCU_height */
   int MCU_sample_width;		/* MCU width in samples, MCU_width*DCT_scaled_size */
   int last_col_width;		/* # of non-dummy blocks across in last MCU */
   int last_row_height;		/* # of non-dummy blocks down in last MCU */

   /* Saved quantization table for component; NULL if none yet saved.
   * See jdinput.c comments about the need for this information.
   * This field is currently used only for decompression.
   */
   JQUANT_TBL * quant_table;

   /* Private per-component storage for DCT or IDCT subsystem. */
   void * dct_table;
} jpeg_component_info;

struct jpeg_decompress_struct {
   jpeg_common_fields;		/* Fields shared with jpeg_compress_struct */

   /* Source of compressed data */
   struct jpeg_source_mgr * src;

   /* Basic description of image --- filled in by jpeg_read_header(). */
   /* Application may inspect these values to decide how to process image. */

   JDIMENSION image_width;	/* nominal image width (from SOF marker) */
   JDIMENSION image_height;	/* nominal image height */
   int num_components;		/* # of color components in JPEG image */
   J_COLOR_SPACE jpeg_color_space; /* colorspace of JPEG image */

   /* Decompression processing parameters --- these fields must be set before
   * calling jpeg_start_decompress().  Note that jpeg_read_header() initializes
   * them to default values.
   */

   J_COLOR_SPACE out_color_space; /* colorspace for output */

   unsigned int scale_num, scale_denom; /* fraction by which to scale image */

   double output_gamma;		/* image gamma wanted in output */

   boolean buffered_image;	/* TRUE=multiple output passes */
   boolean raw_data_out;		/* TRUE=downsampled data wanted */

   J_DCT_METHOD dct_method;	/* IDCT algorithm selector */
   boolean do_fancy_upsampling;	/* TRUE=apply fancy upsampling */
   boolean do_block_smoothing;	/* TRUE=apply interblock smoothing */

   boolean quantize_colors;	/* TRUE=colormapped output wanted */
   /* the following are ignored if not quantize_colors: */
   J_DITHER_MODE dither_mode;	/* type of color dithering to use */
   boolean two_pass_quantize;	/* TRUE=use two-pass color quantization */
   int desired_number_of_colors;	/* max # colors to use in created colormap */
   /* these are significant only in buffered-image mode: */
   boolean enable_1pass_quant;	/* enable future use of 1-pass quantizer */
   boolean enable_external_quant;/* enable future use of external colormap */
   boolean enable_2pass_quant;	/* enable future use of 2-pass quantizer */

   /* Description of actual output image that will be returned to application.
   * These fields are computed by jpeg_start_decompress().
   * You can also use jpeg_calc_output_dimensions() to determine these values
   * in advance of calling jpeg_start_decompress().
   */

   JDIMENSION output_width;	/* scaled image width */
   JDIMENSION output_height;	/* scaled image height */
   int out_color_components;	/* # of color components in out_color_space */
   int output_components;	/* # of color components returned */
   /* output_components is 1 (a colormap index) when quantizing colors;
   * otherwise it equals out_color_components.
   */
   int rec_outbuf_height;	/* min recommended height of scanline buffer */
   /* If the buffer passed to jpeg_read_scanlines() is less than this many rows
   * high, space and time will be wasted due to unnecessary data copying.
   * Usually rec_outbuf_height will be 1 or 2, at most 4.
   */

   /* When quantizing colors, the output colormap is described by these fields.
   * The application can supply a colormap by setting colormap non-NULL before
   * calling jpeg_start_decompress; otherwise a colormap is created during
   * jpeg_start_decompress or jpeg_start_output.
   * The map has out_color_components rows and actual_number_of_colors columns.
   */
   int actual_number_of_colors;	/* number of entries in use */
   JSAMPARRAY colormap;		/* The color map as a 2-D pixel array */

   /* State variables: these variables indicate the progress of decompression.
   * The application may examine these but must not modify them.
   */

   /* Row index of next scanline to be read from jpeg_read_scanlines().
   * Application may use this to control its processing loop, e.g.,
   * "while (output_scanline < output_height)".
   */
   JDIMENSION output_scanline;	/* 0 .. output_height-1  */

   /* Current input scan number and number of iMCU rows completed in scan.
   * These indicate the progress of the decompressor input side.
   */
   int input_scan_number;	/* Number of SOS markers seen so far */
   JDIMENSION input_iMCU_row;	/* Number of iMCU rows completed */

   /* The "output scan number" is the notional scan being displayed by the
   * output side.  The decompressor will not allow output scan/row number
   * to get ahead of input scan/row, but it can fall arbitrarily far behind.
   */
   int output_scan_number;	/* Nominal scan number being displayed */
   JDIMENSION output_iMCU_row;	/* Number of iMCU rows read */

   /* Current progression status.  coef_bits[c][i] indicates the precision
   * with which component c's DCT coefficient i (in zigzag order) is known.
   * It is -1 when no data has yet been received, otherwise it is the point
   * transform (shift) value for the most recent scan of the coefficient
   * (thus, 0 at completion of the progression).
   * This pointer is NULL when reading a non-progressive file.
   */
   int (*coef_bits)[DCTSIZE2];	/* -1 or current Al value for each coef */

   /* Internal JPEG parameters --- the application usually need not look at
   * these fields.  Note that the decompressor output side may not use
   * any parameters that can change between scans.
   */

   /* Quantization and Huffman tables are carried forward across input
   * datastreams when processing abbreviated JPEG datastreams.
   */

   JQUANT_TBL * quant_tbl_ptrs[NUM_QUANT_TBLS];
   /* ptrs to coefficient quantization tables, or NULL if not defined */

   JHUFF_TBL * dc_huff_tbl_ptrs[NUM_HUFF_TBLS];
   JHUFF_TBL * ac_huff_tbl_ptrs[NUM_HUFF_TBLS];
   /* ptrs to Huffman coding tables, or NULL if not defined */

   /* These parameters are never carried across datastreams, since they
   * are given in SOF/SOS markers or defined to be reset by SOI.
   */

   int data_precision;		/* bits of precision in image data */

   jpeg_component_info * comp_info;
   /* comp_info[i] describes component that appears i'th in SOF */

   boolean progressive_mode;	/* TRUE if SOFn specifies progressive mode */
   boolean arith_code;		/* TRUE=arithmetic coding, FALSE=Huffman */

   UINT8 arith_dc_L[NUM_ARITH_TBLS]; /* L values for DC arith-coding tables */
   UINT8 arith_dc_U[NUM_ARITH_TBLS]; /* U values for DC arith-coding tables */
   UINT8 arith_ac_K[NUM_ARITH_TBLS]; /* Kx values for AC arith-coding tables */

   unsigned int restart_interval; /* MCUs per restart interval, or 0 for no restart */

   /* These fields record data obtained from optional markers recognized by
   * the JPEG library.
   */
   boolean saw_JFIF_marker;	/* TRUE iff a JFIF APP0 marker was found */
   /* Data copied from JFIF marker; only valid if saw_JFIF_marker is TRUE: */
   UINT8 JFIF_major_version;	/* JFIF version number */
   UINT8 JFIF_minor_version;
   UINT8 density_unit;		/* JFIF code for pixel size units */
   UINT16 X_density;		/* Horizontal pixel density */
   UINT16 Y_density;		/* Vertical pixel density */
   boolean saw_Adobe_marker;	/* TRUE iff an Adobe APP14 marker was found */
   UINT8 Adobe_transform;	/* Color transform code from Adobe marker */

   boolean CCIR601_sampling;	/* TRUE=first samples are cosited */

   /* Aside from the specific data retained from APPn markers known to the
   * library, the uninterpreted contents of any or all APPn and COM markers
   * can be saved in a list for examination by the application.
   */
   jpeg_saved_marker_ptr marker_list; /* Head of list of saved markers */

   /* Remaining fields are known throughout decompressor, but generally
   * should not be touched by a surrounding application.
   */

   /*
   * These fields are computed during decompression startup
   */
   int max_h_samp_factor;	/* largest h_samp_factor */
   int max_v_samp_factor;	/* largest v_samp_factor */

   int min_DCT_scaled_size;	/* smallest DCT_scaled_size of any component */

   JDIMENSION total_iMCU_rows;	/* # of iMCU rows in image */
   /* The coefficient controller's input and output progress is measured in
   * units of "iMCU" (interleaved MCU) rows.  These are the same as MCU rows
   * in fully interleaved JPEG scans, but are used whether the scan is
   * interleaved or not.  We define an iMCU row as v_samp_factor DCT block
   * rows of each component.  Therefore, the IDCT output contains
   * v_samp_factor*DCT_scaled_size sample rows of a component per iMCU row.
   */

   JSAMPLE * sample_range_limit; /* table for fast range-limiting */

   /*
   * These fields are valid during any one scan.
   * They describe the components and MCUs actually appearing in the scan.
   * Note that the decompressor output side must not use these fields.
   */
   int comps_in_scan;		/* # of JPEG components in this scan */
   jpeg_component_info * cur_comp_info[MAX_COMPS_IN_SCAN];
   /* *cur_comp_info[i] describes component that appears i'th in SOS */

   JDIMENSION MCUs_per_row;	/* # of MCUs across the image */
   JDIMENSION MCU_rows_in_scan;	/* # of MCU rows in the image */

   int blocks_in_MCU;		/* # of DCT blocks per MCU */
   int MCU_membership[D_MAX_BLOCKS_IN_MCU];
   /* MCU_membership[i] is index in cur_comp_info of component owning */
   /* i'th block in an MCU */

   int Ss, Se, Ah, Al;		/* progressive JPEG parameters for scan */

   /* This field is shared between entropy decoder and marker parser.
   * It is either zero or the code of a JPEG marker that has been
   * read from the data source, but has not yet been processed.
   */
   int unread_marker;

   /*
   * Links to decompression subobjects (methods, private variables of modules)
   */
   struct jpeg_decomp_master * master;
   struct jpeg_d_main_controller * main;
   struct jpeg_d_coef_controller * coef;
   struct jpeg_d_post_controller * post;
   struct jpeg_input_controller * inputctl;
   struct jpeg_marker_reader * marker;
   struct jpeg_entropy_decoder * entropy;
   struct jpeg_inverse_dct * idct;
   struct jpeg_upsampler * upsample;
   struct jpeg_color_deconverter * cconvert;
   struct jpeg_color_quantizer * cquantize;
};



#endif // _JPEGDEF_H_