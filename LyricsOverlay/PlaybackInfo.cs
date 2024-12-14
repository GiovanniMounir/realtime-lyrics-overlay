/* Copyright (c) 2024 giovannimounir.net */
using Windows.Media.Control;

namespace LyricsOverlay
{
    class PlaybackInfo
    {
        public double position { get; set; }
        public string title { get; set; }
        public GlobalSystemMediaTransportControlsSessionPlaybackStatus status { get; set; }
        public PlaybackInfo(GlobalSystemMediaTransportControlsSessionPlaybackStatus status, double position, string title)
        {
            this.status = status;
            this.position = position;
            this.title = title;
        }
    }
}
