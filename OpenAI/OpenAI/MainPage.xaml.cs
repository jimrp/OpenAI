using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using OpenAI.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.Forms.Internals.GIFBitmap;

namespace OpenAI
{
    public partial class MainPage : ContentPage
    {
        HttpClients httpClients;
        List<Message> quests = new List<Message>();
        List<Message> answs = new List<Message>();
        private ISpeechToText _speechRecongnitionInstance;
        bool soundenabled = false;

        public MainPage()
        {
            InitializeComponent();
            httpClients = new HttpClients();
            _speechRecongnitionInstance = DependencyService.Get<ISpeechToText>();

            MessagingCenter.Subscribe<ISpeechToText, string>(this, "STT", (sender, args) =>
            {
                SpeechToTextFinalResultRecieved(args);
            });

            MessagingCenter.Subscribe<IMessageSender, string>(this, "STT", (sender, args) =>
            {
                SpeechToTextFinalResultRecieved(args);
            });

            //Begin();
        }

        async void Begin()
        {
            await TextToSpeech.SpeakAsync("Hi! How may i help you?");
            _speechRecongnitionInstance.StartSpeechToText();
        }

        void Microphone_Clicked(System.Object sender, System.EventArgs e)
        {
            _speechRecongnitionInstance.StartSpeechToText();
        }

        private async void SpeechToTextFinalResultRecieved(string args)
        {
            UserDialogs.Instance.ShowLoading("Thinking...");
            try
            {
                
                if (args == "τοποθεσία" || args == "location" ||
                    args == "Τοποθεσία" || args == "Location")
                {
                    string area = "";
                    try
                    {
                        var permissions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                        if (permissions != PermissionStatus.Granted)
                        {
                            permissions = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                        }
                        var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                        {
                            DesiredAccuracy = GeolocationAccuracy.Medium,
                            Timeout = TimeSpan.FromSeconds(30)
                        });
                        if (location != null)
                        {
                            var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                            var placemark = placemarks?.FirstOrDefault();
                            if (placemark != null)
                            {
                                area = placemark.Locality;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("There was a problem " + ex);
                    }
                    args = args.Replace("τοποθεσία", "Η τοποθεσία μου είναι η " + area);
                    args = args.Replace("location", "Η τοποθεσία μου είναι η " + area);
                    args = args.Replace("Τοποθεσία", "Η τοποθεσία μου είναι η " + area);
                    args = args.Replace("Location", "Η τοποθεσία μου είναι η " + area);
                }
                quests.Add(new Message
                {
                    role = "user",
                    content = args
                });
                Label q = new Label
                {
                    TextColor = Color.Black,
                    Text = args,
                    HorizontalOptions = LayoutOptions.End
                };
                response.Children.Add(q);
                ask.Text = "";
                answs = await httpClients.AskQuestion(quests);
                if (answs != null)
                {
                    if (answs.Count > 0)
                    {
                        for (int i = 0; i < answs.Count; i++)
                        {
                            Label a = new Label
                            {
                                TextColor = Color.DarkBlue,
                                Text = answs[i].content,
                                HorizontalOptions = LayoutOptions.Start
                            };
                            response.Children.Add(a);
                            if (soundenabled)
                            {
                                UserDialogs.Instance.HideLoading();
                                await TextToSpeech.SpeakAsync(answs[i].content);
                            }
                        }
                        delicon.IsVisible = true;
                    }
                    else
                    {
                        Label p = new Label
                        {
                            TextColor = Color.DarkBlue,
                            Text = "...",
                            HorizontalOptions = LayoutOptions.Start
                        };
                        response.Children.Add(p);
                    }
                }
                else
                {
                    Label p = new Label
                    {
                        TextColor = Color.DarkBlue,
                        Text = "...",
                        HorizontalOptions = LayoutOptions.Start
                    };
                    response.Children.Add(p);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem " + ex);
            }
            UserDialogs.Instance.HideLoading();

            if (soundenabled)
            {
                await Task.Delay(2000);
                _speechRecongnitionInstance.StartSpeechToText();
            }
        }

        async void Send_Clicked(System.Object sender, System.EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Thinking...");
            try
            {
                if (ask.Text == "τοποθεσία" || ask.Text == "location" ||
                    ask.Text == "Τοποθεσία" || ask.Text == "Location")
                {
                    string area = "";
                    try
                    {
                        var permissions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                        if (permissions != PermissionStatus.Granted)
                        {
                            permissions = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                        }
                        var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                        {
                            DesiredAccuracy = GeolocationAccuracy.Medium,
                            Timeout = TimeSpan.FromSeconds(30)
                        });
                        if (location != null)
                        {
                            var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                            var placemark = placemarks?.FirstOrDefault();
                            if (placemark != null)
                            {
                                area = placemark.Locality;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("There was a problem " + ex);
                    }
                    ask.Text = ask.Text.Replace("location", "Η τοποθεσία μου είναι η " + area);
                    ask.Text = ask.Text.Replace("τοποθεσία", "Η τοποθεσία μου είναι η " + area);
                    ask.Text = ask.Text.Replace("Location", "Η τοποθεσία μου είναι η " + area);
                    ask.Text = ask.Text.Replace("Τοποθεσία", "Η τοποθεσία μου είναι η " + area);
                }
                quests.Add(new Message
                {
                    role = "user",
                    content = ask.Text
                });
                Label q = new Label
                {
                    TextColor = Color.Black,
                    Text = ask.Text,
                    HorizontalOptions = LayoutOptions.End
                };
                response.Children.Add(q);
                ask.Text = "";
                answs = await httpClients.AskQuestion(quests);
                if (answs != null)
                {
                    if (answs.Count > 0)
                    {
                        for (int i = 0; i < answs.Count; i++)
                        {
                            Label a = new Label
                            {
                                TextColor = Color.DarkBlue,
                                Text = answs[i].content,
                                HorizontalOptions = LayoutOptions.Start
                            };
                            response.Children.Add(a);
                            if (soundenabled)
                            {
                                UserDialogs.Instance.HideLoading();
                                await TextToSpeech.SpeakAsync(answs[i].content);
                            }
                        }
                        delicon.IsVisible = true;
                    }
                    else
                    {
                        Label p = new Label
                        {
                            TextColor = Color.DarkBlue,
                            Text = "...",
                            HorizontalOptions = LayoutOptions.Start
                        };
                        response.Children.Add(p);
                    }
                }
                else
                {
                    Label p = new Label
                    {
                        TextColor = Color.DarkBlue,
                        Text = "...",
                        HorizontalOptions = LayoutOptions.Start
                    };
                    response.Children.Add(p);
                }

                await Task.Delay(100);

                var lastChild = response.Children.LastOrDefault();
                if (lastChild != null)
                {
                    await scrollchat.ScrollToAsync(lastChild, ScrollToPosition.MakeVisible, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem " + ex);
            }
            UserDialogs.Instance.HideLoading();
        }

        async void Paint_Clicked(System.Object sender, System.EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Drawing...");
            try
            {
                Label q = new Label
                {
                    TextColor = Color.Black,
                    Text = ask.Text,
                    HorizontalOptions = LayoutOptions.End
                };
                response.Children.Add(q);
                List<string> temp = new List<string>();
                temp = await httpClients.CreateImage(ask.Text);
                ask.Text = "";
                if (temp != null)
                {
                    if (temp.Count > 0)
                    {
                        for (int i = 0; i < temp.Count; i++)
                        {
                            Label p = new Label
                            {
                                TextColor = Color.DarkBlue,
                                Text = temp[i],
                                FontSize = 7,
                                HorizontalOptions = LayoutOptions.Start
                            };
                            Image a = new Image
                            {
                                Source = temp[i],
                                WidthRequest = 300,
                                HorizontalOptions = LayoutOptions.Start,
                                BackgroundColor = Color.Transparent
                            };
                            response.Children.Add(p);
                            response.Children.Add(a);
                        }
                        delicon.IsVisible = true;
                    }
                    else
                    {
                        Label p = new Label
                        {
                            TextColor = Color.DarkBlue,
                            Text = "...",
                            HorizontalOptions = LayoutOptions.Start
                        };
                        response.Children.Add(p);
                    }
                }
                else
                {
                    Label p = new Label
                    {
                        TextColor = Color.DarkBlue,
                        Text = "...",
                        HorizontalOptions = LayoutOptions.Start
                    };
                    response.Children.Add(p);
                }

                await Task.Delay(100);

                var lastChild = response.Children.LastOrDefault();
                if (lastChild != null)
                {
                    await scrollchat.ScrollToAsync(lastChild, ScrollToPosition.MakeVisible, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem " + ex);
            }
            UserDialogs.Instance.HideLoading();
        }

        void sound_Clicked(System.Object sender, System.EventArgs e)
        {
            if (soundenabled)
            {
                soundenabled = false;
                sound.Source = "nosound";
            }
            else
            {
                soundenabled = true;
                sound.Source = "sound";
            }
        }

        async void Trash_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                quests.Clear();
                answs.Clear();
                ask.Text = "";
                response.Children.Clear();
                delicon.IsVisible = false;
                await TextToSpeech.SpeakAsync("");
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem " + ex);
            }
        }
    }
}

