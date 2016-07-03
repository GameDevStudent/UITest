using UnityEngine;
using System;
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
	private string m_type= "字词";
	private int m_timer = 0;

	enum AnswerType { Default, Choice, Input };
	private AnswerType m_answerType;

	public int Time
	{
		get
		{
			return m_timer;
		}
	}

	public bool NeedInput() { return m_answerType == AnswerType.Input; }
	public bool NeedChoice() { return m_answerType == AnswerType.Choice; }

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

	static string m_headerTag = "header";
	static string m_typeTag = "type";
	static string m_answerTag = "answer";
	static string m_contentTag = "content";
	static string m_itemTag = "item";
	static string m_timeTag = "time";

	// attribute
	static string m_idTag = "id";

	public void ReadBook(Progress progress)
	{
		string xmlpath = BookDirectory() + m_bookName + ".xml";

		XmlDocument doc = new XmlDocument();
		doc.Load(xmlpath);
		XmlNodeList headerList = doc.DocumentElement.GetElementsByTagName(m_headerTag);
		IEnumerator headerEnum = headerList.GetEnumerator();
		if(headerEnum.MoveNext())
		{
			XmlNode headerNode = (XmlNode) headerEnum.Current;
			var typeAttr = headerNode.Attributes[m_typeTag];
			if(typeAttr != null)
			{
				m_type = typeAttr.Value;
			}
			m_answerType = AnswerType.Default;
			var answerAttr = headerNode.Attributes[m_answerTag];
			if(answerAttr != null)
			{
				m_answerType = (AnswerType) Enum.Parse(typeof(AnswerType), answerAttr.Value); 
			}

			var timeAttr = headerNode.Attributes[m_timeTag];
			if(timeAttr != null)
			{
				m_timer = Convert.ToInt32(timeAttr.Value);
			}
		}

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
					BookItem item = new BookItem(word, this);
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
		XmlDocument doc = new XmlDocument();
		XmlElement rootElement = doc.CreateElement("document");
		doc.AppendChild(rootElement);

		XmlElement headerElem = doc.CreateElement("header");
		headerElem.SetAttribute(m_typeTag, m_type);
		headerElem.SetAttribute(m_timeTag, m_timer.ToString());
		headerElem.SetAttribute(m_answerTag, m_answerType.ToString());

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
