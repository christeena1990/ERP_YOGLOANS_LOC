using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Drawing;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class FileCompressor
    {

        public const int MaxFileSize = 250 * 1024; // 250 KB in bytes


        public byte[] CompressFile(byte[] inputBytes, string fileExtension)
        {
            if (inputBytes.Length <= MaxFileSize)
            {
                // File size is already less than or equal to 250 KB, return the original byte array
                return inputBytes;
            }

            string extension = fileExtension.ToLower();

            if (extension == ".pdf")
            {
                return CompressPdf(inputBytes);
            }
            else if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp" || extension == ".gif")
            {
                return CompressImage(inputBytes, extension);
            }

            throw new NotSupportedException($"File type {extension} is not supported for compression.");
        }

        private byte[] CompressPdf(byte[] inputBytes)
        {
            try
            {
                using (var inputStream = new MemoryStream(inputBytes))
                using (var outputStream = new MemoryStream())
                {
                    // Read the PDF
                    var reader = new PdfReader(inputStream);

                    // Create a PdfStamper object for writing
                    using (var stamper = new PdfStamper(reader, outputStream))
                    {
                        stamper.SetFullCompression(); // Enable full compression
                        stamper.FormFlattening = true; // Flatten form fields
                        stamper.Writer.CompressionLevel = PdfStream.BEST_COMPRESSION; // Set compression level
                    }

                    return outputStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                // Log exception or handle it as needed
                throw new Exception("Error compressing PDF.", ex);
            }
        }

        private byte[] CompressImage(byte[] inputBytes, string extension)
        {
            try
            {
                using (var inputStream = new MemoryStream(inputBytes))
                using (var image = Image.FromStream(inputStream))
                using (var outputStream = new MemoryStream())
                {
                    // Determine quality level for compression
                    long quality = 50L; // Adjust the quality level as needed (0L to 100L)

                    // Create an encoder parameter for image quality
                    var qualityParam = new EncoderParameter(Encoder.Quality, quality);
                    var imageCodec = GetEncoderInfo($"image/{extension.TrimStart('.')}");

                    if (imageCodec == null)
                        throw new Exception("Unable to find image codec for the specified format.");

                    var encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = qualityParam;

                    // Save the compressed image
                    image.Save(outputStream, imageCodec, encoderParams);

                    return outputStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                // Log exception or handle it as needed
                throw new Exception("Error compressing image.", ex);
            }
        }

        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get all available image encoders
            var codecs = ImageCodecInfo.GetImageEncoders();

            // Iterate through each codec and return the matching one
            foreach (var codec in codecs)
            {
                // Compare mime types case-insensitively
                if (codec.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase))
                    return codec;
            }

            // Try alternative MIME types for common formats (optional)
            if (mimeType.Equals("image/jpg", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var codec in codecs)
                {
                    if (codec.MimeType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase))
                        return codec;
                }
            }

            // If no codec found, return null
            return null;
        }


    }
}