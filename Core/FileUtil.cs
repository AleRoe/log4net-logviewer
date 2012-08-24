using System.IO;

namespace LogViewer
{
    public class FileUtil
    {
        public static string ReadAllText(string path)
        {
            using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(file))
            {
                return reader.ReadToEnd();
            }
        }
    }
}