using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour
{
    public GameObject tutorialPanel;        //Tutorial root panel;
    public GameObject jumpTooltip;          //Jump tooltip panel;
    public GameObject chillTooltip;         //Chill tooltip panel;
    public bool enableTutorial = true;

    private bool firstPlay;

	// Use this for initialization
	void Start ()
    {
        //Load first game start key to know if game was already played to not showing tutorial every game start;
        firstPlay = PlayerPrefs.HasKey("played");
	}
	
	// Update is called once per frame
	public void StartTutorial ()
    {
        //Show tutorial if game started firste time;
        if (!firstPlay && enableTutorial)
            StartCoroutine(ShowTutorial());
        else
            tutorialPanel.SetActive(false);
	}

    //Show first tutorial tooltip;
    IEnumerator ShowTutorial()
    {
        yield return new WaitForSeconds(0.75F);     //Wait a little;
        Time.timeScale = 0.0001F;                   //Freeze time;
        jumpTooltip.SetActive(true);       //Show first tooltip;
    }

    //Show second tutorial tooltip. This function is assigned as OnPointerClick event on first tooltip rect;
    public void ProceedTutorial()
    {
        jumpTooltip.SetActive(false);  //Disable first tooltip;
        chillTooltip.SetActive(true);  //Enable second tooltip;
    }

    //End tutorial. This function is assigned as OnPointerClick event on second tooltip rect;
    public void EndTutorial()
    {
        //disable tooltips objects;
        chillTooltip.SetActive(false);
        tutorialPanel.SetActive(false);
        //Unfreeze time;
        Time.timeScale = 1;
        //Set game as played;
        PlayerPrefs.SetString("played", "");
    }
}
