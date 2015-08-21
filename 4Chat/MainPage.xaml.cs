using Enough.Async;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace _4Chat
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage() {
			this.InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e) {
			HistoryBox.LoadCompleted += async (s, nea) => {
				await HistoryBox.InvokeScriptAsync("scrollToEnd", null);
			};
			_html += "<font color=\"grey\" face=\"Segoe UI\" size=\"4\">" +
							WebUtility.HtmlEncode(DateTime.Now.ToString()) + " Chat initialized.<br/></font>";
			HistoryBox.NavigateToString(_html + _htmlSuffix);

			_api = new ChatAPI("dMnnHfYH4X7M2irQ") {
				Name = "Anon", Channel = "b"
			};
        }

		private async void button_Click(object sender, RoutedEventArgs e) {
			if (InputBox.Text == "" && _textBuffer == "")
				return;
			using (await _sendLock.LockAsync()) {
				try {
					_textBuffer = _textBuffer == "" ? InputBox.Text : _textBuffer;
					await _api.PostMsg(InputBox.Text);
					_textBuffer = "";
				} catch (Exception ex) {
					Debug.WriteLine(ex);
					button_Click(sender, e);
                }
			}
		}

		private void PrintMsg(string msg, bool isUser) {
			if (!_backdoor)
				msg = WebUtility.HtmlEncode(msg);
			var name = isUser ? "You" : "Anon";
			_html += "<font color=\"teal\" face=\"Segoe UI\" size=\"4\">"
				+ ">> " + name + ": " + "</font>";
			_html += "<font color=\"black\" face=\"Segoe UI\" size=\"4\">"
				+ msg + "</font><br/>";
		}

		private void HamburguerButton_Tapped(object sender, TappedRoutedEventArgs e) {
			Splitter.IsPaneOpen = !Splitter.IsPaneOpen;
        }

		private void InputBox_KeyUp(object sender, KeyRoutedEventArgs e) {
			switch (e.Key) {
				case VirtualKey.Enter:
					button_Click(null, null);
					break;
				case VirtualKey.Up:
					_historyIndex--;
					if (_historyIndex < 0) {
						_historyIndex = 0;
					}
					InputBox.Text = _history[_historyIndex];
					break;
				case VirtualKey.Down:
					_historyIndex++;
					if (_historyIndex >= _history.Count) {
						_historyIndex = _history.Count;
						InputBox.Text = "";
					}
					else {
						InputBox.Text = _history[_historyIndex];
					}
					break;
			}
		}
		
		private string _html = "<html><head><script type=\"text/javascript\">function scrollToEnd()" +
								" {window.scrollTo(0,document.documentElement.offsetHeight);}</script></head><body><p>";
		private string _htmlSuffix = "</p></body></html>";
		private List<string> _history = new List<string>();
		private int _historyIndex;
		private bool _backdoor = false;

		private readonly AsyncLock _sendLock = new AsyncLock();

		private ChatAPI _api;
		private string _textBuffer = "";
    }
}
