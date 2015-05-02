using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace PackResFramework
{
    public class PackRes_Main
    {
        static string platform = "webplayer";

        const string binPath = "../bin";
        const string expPath = "../exp";
        const string idxFileName = "idxmap.txt";
        const string bundleMd5FileName = "bundleCheckSum.txt";

        [MenuItem("Assets/PackRes/Clean exp")]
        public static void PackAllRes_Clean()
        {
            string expfullPath = Path.GetFullPath(expPath);
            string binfullPath = Path.GetFullPath(binPath);
            if (Directory.Exists(expfullPath))
            {
                Directory.Delete(expfullPath, true);
            }
            if (Directory.Exists(binfullPath))
            {
                Directory.Delete(binfullPath, true);
            }
            Debug.Log("remove exp and bin directory");
        }

        [MenuItem("Assets/PackRes/Pack all resources_webPlayer")]
        public static void PackAllRes_webPlayer()
        {
            platform = "webplayer";

            PackRes_Textures.platform = platform;
            PackRes_Textures.Pack_Textures();

            PackRes_Gui.platform = platform;
            PackRes_Gui.Pack_Gui();

            PackRes_Scene.platform = platform;
            PackRes_Scene.Pack_Scene();

            //**********************
            GenerateIndexFile();
            CopyResourceToStreamAssetDirectory(platform);

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
            //EditorApplication.ExecuteMenuItem("Assets/Sync MonoDevelop Project");
        }

        [MenuItem("Assets/PackRes/Pack all resources_iphone")]
        public static void PackAllRes_iphone()
        {
            platform = "iphone";

            PackRes_Textures.platform = platform;
            PackRes_Textures.Pack_Textures();

            PackRes_Gui.platform = platform;
            PackRes_Gui.Pack_Gui();

            PackRes_Scene.platform = platform;
            PackRes_Scene.Pack_Scene();

            //**********************
            GenerateIndexFile();
            CopyResourceToStreamAssetDirectory(platform);
        }

        [MenuItem("Assets/PackRes/Pack all resources_android")]
        public static void PackAllRes_android()
        {
            platform = "android";

            PackRes_Textures.platform = platform;
            PackRes_Textures.Pack_Textures();

            PackRes_Gui.platform = platform;
            PackRes_Gui.Pack_Gui();

            PackRes_Scene.platform = platform;
            PackRes_Scene.Pack_Scene();

            //**********************
            GenerateIndexFile();
            CopyResourceToStreamAssetDirectory(platform);
        }

        private static void CopyResourceToStreamAssetDirectory(string platform)
        {
            string buildPlatform = platform;

            string platformDir = Path.Combine(Path.GetFullPath(binPath), buildPlatform);

            string streamAssetDir = Path.Combine(Application.streamingAssetsPath, "res");
            streamAssetDir = PackRes_Def.GetPlatformPathString(streamAssetDir);

            //Debug.Log("platformDir " + platformDir);
            //Debug.Log("streamAssetDir " + streamAssetDir);
            //Debug.Log("GetCurrentDirectory " + Directory.GetCurrentDirectory());
            if (Directory.Exists(streamAssetDir))
            {
                Directory.Delete(streamAssetDir, true);
            }
            Directory.CreateDirectory(streamAssetDir);

            DirectoryInfo source = new DirectoryInfo(platformDir);
            FileInfo[] files = source.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                string destName = Path.Combine(streamAssetDir, files[i].Name);
                File.Copy(files[i].FullName, destName);
            }

            //copy the idx file and md5 file to resources folder
            string idxFilePath = Path.Combine(platformDir, idxFileName);
            string checksumFilePath = Path.Combine(platformDir, bundleMd5FileName);
            string destIdxFilePath = Path.Combine(Path.GetFullPath("Assets/Resources"), idxFileName);
            string destChecksumFilePath = Path.Combine(Path.GetFullPath("Assets/Resources"), bundleMd5FileName);
            if (File.Exists(destIdxFilePath))
            {
                File.Delete(destIdxFilePath);
            }
            File.Copy(idxFilePath, destIdxFilePath);
            if (File.Exists(destChecksumFilePath))
            {
                File.Delete(destChecksumFilePath);
            }
            File.Copy(checksumFilePath, destChecksumFilePath);

            EditorApplication.ExecuteMenuItem("Assets/Refresh");
        }

        private static void GenerateIndexFile()
        {
            PackRes_Def.PackSetting textureSetting = PackRes_Textures.packSetting;
            PackRes_Def.PackSetting guiSetting = PackRes_Gui.packSetting;
            PackRes_Def.PackSetting sceneSetting = PackRes_Scene.packSetting;



            List<PackRes_Def.PackSetting> packSettingList = new List<PackRes_Def.PackSetting>();
            packSettingList.Add(textureSetting);
            packSettingList.Add(guiSetting);
            packSettingList.Add(sceneSetting);

            string platformDir = Path.Combine(Path.GetFullPath(binPath), platform);
            if (Directory.Exists(platformDir))
            {
                Directory.Delete(Path.Combine(Path.GetFullPath(binPath), platform), true);
            }
            Directory.CreateDirectory(Path.Combine(Path.GetFullPath(binPath), platform));

            Dictionary<string, BundleCheckSumData> bundleChecksumDict = new Dictionary<string, BundleCheckSumData>();
            Dictionary<string, string> idxMap = new Dictionary<string, string>();

            foreach (PackRes_Def.PackSetting packSetting in packSettingList)
            {

                string idxfile = packSetting.GetPlatformIdxFileFullPath(platform);
                idxfile = Path.Combine(idxfile, packSetting.index_file);

                string srcBundlePath = packSetting.GetPlatformBundleFullPath(platform);
                string destBundlePath = Path.Combine(Path.GetFullPath(binPath), platform);

                Debug.Log("idxfile: " + idxfile);

                Dictionary<string, string> tempIdx = LoadIdxMapFile(idxfile);
                foreach (KeyValuePair<string, string> pair in tempIdx)
                {
                    if (idxMap.ContainsKey(pair.Key))
                    {
                        Debug.LogError("Dupilate asset : " + pair.Key + "  bundle: " + pair.Value);
                        throw new PackRes_Def.PckException("Dupilate asset : " + pair.Key + "  bundle: " + pair.Value);
                    }
                    else
                    {
                        idxMap.Add(pair.Key, pair.Value);

                        //copy the bundle file to bin path
                        string srcfile = Path.Combine(srcBundlePath, pair.Value);
                        string destfile = Path.Combine(destBundlePath, pair.Value);
                        if (!File.Exists(srcfile))
                        {
                            Debug.LogError("srcfile not exist: " + srcfile);
                            throw new PackRes_Def.PckException("srcfile not exist: " + srcfile);
                        }
                        if (!File.Exists(destfile))
                        {
                            File.Copy(srcfile, destfile);
                        }

                        //create the bundleMD5Dict
                        if (!bundleChecksumDict.ContainsKey(pair.Value))
                        {
                            BundleCheckSumData checksum = new BundleCheckSumData();
                            checksum.md5 = PackRes_Common.ComputeFileMD5Value(destfile);
                            checksum.size = PackRes_Common.GetFileSize(destfile);
                            bundleChecksumDict.Add(pair.Value, checksum);
                        }
                    }
                }
            }
            string temppath = Path.Combine(binPath, platform);
            PackRes_Common.ExpIdxFile(temppath, idxMap, idxFileName);
            AddIdxFileToChecksum(ref bundleChecksumDict, temppath, idxFileName);
            PackRes_Common.ExpBundleCheckSumFile(temppath, bundleChecksumDict, bundleMd5FileName);
        }

        private static void AddIdxFileToChecksum(ref Dictionary<string, BundleCheckSumData> checksumDict, string idxPath, string fileName)
        {
            string dp = PackRes_Common.GetOrCreateAbsPath(idxPath);
            string dstFile = Path.Combine(dp, fileName.Trim(PackRes_Def.trim));
            BundleCheckSumData idxChecksum = new BundleCheckSumData();
            idxChecksum.md5 = PackRes_Common.ComputeFileMD5Value(dstFile);
            idxChecksum.size = PackRes_Common.GetFileSize(dstFile);
            checksumDict.Add(fileName, idxChecksum);
        }

        private static string GetIndexFilePath(PackRes_Def.PackSetting packSetting, string pPlatform)
        {
            string exp_r_path = "../";
            string exp_path = Path.GetFullPath(exp_r_path);
            string temp = Path.Combine(exp_path, pPlatform);
            temp = Path.Combine(temp, packSetting.res_idx_path);
            temp = Path.Combine(temp, packSetting.index_file);

            return temp;
        }

        public static Dictionary<string, string> LoadIdxMapFile(string file)
        {
            if (!File.Exists(file))
            {
                return null;
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();
            FileStream fs = new FileStream(file, FileMode.Open);
            TextReader reader = new StreamReader(fs);
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                string[] kv = line.Split(":".ToCharArray());
                if (kv.Length != 2)
                {
                    continue;
                }
                dict[kv[0].Trim()] = kv[1].Trim();
            }
            reader.Close();
            fs.Close();

            return dict;
        }
    }
}