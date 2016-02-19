using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PanelMain : MonoBehaviour {

	public Progress BookProgress
	{
		get {
			return m_progress;
		}
		set {
			m_progress = value;
			m_wordChanged = true;
		}
	}

	public GameObject m_panelInfoText;
	public GameObject m_wordInfoText;

	public GameObject m_buttonPanel;
	public GameObject m_choicePanel;

	private Progress m_progress;
	private Record m_currentWord;
	private GameObject m_wordObject;
	private Text m_wordText;
	private bool m_wordChanged;

	void UpdateText()
	{
		m_currentWord = m_progress.GetCurrentWord();
		if(m_currentWord != null)
		{
			m_wordText.text = m_currentWord.m_word;
			m_currentWord.UpdateInfoText(m_wordInfoText);
		}
		else
		{
			m_wordText.text = "Completed!";
		}
		m_progress.UpdateInfoText(m_panelInfoText);

		if(m_currentWord.Item.Answer.Length > 0)
		{
			List<string> choice = new List<string>();
			choice.Add(m_currentWord.Item.Answer);
			choice.AddRange(m_currentWord.Item.Choices);
			if(choice.Count == 4)
			{
				m_choicePanel.SetActive(true);
				m_buttonPanel.SetActive(false);
				return;
			}
		}
		m_choicePanel.SetActive(false);
		m_buttonPanel.SetActive(true);
	}

	public void ClearText()
	{
		m_wordText.text = "";
	}

	// Use this for initialization
	void Start ()
	{
		m_wordObject = GameObject.Find ("WordText");
		m_wordText = m_wordObject.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(m_wordChanged)
		{
			UpdateText();
			m_wordChanged = false;
		}
	}

	public void OnClickKnown()
	{
		m_currentWord.UpdateRecord(true);
		m_progress.ReBelance();
		m_wordChanged = true;
	}

	public void OnClickUnknown()
	{
		m_currentWord.UpdateRecord(false);
		m_progress.ReBelance();
		m_wordChanged = true;
	}
}
