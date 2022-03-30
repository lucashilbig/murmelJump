using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    private static float _scaleFactor;//scaling factor for resolution
    public static float ScaleFactor() => _scaleFactor;

    private static AspectRatio _aspectRatio;
    public static AspectRatio AspectRatio() => _aspectRatio;

    public static Characters character;
    
    void Start ()
    {
        character = (Characters)PlayerPrefs.GetInt("character");
        var aspect = Camera.main.aspect;
        if (aspect <= 0.48f)
        {
            _aspectRatio = global::AspectRatio.NineTwenty;
            _scaleFactor = 0.8f;
        }
        else if (aspect > 0.48f && aspect <= 0.52f)
        {
            _aspectRatio = global::AspectRatio.NineEighteen;
            _scaleFactor = 0.89f;
        }
        else if (aspect > 0.52f)
        {
            _aspectRatio = global::AspectRatio.NineSixteen;
            _scaleFactor = 1.0f;
        }
        SceneManager.LoadSceneAsync(1); //StartScene
    }

}

public enum AspectRatio
{
    NineSixteen,
    NineEighteen,
    NineTwenty
}

public enum Characters
{
    Murmel,
    Tabsi,
    Filu
}
