// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using UnityEngine;

[CreateAssetMenu(fileName = "New Achievement", menuName = "ScriptableObjects/Achievement Data", order = 1)]
public class AchievementData : ScriptableObject
{
    public string title;
    [TextArea(2, 10)]
    public string description;
    public StaticSaveStateManager.Achievement associatedEnum;
    public Sprite medal;
}
