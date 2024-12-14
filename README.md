# Features

1. Displays real-time lyrics for the currently playing media track (i.e one line per timestamp) using `api.textyl.co`. Uses a 500ms timer to detect timestamps.
2. Supports positioning the lyric/form anywhere on the screen.
3. Supports staying on top or slideshow modes by pressing the F6 and F5 buttons respectively (only applies when the form is in focus).
4. Does not integrate with a particular media app: uses Windows RT API to read media track/playback information, such as media playing through Spotify, Music Groove, Chrome tabs, etc.

# How to Use

1. Run the `LyricsOverlay.exe` executable.
2. Position/resize the form as needed and press F5 to enter slideshow mode (hides other form controls and maximizes the lyric text).
3. Play the media track through your preferred media player and the lyrics should appear in the slideshow in real-time. Tested with Spotify and Groove Music.

# Notes

1. It is important that the song title and artist names are stored correctly into the media file when playing media locally. A "No lyrics found" error may appear otherwise.

# License

This work is available under the MIT license.
- <a href="https://www.flaticon.com/free-icons/song-lyrics" title="song lyrics icons">Song lyrics icons created by Pixel perfect - Flaticon</a>
