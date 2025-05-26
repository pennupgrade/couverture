using System;
using UnityEngine;

public class MedalsManager : MonoBehaviour
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
        UnlockMedal(MedalType.Silver);
        UnlockMedal(MedalType.Diamond);
    }

    private static void EnableMedalForeground(GameObject medal) =>
        medal.transform.GetChild(1).gameObject.SetActive(true);

    private void UnlockMedal(MedalType type) {
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
            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}