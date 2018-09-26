using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Game
{
    //Games counter, count game restarts;
    public static int gamesCount;

    //Game started state;
    public static bool isGameStarted;

    //Sounds state, responsible for ebable or disable ingame sounds;
    public static bool sounds = true;

    //Music state responsible for ebable or disable ambience music;
    public static bool music = true;

    //Play sound effect function; Plays target audio clip on target source with optional volume and pitch;
    public static void PlaySound(AudioSource source, AudioClip clip, float volume = 1, float pitch = 1)
    {
        //Do nothing if sounds state fals;
        if (!sounds)
            return;
        //Else play sound effect;
        source.volume = volume;
        source.pitch = pitch;
        source.clip = clip;
        source.Play();
    }

    //Player prefs bool implementation;
    public static void SetBool(string name, bool booleanValue)
    {
        PlayerPrefs.SetInt(name, booleanValue ? 1 : 0);
    }
    public static bool GetBool(string name)
    {
        return PlayerPrefs.GetInt(name) == 1 ? true : false;
    }
}