using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BookDict
{

	static public string m_bookExt = ".book";
	public string m_bookPath = "Book";

	private string m_bookName = "Default";
	private string m_rootPath;

	private List<string> m_bookWords;
	private HashSet<string> m_wordSet;

	public BookDict(string path, string name)
	{
		m_bookWords = new List<string>();
		m_wordSet = new HashSet<string>();

		m_rootPath = path;
		m_bookName = name;

		string bookdir = BookDirectory ();
		Directory.CreateDirectory (bookdir);
		string fullpath = BookFullPath();
		if (!File.Exists (fullpath))
		{
			using (StreamWriter file = new StreamWriter (fullpath))
			{
				file.WriteLine ("春天");
				file.WriteLine ("吃饭");
				file.WriteLine ("妈妈");
				file.WriteLine ("西瓜");
			}
		}
	}

	static private string BookFileName(string book)
	{
		return book + m_bookExt;
	}

	private string BookDirectory()
	{
		return m_rootPath + m_bookPath + @"\";
	}

	private string BookFullPath()
	{
		return BookDirectory() + BookFileName(m_bookName);
	}

	public void ReadBook(Progress progress)
	{
		string fullpath = BookFullPath();
		if (File.Exists (fullpath))
		{
			using (StreamReader file = new StreamReader (fullpath))
			{
				while (file.Peek () >= 0)
				{
					string word = file.ReadLine ();
					if(word.Length > 0)
					{
						if(!m_wordSet.Contains(word))
						{
							m_wordSet.Add(word);
							m_bookWords.Add(word);
							progress.AddWord (word);
						}
					}
				}
			}
		}
		m_bookWords.Sort();
	}

	public void SaveBook()
	{
		string fullpath = BookFullPath();
		using (StreamWriter file = new StreamWriter (fullpath))
		{
			foreach(string word in m_bookWords)
			{
				file.WriteLine (word);
			}
		}
	}

}
