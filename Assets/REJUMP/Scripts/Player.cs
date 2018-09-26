using UnityEngine;
using UnityEngine.UI;
using System.Collections;


//Player sounds class;
[System.Serializable]
public class SoundEffects
{
    public AudioClip jumpSFX;           //Jump sound effect;
    public AudioClip landSFX;           //Land sound effect;
    public AudioClip deathSFX;          //Death sound effect;
    public AudioClip collectSFX;        //Collect sound effect;
}

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour 
{
    public SpriteRenderer graphics;                         //Player SpriteRenderer;
    public float gravityScale = 2.65F;                      //Player gravity scale;
    public Vector3 jumpForce = new Vector3(75, 95, 0);      //Maximum player jump force;
    public float respawnDelay = 0.5F;                       //Player respawn delay;
    public SoundEffects soundEffects;                       //Sound effects;

    private bool isGrounded;
    private int airCoins;
    private Rigidbody2D rb2d;
    private GameManager gm;
    private Vector3 spawnPosition;
    private Transform thisT;
    private Collider2D curBlock;
    private AudioSource[] sources = new AudioSource[2];

    void Awake()
    {
        //Set player tag;
        gameObject.tag = "Player";
    }

	// Use this for initialization
	void Start () 
	{
        //Cache components;
        thisT = transform;
        rb2d = GetComponent<Rigidbody2D>();
        gm = FindObjectOfType<GameManager>();
        for (int i = 0; i < sources.Length; i++)
            sources[i] = gameObject.AddComponent<AudioSource>();

        //Disable player rigidbody and sprite renderer
        rb2d.isKinematic = true;
        graphics.enabled = false;

        //Set player gravity scale;
        rb2d.gravityScale = gravityScale;
        //Set player spawn position to it's start position;
        spawnPosition = thisT.position;
        //Wait for game starting;
        StartCoroutine(WaitForStart());
	}

    IEnumerator WaitForStart()
    {
        //Do nothing if game is nor started;
        while (!Game.isGameStarted)
            yield return null;
        //If game has started, enable player rigidbody and sprite renderer;
        rb2d.isKinematic = false;
        graphics.enabled = true;
    }

    private void Update() {
        if (Input.GetKeyDown (KeyCode.JoystickButton0))
        {
            Jump();
        }
    }
    //Jump function, this function is asigned as OnPointerClick event on JumpPad rect under HUD panel;
    public void Jump()
    {
        //Do nothing if player is not grounded and is moving;
        if (!IsGrounded())
            return;
        //Else
        rb2d.AddForce(jumpForce * gm.Power() * 10);             //Add jump force to player rigidbody;       
        isGrounded = false;                                     //Set grounded flag to false;
        curBlock.enabled = false;                               //Disable current block collider so player cant jump on same block twice;
        Game.PlaySound(sources[0], soundEffects.jumpSFX, 0.5F); //Play jump sound effect with desired volume (optional);
    }

    //Check if player is grounded and not moving; Also because we checking isGrounded based on OnCollisionEnter function
    //we also check if player position is higher then block position;
    public bool IsGrounded()
    {
        return isGrounded && rb2d.velocity.magnitude <= 0.01F && rb2d.angularVelocity < 0.05F &&
            curBlock != null && thisT.position.y > curBlock.bounds.max.y;
    }

    //On entering trigger:
    void OnTriggerEnter2D(Collider2D col)
    {
        //If entering dead zone trigger
        if (col.CompareTag("DeathZone"))
        {
            Game.PlaySound(sources[0], soundEffects.deathSFX, 0.35F, 0.8F);     //Play death sound effect with desired volume and pitch (optional);
            StartCoroutine(Respawn());                                          //Start respawn;
            rb2d.velocity = Vector3.zero;                                       //Reset velocity;
            airCoins = 0;                                                       //Null collected in air coins;
            curBlock = null;                                                    //Null current block;
            Game.gamesCount++;                          //Increase games counter (this is static variable, so you can acces it from any script if you need);
            AdsController.Instance.RequireAd();         //Check if ad can be played;
        }
        //If entring coin trigger
        else if (col.CompareTag("Coin"))
        {
            //If player is in air
            if (!isGrounded)
                airCoins += 1;      //Add air coin (air coins are coins we have collected in air, this coins will be added to coins counter if player will land succesfull.
            else
                gm.AddCoin(1);      //Add coin;

            Game.PlaySound(sources[1], soundEffects.collectSFX);  //Player collect sound effect;
            //Hide coin object;
            col.gameObject.SetActive(false);
        }
        //The third and last trigger in game is score up trigger (part of every block object), when player entering this block we:
        else
        {
            gm.AddScore();                          //Adding score;
            gm.AddCollectedCoins(airCoins);         //Adding coins, collected in air to coins counter;
            airCoins = 0;                           //Null collected in air coins;
        }
    }

    //On collision:
    void OnCollisionEnter2D(Collision2D col)
    {
        rb2d.freezeRotation = false;                            //Enable rigidbody rotation;
        isGrounded = true;                                      //Set grounded flag to true;

        //To make some things to be done once, we checking if the current block is not a collision block,
        if (curBlock != col.collider)
        {
            //and if not:
            Game.PlaySound(sources[0], soundEffects.landSFX);   //Play land sound effect;
            gm.ResetForceScale();                               //Reset jump force scale;
            curBlock = col.collider;                            //Set collision block as current block;
            gm.CheckBlock(curBlock);                            //Check if current block is chill block
        }
    }

    //Set grounded flag based on collision stay;
    void OnCollisionStay2D()
    {
        isGrounded = true;
    }

    //Set grounded flag to false if player leaves scoreup trigger (because if he is, he might already falling);
    void OnTriggerExit2D()
    {
        isGrounded = false;
    }

    //Respawn function:
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);  //Wait for respawn delay;
        rb2d.velocity = Vector3.zero;                   //Set velocity to zero;
        thisT.eulerAngles = Vector3.zero;               //Reset transform rotation;
        thisT.position = spawnPosition;                 //Set transform position to start position;
        rb2d.freezeRotation = true;                     //Disable rigidbody rotation;
        gm.Restart();                                   //Restart GameManager;
        gm.ResetForceScale();                           //Reset jump power force scale;
    }

    //Restart function (almost same as respawn, we call this function when returning to Main Menu, from pause screen);
    public void Restart()
    {
        rb2d.velocity = Vector3.zero;           //Set velocity to zero;
        thisT.eulerAngles = Vector3.zero;       //Reset transform rotation;
        thisT.position = spawnPosition;         //Set transform position to start position;
        rb2d.freezeRotation = true;             //Disable rigidbody rotation;
        rb2d.isKinematic = true;                //Disable rigidbody;
        graphics.enabled = false;               //Hide player graphics;
        gm.Restart();                           //Restart GameManager;
        gm.ResetForceScale();                   //Reset jump power force scale;
        StartCoroutine(WaitForStart());         //Wait for game start;
    }
}