using System.IO;
using System.Text;
using PhoneLib.DAL.DictReaders;

namespace PhoneLib.Helpers
{
    public static class FileHelper
    {
        public static string GetBlockFromFile(FileTextBlock textBlock, string fileName, bool isPhone = false)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var reader = new BinaryReader(stream);
                stream.Position = textBlock.StartsAtByte;
                byte[] blockAsBytes = reader.ReadBytes(textBlock.Length);
                return Encoding.Unicode.GetString(blockAsBytes, 0, blockAsBytes.Length);
            }
        }

        public static string GetBlockFromStream(FileTextBlock textBlock, Stream stream)
        {
            var reader = new BinaryReader(stream);
            stream.Position = textBlock.StartsAtByte;
            byte[] blockAsBytes = reader.ReadBytes(textBlock.Length);
            return Encoding.Unicode.GetString(blockAsBytes, 0, blockAsBytes.Length);
        }
    }
}
