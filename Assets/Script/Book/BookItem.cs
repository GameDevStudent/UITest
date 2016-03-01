using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class BookItem
{
	private string m_text;
	private string m_answer;
	private List<string> m_altenative;
	public BookDict m_dict;

	public BookItem(string text, BookDict dict)
	{
		m_text = text;
		m_altenative = new List<string>();
		m_dict = dict;
	}

	public List<string> Choices
	{
		get
		{
			return m_altenative;
		}
	}

	public string Word
	{
		get
		{
			return m_text;
		}
	}

	public string Answer
	{
		get
		{
			return m_answer;
		}
	}

	// attribute
	static string m_idTag = "id";
	static string m_answerTag = "answer";
	static string m_errorTag = "error";

	public void SaveItem(XmlElement element, XmlDocument doc)
	{
		element.SetAttribute(m_idTag, m_text);
		element.SetAttribute(m_answerTag, m_answer);

		foreach(string err in m_altenative)
		{
			XmlElement errElem = doc.CreateElement(m_errorTag);
			errElem.SetAttribute(m_answerTag, err);
			element.AppendChild(errElem);
		}
		if(m_altenative.Count == 0)
		{
			XmlElement errElem = doc.CreateElement(m_errorTag);
			errElem.SetAttribute(m_answerTag, "empty");
			element.AppendChild(errElem);
		}
	}

	public void ReadItem(XmlNode itemNode)
	{
		m_answer = itemNode.Attributes[m_answerTag].Value;
		XmlNodeList errorList = itemNode.SelectNodes(m_errorTag);
		IEnumerator errorEnum = errorList.GetEnumerator();
		while (errorEnum.MoveNext())
		{
			XmlNode errNode = (XmlNode) errorEnum.Current;
			string answer = errNode.Attributes[m_answerTag].Value;
			m_altenative.Add(answer);
		}
	}
}
