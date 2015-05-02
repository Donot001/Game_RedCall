using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using System.IO;

namespace PackResFramework
{
	public class PackRes_Gui 
	{
		
		const string GUI_PACK_CONFIG_PATH = "Assets/Editor/PackResConfig/gui_settings_cfg.ini";
	
		
		public static PackRes_Def.PackSetting packSetting;
		
		
		public static string platform = "webplayer";
		
		[MenuItem("Assets/PackRes/other/Pack gui")]
		public static void Pack_Gui()
		{
			ParseSetting();
			Pack();
		}
		private static void ParseSetting()
		{
			packSetting = PackRes_Common.ParsePackSetting(Path.GetFullPath(GUI_PACK_CONFIG_PATH));
			
//			packSetting.exp_path = Path.Combine(packSetting.exp_path, platform);
//			
//			packSetting.res_idx_path = Path.Combine(packSetting.exp_path, packSetting.res_idx_path);
//			
//			packSetting.exp_path = Path.Combine(packSetting.exp_path, packSetting.bundle_path);
			
			PackRes_Common.PrintObjectInfo(packSetting);
			
		}
		private static void Pack()
		{
			List<PackRes_Def.BundleData> bundleDataList = null;
			Dictionary<string, string> idxMap = new Dictionary<string, string>();
			
			if(packSetting.policy == PackRes_Def.packPolicy_configFile)
			{
				bundleDataList = InitCfgDataList();
				PackRes_Common.PackBundle(bundleDataList, packSetting.GetPlatformBundleRelativePath( platform ), platform, idxMap);
				
				foreach(PackRes_Def.BundleData data in bundleDataList)
				{
					PackRes_Common.PrintObjectInfo(data);
				}
			}
			else if(packSetting.policy == PackRes_Def.packPolicy_standalone)
			{
				bundleDataList = InitStandaloneDataList();
				PackRes_Common.PackBundle(bundleDataList, packSetting.GetPlatformBundleFullPath(platform), platform, idxMap);
				
				foreach(PackRes_Def.BundleData data in bundleDataList)
				{
					PackRes_Common.PrintObjectInfo(data);
				}
			}
			else if(packSetting.policy == PackRes_Def.packPolicy_crossDependency)
			{
				
			}
			else
			{
				Debug.LogError("PackPrefabGui not support policy: " + packSetting.policy);
				return;
			}
			
						
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