using LedCSharp;
using Serial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        Dictionary<Image, Size> dict = new Dictionary<Image, Size>();
        bool keyDown;

        List<LogiKey> keys;
        // Create the serial port with basic settings 
        private SerialPort port = new SerialPort("COM3",
          9600, Parity.None, 8, StopBits.One);

        public MainWindow()
        {
            LogitechGSDK.LogiLedInit();
            LogitechGSDK.LogiLedSetLighting(0, 0, 0);

            keys = new List<LogiKey>();
            var f1 = new LogiKey(LedCSharp.keyboardNames.F1);
            f1.onTouchBegan += SetRedLight;
            f1.onTouchEnded += PulseRed;
            f1.onTouchBegan += OnKeyDownHandler;
            f1.onTouchEnded += OnKeyUpHandler;
            keys.Add(f1);

            var f2 = new LogiKey(LedCSharp.keyboardNames.F2);
            f2.onTouchBegan += SetRedLight;
            f2.onTouchEnded += PulseRed;
            f2.onTouchBegan += OnKeyDownHandler;
            f2.onTouchEnded += OnKeyUpHandler;
            keys.Add(f2);

            var f3 = new LogiKey(LedCSharp.keyboardNames.F3);
            f3.onTouchBegan += SetRedLight;
            f3.onTouchEnded += PulseRed;
            f3.onTouchBegan += OnKeyDownHandler;
            f3.onTouchEnded += OnKeyUpHandler;
            keys.Add(f3);

            var f4 = new LogiKey(LedCSharp.keyboardNames.F4);
            f4.onTouchBegan += SetRedLight;
            f4.onTouchEnded += PulseRed;
            f4.onTouchBegan += OnKeyDownHandler;
            f4.onTouchEnded += OnKeyUpHandler;
            keys.Add(f4);

            // Attach a method to be called when there
            // is data waiting in the port's buffer 

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (sender, e) =>
            {
                port = new SerialPort("COM3",
                    9600, Parity.None, 8, StopBits.One);

                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                // Begin communications 
                port.Open();
                //while (true)
                //{

                //}
            };

            worker.RunWorkerCompleted += (sender, e) => { };
            worker.RunWorkerAsync();

            InitializeComponent();

            dict[link] = new Size(link.Width, link.Height);
            dict[rocket] = new Size(rocket.Width, rocket.Height);
            dict[hammer] = new Size(hammer.Width, hammer.Height);
            dict[rail] = new Size(rail.Width, rail.Height);

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler((sender, e) =>
            {
                dispatcherTimer.Stop();
                keyDown = false;
                MoveTo(rocket, newY: 750);
                MoveTo(rail, newY: 750);
                MoveTo(link, newY: 750);
                MoveTo(hammer, newY: 750);
                resetSources();
            });
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);    
        }

        private void SetRedLight(object sender, Serial.KeyEventArgs e)
        {
            ((LogiKey)sender).SetLighting(System.Drawing.Color.FromArgb(0, 255, 0, 0));
        }

        private void PulseRed(object sender, Serial.KeyEventArgs e)
        {
            ((LogiKey)sender).SetLighting(System.Drawing.Color.FromArgb(0, 0, 0, 0));
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Show all the incoming data in the port's buffer
            // String.
            // Regex.Split()
            var data = port.ReadLine();
            var lines = data.Split(new[] { '\n' });
            foreach (var line in lines)
            {
                try
                {
                    var num = Int32.Parse(line);
                    Console.WriteLine(num);
                    //var keyName = LedCSharp.keyboardNames.ESC;
                    switch (num)
                    {
                        case 1:
                            keys[0].Touch(Serial.Key.KeyState.Down);
                            //keyName = LedCSharp.keyboardNames.F1;
                            break;
                        case 2:
                            keys[1].Touch(Serial.Key.KeyState.Down);
                            //keyName = LedCSharp.keyboardNames.F2;
                            break;
                        case 3:
                            keys[2].Touch(Serial.Key.KeyState.Down);
                            //keyName = LedCSharp.keyboardNames.F3;
                            break;
                        case 4:
                            keys[3].Touch(Serial.Key.KeyState.Down);
                            //keyName = LedCSharp.keyboardNames.F4;
                            break;
                        case -1:
                            keys[0].Touch(Serial.Key.KeyState.Up);
                            //keyName = LedCSharp.keyboardNames.F1;
                            break;
                        case -2:
                            keys[1].Touch(Serial.Key.KeyState.Up);
                            //keyName = LedCSharp.keyboardNames.F2;
                            break;
                        case -3:
                            keys[2].Touch(Serial.Key.KeyState.Up);
                            //keyName = LedCSharp.keyboardNames.F3;
                            break;
                        case -4:
                            keys[3].Touch(Serial.Key.KeyState.Up);
                            break;
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void resetSources()
        {
            hammer.Dispatcher.BeginInvoke(new Action(delegate () {
                hammer.Source = new BitmapImage(new Uri("/Images/hammer.png", UriKind.Relative)); }));
            link.Dispatcher.BeginInvoke(new Action(delegate () {
                link.Source = new BitmapImage(new Uri("/Images/link.png", UriKind.Relative));
            }));
            rocket.Dispatcher.BeginInvoke(new Action(delegate () {
                rocket.Source = new BitmapImage(new Uri("/Images/rocket.png", UriKind.Relative));
            }));
            rail.Dispatcher.BeginInvoke(new Action(delegate () {
                rail.Source = new BitmapImage(new Uri("/Images/shockut.png", UriKind.Relative));
            }));
        }

        private void OnKeyUpHandler(object sender, Serial.KeyEventArgs e)
        {
            if (keys.FirstOrDefault(key => key.keyState == Serial.Key.KeyState.Down) == null)
                dispatcherTimer.Start();
        }

        private void OnKeyDownHandler(object sender, Serial.KeyEventArgs e)
        {
            if (keys.FirstOrDefault(key => key.keyState == Serial.Key.KeyState.Down) != null)
            {
                resetSources();
                dispatcherTimer.Stop();
                if(!keyDown)
                {
                    MoveTo(rocket, newY: 450);
                    MoveTo(rail, newY: 450);
                    MoveTo(link, newY: 450);
                    MoveTo(hammer, newY: 450);
                    keyDown = true;
                }
                
                if(keys[0].keyState == Serial.Key.KeyState.Down)
                {
                    hammer.Dispatcher.BeginInvoke(new Action(delegate () {
                        hammer.Source = new BitmapImage(new Uri("/Images/hammerHalo.png", UriKind.Relative));
                    }));
                    //hammer.Source = new BitmapImage(new Uri("/Images/hammerHalo.png", UriKind.Relative));
                }
                else if (keys[1].keyState == Serial.Key.KeyState.Down)
                {
                    rocket.Dispatcher.BeginInvoke(new Action(delegate () {
                        rocket.Source = new BitmapImage(new Uri("/Images/rocketHalo.png", UriKind.Relative));
                    }));
                    //rocket.Source = new BitmapImage(new Uri("/Images/rocketHalo.png", UriKind.Relative));
                }
                else if (keys[2].keyState == Serial.Key.KeyState.Down)
                {
                    rail.Dispatcher.BeginInvoke(new Action(delegate () {
                        rail.Source = new BitmapImage(new Uri("/Images/shockutHalo.png", UriKind.Relative));
                    }));
                    //rail.Source = new BitmapImage(new Uri("/Images/shockutHalo.png", UriKind.Relative));
                }
                else if (keys[3].keyState == Serial.Key.KeyState.Down)
                {
                    link.Dispatcher.BeginInvoke(new Action(delegate () {
                        link.Source = new BitmapImage(new Uri("/Images/linkHalo.png", UriKind.Relative));
                    }));
                    //link.Source = new BitmapImage(new Uri("/Images/linkHalo.png", UriKind.Relative));
                }
            }
        }

        public void MoveTo(Image target, double newX = -1000, double newY = -1000)
        {
            var vec = VisualTreeHelper.GetOffset(target);
            var top = vec.Y;
            var left = vec.X;
            if(newX == -1000)
            {
                newX = left;
            }

            target.Dispatcher.BeginInvoke(new Action(delegate () {
                var yOffset = newY - top;
                var xOffset = newX - left;
                var T = new TranslateTransform(xOffset, 0);
                target.RenderTransform = T;
                Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
                DoubleAnimation anim1 = new DoubleAnimation(yOffset, duration);
                DoubleAnimation anim2 = new DoubleAnimation(xOffset, duration);
                T.BeginAnimation(TranslateTransform.YProperty, anim1);
                T.BeginAnimation(TranslateTransform.XProperty, anim2);
            }));
            
        }
    }
}
