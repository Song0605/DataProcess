using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcess.Method
{
    /// <summary>
    /// 处理城建接口
    /// </summary>
    public static class ChengJianAutoProcess
    {
        public static string CurrentDirectoryPath = Environment.CurrentDirectory;
        //public static string OriginFilePath = Path.Combine(CurrentDirectoryPath, "cjData.txt");

        public static string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static string OriginFilePath = Path.Combine(DesktopPath, "cjData.txt");
        public static string OutputFilePath = Path.Combine(DesktopPath, "outputData.txt");
        public static string OutputNamePath = Path.Combine(DesktopPath, "outputDataName.txt");
        public static void DealData()
        {
            var apiName = string.Empty;
            var methodName = string.Empty;
            var methodDescription = string.Empty;
            var propDic = new Dictionary<string, List<Tuple<string, string>>>();

            using (var fs = new FileStream(OutputNamePath, FileMode.Create))
            {
                fs.SetLength(0);
            }
            using (var sw = new StreamWriter(OutputNamePath))
            {
                using (var sr = new StreamReader(OriginFilePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) return;

                        if (line.Contains(nameof(apiName)))
                            apiName = line.Trim().Split(':')[1];
                        else if (line.Contains(nameof(methodName)))
                            methodName = line.Trim().Split(':')[1];
                        else if (line.Contains(nameof(methodDescription)))
                        {
                            methodDescription = line.Trim().Split(':')[1];
                            break;
                        }
                        else
                            return;
                    }

                    sw.WriteLine($"[ContextStatic]");
                    sw.WriteLine($"[Display(Name = \"{methodDescription}\")]");
                    sw.WriteLine($"[Frequency()]");
                    sw.WriteLine($"{methodName},");
                    sw.WriteLine();
                    sw.WriteLine($"public class {methodName} {{");

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        if (!line.Contains("\",//") && line.Contains("\"//"))
                            line = line.Replace("\"//", "\",//");
                        var strList = new List<string>();
                        var array1 = line.Trim().Split(':');
                        strList.Add(array1[0].Replace('"', ' ').Trim());
                        var array2 = array1[1].Split(',');
                        strList.Add(array2[0].Replace('"', ' ').Trim().Trim('"'));
                        strList.Add(array2[1].Trim().Trim('"'));
                        if (!propDic.ContainsKey(strList[1]))
                            propDic.Add(strList[1], new List<Tuple<string, string>>());
                        propDic[strList[1]].Add(Tuple.Create(strList[0], strList[2]));
                        sw.WriteLine($"public const string {strList[0]} = \"{strList[0]}\";{strList[2]} {strList[1]}");
                    }
                }
                sw.WriteLine($"}}");
            }

            using (var fs = new FileStream(OutputFilePath, FileMode.Create))
            {
                fs.SetLength(0);
            }

            using (var sw = new StreamWriter(OutputFilePath))
            {
                sw.WriteLine($"/// <summary>");
                sw.WriteLine($"/// 上传{methodDescription}数据");
                sw.WriteLine($"/// </summary>");
                sw.WriteLine($"/// <param name=\"mode\">数据类型，0-全量数据；1-增量数据</param>");
                sw.WriteLine($"[Route(\"{apiName}\")]");
                sw.WriteLine($"[HttpPost]");
                sw.WriteLine($"[RouteInfo(\"guohh\", ApiLevel.Level5, \"上传{methodDescription}数据\")]");
                sw.WriteLine($"[AllowAnonymous]");
                sw.WriteLine($"[VerifyPartner]");
                sw.WriteLine($"public async Task<Result> Upload{methodName}Data(int mode){{");
                sw.WriteLine($"    return await Handle3rdData.SaveDataAsync(Request, DataName.{methodName}, mode, (record, errStrBuilder) =>{{");
                sw.WriteLine($"record.VerifyNotNullMsg(errStrBuilder, new string[]{{");
                sw.WriteLine($"Id.Name,");
                sw.WriteLine($"}});");
                sw.WriteLine();
                foreach (var propList in propDic)
                {
                    var type = propList.Key;
                    sw.WriteLine($"record.VerifyAllExistNotMust<{type}>(errStrBuilder, new string[]{{");
                    foreach (var prop in propList.Value)
                        sw.WriteLine($"DataHubs.{methodName}.{prop.Item1},");
                    sw.WriteLine($"}});");
                }
                sw.WriteLine($"}});}}");
            }
        }
    }
}
