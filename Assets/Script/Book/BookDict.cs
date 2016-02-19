using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class BookDict
{

	static public string m_bookExt = ".book";
	static public string m_bookPath = "Book";

	private string m_bookName = "Default";
	private string m_rootPath;

	private List<string> m_bookWords;
	private List<BookItem> m_bookItems;
	private HashSet<string> m_wordSet;

	public BookDict(string path, string name)
	{
		m_bookWords = new List<string>();
		m_bookItems = new List<BookItem>();
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

	static string m_contentTag = "content";
	static string m_itemTag = "item";

	// attribute
	static string m_idTag = "id";

	public void ReadBook(Progress progress)
	{
		/*
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
							BookItem item = new BookItem(word);
							progress.AddWord (item);
							m_bookItems.Add(item);
						}
					}
				}
			}
		}
		m_bookWords.Sort();
		*/

		string xmlpath = BookDirectory() + m_bookName + ".xml";

		XmlDocument doc = new XmlDocument();
		doc.Load(xmlpath);
		XmlNodeList contentList = doc.DocumentElement.GetElementsByTagName(m_contentTag);
		IEnumerator contentEnum = contentList.GetEnumerator();
		while (contentEnum.MoveNext())
		{   
			XmlNode contentNode = (XmlNode) contentEnum.Current;
			XmlNodeList itemList = contentNode.SelectNodes(m_itemTag);
			IEnumerator itemEnum = itemList.GetEnumerator();
			while (itemEnum.MoveNext())
			{
				XmlNode itemNode = (XmlNode) itemEnum.Current;
				string word = itemNode.Attributes[m_idTag].Value;
				if(!m_wordSet.Contains(word))
				{
					m_wordSet.Add(word);
					m_bookWords.Add(word);
					BookItem item = new BookItem(word);
					item.ReadItem(itemNode);
					progress.AddWord (item);
					m_bookItems.Add(item);
				}
			}
		}
		ItemComparer comparer = new ItemComparer();
		m_bookItems.Sort(comparer);
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

		XmlDocument doc = new XmlDocument();
		XmlElement rootElement = doc.CreateElement("document");
		doc.AppendChild(rootElement);

		XmlElement headerElem = doc.CreateElement("header");
		rootElement.AppendChild(headerElem);

		XmlElement contentElem = doc.CreateElement(m_contentTag);
		foreach(BookItem item in m_bookItems)
		{
			XmlElement itemElem = doc.CreateElement(m_itemTag);
			item.SaveItem(itemElem, doc);
			contentElem.AppendChild(itemElem);
		}
		rootElement.AppendChild(contentElem);

		doc.PreserveWhitespace = true;
		string xmlpath = BookDirectory() + m_bookName + ".xml";

		using (XmlTextWriter writer = new XmlTextWriter(xmlpath, null))
		{
			writer.Formatting = Formatting.Indented;
			doc.Save(writer);
		}
	}

}
