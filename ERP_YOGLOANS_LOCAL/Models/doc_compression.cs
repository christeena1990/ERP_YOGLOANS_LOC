using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class doc_compression
    {

        public byte[] CompressPdfToSize(Stream inputStream, long targetSizeInBytes = 256 * 1024) // 256 KB
        {
            byte[] compressedPdf = null;
            float quality = 0.5f; // Start with 50% quality for images

            using (MemoryStream memoryStream = new MemoryStream())
            {
                inputStream.CopyTo(memoryStream);
                memoryStream.Position = 0; // Reset the position of the memory stream

                while (true)
                {
                    using (PdfReader pdfReader = new PdfReader(memoryStream))
                    {
                        using (MemoryStream outputStream = new MemoryStream())
                        {
                            Document document = new Document();
                            PdfCopy copy = new PdfSmartCopy(document, outputStream)
                            {
                                CompressionLevel = PdfStream.BEST_COMPRESSION
                            };

                            document.Open();
                            copy.AddDocument(pdfReader);
                            document.Close();

                            compressedPdf = outputStream.ToArray();
                        }
                    }

                    if (compressedPdf.Length <= targetSizeInBytes || quality <= 0.1f)
                    {
                        break;
                    }

                    quality -= 0.1f;

                    using (MemoryStream tempStream = new MemoryStream(compressedPdf))
                    {
                        using (PdfReader pdfReader = new PdfReader(tempStream))
                        {
                            using (MemoryStream outputStream = new MemoryStream())
                            {
                                Document document = new Document();
                                PdfStamper stamper = new PdfStamper(pdfReader, outputStream);
                                stamper.Writer.CompressionLevel = PdfStream.BEST_COMPRESSION;
                                stamper.Close();

                                compressedPdf = outputStream.ToArray();
                            }
                        }
                    }
                }
            }

            return compressedPdf;
        }


        public byte[] CompressImageToSize(Stream inputStream, ImageFormat format, long targetSizeInBytes = 256 * 1024) // 256 KB
        {
            using (var image = System.Drawing.Image.FromStream(inputStream))
            {
                long quality = 25L; // Start with 25% quality
                byte[] compressedImage = null;

                while (true)
                {
                    using (var stream = new MemoryStream())
                    {
                        var jpegEncoder = GetEncoder(format);
                        var encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

                        image.Save(stream, jpegEncoder, encoderParameters);
                        compressedImage = stream.ToArray();

                        if (compressedImage.Length <= targetSizeInBytes || quality <= 10L)
                        {
                            break;
                        }

                        quality -= 5L; // Reduce quality further
                    }
                }

                return compressedImage;
            }
        }
        public ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }



    }
}