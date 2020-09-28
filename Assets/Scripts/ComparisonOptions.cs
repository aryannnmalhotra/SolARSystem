using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComparisonOptions : MonoBehaviour
{
    public Interactivity InteractivityInstance;
    void OnEnable()
    {
        int currentIndex = InteractivityInstance.CurrentIndex;
        int i = 0;
        foreach(RectTransform button in transform)
        {
            if (i == currentIndex)
                button.gameObject.SetActive(false);
            else
                button.gameObject.SetActive(true);
            i++;
        }
    }
}
