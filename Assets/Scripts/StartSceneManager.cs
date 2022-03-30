using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    public TextMeshProUGUI highscore;
    public GameObject christmasBall;
    public GameObject snowGenerator;
    public GameObject murmel;
    public GameObject tabsi;
    public GameObject filu;

    private bool isSnowing;
    private GameObject activeChar;
    
    // Start is called before the first frame update
    void Start()
    {
        var scaler = GameObject.Find("Scaler");
        if (scaler != null)
            scaler.transform.localScale = new Vector3(SceneSelector.ScaleFactor(),
                SceneSelector.ScaleFactor(), SceneSelector.ScaleFactor());
        
        SetActiveCharacter();
        isSnowing = false;
        snowGenerator.GetComponent<ParticleSystem>().Stop();
        activeChar.transform.GetChild(0).gameObject.SetActive(false);//disable christmas hat
        highscore.text = PlayerPrefs.GetInt("highscore").ToString();
        StartCoroutine(PulseTMP(highscore, 1.2f));
        StartCoroutine(PulseGameObject(christmasBall, 1.4f));
    }


    public void OnPressPlay(Button button)
    {
        button.interactable = false;
        SceneManager.LoadSceneAsync(2);//Game Scene
    }

    public void OnCharacterButtonClick(Button button)
    {
        StartCoroutine(NextCharacter(button));
    }

    public void OnChristmasBallClick()
    {
        StartCoroutine(ActivateSnow());
    }
    
    private void SetActiveCharacter()
    {
        switch (SceneSelector.character)
        {
            case Characters.Murmel:
                murmel.SetActive(true);
                tabsi.SetActive(false);
                filu.SetActive(false);
                activeChar = murmel;
                break;
            case Characters.Tabsi:
                tabsi.SetActive(true);
                murmel.SetActive(false);
                filu.SetActive(false);
                activeChar = tabsi;
                break;
            case Characters.Filu:
                filu.SetActive(true);
                murmel.SetActive(false);
                tabsi.SetActive(false);
                activeChar = filu;
                break;
            default:
                murmel.SetActive(true);
                tabsi.SetActive(false);
                filu.SetActive(false);
                activeChar = murmel;
                break;
        }
        
        activeChar.transform.GetChild(0).gameObject.SetActive(isSnowing);
    }
    
    private IEnumerator ActivateSnow()
    {
        var button = christmasBall.GetComponent<Button>();
        button.interactable = false;
        if (isSnowing)
        {
            isSnowing = false;
            snowGenerator.GetComponent<ParticleSystem>().Stop();
            activeChar.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            isSnowing = true;
            snowGenerator.GetComponent<ParticleSystem>().Play();
            activeChar.transform.GetChild(0).gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(3f);
        button.interactable = true;
    }

    /// <summary>
    /// Changes the active character to the next one in Characters-Enum and saves it in PlayerPrefs.
    /// Also disables button for short while
    /// </summary>
    /// <param name="button">button that called this function</param>
    /// <returns></returns>
    private IEnumerator NextCharacter(Button button)
    {
        button.interactable = false;

        var currentChar = (int)SceneSelector.character;
        var nextChar = (currentChar + 1 >= Enum.GetNames(typeof(Characters)).Length) ? 0 : currentChar + 1;
        
        SceneSelector.character = (Characters)nextChar;
        SetActiveCharacter();
        PlayerPrefs.SetInt("character", nextChar);

        yield return new WaitForSeconds(1.5f);
        button.interactable = true;
    }
    
    /// <summary>
    /// Lets a textMeshPro Text pulsate via scaling
    /// </summary>
    /// <returns></returns>
    public static IEnumerator PulseTMP(TextMeshProUGUI text, float scaleAmount)
    {
        while (true)
        {
            for (float i = 1f; i <= scaleAmount; i += 0.05f)
            {
                text.rectTransform.localScale = new Vector3(i, i, i);
                yield return new WaitForSecondsRealtime(0.05f);
            }

            text.rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

            for (float i = scaleAmount; i >= 1f; i -= 0.05f)
            {
                text.rectTransform.localScale = new Vector3(i, i, i);
                yield return new WaitForSecondsRealtime(0.05f);
            }

            text.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private IEnumerator PulseGameObject(GameObject gameObject, float scaleAmount)
    {
        while (true)
        {
            for (float i = 1f; i <= scaleAmount; i += 0.007f)
            {
                gameObject.transform.localScale = new Vector3(i, i, i);
                yield return new WaitForSecondsRealtime(0.05f);
            }

            gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

            for (float i = scaleAmount; i >= 1f; i -= 0.007f)
            {
                gameObject.transform.localScale = new Vector3(i, i, i);
                yield return new WaitForSecondsRealtime(0.05f);
            }

            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
