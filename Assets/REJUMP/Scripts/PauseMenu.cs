using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Characters shop class;
[System.Serializable]
public class Shop
{
    public SpriteRenderer playerSprite;                 //Player sprite renderer component;
    public Text shopCoinsText;                          //Coins display text  in shop panel;
    public ButtonIcons unlockButtonIcons;               //Unlock button icons;
    public AudioClip unlockSFX;                         //Unlock sound effect;
    public Characters[] characters = new Characters[7]; //Shop characters array;
}

//Character class
[System.Serializable]
public class Characters
{
    public string characterName = "Name";               //Unique character name;
    public Sprite characterSprite;                      //Character sprite;
    public Sprite coinSprite;                           //Coin sprite for this character;
    public int characterPrice = 100;                    //Character price;
    public GameObject priceObject;                      //Price gameObject;
    public Text priceText;                              //Price text;
    public Button unlockButton;                         //Unlock button;
    public Image buttonIcon;                            //Unlock button image component;
    public bool unlocked;                               //Character unlock state;
    public bool selected;                               //Character selected state;
}

//Unlock button icons class;
[System.Serializable]
public class ButtonIcons
{
    public Sprite unlocked;                             //Ulocked icon sprite;
    public Sprite selected;                             //Selected icon sprite;
}

[RequireComponent(typeof(HUD))]
[RequireComponent(typeof(GameManager))]
public class PauseMenu : MonoBehaviour 
{
    public Button pauseButton;                      //Game pause button;
    public Button continueButton;                   //Game continue button;
    public Button exitButton;                       //Exit game button;

    public GameObject menuPanel;                    //Menu panel, menu ui elements root;
    public GameObject HUDPanel;                     //Hud panel, game ui elements root;
    public GameObject pausePanel;                   //Pause panel, pause menu ui elements root;

    public AudioClip clickSFX;                      //Button click sound effect;
    public Shop shop;                               //Shop settings;
    public SpriteRenderer[] coinsPoolSprites;       //Coins pool sprite renderers;

    private HUD hud;
    private GameManager gm;
    private AudioSource source;
	// Use this for initialization

	void Start () 
	{
        //Cache components;
        source = GetComponent<AudioSource>();
        hud = GetComponent<HUD>();
        gm = GetComponent<GameManager>();


        pausePanel.SetActive(false);    //Disable pause panel;
        SetUpListeners();               //Setup buttons listeners;             
        LoadShop();                     //Load shop prefs;
	}

    //Setup buttons listeners function;
    void SetUpListeners()
    {
        pauseButton.onClick.AddListener(Pause);         //Call pause function;
        continueButton.onClick.AddListener(Continue);   //Call continue function;
        exitButton.onClick.AddListener(()=>             //Call manu function;
        {
            StartCoroutine(GoToMenu());
        });

        //Add listeners for characters unlock buttons to call index based BuyCharacter function;
        for (int i = 0; i < shop.characters.Length; i++)
        {
            int index = i;
            shop.characters[index].unlockButton.onClick.AddListener(() =>
            {
                BuyCharacter(index);
            });
        }
    }

    //Pause game function;
    void Pause()
    {
        Game.PlaySound(source, clickSFX);                           //Play click sound effect;
        Time.timeScale = 0.00001F;                                  //Freeze time;
        HUDPanel.SetActive(false);                                  //Disable hud panel;
        pausePanel.SetActive(true);                                 //Enable pause panel;
        shop.shopCoinsText.text = hud.CoinsCount().ToString();    //Update shop coins display text;
    }

    //Continue game function;
    void Continue()
    {
        Game.PlaySound(source, clickSFX);                           //Play click sound effect;
        SaveCharacters();                                           //Save characters;
        pausePanel.SetActive(false);                                //Disable pause panel;
        Time.timeScale = 1;                                         //Unfreeze time;
        HUDPanel.SetActive(true);                                   //Enable hud panel;
    }

    //Buy charater function;
    void BuyCharacter(int index)
    {
        //If character is not unlocked and we have enough coins:
        if (!shop.characters[index].unlocked && hud.CoinsCount() >= shop.characters[index].characterPrice)
        {
            shop.characters[index].unlocked = true;                                         //Set character as unlocked;
            Game.PlaySound(source, shop.unlockSFX);                                         //Play unlock sound effect;
            shop.characters[index].buttonIcon.sprite = shop.unlockButtonIcons.unlocked;     //Change button sprite to unlocked;
            shop.characters[index].priceObject.SetActive(false);                            //Disable price display object;
            hud.DecreaseCoinsCount(shop.characters[index].characterPrice);                  //Decrease coins count by cost amount;
            shop.shopCoinsText.text = hud.CoinsCount().ToString();                        //Update shop coins display text;
        }
        //Else if character is unlocked but not selected:
        else if (shop.characters[index].unlocked && !shop.characters[index].selected)
        {
            shop.characters[index].selected = true;                                         //Set character as selected;                            
            Game.PlaySound(source, clickSFX);                                               //Play click sound effect;
            shop.characters[index].buttonIcon.sprite = shop.unlockButtonIcons.selected;     //Change button sprite to selected;
            shop.characters[index].unlockButton.interactable = false;                       //Set unlock button to not interactable;
            shop.playerSprite.sprite = shop.characters[index].characterSprite;              //Change player sprite to selected character sprite;
            SetCoinSprite(shop.characters[index].coinSprite);                               //Set coins scprites to selected character coin sprite;

            //Unselect any other selected charater;
            for (int i = 0; i < shop.characters.Length; i++)
            {
                if (i != index && shop.characters[i].selected)
                {
                    shop.characters[i].selected = false;                                    //Set character as not selected;
                    shop.characters[i].unlockButton.interactable = true;                    //Set unlock button to interactable;
                    shop.characters[i].buttonIcon.sprite = shop.unlockButtonIcons.unlocked; //Set unlock button sprite to unlocked;
                }
            }
        }
    }

    //Save characters state to player prefs;
    void SaveCharacters()
    {
        for (int i = 0; i < shop.characters.Length; i++)
        {
            Game.SetBool("Unlocked" + shop.characters[i].characterName, shop.characters[i].unlocked);
            Game.SetBool("Selected" + shop.characters[i].characterName, shop.characters[i].selected);
        }
    }

    //Load shop from player prefs;
    void LoadShop()
    {
        for (int i = 0; i < shop.characters.Length; i++)
        {
            if (PlayerPrefs.HasKey("Unlocked" + shop.characters[i].characterName))
                shop.characters[i].unlocked = Game.GetBool("Unlocked" + shop.characters[i].characterName);
            if (PlayerPrefs.HasKey("Selected" + shop.characters[i].characterName))
                shop.characters[i].selected = Game.GetBool("Selected" + shop.characters[i].characterName);
        }

        //Setup shop after prefs loaded;
        SetUpShop();
    }

    //Setup shop function;
    void SetUpShop()
    {
        //After shop prefs loaded, setting up characters based on it;
        for (int i = 0; i < shop.characters.Length; i++)
        {
            //Assign price values;
            shop.characters[i].priceText.text = shop.characters[i].characterPrice.ToString();
            //If character unlocked, disable its price display object and assign unlocked icon to its unlock button;
            if (shop.characters[i].unlocked)
            {
                shop.characters[i].priceObject.SetActive(false);
                shop.characters[i].buttonIcon.sprite = shop.unlockButtonIcons.unlocked;
            }
            //If character selected:
            if (shop.characters[i].selected)
            {
                shop.characters[i].unlockButton.interactable = false;                   //Set its unlock button interactable to false;
                shop.characters[i].buttonIcon.sprite = shop.unlockButtonIcons.selected; //Set its unlock button to selected; 
                shop.playerSprite.sprite = shop.characters[i].characterSprite;          //Set player sprite to this selected character sprite;
                SetCoinSprite(shop.characters[i].coinSprite);                           //Set coins sprites ti this selected character coin sprite;
                shop.characters[i].unlockButton.interactable = false;                   //Set unlock button to not interactable;
            }
        }
    }

    //Set coins sprite;
    void SetCoinSprite(Sprite sprite)
    {
        for (int i = 0; i < coinsPoolSprites.Length; i++)
        {
            coinsPoolSprites[i].sprite = sprite;
        }
    }

    //Go to menu function;
    IEnumerator GoToMenu()
    {
        //Play click sound effect;
        Game.PlaySound(source, clickSFX);

        //Wait while click sound effect is playing
        if (source.isPlaying)
            yield return null;

        pausePanel.SetActive(false);    //Disable pause panel object;
        menuPanel.SetActive(true);      //Enable menu panel object;
        Game.isGameStarted = false;     //Set game started state to false;
        gm.ResetPlayer();               //Reset player;
        gm.Restart();                   //Restart game maneger;
        Time.timeScale = 1;             //Unfreze time;
    }
}