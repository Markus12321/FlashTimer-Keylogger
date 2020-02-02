using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Keystroke.API;
using Newtonsoft.Json;

namespace FLash
{
    class Flash
    {
        bool[] hasFlash;
        String flashText;

        Stopwatch stopwatch;


        public Flash(KeystrokeAPI api)
        {
            Console.WriteLine("Click F8 at 01:00");
            Console.WriteLine("Click F1 to print flashes");
            Console.WriteLine("F2: top, F3: jungle, F4: mid, F5:ADC, F6:support");
            stopwatch = new Stopwatch();
            hasFlash = new bool[] { true, true, true, true, true, true };
            flashText = "";
            Clipboard.Clear();
            StartApp(api);
        }

        private void StartApp(KeystrokeAPI api)
        {
            using (api)
            {
                api.CreateKeyboardHook((character) =>
                {

                    String number = character.ToString();

                    if (number == "<F2>" && hasFlash[1])
                    {
                        AddToClippboard(1);
                    }
                    else if (number == "<F3>" && hasFlash[2])
                    {
                        AddToClippboard(2);

                    }
                    else if (number == "<F4>" && hasFlash[3])
                    {
                        AddToClippboard(3);

                    }
                    else if (number == "<F5>" && hasFlash[4])
                    {
                        AddToClippboard(4);

                    }
                    else if (number == "<F6>" && hasFlash[5])
                    {
                        AddToClippboard(5);
                    }
                    else if (number == "<F8>")
                    {
                        stopwatch.Start();

                    }else if(number == "<F1>")
                    {
                        SendKeys.Send("{ENTER}");
                        SendKeys.Send(flashText);
                        SendKeys.Send("{ENTER}");

                    }

                });
                Application.Run();
            }
        }




        private void AddToClippboard(int index)
        {
            String lane = "";
            if (index == 1)
            {
                lane = "Top";
            }
            else if (index == 2)
            {
                
                lane = "Jungle";
            }
            else if (index == 3)
            {
                lane = "Mid";
            }
            else if (index == 4)
            {
                lane = "ADC";
            }
            else if (index == 5)
            {
                lane = "Support";
            }

            hasFlash[index] = false;

            long timeWhenFlashReturns = stopwatch.ElapsedMilliseconds / 1000 + 360 + 5; //start the program after 10 seconds(gives you 5 second window to time the flash)
            long minutes = timeWhenFlashReturns / 60;



            String textToInsert = $"{lane} Flash {minutes.ToString("d2")}:{(timeWhenFlashReturns % 60).ToString("d2")}";
            this.flashText+= "  " + textToInsert;

            Task.Delay(new TimeSpan(0, 5, 0)).ContinueWith(o =>
            {
                hasFlash[index] = true;
                removeFromClipboard(lane, textToInsert);
            });
        }
        private void removeFromClipboard(String role, string textToInsert)
        {
           

            String text = this.flashText.Replace(textToInsert, "");

            if (text == "")
            {


            Thread STAThread = new Thread(
                delegate ()
                {
                    this.flashText = "";
                });
                STAThread.SetApartmentState(ApartmentState.STA);
                STAThread.Start();
                STAThread.Join();
            }
            else
            {

                Thread STAThread = new Thread(
               delegate ()
               {
                   // Use a fully qualified name for Clipboard otherwise it
                   // will end up calling itself.
                   this.flashText = text;
               });
                STAThread.SetApartmentState(ApartmentState.STA);
                STAThread.Start();
                STAThread.Join();

            }





        }


        //riot api sucks, deprecated function
        public string getTimeFromLeagueInSeconds()
        {
            string apikey = "?api_key=" + "RGAPI-e574cfd8-1e47-4cfb-a57d-67d9fb90c08e";
            String summonerName = "drxpatricia";
            var client = new WebClient();

            //get id of participating summoner
            var IDJson = client.DownloadString("https://euw1.api.riotgames.com/lol/summoner/v4/summoners/by-name/" + summonerName + apikey);
            dynamic jsonSummoner = JsonConvert.DeserializeObject(IDJson);

            string StringIDOfSummoner = jsonSummoner.id;

            var CurrentGameJson = client.DownloadString("https://euw1.api.riotgames.com/lol/spectator/v4/active-games/by-summoner/" + StringIDOfSummoner + apikey);
            //Console.WriteLine(response);

            dynamic CurrentGameObject = JsonConvert.DeserializeObject(CurrentGameJson);



            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            double MillisecondsSinceEpoch = (double)t.TotalMilliseconds;

            double timeSinceGameStartedInMilliseconds = ((MillisecondsSinceEpoch - (double)CurrentGameObject.gameStartTime));
            long timeSinceGameStartedInSeconds = (long)timeSinceGameStartedInMilliseconds / 1000;


            long minutesSinceGameStarted = timeSinceGameStartedInSeconds / 60;
            long SecondsInCurrentMinute = timeSinceGameStartedInSeconds % 60;



            Console.WriteLine(minutesSinceGameStarted.ToString("D2") + ":" + SecondsInCurrentMinute.ToString("D2"));




            return CurrentGameJson;


        }
    }
}
