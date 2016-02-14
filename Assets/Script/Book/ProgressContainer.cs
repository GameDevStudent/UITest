using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public delegate bool CheckProfile(string book);

public class ProgressContainer
{
	static public string m_ext = ".his";
	static public string m_path = "His";

	static public string ProgressDirectory(string rootpath)
	{
		return rootpath + m_path + @"\";
	}

	static public string FileName(string id)
	{
		return id + m_ext;
	}

	static public string FullPath(string rootpath, string id)
	{
		return ProgressDirectory(rootpath) + FileName(id);
	}

	private List<string> m_profileList;

	public ProgressContainer()
	{
		m_profileList = new List<string>();
	}

	public bool AccessProfiles(CheckProfile del)
	{
		foreach(string profile in m_profileList)
		{
			if(del(profile))
			{
				return true;
			}
		}
		return false;
	}

	public void RecollectProgress(string rootPath)
	{
		m_profileList.Clear();
		string path = ProgressDirectory(rootPath);
		string pattern = "*" + m_ext;
		string[] files = Directory.GetFiles(path, pattern);
		foreach(string file in files)
		{
			string name = Path.GetFileNameWithoutExtension(file);
			m_profileList.Add(name);
		}
	}
}
