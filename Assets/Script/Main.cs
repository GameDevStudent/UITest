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

	public GameObject m_BookList;
	ProfileUpdate m_profileSetup;

	public Main()
	{
		instance = this;
		m_availableProgress = new ProgressContainer();
	}

	public bool IsProfileValid(CheckProfile check)
	{
		return !m_availableProgress.AccessProfiles(check);
	}

	// Use this for initialization
	void Start () {
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

		m_panelNewProfile.SetActive(false);
		m_panelMain.SetActive(false);
		m_panelSetup.SetActive(true);
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

		m_panelNewProfile.SetActive(false);
		m_panelMain.SetActive(true);
		m_panelSetup.SetActive(false);
	}

	public void OnNewProfile()
	{
		m_panelNewProfile.SetActive(true);
		m_panelMain.SetActive(false);
		m_panelSetup.SetActive(false);
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
		m_panelNewProfile.SetActive(false);
		m_panelMain.SetActive(false);
		m_panelSetup.SetActive(true);
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public void OnClickBack()
	{
		m_progress.Save(m_RootPath);
		m_progress = null;

		m_panelMain.SetActive(false);
		m_panelSetup.SetActive(true);

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
