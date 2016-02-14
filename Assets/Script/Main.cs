using UnityEngine;
using System.Collections;
using System.IO;

public class Main : MonoBehaviour {

	public string m_FileName = "word.dat";
	public string m_RootPath = @"BookDict\";

	private Progress m_progress;
	private BookDict m_book;

	public GameObject m_panelMain;
	public GameObject m_panelSetup;

	// Use this for initialization
	void Start () {
		m_progress = new Progress(m_RootPath);
		m_progress.Load(m_RootPath);
		m_book = new BookDict (m_RootPath, "Default");
		m_book.ReadBook (m_progress);
		m_book.SaveBook();
		m_progress.ReOrganize();

		m_panelMain = GameObject.Find ("PanelMain");
		PanelMain panelMainScript = (PanelMain)m_panelMain.GetComponent (typeof(PanelMain));
		panelMainScript.BookProgress = m_progress;
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public void OnClickBack()
	{
		m_panelMain.SetActive(false);
		m_panelSetup.SetActive(true);
	}

	void OnApplicationQuit()
	{
		m_progress.Save(m_RootPath);
	}
}
