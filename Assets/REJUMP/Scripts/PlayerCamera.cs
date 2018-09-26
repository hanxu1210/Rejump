using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerCamera : MonoBehaviour 
{
    public Transform Player;
    public bool follow = true;
    public float offset;
    public float smoothDamp = 5;

    private Vector3 defaultPos;
    private Vector3 followPos;
    private Transform thisT;

    void Awake()
    {
        gameObject.tag = "MainCamera";
    }

	// Use this for initialization
    void Start()
    {
        thisT = transform;
        thisT.position = new Vector3(Player.position.x + offset, thisT.position.y, thisT.position.z);
        defaultPos = thisT.position;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
	{
        if (!follow)
            return;

        followPos = new Vector3(Player.position.x + offset, thisT.position.y, thisT.position.z);
        thisT.position = Vector3.Lerp(thisT.position, followPos, smoothDamp * Time.deltaTime);
	}

    public void ResetCameraPosition()
    {
        thisT.position = defaultPos;
    }
}