using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewFileController : MonoBehaviour {

	public GameObject m_pageProfile;
	public GameObject m_inputField;
	public GameObject m_buttonCreate;

	bool CompareProfile(string profile)
	{
		InputField profilename = m_inputField.GetComponent<InputField> ();
		if(profilename.text == profile)
		{
			return true;
		}
		return false;
	}

	SelectBook FindSelectScript()
	{
		SelectBook selectScript = null;
		foreach (Transform child in gameObject.transform)
		{
			if(child.gameObject.name == "BookList")
			{
				selectScript = (SelectBook)child.gameObject.GetComponent(typeof(SelectBook));
				return selectScript;
			}
		}
		return null;
	}

	public void OnTextChange()
	{
		SelectBook selectScript = FindSelectScript();
		if(selectScript)
		{
			if(selectScript.BooksSelected.Count > 0)
			{
				InputField profilename = m_inputField.GetComponent<InputField> ();
				if(profilename.text.Length > 0)
				{
					if(Main.instance.IsProfileValid(CompareProfile))
					{
						m_buttonCreate.GetComponent<Button>().interactable = true;
						return;
					}
				}
			}
		}
		m_buttonCreate.GetComponent<Button>().interactable = false;
	}

	public void OnClickCreate()
	{
		SelectBook selectScript = FindSelectScript();
		if(selectScript != null)
		{
			InputField profilename = m_inputField.GetComponent<InputField> ();
			Main.instance.CreateNewProfile(profilename.text, selectScript.BooksSelected);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
