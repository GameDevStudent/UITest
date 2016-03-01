using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Main : MonoBehaviour {

	public static Main instance;

	public string m_RootPath = @"BookDict\";

	private Progress m_progress;
	private ProgressContainer m_availableProgress;

	public GameObject m_panelMain;
	public GameObject m_panelSetup;
	public GameObject m_panelNewProfile;
	public GameObject m_panelEditBook;

	public GameObject[] m_panelRef;

	public GameObject m_BookList;
	ProfileUpdate m_profileSetup;

	public Main()
	{
		instance = this;
		m_availableProgress = new ProgressContainer();
		m_panelRef = new GameObject[4];
	}

	void SwitchToPanel(GameObject panel)
	{
		foreach(GameObject obj in m_panelRef)
		{
			if(obj == panel)
			{
				obj.SetActive(true);
			}
			else
			{
				obj.SetActive(false);
			}
		}
	}

	public bool IsProfileValid(CheckProfile check)
	{
		return !m_availableProgress.AccessProfiles(check);
	}

	// Use this for initialization
	void Start () {
		m_panelRef[0] = m_panelMain;
		m_panelRef[1] = m_panelSetup;
		m_panelRef[2] = m_panelNewProfile;
		m_panelRef[3] = m_panelEditBook;

		SelectBook script = (SelectBook)m_BookList.GetComponent (typeof(SelectBook));
		script.CollectBooks(m_RootPath, m_BookList);
		m_availableProgress.RecollectProgress(m_RootPath);

		foreach(Transform child in m_panelSetup.transform)
		{
			if(child.gameObject.name == "ListBox")
			{
				m_profileSetup = (ProfileUpdate)child.gameObject.GetComponent (typeof(ProfileUpdate));
			}
		}
		m_profileSetup.InitProfiles(m_availableProgress);

		SwitchToPanel(m_panelSetup);
	}

	public void OpenProgress(string id, List<string> books)
	{
		m_progress = new Progress(m_RootPath);
		m_progress.ProgressID = id;
		foreach(string book in books)
		{
			m_progress.AddBook(book);
		}
		m_progress.Load(m_RootPath);

		PanelMain panelMainScript = (PanelMain)m_panelMain.GetComponent (typeof(PanelMain));
		panelMainScript.BookProgress = m_progress;
		SwitchToPanel(m_panelMain);
	}

	public void OnNewProfile()
	{
		SwitchToPanel(m_panelNewProfile);
	}

	public void CreateNewProfile(string id, List<string> books)
	{
		Progress progress = new Progress(m_RootPath);
		progress.ProgressID = id;
		foreach(string book in books)
		{
			progress.AddBook(book);
		}
		progress.Save(m_RootPath);

		m_availableProgress.RecollectProgress(m_RootPath);
		m_profileSetup.InitProfiles(m_availableProgress);

		SwitchToPanel(m_panelSetup);
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public void EditBook()
	{
		SwitchToPanel(m_panelEditBook);
	}

	public void OnClickBack()
	{
		m_progress.Save(m_RootPath);
		m_progress = null;

		SwitchToPanel(m_panelSetup);

		PanelMain panelMainScript = (PanelMain)m_panelMain.GetComponent (typeof(PanelMain));
		panelMainScript.ClearText();
	}

	void OnApplicationQuit()
	{
		if(m_progress != null)
		{
			m_progress.Save(m_RootPath);
		}
	}

	public void QuitApp()
	{
		Application.Quit();
	}
}
