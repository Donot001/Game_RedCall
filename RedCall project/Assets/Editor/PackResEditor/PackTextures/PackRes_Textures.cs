using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

using IniParser;

namespace PackResFramework
{
	public class PackRes_Textures 
	{
		
		const string TEXTURE_PACK_CONFIG_PATH = "Assets/Editor/PackResConfig/texture_settings_cfg.ini";
	
		
		public static PackRes_Def.PackSetting packSetting;
		
		
		public static string platform = "webplayer";
		
		
		[MenuItem("Assets/PackRes/other/Pack texture")]
		public static void Pack_Textures()
		{			
			ParseSetting();
			Pack();
		}
		private static void ParseSetting()
		{
			string configpath = Path.GetFullPath(TEXTURE_PACK_CONFIG_PATH);
			Debug.Log(configpath);
			
			packSetting = PackRes_Common.ParsePackSetting(configpath);

			
			PackRes_Common.PrintObjectInfo(packSetting);
		}
		
		private static void Pack()
		{
			List<PackRes_Def.BundleData> bundleDataList;
			if(packSetting.policy == PackRes_Def.packPolicy_configFile)
			{
				bundleDataList = InitCfgDataList();
			}
			else if(packSetting.policy == PackRes_Def.packPolicy_standalone)
			{
				bundleDataList = InitStandaloneDataList();
			}
			else
			{
				Debug.LogError("PackTexture not support policy: " + packSetting.policy);
				return;
			}
			
			foreach(PackRes_Def.BundleData data in bundleDataList)
			{
				PackRes_Common.PrintObjectInfo(data);
			}
			
			Dictionary<string, string> idxMap = new Dictionary<string, string>();
			
			PackRes_Common.PackBundle(bundleDataList, packSetting.GetPlatformBundleFullPath( platform ), platform, idxMap);
			
			PackRes_Common.ExpIdxFile(packSetting.GetPlatformIdxFileFullPath( platform ), idxMap, packSetting.index_file);
		}
		
		private static List<PackRes_Def.BundleData> InitCfgDataList()
		{
			string localpath = Path.Combine(packSetting.cfg_path, packSetting.cfg_file);
			string packCfgPath = Path.GetFullPath(localpath);
			List<PackRes_Def.BundleData> bundleList = PackRes_Common.ParseResCfg(packCfgPath, packSetting.res_path, packSetting.option, packSetting.compress);
			return bundleList;	
		}
		private static List<PackRes_Def.BundleData> InitStandaloneDataList()
		{
			List<PackRes_Def.BundleData> bundleDataList = new List<PackRes_Def.BundleData>();
			
			List<string> files = PackRes_Common.GetFileListRecursively(packSetting.res_path, packSetting.fileTypeList);
			foreach(string file in files)
			{
				PackRes_Def.BundleData bundleData = new PackRes_Def.BundleData();
				bundleData.bundleFile = PackRes_Def.bundlePrefix_standalone + Path.GetFileName(file) + ".bundle";
				bundleData.srcFiles = new string[1];
				bundleData.srcFiles[0] = file;
				bundleData.names = new string[1];
				bundleData.names[0] = Path.GetFileName(file);
				bundleData.compress = packSetting.compress;
				bundleData.option = packSetting.option;
				
				bundleDataList.Add(bundleData);
			}
			return bundleDataList;
		}
	}
}