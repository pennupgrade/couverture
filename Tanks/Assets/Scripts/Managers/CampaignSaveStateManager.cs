using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class CampaignSaveStateManager : SaveStateManager {

    public static CampaignSaveStateManager TryLoadSaveState(string saveFilePath) {
        try {
            return LoadInventory(saveFilePath);
        }
        catch (FileNotFoundException) {
            return null;
        }
    }

    public static CampaignSaveStateManager LoadInventory(string saveLocation) {
        CreateSaveDirectory();
        CampaignSaveStateManager outManager;
        using (StreamReader reader = new(saveLocation)) {
            string jsonData = reader.ReadToEnd();
            outManager = JsonUtility.FromJson<CampaignSaveStateManager>(jsonData);
            outManager.SetSaveLocation(saveLocation);
        }

        return outManager;
    }

    public static CampaignSaveStateManager CreateSave(string saveLocation) {
        CreateSaveDirectory(); // this shouldn't ever be used, but it's here just for safety
        CampaignSaveStateManager outManager = new();
        outManager.CreateNewSave(saveLocation);
        outManager.SaveToFile();
        return outManager;
    }

    [SerializeField] private DateTimeSerializable startTime;

    [SerializeField] private DateTimeSerializable lastPlayedTime;

    [SerializeField] private TimeSpanSerializable timePlayed;

    private DateTime startOfSession;

    public override void CreateNewSave(string saveLocation)
    {
        base.CreateNewSave(saveLocation);

        // initialize values
        startTime = DateTimeSerializable.Now();
        startOfSession = DateTime.Now;
        lastPlayedTime = DateTimeSerializable.Now();
        timePlayed = new TimeSpanSerializable(TimeSpan.Zero);
    }

    public override void BeginSession()
    {
        startOfSession = DateTime.Now;
        base.BeginSession();
    }

    protected override void WriteToSaveFile()
    {
        // calculate last played time and total play time
        DateTime now = DateTime.Now;

        // TODO: could have issues if crossing between time zones
        lastPlayedTime = new DateTimeSerializable(now);
        timePlayed = new TimeSpanSerializable(GetTimePlayed().Add(now.Subtract(startOfSession)));
        startOfSession = now;

        base.WriteToSaveFile();
    } 

    // Getters
    public DateTime GetStartTime() => startTime.ToDateTime();

    public DateTime GetLastPlayedTime() => lastPlayedTime.ToDateTime();

    public TimeSpan GetTimePlayed() => timePlayed.ToTimeSpan();
}