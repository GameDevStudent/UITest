using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SelectBook : MonoBehaviour {

	public GameObject m_bookPrefab;
	private List<string> m_Selected;

	public List<string> BooksSelected
	{
		get
		{
			return m_Selected;
		}
	}

	SelectBook()
	{
		m_Selected = new List<string>();
	}

	BookButton GetBookButtonScript(GameObject holder)
	{
		BookButton script = null;
		foreach (Transform child in holder.transform)
		{
			if(child.gameObject.name == "BookButton")
			{
				script = (BookButton)child.gameObject.GetComponent(typeof(BookButton));
				return script;
			}
		}
		return null;
	}

	public void CollectBooks(string root_path, GameObject listbox)
	{
		string path = root_path + BookDict.m_bookPath + @"\";
		string pattern = "*" + BookDict.m_bookExt;
		string[] files = Directory.GetFiles(path, pattern);
		foreach(string file in files)
		{
			GameObject book = Instantiate(m_bookPrefab);
			BookButton script = GetBookButtonScript(book);
			script.BookPath = Path.GetFileNameWithoutExtension(file);
			book.transform.SetParent(listbox.transform, false);
		}
	}

	public void Selected(string book)
	{
		m_Selected.Add(book);
		GameObject parent = gameObject.transform.parent.gameObject;
		NewFileController script = (NewFileController)parent.GetComponent(typeof(NewFileController));
		script.OnTextChange();
	}

	public void Deselected(string book)
	{
		m_Selected.Remove(book);
		GameObject parent = gameObject.transform.parent.gameObject;
		NewFileController script = (NewFileController)parent.GetComponent(typeof(NewFileController));
		script.OnTextChange();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
