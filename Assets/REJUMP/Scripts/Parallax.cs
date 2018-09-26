using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour 
{
    public Layers[] layers = new Layers[1];     //Paralax layers array;

    private Transform camT;
    private Vector3 origin;
    private float camOffset;

	// Use this for initialization
	void Start () 
    {
        //Cache camera transform;
        camT = Camera.main.transform;

        //Cache camera start position;
        origin = camT.position;

        //Set background layers sorting order;
        for (int i = 0; i < layers.Length; i++)
            layers[i].background.sortingOrder = layers[i].sortingOrder;
	}
	
	// Update is called once per frame
	void Update () 
    {
        //Calculate camera offset based on its start and current pistion;
        camOffset = (origin - camT.position).sqrMagnitude / 100;

        //Change background layers texture offset based on camera offset and its scroll speed;
        for (int i = 0; i < layers.Length; i++)
            layers[i].background.material.mainTextureOffset = new Vector2(camOffset * layers[i].scrollSpeed / 100, 0);
	}
}

//Background layers class;
[System.Serializable]
public class Layers
{
    public MeshRenderer background;         //Layer mesh renderer;
    public float scrollSpeed = 10;          //Layer scroll speed;
    public int sortingOrder;                //Layer sorting order;
}
