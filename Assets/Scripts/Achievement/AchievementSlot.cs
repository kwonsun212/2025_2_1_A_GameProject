using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSlot : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public Text nameText;
    public Text descriptionTxt;
    public Text progressText;
    public Slider progressSlier;

    public void SetAchievement(AchievementData achievement , float progress)
    {
        if (nameText != null)
            nameText.text = achievement.achivementName;

        if (descriptionTxt != null)
            descriptionTxt.text = achievement.description;

        if (progressSlier != null)
            progressSlier.value = achievement.isUnlocked ? 1f : progress;

        if(progressText != null)
        {
            if(achievement.isUnlocked)
            {
                progressText.text = "¿Ï·á!";
            }
            else
            {
                int current = Mathf.FloorToInt(progress * achievement.requiredAmount);
                progressText.text = current + "/" + achievement.requiredAmount;
            }
        }
    }
}
