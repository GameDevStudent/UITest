using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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
	public GameObject m_inputPanel;

	public GameObject m_judgeText;
	float m_alphaTime = 0.0f;
	const float m_alphaMaxTime = 1.5f;

	int m_wordTime = 0;
	float m_wordCoundown = 0.0f;

	private Progress m_progress;
	private Record m_currentWord;
	private GameObject m_wordObject;
	private Text m_wordText;
	private bool m_wordChanged;

	private GameObject m_timerObject;
	private float m_timerScale;

	List<string> m_choices;

	PanelMain()
	{
		m_choices = new List<string>();
	}

	public void UpdateText()
	{
		m_currentWord = m_progress.GetCurrentWord();
		if(m_currentWord != null)
		{
			m_wordText.text = m_currentWord.m_word;
            if (m_currentWord.m_word.Length > 6)
            {
                m_wordText.fontSize = 140;
            }
            else
            {
                m_wordText.fontSize = 200;
            }
            m_currentWord.UpdateInfoText(m_wordInfoText);
		}
		else
		{
			m_wordText.text = "Completed!";
		}
		m_progress.UpdateInfoText(m_panelInfoText);


		m_wordTime = m_currentWord.Item.m_dict.Time;
		if(m_wordTime > 0)
		{
			m_timerObject.SetActive(true);
			m_wordCoundown = m_wordTime;
		}

		if(m_currentWord.Item.Answer.Length > 0)
		{
			List<string> choices = new List<string>();
			choices.Add(m_currentWord.Item.Answer);
			choices.AddRange(m_currentWord.Item.Choices);
			if(choices.Count == 4 && m_currentWord.Item.m_dict.NeedChoice())
			{
				m_choicePanel.SetActive(false);
				SetChoiceButton(choices);
				m_choicePanel.SetActive(true);
				m_buttonPanel.SetActive(false);
				m_inputPanel.SetActive(false);
				return;
			}
			else if(m_currentWord.Item.m_dict.NeedInput())
			{
				m_choicePanel.SetActive(false);
				m_inputPanel.SetActive(true);
				m_buttonPanel.SetActive(false);

				GameObject parentField = m_inputText.transform.parent.gameObject;
				EventSystem.current.SetSelectedGameObject(parentField, null);
				InputField inputField = parentField.GetComponent<InputField>();
				inputField.OnPointerClick(new PointerEventData(EventSystem.current));
				return;
			}
		}
		m_choicePanel.SetActive(false);
		m_inputPanel.SetActive(false);
		m_buttonPanel.SetActive(true);
	}

	void SetChoiceButton(List<string> choices)
	{
		m_choices.Clear();
		System.Random random = new System.Random();
		while(choices.Count > 0)
		{
			int index = random.Next(0, choices.Count);
			if(index < choices.Count)
			{
				m_choices.Add(choices[index]);
				choices.RemoveAt(index);
			}
		}
		int idx = 0;
		foreach(Transform child in m_choicePanel.transform)
		{
			Image btnimg = child.gameObject.GetComponent<Image>();
			btnimg.color = Color.white;
			foreach(Transform txtChild in child.gameObject.transform)
			{
				Text btntext = txtChild.gameObject.GetComponent<Text>();
				if(idx < 4)
				{
					btntext.text = m_choices[idx];
				}
				else
				{
					btntext.text = "不认识";
				}
			}
			idx++;
		}
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
		m_wordText.text = "";
		m_timerObject = GameObject.Find("TimerText");
		m_timerObject.SetActive(false);
		m_judgeText.SetActive(false);

        InputField field = m_inputPanel.GetComponentInChildren<InputField>();
        if (field != null)
        {
            field.onEndEdit.AddListener(val =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    OnInput();
                }
            });
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
		if(m_wordChanged)
		{
			UpdateText();
			m_wordChanged = false;
		}

		if(m_alphaTime > 0)
		{
			float scale = (m_alphaMaxTime - m_alphaTime) / m_alphaMaxTime;
				m_judgeText.transform.localScale = new Vector3(scale,scale,1);
			Text judge = m_judgeText.GetComponent<Text> ();
			Color color = judge.color;
			judge.color = color;
			m_alphaTime = m_alphaTime - Time.deltaTime;
			if(m_alphaTime <= 0)
			{
				m_alphaTime = 0;
				m_judgeText.SetActive(false);
				m_wordChanged = true;
			}
		}

		if(m_wordCoundown > 0)
		{
			m_wordCoundown = m_wordCoundown - Time.deltaTime;
			Text timeText = m_timerObject.GetComponent<Text> ();
			timeText.text = ((int)m_wordCoundown).ToString();
			Color color = Color.red;

			float timestart = m_wordTime * 0.7f;
			if(m_wordCoundown > timestart)
			{
				color.a = 0;
			}
			else
			{
				color.a = 1.0f - m_wordCoundown / timestart;
			}
			timeText.color = color;

			int timesjump = (int)(m_wordTime * 0.4f);
			float scale = 0.5f;
			if(m_wordCoundown < timesjump)
			{
				float timeleft = m_wordCoundown - (int)m_wordCoundown;
				scale = (1.0f - timeleft) * 0.5f + 0.5f;
			}


			m_timerObject.transform.localScale = new Vector3(scale, scale, 1);
			if(m_wordCoundown <= 0)
			{
				m_timerObject.SetActive(false);
				OnClickChoice(4);
			}
		}
	}

	public GameObject m_inputText;

	public void OnInput()
	{
        m_alphaTime = m_alphaMaxTime;
        m_wordCoundown = 0.0f;

        bool correct = false;
		Text inputText = m_inputText.GetComponent<Text>();
        if (inputText.text == null)
        {
            // null input.
            return;
        }
		if(inputText.text == m_currentWord.Item.Answer)
		{
			correct = true;
		}
		else
		{
			m_currentWord.UpdateRecord(false);
		}

		Text judge = m_judgeText.GetComponent<Text> ();
		float alpha = 1.0f;
		if(correct)
		{
			judge.text = "正 确！";
			Color color = Color.green;
			color.a = alpha;
			judge.color = color;
		}
		else
		{
			judge.text = "错: " + m_currentWord.Item.Answer;
			Color color = Color.red;
			color.a = alpha;
			judge.color = color;
		}

        InputField field = m_inputPanel.GetComponentInChildren<InputField>();
        field.text = "";

        m_judgeText.SetActive(true);
		m_currentWord.UpdateRecord(correct);
		m_progress.ReBelance();
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

	public void OnClickChoice(int choice)
	{
		m_timerObject.SetActive(false);
		m_wordCoundown = 0.0f;

		bool correct = false;
		if(choice < 4 && m_currentWord.Item.Answer == m_choices[choice])
		{
			correct = true;
		}
		Text judge = m_judgeText.GetComponent<Text> ();
		float alpha = 1.0f;
		if(correct)
		{
			judge.text = "正 确！";
			Color color = Color.green;
			color.a = alpha;
			judge.color = color;
		}
		else
		{
			if(choice == 4)
			{
				judge.text = "不认识！" + m_currentWord.Item.Answer;
			}
			else
			{
				judge.text = "错 误！";
			}
			Color color = Color.red;
			color.a = alpha;
			judge.color = color;
		}

		int answerIdx = 0;
		for(int i = 0; i < m_choices.Count; i++)
		{
			if(m_currentWord.Item.Answer == m_choices[i])
			{
				answerIdx = i;
				break;
			}
		}

		int idx = 0;
		foreach(Transform child in m_choicePanel.transform)
		{
			Image btnimg = child.gameObject.GetComponent<Image>();
			if(idx == answerIdx)
			{
				btnimg.color = Color.yellow;
				break;
			}
			idx++;
		}

		m_alphaTime = m_alphaMaxTime;
		m_judgeText.SetActive(true);
		m_currentWord.UpdateRecord(correct);
		m_progress.ReBelance();
	}
}
