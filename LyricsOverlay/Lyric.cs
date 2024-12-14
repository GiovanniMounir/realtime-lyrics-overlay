/* Copyright (c) 2024 giovannimounir.net */
using Newtonsoft.Json;

namespace LyricsOverlay
{
    public class Lyric
    {
        [JsonProperty("seconds")]
        public int seconds { get; set; }

        [JsonProperty("lyrics")]
        public string lyrics { get; set; }
    }
}
