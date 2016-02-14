using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class BookButton : MonoBehaviour {
	/*
	GameObject GetBookButtonObject()
	{
		foreach(Transform child in gameObject.transform)
		{
			if(child.gameObject.name == "BookButton")
			{
				return child.gameObject;
			}
		}
		return null;
	}
	*/
	Text GetObjectText(GameObject uiobj)
	{
		foreach(Transform child in uiobj.transform)
		{
			if(child.gameObject.CompareTag("Text"))
			{
				return child.gameObject.GetComponent<Text>();
			}
		}
		return null;
	}

	private string m_bookPath;
	public string BookPath
	{
		get
		{
			return m_bookPath;
		}
		set
		{
			m_bookPath = value;
			Text btntext = GetObjectText(gameObject);
			btntext.text = m_bookPath;
		}
	}
	private bool m_selected = false;
	public Sprite m_selectImage;
	public Sprite m_unselectImage;

	public void OnSelect(BaseEventData eventdata)
	{
		SelectBook script = (SelectBook)GetComponentInParent (typeof(SelectBook));
		Image background = gameObject.GetComponent<Image>();
		string bkname = background.sprite.name;
		if(m_selected)
		{
			script.Deselected(m_bookPath);
			background.sprite = m_unselectImage;
			m_selected = false;
		}
		else
		{
			script.Selected(m_bookPath);
			background.sprite = m_selectImage;
			m_selected = true;
		}
	}

	public void OnEditBook()
	{
	}
}
