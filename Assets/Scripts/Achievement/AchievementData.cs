using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Achivement" , menuName = "Achievement/Achiebement Data")]
public class AchievementData : ScriptableObject
{
    public string achivementName;
    public string description;
    public AchievementType achievementType;
    public int requiredAmount;
    public int rewardCoins;
    public bool isUnlocked;
    public Sprite icon;
    
}
