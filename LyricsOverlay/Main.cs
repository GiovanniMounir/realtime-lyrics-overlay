/* Copyright (c) 2024 giovannimounir.net */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Windows.Media.Control;
using System.Runtime.InteropServices;
namespace LyricsOverlay
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        /* Initialize defaults */
        string songTitle = "";
        Dictionary<int, string> songLyrics;
        double _last_position = 0;
        double _saved_position = 0;
        private async Task<GlobalSystemMediaTransportControlsSessionManager> GetSystemMediaTransportControlsSessionManager() => await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
        private async Task<GlobalSystemMediaTransportControlsSessionMediaProperties> GetMediaProperties(GlobalSystemMediaTransportControlsSession session) => await session.TryGetMediaPropertiesAsync();
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private async Task<PlaybackInfo> getMedia()
        {
            var gsmtcsm = await GetSystemMediaTransportControlsSessionManager();
            var s = gsmtcsm.GetCurrentSession();
            var mediaProperties = await GetMediaProperties(s);
            return new PlaybackInfo(s.GetPlaybackInfo().PlaybackStatus, s.GetTimelineProperties().Position.TotalSeconds, Regex.Replace(mediaProperties.Title + " " + mediaProperties.Artist, @"\((.*?)\)", ""));
        }
        private async Task GetLyrics(string title)
        {
            if (songLyrics != null)
                songLyrics.Clear();
            else
                songLyrics = new Dictionary<int, string>();
            using (HttpClientHandler httpHandler = new HttpClientHandler())
            {
                httpHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                httpHandler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => { return true; };
                using (HttpClient client = new HttpClient(httpHandler))
                {
                    try
                    {
                        var response = await client.GetAsync("https://api.textyl.co/api/lyrics?q=" + HttpUtility.UrlEncode(title));
                        var responseString = await response.Content.ReadAsStringAsync();
                        if (responseString.Length > 0)
                        {
                            try
                            {
                                var root = JsonConvert.DeserializeObject<List<Lyric>>(responseString);
                                foreach (var p in root)
                                {
                                    songLyrics[p.seconds] = p.lyrics;
                                }
                            }
                            catch
                            {
                                label3.Invoke((MethodInvoker)delegate
                                {
                                    label3.Text = "No lyrics available\n- " + title;
                                });
                            }
                        }
                    }catch
                    {
                        label3.Invoke((MethodInvoker)delegate
                        {
                            label3.Text = "Could not retrieve lyrics information. Connection issue?\n- " + title;
                        });
                    }
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                var _media = getMedia().Result;
                if (_media.status == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
                {
                    if (_last_position != _media.position)
                    {
                        _last_position = _media.position;
                        _saved_position = _media.position;
                    }
                    else
                    {
                        _saved_position += 0.5;
                    }
                    if (songTitle != _media.title)
                    {
                        _last_position = 0;
                        _saved_position = 0;
                        songTitle = _media.title;
                        label2.Invoke((MethodInvoker)delegate
                        {
                            label2.Text = _saved_position.ToString();
                        });
                        label3.Invoke((MethodInvoker)delegate
                        {
                            label3.Text = "";
                        });
                        label1.Invoke((MethodInvoker)delegate
                        {
                            label1.Text = songTitle;
                        });
                        GetLyrics(songTitle).Wait();
                    }
                    label2.Invoke((MethodInvoker)delegate
                    {
                        label2.Text = _saved_position.ToString();
                    });
                    if (songLyrics != null && songLyrics.Count > 0)
                    {
                        try
                        {
                            label3.Invoke((MethodInvoker)delegate
                            {
                                label3.Text = songLyrics.Where(x => (_saved_position >= x.Key)).Last().Value;
                            });
                        }
                        catch { }
                    }
                }
            });
        }
        private void ToggleSlideshow()
        {
            this.TransparencyKey = this.TransparencyKey == Color.WhiteSmoke ? Color.LimeGreen : Color.WhiteSmoke;
            this.FormBorderStyle = this.FormBorderStyle == FormBorderStyle.None ? FormBorderStyle.Sizable : FormBorderStyle.None;
            this.label1.Visible = !this.label1.Visible;
            this.label2.Visible = !this.label2.Visible;
            this.label3.Dock = this.label3.Dock == DockStyle.Top ? DockStyle.Fill : DockStyle.Top;
            this.label4.Visible = !this.label4.Visible;
        }
        private void _MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }

        }
        private void Main_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
        private void Main_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                ToggleSlideshow();
            }
            if (e.KeyData == Keys.F6)
            {
                TopMost = !TopMost;
            }
        }
    }
}