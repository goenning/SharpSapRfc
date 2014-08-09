using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Xunit;

namespace SharpSapRfc.Test
{
    public static class ImageAssert
    {
        public static void AreEqual(Bitmap expected, Bitmap actual)
        {
            Assert.True(AreImageEquals(expected, actual));
        }

        private static bool AreImageEquals(Bitmap bmp1, Bitmap bmp2)
        {
            if (!bmp1.Size.Equals(bmp2.Size))
            {
                return false;
            }
            for (int x = 0; x < bmp1.Width; ++x)
            {
                for (int y = 0; y < bmp1.Height; ++y)
                {
                    if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
