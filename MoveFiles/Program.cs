using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices; //dll
using System.Text;

namespace MoveFiles
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public class IniFile
        {
            string Path;
            string EXE = Assembly.GetExecutingAssembly().GetName().Name; //MoveFiles

            [DllImport("kernel32", CharSet = CharSet.Unicode)]
            static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

            public IniFile(string IniPath)
            {
                Path = new FileInfo(IniPath).FullName;
            }

            public string Read(string Section, string Key)
            {
                //var today = DateTime.Now.ToString("yyyyMMdd");
                StringBuilder RetVal = new StringBuilder(255);
                //string today = DateTime.Now.ToString("yyyyMMdd");
                //string a = RetVal.AppendFormat($"{today:C}").ToString();
                //string p = System.IO.Path.Combine(RetVal,today);
                GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
                return RetVal.ToString(); //Section、Key、value's path
            }

        }

        //private static void GetAllFiles(string path)
        //{

        //    DirectoryInfo dir = new DirectoryInfo(path);
 
        //    FileInfo[] fi = dir.GetFiles();
        //    foreach (FileInfo f in fi)
        //    {
        //        Console.WriteLine("xml's path：" + f.FullName.ToString() + " xml's filename：" + f.Name + "<br>");
        //        //var xml_file = f.FullName.ToString();
    
        //    }

        //    DirectoryInfo[] subDir = dir.GetDirectories();
        //    foreach (DirectoryInfo d in subDir)
        //    {
        //        GetAllFiles(d.FullName);
        //    }
            
        //}

        private static void Get_Move_Files(string source, string bkp1, string bkp2, string destination)
        {
            while (true)
            {
                DirectoryInfo dir = new DirectoryInfo(source);

                FileInfo[] fi = dir.GetFiles();


                try
                {
                    foreach (FileInfo f in fi)
                    {
                        Console.WriteLine("xml's path：" + f.FullName.ToString() + " xml's filename：" + f.Name + "<br>");
                        //var xml_file = f.FullName.ToString();

                        // Copy file
                        File.Copy(f.FullName, $"{bkp1}\\{f.Name}", true);
                        File.Copy(f.FullName, $"{bkp2}\\{f.Name}", true);
                        Console.WriteLine($"{f.Name} has copied - on: {DateTime.Now}");
                        // Move file
                        File.Move(f.FullName, $"{destination}\\{f.Name}");
                        Console.WriteLine($"{f.Name} has moved - on: {DateTime.Now}", false);
                        log.Info($"File: {f.Name} has moved & backup");

                        Console.WriteLine(dir + " has no data inside.");
                        Directory.Delete(dir.ToString());
                        Console.WriteLine("Remove the " + dir);

                        //reseperate the path "D:\AI\XML\20210810\7700Q\94S6868001E05_TOP\LKA8125425"


                        int lastIndex = dir.ToString().LastIndexOf("\\");
                        string pFilePath = dir.ToString().Substring(0, lastIndex); // "D:\\AI\\XML\\20210810\\7700Q\\94S6868001E05_TOP"
                        int lastIndex1 = pFilePath.ToString().LastIndexOf("\\");
                        string pFilePath1 = dir.ToString().Substring(0, lastIndex1);
                        int lastIndex2 = pFilePath1.ToString().LastIndexOf("\\");
                        string pFilePath2 = dir.ToString().Substring(0, lastIndex2);
                        Get_Move_Files(pFilePath2, bkp1, bkp2, destination);


                    }

                    DirectoryInfo[] subDir = dir.GetDirectories(); //{System.IO.DirectoryInfo[1]}
                    int fileCount = subDir.Length;

                    int i = 0;
                    foreach (DirectoryInfo d in subDir)
                    {
                        if (fileCount == 0)
                        {
                            //DirectoryInfo[]  subDir = subDir[1];
                            //Directory.Delete(dir.ToString()); //Delete mn file
                            //int lastIndex = dir.ToString().LastIndexOf("\\");
                            //string pFilePath = dir.ToString().Substring(0, lastIndex); // "D:\\AI\\XML\\20210810\\7700Q\\94S6868001E05_TOP"
                            //int lastIndex1 = pFilePath.ToString().LastIndexOf("\\");
                            //string pFilePath1 = dir.ToString().Substring(0, lastIndex1);

                            //Get_Move_Files(pFilePath, bkp1, bkp2, destination);
                        }
                        Console.WriteLine("Keep Detecting");
                        Get_Move_Files(d.FullName, bkp1, bkp2, destination);
                    }
                }
                catch (IOException copyError)
                {
                    Console.WriteLine(copyError.Message);
                }
            }
        }

        private static void DirSearch(string source, string bkp1, string bkp2, string destination)
        {
            while(true)
            {
                try
                {
                    foreach (string d in Directory.GetDirectories(source))
                    {
                        foreach (string fi in Directory.GetDirectories(d)) //機種
                        {
                            foreach (string f in Directory.GetDirectories(fi)) //序號"D:\\AI\\XML\\20210811\\7700Q\\002\\LKA7508492"
                            {

                                //當下時間
                                string now_date = DateTime.Now.ToString("yyyyMMdd");
                                string now_hh = DateTime.Now.ToString("HH");
                                string now_mm = DateTime.Now.ToString("mm");
                                string now_ss = DateTime.Now.ToString("ss");
                                int now_time = Int32.Parse(now_hh) * 60 * 60 + Int32.Parse(now_mm) * 60 + Int32.Parse(now_ss);
                                //Console.Write("Current Time is {0}.\r\n", now_time);
                                //序號資料夾更新時間 './LKA7508492/'
                                DateTime file_date = File.GetLastWriteTime(f);
                                string file = file_date.ToString("yyyyMMdd");
                                string file_hh = file_date.ToString("HH");
                                string file_mm = file_date.ToString("mm");
                                string file_ss = file_date.ToString("ss");
                                int file_time = Int32.Parse(file_hh) * 60 * 60 + Int32.Parse(file_mm) * 60 + Int32.Parse(file_ss);
                                //Console.WriteLine("File was edited Time is {0}.", file_time);
                                //Console.Write("Waiting for the XML's file.\r\n");

                                //若日期相同
                                if (now_date == file)
                                {
                                    //若資料夾時間有更新，則移動所有xml
                                    //若日期相同則比較是否為最新檔案
                                    if (file_time <= now_time)
                                    {

                                        foreach (string g in Directory.GetFiles(f, "*.xml"))
                                        {
                                            // Copy file
                                            //File.Copy(g, bkp1, true);
                                            string g_FileName = Path.GetFileNameWithoutExtension(g) + Path.GetExtension(g);
                                            File.Copy(g, $"{bkp1}\\{g_FileName}", true);
                                            File.Copy(g, $"{bkp2}\\{g_FileName}", true);
                                            //File.Copy(g, bkp2, true);
                                            Console.WriteLine($"{g} has copied - on: {DateTime.Now}");
                                            // Move file
                                            //File.Move(g, $"{destination}\\{g}");
                                            File.Move(g, $"{destination}\\{g_FileName}", true);
                                            Console.WriteLine($"{g} has moved - on: {DateTime.Now}", false);
                                            log.Info($"File: {g} has moved & backup");

                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("It's not today's file.");
                                }



                            }
                        }
                        //DirSearch(d, bkp1, bkp2, destination);
                    }
                }
                catch (System.Exception excpt)
                {
                    Console.WriteLine(excpt.Message);
                }
            }
            
        }
        //private static void Copy_Move(string source, string bkp, string destination)
        //{
        //    while (true)
        //    {
        //        DirectoryInfo rootDir = new DirectoryInfo(source);
        //        FileInfo[] files = rootDir.GetFiles();
        //        int fileCount = rootDir.GetFileSystemInfos().Length;

        //        if (fileCount != 0)
        //        {
        //            foreach (FileInfo file in files)
        //            {
        //                GetAllFiles(source);
        //                // Copy file
        //                File.Copy(file.FullName, $"{bkp}\\{file.Name}", true);
        //                Console.WriteLine($"{file.Name} has copied - on: {DateTime.Now}");

        //                // Move file
        //                File.Move(file.FullName, $"{destination}\\{file.Name}");
        //                Console.WriteLine($"{file.Name} has moved - on: {DateTime.Now}", false);
        //                log.Info($"File: {file.Name} has moved & backup");
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("Folder has no any file ");
        //            //log.Error($"Folder has no any file ");
        //        }
        //    }
        //}

        static void Main(string[] args)
        {
            //log
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            //read ini
            var MyIni = new IniFile(@"path.ini");

            //file path
            Console.WriteLine("============== Move File ==============");
            var today = DateTime.Now.ToString("yyyyMMdd");
            string source_ini = MyIni.Read("PATH","source"); //source="D:\\AI\\XML"
            string source = source_ini + "\\" + today ;

            string destination = MyIni.Read("PATH", "destination");
            string bkp1 = MyIni.Read("PATH", "bkp1");
            string bkp2 = MyIni.Read("PATH", "bkp2");
            
            Console.WriteLine($"Source: {source}\nBackup: {bkp1}\n{bkp2}\nDestination: {destination}\n");
            

            Console.WriteLine("============== Waiting for XML's file ==============");
            //Copy_Move(source, bkp, destination);
            //Get_Move_Files(source, bkp1, bkp2, destination);
            DirSearch(source, bkp1, bkp2, destination);


        }
    }
}
