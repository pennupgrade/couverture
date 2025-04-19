using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LyricsSyncDisplay : MonoBehaviour
{
    [System.Serializable]
    public class LyricLine {
        public float duration; // In beats
        public string text;
    }

    [SerializeField] public string[] lyricsText;
    private List<LyricLine> lyrics;

    void ParseLyrics() {
        lyrics = new List<LyricLine>();
        string[] lines = lyricsText;

        foreach (string line in lines) {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] parts = line.Split(':', 2);
            if (parts.Length < 2) continue;

            if (float.TryParse(parts[0], out float duration)) {
                lyrics.Add(new LyricLine { duration = duration, text = parts[1].Trim() });
            }
        }
    }

    [SerializeField] private GameObject gameAudio;
    [SerializeField] private TMPro.TextMeshProUGUI lyricsDisplay;
    [SerializeField] private float beatsPerMinute = 120f;

    private AudioSource audioSource;
    private int currentLine = 0;
    private double nextLyricTime;
    private float secondsPerBeat;


    // Start is called before the first frame update
    void Start() {
        audioSource = gameAudio.GetComponent<AudioSource>();
        secondsPerBeat = 60f / beatsPerMinute;
        ParseLyrics();
        
        // Don't start the lyrics until the audio actually starts playing
        nextLyricTime = 0;
        currentLine = 0;
        lyricsDisplay.text = ""; // Start with an empty display
    }

    // Update is called once per frame
    void Update() {
        if (!audioSource.isPlaying) {
            // If the audio isn't playing, reset and show nothing
            currentLine = 0;
            nextLyricTime = 0;
            lyricsDisplay.text = "";
            return;
        }

        if (nextLyricTime == 0) {
            // Sync with the exact DSP time when the audio starts playing
            nextLyricTime = AudioSettings.dspTime;
        }

        if (currentLine < lyrics.Count && AudioSettings.dspTime >= nextLyricTime) {
            lyricsDisplay.text = lyrics[currentLine].text;
            nextLyricTime += lyrics[currentLine].duration * secondsPerBeat;
            currentLine++;
        }
    }

}
