using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialShow : MonoBehaviour
{
    public GameObject contextClue;
    public TutorialArea tutorialArea;
    
    public void Enable()
    {

        contextClue.SetActive(true);
    }

    public void Disable()
    {
        contextClue.SetActive(false);
    }
}
