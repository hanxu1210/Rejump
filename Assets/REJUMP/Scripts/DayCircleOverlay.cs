using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Day circle class;
[System.Serializable]
public class DayCircle
{
    public string timeName;                         //Name of day time (optional);
    public Color lightColor = Color.white;          //Day time color overlay;
    public float transitionSpeed = 1;               //Color transition speed;
    public float duration = 10;                     //Color duration;
}

public class DayCircleOverlay : MonoBehaviour 
{
    public SpriteRenderer overlaySprite;                //Background overlay sprite renderer;
    public DayCircle[] dayCircle = new DayCircle[4];    //Day circle array;

    private int lightIndex;
    private float time;

	// Use this for initialization
	void Start () 
	{
        time = dayCircle[0].duration;
	}
	
	// Update is called once per frame
	void Update ()
	{
        //Change day circle index based on its duration;
        if (Time.time > time)
        {
            if (lightIndex < dayCircle.Length - 1)
            {
                lightIndex++;
                time = Time.time + dayCircle[lightIndex].duration;
            }
            else
            {
                lightIndex = 0;
                time = Time.time + dayCircle[lightIndex].duration;
            }
        }

        //Change overlay sprite color to current day circle index;
        overlaySprite.color = Color.Lerp(overlaySprite.color, dayCircle[lightIndex].lightColor, 
                                                  dayCircle[lightIndex].transitionSpeed / 10 * Time.deltaTime);
	}
}