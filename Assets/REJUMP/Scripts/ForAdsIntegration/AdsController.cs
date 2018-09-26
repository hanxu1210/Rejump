using UnityEngine;
using System.Collections;
/// <summary>
/// This is example script on how to handle Ads.
/// </summary>
public class AdsController : MonoBehaviour {

    // Static singleton property;
    public static AdsController Instance { get; private set; }

    // Sungleton inplementation;
    void Awake()
    {
        // First we check if there are any other instances conflicting;
        if (Instance != null && Instance != this)
        {
            // If that is the case, we destroy other instances;
            Destroy(gameObject);
        }

        // Here we save our singleton instance;
        Instance = this;

        // Furthermore we make sure that we don't destroy between scenes (this is optional);
        DontDestroyOnLoad(gameObject);
    }

//==============================================================================================\\


    //How often Ad should be shown (every 15th death by default);
    [Header("Show ad every assigned loose count")]
    public int adRange = 15;
    [Header("Coins to add for rewarded video Ad")]
    public int reward = 25;

    private GameManager gameManager;

    //Cache game manager component to acces AddCoin method for rewarded video ad.
    public void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    //Check if the games count equals to specified  looseCount; This function calls every Player death;
    public void RequireAd()
    {
        if (Game.gamesCount > 0 && Game.gamesCount % adRange == 0)
            ShowVideoAd();
    }

    //Show regular Ad function;
    void ShowVideoAd()
    {
        Debug.Log("Show video Ad");
    }

    //Show rewarded Ad function; Can be assigned to a Button or called from any script as AdsController.Instance.ShowRewardedAd();
    public void ShowRewardedAd()
    {
        Debug.Log("Show rewarded Ad");
    }

    //Use this function as on complete callback for rewarded Ad.
    void RewardedAdCallback()
    {
        //Increase coins count;
        gameManager.AddCoin(reward, true);
    }
}