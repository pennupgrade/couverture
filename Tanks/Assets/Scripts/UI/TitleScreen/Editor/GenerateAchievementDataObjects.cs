using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GenerateAchievementDataObjects
{
    [MenuItem("Assets/Create/ScriptableObjects/Generate Assignment Data objects from current achievement enums")]
    public static void Generate()
    {
        var enums = Enum.GetValues(typeof(StaticSaveStateManager.Achievement));
        var medalImagesGuids = AssetDatabase.FindAssets("", new[] { "Assets/Images/UI/Achievements/Medals" });
        var medalImagesPaths = medalImagesGuids.Select(guid => AssetDatabase.GUIDToAssetPath(guid));

        foreach (var value in enums)
        {
            var enumValue = (StaticSaveStateManager.Achievement)(int)value;

            var achievementData = ScriptableObject.CreateInstance<AchievementData>();
            achievementData.title = string.Join(" ", enumValue.ToString().Split("_").Select(word => word[0] + word[1..].ToLower())) + "!";
            achievementData.description = $"This is the description for {enumValue}.";
            achievementData.associatedEnum = enumValue;

            // Try to find corresponding medal texture, assign if found
            foreach (var path in medalImagesPaths)
            {
                if (path.Contains(enumValue.ToString()))
                {
                    achievementData.medal = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    break;
                }
            }

            var name = AssetDatabase.GenerateUniqueAssetPath($"Assets/Prefab/UI/Achievements/{enumValue}.asset");
            AssetDatabase.CreateAsset(achievementData, name);
        }

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();

        var folder = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/Prefab/UI/Achievements");
        if (folder != null)
        {
            Selection.activeObject = folder;
            EditorGUIUtility.PingObject(folder);
        }
    }
}
