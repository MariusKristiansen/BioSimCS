using System.Drawing;

namespace Biosim.Simulation
{
    class Bitmapper
    {
        public string FilePath { get; set; }
        public int ImgNo { get; set; } = 0;
        //public Bitmap CurrentImage { get; set; }
        public Size ImgDimentions { get; set; }
        public Bitmapper(string filePath, int xDim, int yDim)
        {
            FilePath = filePath;
            ImgDimentions = new Size(xDim, yDim);
        }

        /// <summary>
        /// Creates a bitmap from the input string and the given x and y dimention
        /// </summary>
        /// <param name="rawData">
        ///     rawInputData as a space separated string with '\n' char as linebreak
        /// </param>
        /// <returns></returns>
        public string createBitmap(string rawData)
        {
            var bytes = CreateByteArray(rawData);
            //Bitmap image = new Bitmap(ImgDimentions.Width, ImgDimentions.Height);

            for (int i = 0; i < ImgDimentions.Width; i++)
            {
                //var pixels = line.Split(' ');
                for (int j = 0; j < ImgDimentions.Height; j++)
                {
                    //int.TryParse(pixel, out int pValue);
                    //image.SetPixel(i, j, Color.FromArgb(bytes[i][j], 0, 0));
                }

            }

            return $"{FilePath}fileName{ImgNo}.bmp";
        }

        public byte[][] CreateByteArray(string rawData)
        {
            var lines = rawData.Split('\n');
            return null;
        }
    }
}
