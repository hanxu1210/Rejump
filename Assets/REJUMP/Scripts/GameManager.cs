using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Force scale class
[System.Serializable]
public class ForceScale
{
    public Image filledImage;               //Force scale image (filled image);
    public float growSpeed = 0.5F;          //Force scale grow speed;
    public Colors[] colors = new Colors[4]; //Force scale colors for 0.25, 0.50, 0.75 and 1 values. Length shouldn't be higher than 4;
}

[System.Serializable]
public class Colors
{
    public float scaleValue;                //Scale value;
    public Color color = Color.white;       //Color for that value;     
}

[RequireComponent(typeof(LevelGenerator))]
[RequireComponent(typeof(HUD))]
public class GameManager : MonoBehaviour 
{
    public ForceScale forceScale;       //Jump furce scale;
    public bool enableAutoJump = true;

    private Player player;                   
    private float time;
    private float power;
    private Color targetColor;
    private LevelGenerator levelGen;
    private HUD hud;
    private PlayerCamera playerCam;
    private bool chill;

	// Use this for initialization
	void Start () 
	{
        //Cache components;
        levelGen = GetComponent<LevelGenerator>();
        hud = GetComponent<HUD>();
        playerCam = FindObjectOfType<PlayerCamera>();
        player = FindObjectOfType<Player>();

        //Set target color to first from array;
        targetColor = forceScale.colors[0].color;
	}
	
	// Update is called once per frame
	void Update ()
	{
        //If game is not started do nothing;
        if (!Game.isGameStarted)
            return;
        //else:
        forceScale.filledImage.enabled = player.IsGrounded();       //Enable force scale only if player is grounded;

        //Also if player is grounded:
        if (player.IsGrounded())
            time += forceScale.growSpeed * Time.deltaTime;  //increse time based on grow speed;
        else
            time = 0;                                       //Null time;                              

        //Change power with time speed;
        power = Mathf.PingPong(time, 1);
        //Fill forse scale image based on powed;
        forceScale.filledImage.fillAmount = power;

        //if power is 100% and player's current block is not a chill block, make it jump;
        if (power > 0.99F && !chill && enableAutoJump)
            player.Jump();

        //Change target color, based on power value;
        if (power < forceScale.colors[0].scaleValue)
            targetColor = forceScale.colors[0].color;
        else if (power > forceScale.colors[0].scaleValue && power < forceScale.colors[1].scaleValue)
            targetColor = forceScale.colors[1].color;
        else if (power > forceScale.colors[1].scaleValue && power < forceScale.colors[2].scaleValue)
            targetColor = forceScale.colors[2].color;
        else
            targetColor = forceScale.colors[3].color;

        //Lerp force scale image color to target color;
        forceScale.filledImage.color = Color.Lerp(forceScale.filledImage.color, targetColor, 4.5F * Time.deltaTime);
	}

    //Add air coins function caller;
    public void AddCollectedCoins(int value)
    {
        hud.AddAirCoins(value);
    }

    //Add coin function caller;
    public void AddCoin(int value, bool reward = false)
    {
        hud.AddCoin(value, reward);
    }

    //Add score function caller;
    public void AddScore()
    {
        hud.AddScore(1);
    }

    //Reset force scale function;
    public void ResetForceScale()
    {
        power = 0;  //null power value;
        time = 0;   //null time value;
    }

    //Restart function;
    public void Restart()
    {
        levelGen.PlaceBlocks();             //Call place blocks function to replace blocks;
        ResetScore();                       //Reset score value;
        playerCam.ResetCameraPosition();    //Reset camera position;
    }

    //Reset score caller;
    void ResetScore()
    {
        hud.ResetScore();
    }

    //Power accesor;
    public float Power()
    {
        return power;
    }

    //Check block caller;
    public void CheckBlock(Collider2D block)
    {
        chill = levelGen.GetBlockStatus(block);
    }

    //Restart player caller;
    public void ResetPlayer()
    {
        player.Restart();
    }
}