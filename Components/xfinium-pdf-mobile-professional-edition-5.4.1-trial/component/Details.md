**XFINIUM.PDF** library is a .NET library for cross-platform PDF development. Code written for **XFINIUM.PDF** can be compiled 
on all supported platforms without changes. The library features a wide range of capabilities, for both beginers and advanced PDF developers.   
The development style is based on fixed document model, where each page is created as needed and content is placed at fixed position using a grid based layout.
This gives you access to all PDF features, whether you want to create a simple file, create a transparency knockout group at COS level or convert a PDF page 
to an image and lets you build more complex models on top of the library.   
**XFINIUM.PDF** has been developed entirely in C# and it is 100% managed code. 
With **XFINIUM.PDF** you can port your PDF application logic to other platforms with zero effort which means faster time to market.
Simple licensing per developer with royalty free distribution helps you keep your costs under control.
 
The classic HelloWorld sample looks like this:  

```csharp
using Xfinium.Pdf;
using Xfinium.Pdf.Graphics;

// Create a fixed document
PdfFixedDocument document = new PdfFixedDocument();
// Add a new page
PdfPage page = document.Pages.Add();
// Create a font and a brush
PdfStandardFont helvetica = new PdfStandardFont(PdfStandardFontFace.Helvetica, 24);
PdfBrush brush = new PdfBrush();
// Draw the text on the page
page.Graphics.DrawString("Hello World", helvetica, brush, 100, 100);
// Save the document
document.Save("helloworld.pdf");
```

PDF pages can be converted to image with a few lines of code:

```csharp
PdfFixedDocument document = new PdfFixedDocument(NSBundle.MainBundle.BundlePath + "/xfinium.pdf");
PdfPageRenderer renderer = new PdfPageRenderer(document.Pages[0]);
CGImage pageImage = renderer.ConvertPageToImage(96);
```


##### SUPPORTED PLATFORMS
* Windows Phone 7, 8 & 8.1
* Xamarin.iOS
* Xamarin.Android
 
##### DOCUMENT FEATURES
* Create and load PDF documents from files and streams
* Save PDF files to disk and streams
* Document information and custom properties
* Document viewer preferences and display modes
* XMP metadata
* Document file attachments
* Document level Javascripts and actions
* Document outline (bookmarks)
* Create geospatial and CAD enabled PDF files
* Add, remove and read Bates numbers
* Add and remove PDF pages
* Page boxes - media box, crop box, art box, trim box and bleed box
* Page labeling ranges (page numbering)
* Extract pages from external PDF files
* Append PDF pages
* Split PDF files into pages
* Document incremental updates
* Partial document loading and saving
 
##### SECURITY
* User and owner passwords
* Document access rights
* 40 bit and 128 bit RC4 encryption
* 128 bit and 256 bit AES encryption with enhanced password validation (Acrobat X)
* Content redaction
* Disable text copy/paste
 
##### GRAPHICS FEATURES
* All PDF color spaces: DeviceRGB, DeviceCMYK, DeviceGray, Indexed, CalGray, CalRGB, Lab, ICC,
  Separation, DeviceN and PANTONE colors
* Pen and brush objects for stroking and filling operations
* Graphics primitives: lines, ellipses, rectangles, rounded rectangles, arcs, pies, chords, Bezier curves, paths
* Clipping paths
* Images (see Images section) and form XObjects
* Single line and multi line text with vertical and horizontal aligment, including justified text
* Extended graphics states with support for fill and stroke alpha, blend modes and overprinting
* Affine transformations: multiply, translate, rotate and scale
* Shadings - function, axial and radial
* Patterns - colored, uncolored and shading
* Optional content (layers) with support for custom display trees, multipage and mixed layers
* Barcodes (see Barcodes section) 
* Drawing of external page content (page imposition)
* Low level PDF graphics for full control over the page content stream
* Formatted content
 
##### FONTS
* Standard PDF fonts, Western and CJK
* Type1 fonts
* Type3 fonts
* Ansi and Unicode TrueType fonts with support for font subsetting
* Disable text copy/paste for Unicode TrueType fonts
 
##### IMAGES
* Load images from files and streams
* Create images from System.Drawing.Bitmap (WinForms) (BMP, GIF, PNG, TIFF, JPG)
* Native support for TIFF (grayscale, RGB and CMYK), JPEG, PNG and RAW images
* TIFF to PDF conversion with CCITT G4 compression for B/W images
* Image masks: color masks, stencil mask and soft masks
* Alternate images for printing
* SVG to PDF conversion
 
##### BARCODES
* Built in vector barcode engine, no barcode images or barcode fonts
* Unidimensional barcodes: Codabar, Code 11, Code 25, Code 25 Interleaved, Code 39, Code 39 Extended, Code 93, Code 93 Extended, Code 128 A, Code 128 B, Code 128 C, COOP 25, Matrix 25, MSI/Plessey, Code 32, Pharmacode, PZN (Pharma-Zentral-Nummer), EAN 128, EAN-13, EAN-8, ISBN, ISMN, ISSN, JAN-13, UPC-A, UPC-E, FedEx Ground 96, IATA 25, Identcode, Leitcode, KIX, Planet, PostNet, RM4SCC, SCC-14, SingaporePost, SSCC-18, USPS FIM, USPS Horizontal, USPS PIC
* Bidimensional barcodes: DataMatrix, QR, PDF417, Micro PDF417, Codablock F, Code 16K
 
##### PDF ANNOTATIONS
* Add, edit and remove PDF annotations
* Standard and custom appearance for annotations
* Flatten annotations
* Supported annotations: Text (sticky notes) annotations, Rubber stamp annotations, Square and circle annotations, File attachment annotations, Link annotations (hyperlinks), Line annotations, Ink annotations, Polygon and polyline annotations, Text markup annotations: highlight, underline, strikeout, squiggly, Free text (typewritter) annotations, Movie annotations, Rich media (Flash) annotations, redaction annotations, 3D annotations with support for: views, projections, lighting schemes, cross sections, backgrounds and animations
 
##### PDF FORMS
* Create, load and save PDF forms
* Add, edit, remove and rename form fields
* Support for text box fields, combo box fields, list box fields, push button fields, check box fields, radio button fields, signature fields
* Read/Write (fill) form fields
* Create custom appearances for field widgets
* Flatten form fields
* Form actions 
 
##### PDF ACTIONS
* Add, edit and remove PDF actions
* Set actions at document level, page level, annotation level and form field level
* Supported actions: GoTo, Remote GoTo, GoTo 3D view, Lauch, URI, Named, Javascript, Submit form, Reset form, Hide
 
##### PDF FUNCTIONS
* Sample based functions (Type 0)
* Exponential functions (Type 2)
* Stitching functions (Type 3)
* Postscript calculator functions (Type 4)
 
##### TEXT SEARCH
* Search text in PDF pages with support for regular search, case sensitive search, whole word search and regular expression search
 
##### CONTENT EXTRACTION
* Extract text with position information at fragment level and glyph level
* Extract images including image information such as: image size in pixels, bits per pixel, colorspace, image position on the PDF page, image size on the PDF page, image horizontal and vertical resolution
* Extract page content as a sequence of path, text, image and shading objects
* Extract optional content groups as vector drawings
* Extract page content as vector drawings
 
##### CONTENT TRANSFORMATION
* Convert page content to RGB
* Convert page content to CMYK
* Convert page content to Grayscale
* Convert images to Grayscale
* Replace page images
* Remove page images
 
##### CONTENT REDACTION
* Text redaction
* Image redaction
* Redaction annotations

##### PDF PORTFOLIOS
* Create and load PDF portfolios
* Define portfolio attributes and define sort order for portfolio items
* Add and remove portfolio items
* Organize portfolio items into folders

##### PDF RENDERING
* Render PDF pages to images: RAW, BMP, GIF, JPG, PNG and TIFF
* ARGB, RGBA and BGRA byte layouts for RAW images.
* Supported PDF features for rendering: 
   - Filters: Flate, LZW, ASCII 85, ASCII Hex, CCITT Fax, DCT, JBIG2
   - Colorspaces: RGB, CMYK, Gray, CalRGB, CalGray, Lab, ICC, Indexed, Separation, DeviceN
   - Shadings: function based, axial and radial
   - Patterns: colored, uncolored and shading
   - Blend modes: all
   - Vector graphics: move to, line to, curve to (c, y and v), line width, line cap, line join, stroke, fill, form XObject
   - Images: Raw, Jpeg, Ccitt, Jbig2; Inline images
   - Image masks: soft masks, stencil masks, chroma key masks
   - Fonts: standard 14 PDF fonts, TrueType, Type 1 (Postscript and CFF), Type3, CID fonts, Embedded fonts
   - Annotations: all types
   - Form fields: all types

*Product screenshot generated with* [PlaceIt](http://placeit.breezi.com).