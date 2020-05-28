using System;
using System.IO;

namespace QSThumbCreator.Utility
{
    public class CommonApplicationDataPath
    {
        public static string GetCommonApplicationDataPath()
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            var appName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
            var path = Path.Combine(folderPath, appName);

            return path;
        }

        public static string GetFolderWithinCommonApplicationData(string folderName, bool createIfNotExists)
        {
            var commonApplicationDataPath = GetCommonApplicationDataPath();
            var path = Path.Combine(commonApplicationDataPath, folderName);
            path += Path.DirectorySeparatorChar;

            if (createIfNotExists && Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}