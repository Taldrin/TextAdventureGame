using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


class Program
{
    static void Main(string[] args)
    {

        string backgroundImagePath = "output";

        if (!File.Exists(backgroundImagePath))
        {
            Console.WriteLine($"Error: File '{backgroundImagePath}' not found.");
            return;
        }

        int cols = 8;
        int rows = 10;

        string outputDir = args[3];
        Directory.CreateDirectory(outputDir);

        // Load the original image
        using (Image<Rgba32> original = Image.Load<Rgba32>(backgroundImagePath))
        {
            int cellWidth = original.Width / cols;
            int cellHeight = original.Height / rows;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    // Clone the image to draw on
                    using (Image<Rgba32> img = original.Clone())
                    {
                        // Compute circle dimensions (80% of cell size)
                        float diameter = Math.Min(cellWidth, cellHeight) * 0.8f;
                        float centerX = c * cellWidth + cellWidth / 2f;
                        float centerY = r * cellHeight + cellHeight / 2f;

                        // Define the circle geometry
                        var circle = new EllipsePolygon(centerX, centerY, diameter / 2f);

                        // Draw the circle outline
                        img.Mutate(x => x.Draw(Color.Red, 3, circle));

                        // Save the result
                        string fileName = System.IO.Path.Combine(outputDir, $"occupied_{r}_{c}.png");
                        img.Save(fileName);
                    }
                    Console.WriteLine($"Generated: occupied_{r}_{c}.png");
                }
            }
        }

        Console.WriteLine("All images generated.");
    }
}