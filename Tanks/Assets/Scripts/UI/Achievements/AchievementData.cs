using UnityEngine;

[CreateAssetMenu(fileName = "New Achievement", menuName = "ScriptableObjects/Achievement Data", order = 1)]
public class AchievementData : ScriptableObject
{
    public string title;
    [TextArea(2, 10)]
    public string description;
    public StaticSaveStateManager.Achievement associatedEnum;
    public Texture2D medal;
}
