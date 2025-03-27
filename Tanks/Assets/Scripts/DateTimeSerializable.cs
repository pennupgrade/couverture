using System;
using UnityEngine;

[Serializable]
public class DateTimeSerializable {
    [SerializeField] private long ticks;
    private DateTime? dateTime = null;

    
    public DateTimeSerializable(DateTime d) {
        ticks = d.Ticks;
        dateTime = d;
    }

    public DateTime ToDateTime() {
        if (dateTime is null) {
            dateTime = new(ticks);
        }
        return (DateTime) dateTime;
    }

    public static DateTimeSerializable Now() {
        return new DateTimeSerializable(DateTime.Now);
    }
}