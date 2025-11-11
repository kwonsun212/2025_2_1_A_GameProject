using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class achievementManager : MonoBehaviour
{

    public static achievementManager instance;

    [Header("Achievement Setting")]
    public List<AchievementData> allAchievements = new List<AchievementData>();

    [Header("UI References")]
    public GameObject achievementPopupPrefab;
    public Transform popupParent;
    public GameObject achievementPanel;
    public Transform achievementListContent;
    public GameObject achievementSlotPrefab;

    private Dictionary<AchievementType, int> progressData = new Dictionary<AchievementType, int>();     //동계저장

    void Awake()
    {
        if(instance == null)                        //싱글톤 화
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float GetProgress(AchievementData achievement)           //진행도 가져오기
    {
        if (achievement.isUnlocked) return 1f;
        int current = progressData.ContainsKey(achievement.achievementType) ? progressData[achievement.achievementType] : 0;
        return Mathf.Min((float)current / achievement.requiredAmount, 1f);
    }
    void Start()
    {
        ResetAllAchievements();                     //시작시에 리셋 강제로(테스트용)
        foreach(AchievementType type in System.Enum.GetValues(typeof(AchievementType)))
        {
            progressData[type] = 0;
        }
        LoadAchievements();
        UpdateAchhievementUI();
    }

    //업적 UI 업데이트
    public void UpdateAchhievementUI()
    {
        if (achievementListContent == null || achievementSlotPrefab == null)
            return;

        //기존 슬롯 제거
        foreach(Transform child in achievementListContent)
        {
            Destroy(child.gameObject);
        }

        foreach(AchievementData achievement in allAchievements)
        {
            GameObject slot = Instantiate(achievementSlotPrefab, achievementListContent);
            AchievementSlot slotScript = slot.GetComponent<AchievementSlot>();
            if(slotScript != null)
            {
                slotScript.SetAchievement(achievement, GetProgress(achievement));
            }
        }
    }
    void SaveAchievements()
    {
        foreach(var kvp in progressData)
        {
            PlayerPrefs.SetInt("Achievemenyt_" + kvp.Key, kvp.Value);
        }

        foreach(AchievementData achievement in allAchievements)
        {
            PlayerPrefs.SetInt("Unlocked_" + achievement.name, achievement.isUnlocked ? 1 : 0);
        }

        PlayerPrefs.Save();

    }

    void LoadAchievements()     //데이터 로드
    {
        foreach (AchievementType type in System.Enum.GetValues(typeof(AchievementType)))
        {
            progressData[type] = PlayerPrefs.GetInt("Achivement_" + type, 0);
        }

        foreach(AchievementData achievement in allAchievements)
        {
            achievement.isUnlocked = PlayerPrefs.GetInt("Unlocked_" + achievement.name, 0) == 1;
        }
    }
    public void ResetAllAchievements()      //업적 초기화 (리셋)
    {
        foreach (AchievementType type in System.Enum.GetValues(typeof(AchievementType)))    //모든 진행도 초기화
        {
            progressData[type] = 0;
            PlayerPrefs.DeleteKey("Achievement_" + type);
        }

        foreach (AchievementData achievement in allAchievements)
        {
            achievement.isUnlocked = false;
            PlayerPrefs.DeleteKey("Unlocked_" + achievement.name);
        }

        PlayerPrefs.Save();
        UpdateAchhievementUI();
    }
    void ShowAchivementPopup(AchievementData achievement)           //업적 팝업 표시
    {
        if(achievementPopupPrefab != null && popupParent != null)
        {
            GameObject popup = Instantiate(achievementPopupPrefab, popupParent);

            Text titleText = popup.transform.Find("Title")?.GetComponent<Text>();
            Text dessText = popup.transform.Find("Description")?.GetComponent<Text>();

            if (titleText != null) titleText.text = "업적 달성";
            if (dessText != null) dessText.text = achievement.achivementName;

            Destroy(popup, 3.0f);
        }
    }

    public void UpdateProgress(AchievementType type , int amount = 1)   //진행도 업데이트 - 모든 업적이 이 함수응 통해 처리
    {
        progressData[type] += amount;

        //해당 타입의 모든 업적 체크
        foreach(AchievementData achievement in allAchievements)
        {
            if (achievement.achievementType == type && !achievement.isUnlocked)
            {
                if (progressData[type] >= achievement.requiredAmount)
                {
                    UnlockedAchievement(achievement);
                }
            }                      
        }
    }
    void UnlockedAchievement(AchievementData achievement)               //업적 언락
    {
        achievement.isUnlocked = true;
        //보상이 있는 업적일 경우 보상 로직을 여기에 넣는다.
        ShowAchivementPopup(achievement);
        UpdateAchhievementUI();
    }    
}
