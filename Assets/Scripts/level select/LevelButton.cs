using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void LevelButtonCallback(int levelNumber);

public class LevelButton : MonoBehaviour {
	
	[SerializeField] 
	Text m_levelNameText;
	[SerializeField] 
	Text m_levelCompletedText;
	[SerializeField] 
	GameObject m_levelCompletedIcon;
	[SerializeField] 
	GameObject m_levelIcon;
	[SerializeField] 
	Button m_levelButton;

	int m_levelID;

	private LevelButtonCallback m_levelButtonCallback;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Init(int levelNumber, int levelID, string name, LevelButtonCallback callback, bool isActive)
	{
		m_levelNameText.text = "Level "+levelNumber.ToString() + " : " + name;
		m_levelID = levelID;
		SetSelectCallback(callback);
		m_levelButton.interactable = isActive;
	}

	public void PressedButton()
	{
		Debug.Log("PressedButton = "+m_levelID);
		m_levelButtonCallback(m_levelID);
	}

	public void SetCompletedCount(int completed, int outOf)
	{
		if (completed == outOf)
		{
			m_levelCompletedText.gameObject.SetActive(false);
			m_levelCompletedIcon.SetActive(true);
		}
		else
		{
			m_levelCompletedText.gameObject.SetActive(true);
			m_levelCompletedText.text = completed.ToString()+"/"+outOf.ToString();
			m_levelCompletedIcon.SetActive(false);
		}
	}

	public void HideCompletionCount(bool hide)
	{
		m_levelCompletedText.gameObject.SetActive(!hide);
		m_levelCompletedIcon.SetActive(!hide);
	}

	public void SetSelectCallback(LevelButtonCallback callback)
	{
		m_levelButtonCallback = callback;
	}

	public void SetContentAlpha(float alpha)
	{
		Color c = m_levelCompletedText.color;
		c.a = alpha;
		m_levelCompletedText.color = c;

		c = m_levelNameText.color;
		c.a = alpha;
		m_levelNameText.color = c;

		c = m_levelCompletedIcon.GetComponent<RawImage>().color;
		c.a = alpha;
		m_levelCompletedIcon.GetComponent<RawImage>().color = c;

		c = m_levelIcon.GetComponent<Image>().color;
		c.a = alpha;
		m_levelIcon.GetComponent<Image>().color = c;
	}
}
