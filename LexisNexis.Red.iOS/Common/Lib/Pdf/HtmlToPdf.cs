using System;
using System.Collections.Generic;
using System.IO;

using Xfinium.Pdf.Actions;
using Xfinium.Pdf.Graphics;
using Xfinium.Pdf.Graphics.FormattedContent;
using Xfinium.Pdf;

namespace Xfinium.Pdf.HtmlToPdf
{
    /// <summary>
    /// Converts a simple HTML file to PDF
    /// </summary>
    public class HtmlToPdf
    {
        // Stack of fonts
        private Stack<PdfFont> fonts = new Stack<PdfFont>();

        /// <summary>
        /// Gets the active font.
        /// </summary>
        public PdfFont ActiveFont
        {
            get { return fonts.Peek(); }
        }

        private Stack<PdfBrush> textColors = new Stack<PdfBrush>();
        /// <summary>
        /// Gets the active text color.
        /// </summary>
        public PdfBrush ActiveTextColor
        {
            get { return textColors.Peek(); }
        }

        /// <summary>
        /// Converts simple XHTML code to a PDF document.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public PdfFixedDocument Convert(Stream html)
        {
            PdfFixedDocument document = new PdfFixedDocument();

            PdfFormattedContent fc = ConvertHtmlToFormattedContent(html);
            DrawFormattedContent(document, fc);

            return document;
        }

        /// <summary>
        /// Converts simple XHTML to a formatted content object.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private PdfFormattedContent ConvertHtmlToFormattedContent(Stream html)
        {
            PdfFormattedContent fc = new PdfFormattedContent();
            PdfFormattedParagraph currentParagraph = null;
            PdfUriAction currentLinkAction = null;
            PdfFormattedTextBlock bullet = null;
			float currentIndent = 0;

            // Create a default font.
			fonts.Push(new PdfStandardFont(PdfStandardFontFace.Helvetica, 10));



            // Create a default text color.
            textColors.Push(new PdfBrush(PdfRgbColor.Black));

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(html);
            xmlReader.MoveToContent();

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case System.Xml.XmlNodeType.Element:
                        switch (xmlReader.Name)
                        {
                            case "p":
                                currentParagraph = new PdfFormattedParagraph();
                                //currentParagraph.SpacingBefore = 3;
                                //currentParagraph.SpacingAfter = 3;
                                fc.Paragraphs.Add(currentParagraph);
                                break;
                            case "br":
                                if (currentParagraph.Blocks.Count > 0)
                                {
                                    PdfFormattedTextBlock textBlock = 
                                        currentParagraph.Blocks[currentParagraph.Blocks.Count - 1] as PdfFormattedTextBlock;
                                    textBlock.Text = textBlock.Text + "\r\n";
                                }
                                else
                                {
                                    PdfFormattedTextBlock textBlock = new PdfFormattedTextBlock("\r\n", ActiveFont);
                                    currentParagraph.Blocks.Add(textBlock);
                                }
                                break;
                            case "a":
                                while (xmlReader.MoveToNextAttribute())
                                {
                                    if (xmlReader.Name == "href")
                                    {
                                        currentLinkAction = new PdfUriAction();
                                        currentLinkAction.URI = xmlReader.Value;
                                    }
                                }
                                break;
                            case "font":
                                while (xmlReader.MoveToNextAttribute())
                                {
                                    if (xmlReader.Name == "color")
                                    {
                                        PdfBrush color = ActiveTextColor;
                                        string colorCode = xmlReader.Value;
                                        // #RRGGBB
                                        if (colorCode.StartsWith("#") && (colorCode.Length == 7))
                                        {
                                            byte r = byte.Parse(colorCode.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                                            byte g = byte.Parse(colorCode.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                                            byte b = byte.Parse(colorCode.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                                            color = new PdfBrush(new PdfRgbColor(r, g, b));
                                        }

                                        textColors.Push(color);
                                    }
                                }
                                break;
                            case "ul":
                                //bullet = new PdfFormattedTextBlock("\x95 ", ActiveFont);
								currentIndent += 18;
                                break;
                            case "li":
                                currentParagraph = new PdfFormattedParagraph();
                                currentParagraph.SpacingBefore = 3;
                                currentParagraph.SpacingAfter = 3;
                                currentParagraph.Bullet = bullet;
								currentParagraph.LeftIndentation = currentIndent;
                                fc.Paragraphs.Add(currentParagraph);
                                break;
                            case "b":
                            case "strong":
                                PdfFont boldFont = CopyFont(ActiveFont);
                                boldFont.Bold = true;
                                fonts.Push(boldFont);
                                break;
                            case "i":
                            case "em":
                                PdfFont italicFont = CopyFont(ActiveFont);
                                italicFont.Italic = true;
                                fonts.Push(italicFont);
                                break;
                            case "u":
                                PdfFont underlineFont = CopyFont(ActiveFont);
                                underlineFont.Underline = true;
                                fonts.Push(underlineFont);
                                break;
							case "span":
								if (currentParagraph == null) {
									currentParagraph = new PdfFormattedParagraph();
									fc.Paragraphs.Add(currentParagraph);
								}
								break;
							case "h1":
							case "h2":
								currentParagraph = new PdfFormattedParagraph();
								currentParagraph.SpacingBefore = 6;
								currentParagraph.SpacingAfter = 6;
								fc.Paragraphs.Add(currentParagraph);
								fonts.Push(CopyFont(new PdfStandardFont(PdfStandardFontFace.Helvetica, 17)));
								break;
							case "h3":
							case "h4":
								currentParagraph = new PdfFormattedParagraph();
								currentParagraph.SpacingBefore = 6;
								//currentParagraph.SpacingAfter = 6;
								fc.Paragraphs.Add(currentParagraph);
								fonts.Push(CopyFont(new PdfStandardFont(PdfStandardFontFace.Helvetica, 14)));
								break;
							case "h5":
							case "h6":
								currentParagraph = new PdfFormattedParagraph();
								currentParagraph.SpacingBefore = 3;
								currentParagraph.SpacingAfter = 3;
								fc.Paragraphs.Add(currentParagraph);
								fonts.Push(CopyFont(new PdfStandardFont(PdfStandardFontFace.Helvetica, 11)));
								break;
                        }
                        break;
                    case System.Xml.XmlNodeType.EndElement:
                        switch (xmlReader.Name)
                        {
                            case "a":
                                currentLinkAction = null;
                                break;
                            case "ul":
                                //bullet = null;
								currentIndent -= 18;
                                break;
                            case "b":
                            case "strong":
                            case "i":
                            case "em":
                            case "u":
							case "h1":
							case "h2":
							case "h3":
							case "h4":
							case "h5":
							case "h6":
								currentParagraph = new PdfFormattedParagraph();
								currentParagraph.SpacingBefore = 0;
								currentParagraph.SpacingAfter = 0;
								fc.Paragraphs.Add(currentParagraph);
                                fonts.Pop();
                                break;
							case "font":
								if (textColors.Count > 1) {
									textColors.Pop();

								}
                                break;
                        }
                        break;
					case System.Xml.XmlNodeType.Text:
						string text = xmlReader.Value;
						if (currentParagraph != null && text != null && text.Length > 0) {
							// Remove spaces from text that do not have meaning in HTML.
							text = text.Replace("\r", "");
							text = text.Replace("\n", "");
							text = text.Replace("\t", " ");
							PdfFormattedTextBlock block = new PdfFormattedTextBlock(text, ActiveFont);
							block.TextColor = ActiveTextColor;
							if (currentLinkAction != null)
							{
								block.Action = currentLinkAction;
								// Make the links blue.
								block.TextColor = new PdfBrush(PdfRgbColor.Blue);
							}
							
							currentParagraph.Blocks.Add(block);
						}
                        break;
                }
            }

            return fc;
        }

        /// <summary>
        /// Creates a bold copy of the given font.
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        private PdfFont CopyFont(PdfFont font)
        {
            PdfFont copy = null;
            PdfStandardFont standardFont = font as PdfStandardFont;
            if (standardFont != null)
            {
                copy = new PdfStandardFont(standardFont);
            }

            return copy;
        }

        /// <summary>
        /// Draws the formatted content on document's pages.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="fc"></param>
        private void DrawFormattedContent(PdfFixedDocument document, PdfFormattedContent fc)
        {
            double leftMargin, topMargin, rightMargin, bottomMargin;
            leftMargin = topMargin = rightMargin = bottomMargin = 36;

            PdfPage page = document.Pages.Add();


            PdfFormattedContent fragment = fc.SplitByBox(page.Width - leftMargin - rightMargin, page.Height - topMargin - bottomMargin);
            while (fragment != null)
            {
                page.Graphics.DrawFormattedContent(fragment, 
                    leftMargin, topMargin, page.Width - leftMargin - rightMargin, page.Height - topMargin - bottomMargin);
                page.Graphics.CompressAndClose();

                fragment = fc.SplitByBox(page.Width - leftMargin - rightMargin, page.Height - topMargin - bottomMargin);
                if (fragment != null)
                {
                    page = document.Pages.Add();
                }
            }
        }
    }
}
