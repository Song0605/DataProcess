using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcess.Method
{
    public static class FolderProcess
    {
        public static string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public static string FolderPath = @"D:\1_工作用\平岑项目\中期支付报表";
        public static string ResultFilePath = Path.Combine(DesktopPath, "DealFolderResultFile.txt");
        /// <summary>
        /// 获取文件夹中所有的文件名
        /// </summary>
        public static void GetFileNames()
        {
            var fileNames = Directory.GetFiles(FolderPath);
            using (var fs = new FileStream(ResultFilePath, FileMode.Create))
            {
                fs.SetLength(0);
            }
            using (var sw = new StreamWriter(ResultFilePath))
            {
                foreach (var file in fileNames)
                {
                    sw.WriteLine(Path.GetFileNameWithoutExtension(file));
                }
            }
        }
        public static void ReplaceFileNames()
        {
            var prefix = "【平岑项目】";
            var fileNames = Directory.GetFiles(FolderPath);
            foreach (var file in fileNames)
            {
                var oriFileName = Path.GetFileNameWithoutExtension(file);
                if (oriFileName.StartsWith(prefix)) continue;
                var newFileName = prefix + oriFileName;
                var newFilePath = file.Replace(oriFileName, newFileName);
                File.Move(file, newFilePath);
            }
        }
    }
}
