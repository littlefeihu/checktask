Working with PDF files is very easy with XFINIUM.PDF library.  

Here are a few of samples:  

####Hello World
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

####Form filling
```csharp
// Load the form in a PdfFixedDocument object and the Fields collection
// will be populated automatically with the field in the form
PdfFixedDocument document = new PdfFixedDocument("form.pdf");
// Set the generic Value property or set specific field properties to fill the form
document.Form.Fields["lastname"].Value = "Doe";
// For text boxes set the Text property
(document.Form.Fields["firstname"] as PdfTextBoxField).Text = "John";
// For radiobutton set the Checked property of the specific item
(document.Form.Fields["sex"].Widgets[0] as PdfRadioButtonWidget).Checked = true;
// For list type fields set the SelectIndex in the list
(document.Form.Fields["firstcar"] as PdfComboBoxField).SelectedIndex = 0;
(document.Form.Fields["secondcar"] as PdfListBoxField).SelectedIndex = 1;
// For checkbox fields set the Checked property
(document.Form.Fields["agree"] as PdfCheckBoxField).Checked = true;
// Save the filled form
document.Save("form_filled.pdf");
```

####File merging
```csharp
// Create an empty document
PdfFixedDocument document = new PdfFixedDocument();
// Append 2 files to the document
document.AppendFile("file1.pdf");
document.AppendFile("file2.pdf");
// Save the merged file
document.Save("file_merged.pdf");
```

####File spliting
The file splitting operation consists of extracting the required pages from the source document (the one to split) and inserting them into a new one. 
  
```csharp
// Open the file you want to split
PdfFile file = new PdfFile("file.pdf");
for (int i = 0; i < file.PageCount; i++)
{
    PdfPage page = file.ExtractPage(i);
    // For exact page in the source file create a new document,
    // add the page to the document and then save it.
    PdfFixedDocument document = new PdfFixedDocument();
    document.Pages.Add(page);
    document.Save(i.ToString() + ".pdf");
}
```

####PDF rendering
PDF pages can be converted to image with a few lines of code. 
  
```csharp
PdfFixedDocument document = new PdfFixedDocument(NSBundle.MainBundle.BundlePath + "/xfinium.pdf");
PdfPageRenderer renderer = new PdfPageRenderer(document.Pages[0]);
CGImage pageImage = renderer.ConvertPageToImage(96);
```

More samples are included in the installation kit, including a samples browser application for each platform.