using UnityEditor;
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

using System.Security.Cryptography;

using System.Reflection;

using IniParser;

using Object = UnityEngine.Object;

namespace PackResFramework
{
	public class PackRes_Common 
	{
		public static void AddFileChecksum(Dictionary<string, string> dict, string filePath)
		{
			if (!File.Exists(filePath))
			{
				dict[filePath] = "NULL";
				throw new PackRes_Def.PckException("file missing? " + filePath);
			}
			
			if (dict.ContainsKey(filePath))
			{
				return;
			}
			
			MD5 md5 = MD5.Create();
			
			FileStream fs = new FileStream(filePath, FileMode.Open);
			byte[] result = md5.ComputeHash(fs);
			fs.Close();
			dict[filePath] = BitConverter.ToString(result).Replace("-", "").ToLower();
			
			string metaFilePath = AssetDatabase.GetTextMetaDataPathFromAssetPath(filePath);
			if (File.Exists(metaFilePath)) 
			{
				fs = new FileStream(metaFilePath, FileMode.Open);
				result = md5.ComputeHash(fs);
				fs.Close();
				dict[metaFilePath] = BitConverter.ToString(result).Replace("-", "").ToLower();
			}
			
			string[] depends = AssetDatabase.GetDependencies(new string[]{ filePath });
			foreach(string dep in depends) 
			{
				string temp = PackRes_Def.GetPlatformPathString(dep);
				AddFileChecksum(dict, temp);
			}
		}
		
		public static Dictionary<string, string> LoadDict(string file)
		{
			if (!File.Exists(file))
			{
				return null;
			}
			
			Dictionary<string, string> dict = new Dictionary<string, string>();
			FileStream fs = new FileStream(file, FileMode.Open);
			TextReader reader = new StreamReader(fs);
			string line = null;
			while((line = reader.ReadLine()) != null)
			{
				string[] kv = line.Split("=".ToCharArray());
				if (kv.Length != 2)
				{
					continue;
				}
				dict[kv[0]] = kv[1];
			}
			reader.Close();
			fs.Close();
			
			return dict;
		}
		
		public static void SaveDict(string file, Dictionary<string, string> dict)
	    {
	        if (File.Exists(file)) 
			{
	            File.Delete(file);
	        }
	        
	        FileStream fs = new FileStream(file, FileMode.CreateNew);
	        TextWriter writer = new StreamWriter(fs);
	        foreach(string key in dict.Keys)
	        {
	            string line = key + '=' + dict[key];
	            writer.WriteLine(line);
	        }
	        writer.Flush();
	        writer.Close();
	        fs.Close();
	    }
		
		public static bool SameDict(Dictionary<string, string> dictA, Dictionary<string, string> dictB)
		{
			if (dictA == null || dictB == null || dictA.Count != dictB.Count)
			{
				return false;
			}
			
			foreach(string keyA in dictA.Keys)
			{
				if (!dictB.ContainsKey(keyA))
				{
					return false;
				}
				
				if (!dictA[keyA].Equals(dictB[keyA]))
				{
					return false;
				}
			}
			
			return true;
		}
		
		public static string StrList(List<string> strList)
		{
			if (strList == null || strList.Count == 0)
			{
				return "NONE";
			}
			StringBuilder strBuilder = new StringBuilder();
			foreach(string str in strList) 
			{
				strBuilder.Append(str);
				strBuilder.Append(',');
			}
			return strBuilder.ToString();
		}
		
		public static void SearchResource(HashSet<string> fileSet, string path, string searchPattern)
		{
			string[] files = Directory.GetFiles(path, searchPattern);
			foreach(string f in files)
			{
				fileSet.Add(f);
			}
			
			string[] dirs = Directory.GetDirectories(path);
			foreach(string d in dirs)
			{
				SearchResource(fileSet, d, searchPattern);
			}
		}
	    
	    public static List<PackRes_Def.BundleData> ParseResCfg(string pckCfgFile, string srcPath,
	                                                           string option, bool compress)
	    {
	        SectionDataCollection sectionList = GetSectsFromCfg(pckCfgFile);
	        if(sectionList == null || sectionList.Count == 0) return null;
	
	        //get all settings
	        List<PackRes_Def.BundleData> bundlelist = new List<PackRes_Def.BundleData>();
	        foreach(SectionData sect in sectionList)
	        {
	            PackRes_Def.BundleData bundledata = new PackRes_Def.BundleData();
	            bundledata.bundleFile = sect.SectionName.Trim(PackRes_Def.trim);
				bundledata.bundleFile = PackRes_Def.GetPlatformPathString(bundledata.bundleFile);
				
	            bundledata.option = option;
	            bundledata.compress = compress;
	
	            string source = GetValFromCfg(sect, "source");
	            if(source == null || source == "")
				{
	                throw new PackRes_Def.PckException("can not get source file in section: " + sect);
				}
	            else
	            {
	                string[] files = source.Split('|');
	                List<string> srcFiles = new List<string>();
	                List<string> srcNames = new List<string>();
	                foreach(string file in files)
	                {
	                    string tmp = file.Trim(PackRes_Def.trim);
	                    string name = Path.GetFileName(tmp);
	                    if(name.StartsWith("*."))
	                    {
	                        string path = Path.GetDirectoryName(tmp);
	                        string[] names = Directory.GetFiles(Path.Combine(srcPath, path), name, SearchOption.AllDirectories);
	
	                        foreach(string n in names)
	                        {
	                            string n0 = Path.GetFileName(n);
	                            srcFiles.Add(n);//(Path.Combine(srcPath, Path.Combine(path, n0)));
	                            srcNames.Add(n0);
	                        }
	                    }
	                    else 
						{
	                        srcNames.Add(name);
	                        srcFiles.Add(Path.Combine(srcPath,tmp));
	                    }
	                }
	
	                bundledata.srcFiles = srcFiles.ToArray();
	                bundledata.names = srcNames.ToArray();
	            }
	            bundlelist.Add(bundledata);
	        }
	        return bundlelist;
	    }
	
	    //folder is relative path to proj path
	    public static List<string> GetFileListRecursively(string folder, List<string> type_list)
	    {
	        List<string> list = new List<string>();
	        foreach(string ty in type_list) 
			{
	            string pat = "*" + ty.Trim(PackRes_Def.trim);
	            string[] files = Directory.GetFiles(folder, pat);
	            list.AddRange(new List<string>(files));
	        }
	        foreach(string path in Directory.GetDirectories(folder)) 
			{
	            list.AddRange(GetFileListRecursively(path, type_list));
	        }
	        return list;
	    }
	

	    public static List<string> GetNamesFromFileList(List<string> files)
	    {
	        List<string> names = new List<string>();
	
	        foreach(string file in files) 
			{
	            //ignore the relative path
	            string name = Path.GetFileName(file);
	            if(names.Contains(name))
				{
	                throw new PackRes_Def.PckException("duplicate file name: " + name);
				}
	        }
	
	        return names;
	    }
	
	    public static void PackBundle(List<PackRes_Def.BundleData> bundles,
	                                  string dstPath, 
	                                  string platform,
	                                  Dictionary<string, string> idxMap)
	    {
	        if(idxMap != null) idxMap.Clear();
	        
			AssetDatabase.SaveAssets();
	
	        Dictionary<string, string> checksumDict = new Dictionary<string, string>();
			foreach(PackRes_Def.BundleData bundle in bundles)
	        {
	            List<UnityEngine.Object> objs = new List<UnityEngine.Object>();
	            List<string> names = new List<string>();
				
	            if(bundle.srcFiles.Length <= 0)
	                continue;
				
				checksumDict.Clear();
				for(int i = 0; i < bundle.srcFiles.Length; ++i)
	            {
	                string file = bundle.srcFiles[i];
	                string name = bundle.names[i];
	                UnityEngine.Object obj = CreateAsset(file.Trim(PackRes_Def.trim));
	                objs.Add(obj);
	                names.Add(name);
	
	                if(idxMap != null) 
					{
	                    if(idxMap.ContainsKey(name)) 
						{
	                        throw new PackRes_Def.PckException("duplicate file name: " + name);
	                    }
	                    idxMap.Add(name, bundle.bundleFile);
	                }
					AddFileChecksum(checksumDict, file);
	            }
	
	            BuildAssetBundleOptions option = GetBuildOption(bundle.option);
	            string dp = GetOrCreateAbsPath(dstPath);
	            string dstFile = Path.Combine(dp, bundle.bundleFile.Trim(PackRes_Def.trim));
	            BuildTarget pm = GetBuildTarget(platform);
	
	            if(!bundle.compress)
	                option |= BuildAssetBundleOptions.UncompressedAssetBundle;
	
				string dictPath = dstFile + ".hash";
				Dictionary<string, string> existDict = LoadDict(dictPath);
				if (File.Exists(dstFile) && SameDict(existDict, checksumDict))
				{
					//Logger.LogMsg("use exist " + dstFile);
					continue;
				}
				
	            BuildPipeline.BuildAssetBundleExplicitAssetNames(objs.ToArray(), 
	                                                             names.ToArray(), 
	                                                             dstFile, 
	                                                             option, pm);
	
				Debug.Log("checksumDict.length " + checksumDict.Count);
				SaveDict(dictPath, checksumDict);
	        }
	    }
	
	
	    public static void GenIdxMap(List<PackRes_Def.CombineBundleData> bundles, 
	                                 Dictionary<string, string> map)
	    {
	        if(bundles == null || map == null) return;
	
	        foreach(PackRes_Def.CombineBundleData bundle in bundles) 
			{
	            if(map.ContainsKey(bundle.name)) 
				{
	                throw new PackRes_Def.PckException("duplicate res name: " + bundle.name);
	            }
	            map.Add(bundle.name, bundle.bundleFile);
	        }
	    }
	        

	
	    public static bool ExpIdxFile(string idxPath, Dictionary<string, string> idxMap, string idxFile)
	    {
	        if(idxFile == null || idxMap == null) return false;
	        if(idxMap.Count <= 0) return false;
	
			Debug.Log("ExpIdxFile");
	        string dp = GetOrCreateAbsPath(idxPath);
	        string dstFile = Path.Combine(dp, idxFile.Trim(PackRes_Def.trim));
	        FileStream fs = new FileStream(dstFile, FileMode.Create);
	        StreamWriter writer = new StreamWriter(fs);
	        
	        foreach(KeyValuePair<string, string> kvp in idxMap)
	            writer.Write(kvp.Key + ":" + kvp.Value + "\n");
	
	        writer.Flush();
	        writer.Close();
	        fs.Close();
	
	        return true;
	    }

	    
	    public static string GetProjectName()
	    {
	        string name = Application.dataPath;
	        name = name.Replace('\\', '/');
	        string[] subs = name.Split(new char[] {'/'});
	        
	        //hack code, -1 is "Assets"
	        name = subs[subs.Length - 2];
	        name = name.Trim(PackRes_Def.trim);
	        name = name.Trim(new char[] {'/'});
	
	        return name;
	    }
	    
	    public static string GetRelativePathToUnityProject(string path)
	    {
	        return path;
	    }
	       
	    public static string GetOrCreateAbsPath(string path)
	    {
	        string absname = GetAbsPath(path.Trim(PackRes_Def.trim));
	        if(!Directory.Exists(absname))
	        {
	            Directory.CreateDirectory(absname);
	        }
	        return absname;
	    }
	    
	    public static string GetAbsPath(string path)
	    {
	        return System.IO.Path.GetFullPath(path);
	    }
	
	    public static SectionDataCollection GetSectsFromCfg(string file)
	    {
	        FileIniDataParser parser = new FileIniDataParser();
	        IniData data = parser.LoadFile(file);
	        return data.Sections;
	    }
	
	    public static string GetValFromCfg(SectionDataCollection col, string sect,
	                                       string key)
	    {
	        return col.GetSectionData(sect).Keys.GetKeyData(key).Value.Trim(PackRes_Def.trim);
	    }
	
	    public static string GetValFromCfg(SectionData section, string key)
	    {
	        return section.Keys.GetKeyData(key).Value.Trim(PackRes_Def.trim);
	    }
	
	    public static UnityEngine.Object CreateAsset(string file)
	    {
	        string absname = GetRelativePathToUnityProject(file);
	
	        UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(absname);
	        if(obj == null) throw new PackRes_Def.PckException("load asset failed: " + absname);
	        return obj;
	    }
	
	    public static BuildAssetBundleOptions GetBuildOption(string option)
	    {
	        BuildAssetBundleOptions bundleoption = 0;
	        string[] ops = option.Split('|');
	        foreach(string op in ops)
	        {
	            string tmp = op.Trim(PackRes_Def.trim).ToLower();
	
	            if(tmp.Equals("collectdependencies"))
	                bundleoption |= BuildAssetBundleOptions.CollectDependencies;
	            else if(tmp.Equals("completeassets"))
	                bundleoption |= BuildAssetBundleOptions.CompleteAssets;
	            else if(tmp.Equals("deterministicassetbundle"))
	                bundleoption |= BuildAssetBundleOptions.DeterministicAssetBundle;
	        }
	
	        return bundleoption;
	    }
	
	    public static BuildTarget GetBuildTarget(string target)
	    {
	        BuildTarget tar = BuildTarget.StandaloneWindows;
	        switch(target.Trim(PackRes_Def.trim).ToLower())
	        {
	        case "webplayer":
	            tar = BuildTarget.WebPlayer;
	            break;
	        case "iphone":
	            tar = BuildTarget.iPhone;
	            break;
	        case "android":
	            tar = BuildTarget.Android;
	            break;
	        default:
	            throw new PackRes_Def.PckException("unkown build target: " + target);
	        }
	        return tar;
	    }
	    
	    public static void DeActiveCamera(GameObject go)
	    {
	        Camera[] cams = go.GetComponentsInChildren<Camera>();
	        foreach(Camera cam in cams)
	        {
	            //if(cam.gameObject.name.ToLower() == "portrait_camera")
	            cam.enabled = false;
	        }
	    }

		
		public static PackRes_Def.PackSetting ParsePackSetting(string configFilePath)
		{
			FileIniDataParser iniParser = new FileIniDataParser();
			IniData data = iniParser.LoadFile(configFilePath);
			PackRes_Def.PackSetting packSetting = new PackRes_Def.PackSetting();
			
			SectionData sectionData = data.Sections.GetSectionData("global_settings");
			
			
			packSetting.cfg_path = GetValFromCfg(sectionData, "cfg_path");
			packSetting.cfg_path = PackRes_Def.GetPlatformPathString(packSetting.cfg_path);
			
			packSetting.cfg_file = GetValFromCfg(sectionData, "cfg_file");
			packSetting.cfg_file = PackRes_Def.GetPlatformPathString(packSetting.cfg_file);
			
			packSetting.res_path = GetValFromCfg(sectionData, "res_path");
			packSetting.res_path = PackRes_Def.GetPlatformPathString(packSetting.res_path);
			
			packSetting.option = GetValFromCfg(sectionData, "option");
			packSetting.exp_path = Path.GetFullPath(GetValFromCfg(sectionData, "exp_path"));
			packSetting.exp_path = PackRes_Def.GetPlatformPathString(packSetting.exp_path);
			
			packSetting.bundle_path = GetValFromCfg(sectionData, "bundle_path");
			packSetting.bundle_path = PackRes_Def.GetPlatformPathString(packSetting.bundle_path);
			packSetting.res_idx_path = GetValFromCfg(sectionData, "res_idx_path");
			packSetting.res_idx_path = PackRes_Def.GetPlatformPathString(packSetting.res_idx_path);
			
			packSetting.index_file = GetValFromCfg(sectionData, "index_file");
			
			packSetting.policy = GetValFromCfg(sectionData, "policy");
			
			string compress_str = GetValFromCfg(sectionData, "compress");
			if(compress_str == "false")
			{
				packSetting.compress = false;
			}
			else if(compress_str == "true")
			{
				packSetting.compress = true;
			}
			else
			{
				Debug.LogError("invalid compress setting");
			}
			
			//parse the file type list
			packSetting.fileTypeList = new List<string>();
			string type_str = GetValFromCfg(sectionData, "res_type_list");
			string[] type_arry = type_str.Split('|');
			foreach(string ty in type_arry)
			{
				if(ty != null && ty != "")
				{
					packSetting.fileTypeList.Add(ty);
				}
			}
			return packSetting;
		}
		
		
		
		
		public static void PrintObjectInfo(System.Object pObject)
		{
			
			System.Type type = pObject.GetType();
			Debug.Log("PrintObjectInfo " + type.Name);
			//Debug.Log("lenght = " + type.GetMembers().Length);
			Debug.Log("lenght = " + type.GetFields().Length);
			System.Reflection.FieldInfo[] fields = type.GetFields();
			
			string tempvalue = "";
			foreach (System.Reflection.FieldInfo field in fields)
            {
				if(field.FieldType.IsArray)
				{
					tempvalue += field.Name + " = [";
					Array array = (Array)field.GetValue(pObject);
					foreach(System.Object obj in array)
					{
						tempvalue += obj.ToString() + ",";
					}
					tempvalue += " ]\n";
				}
				else
				{
					tempvalue += field.Name + " = " + field.GetValue(pObject) + "\n";
                	//Debug.Log("Name:" + field.Name + " Value:" + field.GetValue(pObject));// field.Name, field.ToString())
				}
			}
			Debug.Log(tempvalue);
		} 
		
		public static string ComputeFileMD5Value(string filePath)
		{
			MD5 md5 = MD5.Create();
			
			FileStream fs = new FileStream(filePath, FileMode.Open);
			byte[] result = md5.ComputeHash(fs);
			fs.Close();
			string md5Value = BitConverter.ToString(result).Replace("-", "").ToLower();
			return md5Value;
		}
		
		public static bool  ExpBundleCheckSumFile(string filePath, Dictionary<string, BundleCheckSumData> dict, string fileName)
	    {
	        if(fileName == null || dict == null) return false;
	        if(dict.Count <= 0) return false;
	
			Debug.Log("ExpBundleCheckSumFile");
	        string dp = GetOrCreateAbsPath(filePath);
	        string dstFile = Path.Combine(dp, fileName.Trim(PackRes_Def.trim));
	        
			if (File.Exists(dstFile)) 
			{
	            File.Delete(dstFile);
	        }
	        
	        FileStream fs = new FileStream(dstFile, FileMode.CreateNew);
	        TextWriter writer = new StreamWriter(fs);
	        foreach(string key in dict.Keys)
	        {
	            string line = key + ':' + dict[key].md5 + ":" + dict[key].size;
	            writer.WriteLine(line);
	        }
	        writer.Flush();
	        writer.Close();
	        fs.Close();
			
	        return true;
	    }
		
		public static long GetFileSize(string filepath)
		{
			FileInfo fi = new FileInfo(filepath);
			return fi.Length;
		}
		
	    public static void PackScenes(List<string> scenes,
                              string dstPath, 
                              string platform,
                              Dictionary<string, string> idxMap)
		{
			if(idxMap == null)
			{
				return;
			}
			AssetDatabase.SaveAssets();
			
			Dictionary<string, string> checksumDict = new Dictionary<string, string>();
			
			if(!Directory.Exists(dstPath))
			{
				Directory.CreateDirectory(dstPath);
			}
			
			for(int i = 0; i < scenes.Count; i++)
			{
				string[] levels = new string[1];
				levels[0] = scenes[i];
				
				string fileName = Path.GetFileName(levels[0]);
				idxMap.Add(fileName, fileName);
				
				string dstFile = Path.Combine(dstPath, fileName);
				BuildTarget buildTarget = GetBuildTarget(platform);
				
				checksumDict.Clear();
				AddFileChecksum(checksumDict, scenes[i]);
				
				string dictPath = dstFile + ".hash";
				Dictionary<string, string> existDict = LoadDict(dictPath);
				if(File.Exists(dictPath) && SameDict(checksumDict, existDict))
				{
					continue;
				}
				
				BuildPipeline.BuildStreamedSceneAssetBundle(levels, dstFile, buildTarget);
				SaveDict(dictPath, checksumDict);
				
			}
			
		}
	}
}