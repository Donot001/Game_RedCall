using System;
using System.Collections.Generic;
using System.IO;

namespace PackResFramework
{
	public class PackRes_Def
	{
		public class BundleData
	    {
	        public string bundleFile;
	        public string[] srcFiles;
	        public string[] names;
	        public string option;
	        public bool compress;
	    };
	
	    public class CombineBundleData
	    {
	        public string srcFile;
	        public string bundleFile;
	        public string name;
	        public string option;
	        public bool compress;
	        //dep map
	        public string[] depFiles;
	    };
	
	    public class BaseBundleData
	    {
	        public string srcFile;
	        public string bundleFile;
	        public string option;
	        public bool compress;
	    }
	    
	    public class PckException : SystemException
	    {
	        public PckException(string msg)
	        :base(msg)
	        {}
	    }
	    
		public class PackSetting
		{
			public string cfg_path;
			public string cfg_file;
			public string res_path;
			
			public string option;
			public string exp_path;
			public string bundle_path;
			public string res_idx_path;
			public string index_file;
			public string policy;
			
			public List<string> fileTypeList;
			public bool compress;
			
			public string GetPlatformBundleRelativePath(string platform)
			{
				string temp = Path.Combine(exp_path, platform);
				temp = Path.Combine(temp, bundle_path);
				return temp;
			}
			public string GetPlatformBundleFullPath(string platform)
			{
				string temp = Path.GetFullPath(exp_path);
				temp = Path.Combine(temp, platform);
				temp = Path.Combine(temp, bundle_path);
				return temp;
			}
			public string GetPlatformIdxFileFullPath(string platform)
			{
				string temp = Path.GetFullPath(exp_path);
				temp = Path.Combine(temp, platform);
				temp = Path.Combine(temp, res_idx_path);
				return temp;
			}
		}
		public const string packPolicy_configFile = "config_file";
		public const string packPolicy_standalone = "standalone";
		public const string packPolicy_bigone = "bigone";
		public const string packPolicy_crossDependency = "cross_dependency";
		
	    public static char[] trim = new char[] { '\0', ' ', '\t', '\r', '\n' };
		
		
		public const string bundlePrefix_standalone = "standalone_";
		public const string bundlePrefix_basebundle = "base_";
		
		public static string GetPlatformPathString(string path)
		{
			char splitChar = (Path.DirectorySeparatorChar == '\\') ? '/': Path.DirectorySeparatorChar;
			string temppath = path.Replace(splitChar, Path.DirectorySeparatorChar);
			return temppath;
		}
		
	}
}