using System;
using UnityEngine;

[Serializable]
public class TimeSpanSerializable {
    [SerializeField] private long ticks;
    private TimeSpan? timeSpan = null;

    
    public TimeSpanSerializable(TimeSpan t) {
        ticks = t.Ticks;
        timeSpan = t;
    }

    public TimeSpan ToTimeSpan() {
        if (timeSpan is null) {
            timeSpan = new(ticks);
        }
        return (TimeSpan) timeSpan;
    }
}