using System;
using Microsoft.SPOT;
using System.IO;
using System.Drawing;
using AForge.Imaging;

namespace PL_FEZ03
{   
        /// <summary>
        /// Image comparison class to match and rate if bitmapped images are similar.
        /// </summary>
        public static class ImageComparer
        {
            // The file extension for the generated Bitmap files
            private const string BitMapExtension = ".bmp";

            /// <summary>
            /// Compares the images.
            /// </summary>
            /// <param name="image">The image.</param>
            /// <param name="targetImage">The target image.</param>
            /// <param name="compareLevel">The compare level.</param>
            /// <param name="filepath">The filepath.</param>
            /// <param name="similarityThreshold">The similarity threshold.</param>
            /// <returns>Boolean result</returns>
            public static Boolean CompareImages(string image, string targetImage, double compareLevel, string filepath, float similarityThreshold)
            {
                // Load images into bitmaps
                var imageOne = new System.Drawing.Bitmap(image);
                var imageTwo = new System.Drawing.Bitmap(targetImage);

                var newBitmap1 = ChangePixelFormat(new System.Drawing.Bitmap(imageOne), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                var newBitmap2 = ChangePixelFormat(new System.Drawing.Bitmap(imageTwo), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                newBitmap1 = SaveBitmapToFile(newBitmap1, filepath, image, BitMapExtension);
                newBitmap2 = SaveBitmapToFile(newBitmap2, filepath, targetImage, BitMapExtension);

                // Setup the AForge library
                var tm = new ExhaustiveTemplateMatching(similarityThreshold);

                // Process the images
                var results = tm.ProcessImage(newBitmap1, newBitmap2);

                // Compare the results, 0 indicates no match so return false
                if (results.Length <= 0)
                {
                    return false;
                }

                // Return true if similarity score is equal or greater than the comparison level
                var match = results[0].Similarity >= compareLevel;

                return match;
            }

            /// <summary>
            /// Saves the bitmap automatic file.
            /// </summary>
            /// <param name="image">The image.</param>
            /// <param name="filepath">The filepath.</param>
            /// <param name="name">The name.</param>
            /// <param name="extension">The extension.</param>
            /// <returns>Bitmap image</returns>
            private static System.Drawing.Bitmap SaveBitmapToFile(System.Drawing.Bitmap image, string filepath, string name, string extension)
            {
                var savePath = string.Concat(filepath, "\\", Path.GetFileNameWithoutExtension(name), extension);

                image.Save(savePath, System.Drawing.Imaging.ImageFormat.Bmp);

                return image;
            }

            /// <summary>
            /// Change the pixel format of the bitmap image
            /// </summary>
            /// <param name="inputImage">Bitmapped image</param>
            /// <param name="newFormat">Bitmap format - 24bpp</param>
            /// <returns>Bitmap image</returns>
            private static System.Drawing.Bitmap ChangePixelFormat(System.Drawing.Bitmap inputImage, System.Drawing.Imaging.PixelFormat newFormat)
            {
                return (inputImage.Clone(new Rectangle(0, 0, inputImage.Width, inputImage.Height), newFormat));
            }
        }    
}
