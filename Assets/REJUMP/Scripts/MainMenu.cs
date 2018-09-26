using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Music class;
[System.Serializable]
public class Music
{
    public Button musicButton;              //Music toggle button;
    public Sprite onIcon;                   //Music on image icon;
    public Sprite offIcon;                  //Misoc off image icon;
    public AudioSource ambientSource;       //Mucic audio source;
}

//Sounds class;
[System.Serializable]
public class Sounds
{
    public Button soundsButton;             //Sounds toggle button;
    public Sprite onIcon;                   //Sounds on image icon;
    public Sprite offIcon;                  //Sounds off image icon;
}

public class MainMenu : MonoBehaviour 
{
    public Button playButton;           //Start game button;
    public Button exitButton;           //Quit game button;
    public Text highscoreText;          //Menu highscore display text;
    public GameObject menuPanel;        //Menu panel object;
    public GameObject HUDPanel;         //Hud panel object;
    public AudioClip clickSFX;          //Click sound effect;
    public Music music;                 //Music class;
    public Sounds sounds;               //Sounds class;

//    private bool firstPlay;
    private AudioSource source;
    private Image soundImage, musicImage;
    private Tutorial tutorial;           //Tutorial clss;

	// Use this for initialization
	void Start () 
	{
        //Cache components;
        source = GetComponent<AudioSource>();
        soundImage = sounds.soundsButton.GetComponent<Image>();
        musicImage = music.musicButton.GetComponent<Image>();
        tutorial = GetComponent<Tutorial>();

        HUDPanel.SetActive(false);      //Disable hud panel;
        menuPanel.SetActive(true);      //Enable menu panel;

        //Assign buttons listeners;
        playButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(() =>
        {
            StartCoroutine(ExitGame());
        });
        sounds.soundsButton.onClick.AddListener(ToggleSound);
        music.musicButton.onClick.AddListener(ToggleMusic);

        //Load prefs;
        LoadHighscore();
        LoadAudioSettings();
	}

    //Start game function;
    void StartGame()
    {
        Game.isGameStarted = true;              //Set game started state to true;
        Game.PlaySound(source, clickSFX);       //Play click sound effect;
        HUDPanel.SetActive(true);               //Enable hud panel;
        menuPanel.SetActive(false);             //Disable menu panel;

        if (tutorial)
            tutorial.StartTutorial();
    }

    //Load highscore function;
    void LoadHighscore()
    {
        if (PlayerPrefs.HasKey("High"))
            highscoreText.text = "HIGHSCORE: " + PlayerPrefs.GetInt("High").ToString();
    }

    //Quit game function;
    IEnumerator ExitGame()
    {
        Game.PlaySound(source, clickSFX);   //Play click sound effect;
        SaveSettings();                     //Save settings;

        //Wait for click sound effect finished and quie application;
        if (source.isPlaying)
            yield return null;

        Debug.Log("Quit");
        Application.Quit();
    }

    //Toggle sound effects function;
    void ToggleSound()
    {
        Game.sounds = !Game.sounds;                                         //Toggle sounds bool;
        soundImage.sprite = Game.sounds ? sounds.onIcon : sounds.offIcon;   //Change sound button image sprite based on sounds state;
    }

    //Toggle music function;
    void ToggleMusic()
    {
        Game.music = !Game.music;                                           //Toggle music bool;
        musicImage.sprite = Game.music ? music.onIcon : music.offIcon;      //Change music button image sprite based on music state;
        //Pause or play music based on music state;
        if (Game.music)
            music.ambientSource.Play();
        else
            music.ambientSource.Pause();
    }

    //Load audio settings;
    void LoadAudioSettings()
    {
        if (PlayerPrefs.HasKey("Sounds"))
            Game.sounds = Game.GetBool("Sounds");
        soundImage.sprite = Game.sounds ? sounds.onIcon : sounds.offIcon;

        if (PlayerPrefs.HasKey("Music"))
            Game.music = Game.GetBool("Music");
        musicImage.sprite = Game.music ? music.onIcon : music.offIcon;

        if (Game.music)
            music.ambientSource.Play();
        else
            music.ambientSource.Pause();
    }

    //Save audio settings;
    void SaveSettings()
    {
        Game.SetBool("Sounds", Game.sounds);
        Game.SetBool("Music", Game.music);
    }
}