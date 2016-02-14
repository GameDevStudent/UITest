using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ProfileButtonController : MonoBehaviour {

	private List<string> m_books;
	private string m_profileName;

	public ProfileButtonController()
	{
		m_books = new List<string>();
	}

	public void SetButtonText(string text, bool isnew)
	{
		if(!isnew)
		{
			m_profileName = text;
		}
		foreach(Transform child in gameObject.transform)
		{
			if(child.gameObject.CompareTag("Text"))
			{
				Text btntext = child.gameObject.GetComponent<Text>();
				btntext.text = text;
			}
		}
	}

	public void AddBook(string book)
	{
		m_books.Add(book);
	}

	public void OnClick()
	{
		if(m_profileName == null)
		{
			Main.instance.OnNewProfile();
		}
		else
		{
			Main.instance.OpenProgress(m_profileName, m_books);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
