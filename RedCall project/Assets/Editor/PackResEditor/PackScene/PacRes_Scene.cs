using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace PackResFramework
{
	public class PackRes_Scene
	{
		const string SCENE_PACK_CONFIG_PATH = "Assets/Editor/PackResConfig/scene_settings_cfg.ini";
		
		public static PackRes_Def.PackSetting packSetting;
		
		public static string platform = "webPlayer";
		
		[MenuItem("Assets/PackRes/other/PackScene")]
		public static void Pack_Scene()
		{
			ParseSetting();
			Pack();
		}
		
		private static void ParseSetting()
		{
			packSetting = PackRes_Common.ParsePackSetting(Path.GetFullPath(SCENE_PACK_CONFIG_PATH));
		}
		
		private static void Pack()
		{
			string exppath = packSetting.GetPlatformBundleRelativePath(platform);
			
			PackRes_Common.PrintObjectInfo(packSetting);
			
			List<string> files = PackRes_Common.GetFileListRecursively(packSetting.res_path, packSetting.fileTypeList);
			foreach(string scene in files)
			{
				Debug.Log(scene);
			}
			Dictionary<string, string> idxMap = new Dictionary<string, string>();
			PackRes_Common.PackScenes(files, exppath, platform, idxMap);
			
			PackRes_Common.ExpIdxFile(packSetting.GetPlatformIdxFileFullPath( platform ), idxMap, packSetting.index_file);
		}
	}
}