using System;
using UnityEngine;

public class ClassicMedalsManager : MonoBehaviour
{
    [SerializeField] private GameObject bronze;
    [SerializeField] private GameObject silver;
    [SerializeField] private GameObject gold;
    [SerializeField] private GameObject diamond;

    private enum MedalType
    {
        Bronze,
        Silver,
        Gold,
        Diamond
    }

    private void Start() {
        var achievements = SaveStateManagerGameObject.GetAchievements();

        if (achievements.Contains(StaticSaveStateManager.Achievement.WIN_CLASSIC_PART_ONE)) {
            UnlockMedal(MedalType.Bronze);
        }

        if (achievements.Contains(StaticSaveStateManager.Achievement.WIN_CLASSIC_PART_TWO)) {
            UnlockMedal(MedalType.Silver);
        }

        if (achievements.Contains(StaticSaveStateManager.Achievement.WIN_CLASSIC_FULL)) {
            UnlockMedal(MedalType.Gold);
        }

        if (achievements.Contains(StaticSaveStateManager.Achievement.WIN_CLASSIC_FULL_NO_SKIP)) {
            UnlockMedal(MedalType.Diamond);
        }
    }

    private void UnlockMedal(MedalType type) {
        var EnableMedalForeground =
            new Action<GameObject>(medal => medal.transform.GetChild(1).gameObject.SetActive(true));

        switch (type) {
        case MedalType.Bronze:
            EnableMedalForeground(bronze);
            break;
        case MedalType.Silver:
            EnableMedalForeground(silver);
            break;
        case MedalType.Gold:
            EnableMedalForeground(gold);
            break;
        case MedalType.Diamond:
            EnableMedalForeground(diamond);
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(type), type, "Medal type not handled");
        }
    }
}