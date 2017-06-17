using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProfileUpdate : MonoBehaviour {

	public GameObject m_profilePrefab;

	void DestroyAllTabs()
	{
		foreach (Transform child in gameObject.transform)
		{
			GameObject.Destroy(child.gameObject);
		}
	}

	public bool AddProfile(string book)
	{
		GameObject newprofile = Instantiate(m_profilePrefab);
		ProfileButtonController script = (ProfileButtonController)newprofile.GetComponent (typeof(ProfileButtonController));
		script.SetButtonText(book, false);
		//script.AddBook("Default");
		newprofile.transform.SetParent(gameObject.transform, false);
		return false;
	}

	public void InitProfiles(ProgressContainer container)
	{
		DestroyAllTabs();
        /*
		GameObject newprofile = Instantiate(m_profilePrefab);
		ProfileButtonController script = (ProfileButtonController)newprofile.GetComponent (typeof(ProfileButtonController));
		script.SetButtonText("新进度", true);
		//script.AddBook("Default");
		newprofile.transform.SetParent(gameObject.transform, false);
        */
		container.AccessProfiles(AddProfile);
	}

	// Use this for initialization
	void Start () {
		/*
		GameObject newprofile = Instantiate(m_profilePrefab);
		ProfileButtonController script = (ProfileButtonController)newprofile.GetComponent (typeof(ProfileButtonController));
		script.SetButtonText("中文字");
		script.AddBook("Default");
		newprofile.transform.SetParent(gameObject.transform, false);

		newprofile = Instantiate(m_profilePrefab);
		script = (ProfileButtonController)newprofile.GetComponent (typeof(ProfileButtonController));
		script.SetButtonText("九九表");
		script.AddBook("9x9");
		newprofile.transform.SetParent(gameObject.transform, false);
		*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
