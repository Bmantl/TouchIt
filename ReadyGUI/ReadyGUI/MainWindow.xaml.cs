using LedCSharp;
using Serial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ReadyGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer getReadyTimer = new DispatcherTimer();
        DispatcherTimer wrongTimer = new DispatcherTimer();
        DispatcherTimer readySetGoTimer = new DispatcherTimer();

        List<LogiKey> keys;
        // Create the serial port with basic settings 
        private SerialPort port = new SerialPort("COM3",
          9600, Parity.None, 8, StopBits.One);

        int currentImage = 1;

        public object Keys { get; private set; }

        public MainWindow()
        {
            LogitechGSDK.LogiLedInit();
            LogitechGSDK.LogiLedSetLighting(0, 0, 0);

            keys = new List<LogiKey>();
            var f1 = new LogiKey(LedCSharp.keyboardNames.F1);
            f1.onTouchBegan += SetRedLight;
            f1.onTouchEnded += PulseRed;
            f1.onTouchBegan += keyDown;
            f1.onTouchEnded += keyUp;
            keys.Add(f1);

            var f2 = new LogiKey(LedCSharp.keyboardNames.F2);
            f2.onTouchBegan += SetRedLight;
            f2.onTouchEnded += PulseRed;
            f2.onTouchBegan += keyDown;
            f2.onTouchEnded += keyUp;
            keys.Add(f2);

            var f3 = new LogiKey(LedCSharp.keyboardNames.F3);
            f3.onTouchBegan += SetRedLight;
            f3.onTouchEnded += PulseRed;
            f3.onTouchBegan += keyDown;
            f3.onTouchEnded += keyUp;
            keys.Add(f3);

            var f4 = new LogiKey(LedCSharp.keyboardNames.F4);
            f4.onTouchBegan += keyDown;
            f4.onTouchEnded += keyUp;
            f4.onTouchBegan += SetRedLight;
            f4.onTouchEnded += PulseRed;
            keys.Add(f4);

            // Attach a method to be called when there
            // is data waiting in the port's buffer 
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (sender, e) =>
            {
                port = new SerialPort("COM3",
                    9600, Parity.None, 8, StopBits.One);

                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                // Begin communications 
                port.Open();
                //while (true)
                //{

                //};
            };
            worker.RunWorkerCompleted += (sender, e) => { };
            worker.RunWorkerAsync();

            InitializeComponent();
            getReadyTimer.Tick += timerElapsed;
            getReadyTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            getReadyTimer.Start();

            wrongTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            wrongTimer.Tick += wrongTimerElapsed;

            readySetGoTimer.Interval = new TimeSpan(0, 0, 0, 0, 3000);
            readySetGoTimer.Tick += lightCountDown;
        }
        
        private void keyDown(Object sender, Serial.KeyEventArgs e)
        {
            if(keys[1].keyState == Serial.Key.KeyState.Down 
                && keys[2].keyState == Serial.Key.KeyState.Down
                && keys[3].keyState == Serial.Key.KeyState.Down)
            {
                wrongKeysPressed();
                wrongKeysAlert();
            }
            else if (keys[0].keyState == Serial.Key.KeyState.Down
                && keys[1].keyState == Serial.Key.KeyState.Down
                && keys[2].keyState == Serial.Key.KeyState.Down)
            {
                goodKeysPressed();
                goodKeysAlert();
                readySetGo();
            }
            else
            {
                getReadyTimer.Start();
                readySetGoTimer.Stop();
            }
        }

        private void keyUp(Object sender, Serial.KeyEventArgs e)
        {
            getReadyTimer.Start();
        }

        private void SetRedLight(object sender, Serial.KeyEventArgs e)
        {
            ((LogiKey)sender).SetLighting(System.Drawing.Color.FromArgb(0, 120, 120, 0));
        }

        private void PulseRed(object sender, Serial.KeyEventArgs e)
        {
            ((LogiKey)sender).SetLighting(System.Drawing.Color.FromArgb(0, 0, 0, 0));
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var data = port.ReadLine();
            var lines = data.Split(new[] { '\n' });
            foreach (var line in lines)
            {
                try
                {
                    var num = Int32.Parse(line);
                    Console.WriteLine(num);
                    switch (num)
                    {
                        case 1:
                            keys[0].Touch(Serial.Key.KeyState.Down);
                            break;
                        case 2:
                            keys[1].Touch(Serial.Key.KeyState.Down);
                            break;
                        case 3:
                            keys[2].Touch(Serial.Key.KeyState.Down);
                            break;
                        case 4:
                            keys[3].Touch(Serial.Key.KeyState.Down);
                            break;
                        case -1:
                            keys[0].Touch(Serial.Key.KeyState.Up);
                            break;
                        case -2:
                            keys[1].Touch(Serial.Key.KeyState.Up);

                            break;
                        case -3:
                            keys[2].Touch(Serial.Key.KeyState.Up);
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

        public void timerElapsed(object sender, EventArgs e)
        {
            if (currentImage == 1)
            {
                currentImage = 2;
                key.Dispatcher.BeginInvoke(new Action(delegate ()
                {
                    key.Source = new BitmapImage(new Uri("/Images/keysGreen.png", UriKind.Relative));
                }));

            }
            else if (currentImage == 2)
            {
                currentImage = 1;
                key.Dispatcher.BeginInvoke(new Action(delegate ()
                {
                    key.Source = new BitmapImage(new Uri("/Images/keysReg.png", UriKind.Relative));
                }));
            }
            else if (currentImage == 3)
            {
                currentImage = 1;
                key.Dispatcher.BeginInvoke(new Action(delegate ()
                {
                    key.Source = new BitmapImage(new Uri("/Images/keysReg.png", UriKind.Relative));
                }));
            }

        }

        public void wrongTimerElapsed(object sender, EventArgs e)
        {
            if (currentImage == 1)
            {
                currentImage = 3;
                key.Dispatcher.BeginInvoke(new Action( delegate()
                {
                    key.Source = new BitmapImage(new Uri("/Images/keysRed.png", UriKind.Relative));
                }));
                
            }
            else if (currentImage == 2)
            {
                currentImage = 1;
                key.Dispatcher.BeginInvoke(new Action(delegate ()
                {
                    key.Source = new BitmapImage(new Uri("/Images/keysReg.png", UriKind.Relative));
                }));
            }
            else if (currentImage == 3)
            {
                currentImage = 1;
                key.Dispatcher.BeginInvoke(new Action(delegate ()
                {
                    key.Source = new BitmapImage(new Uri("/Images/keysReg.png", UriKind.Relative));
                }));
            }

        }

        public void wrongKeysPressed()
        {
            getReadyTimer.Stop();
            readySetGoTimer.Stop();
            wrongTimer.Start();      
        }

        public void wrongKeysAlert()
        {
            LogitechGSDK.LogiLedFlashLighting(100, 0, 0, 3000, 300);
        }

        public void goodKeysPressed()
        {
            getReadyTimer.Stop();
            wrongTimer.Stop();
            currentImage = 2;
            //key.Source = new BitmapImage(new Uri("/Images/keysGreen.png", UriKind.Relative));
            key.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                key.Source = null;
            }));
            
        }

        public void goodKeysAlert()
        {

        }

        public void readySetGo()
        {
            readySetGoTimer.Start();
        }

        List<LogiKey> Three = new List<LogiKey>() { new LogiKey(keyboardNames.PAUSE_BREAK), new LogiKey(keyboardNames.NUM_SLASH),
            new LogiKey(keyboardNames.NUM_ASTERISK), new LogiKey(keyboardNames.NUM_NINE), new LogiKey(keyboardNames.NUM_SIX),
            new LogiKey(keyboardNames.NUM_FIVE), new LogiKey(keyboardNames.NUM_FOUR), new LogiKey(keyboardNames.NUM_THREE),
            new LogiKey(keyboardNames.NUM_PERIOD), new LogiKey(keyboardNames.NUM_ZERO) };

        List<LogiKey> Two = new List<LogiKey>() { new LogiKey(keyboardNames.PAUSE_BREAK), new LogiKey(keyboardNames.NUM_SLASH),
            new LogiKey(keyboardNames.NUM_ASTERISK), new LogiKey(keyboardNames.NUM_NINE), new LogiKey(keyboardNames.NUM_SIX),
            new LogiKey(keyboardNames.NUM_FIVE), new LogiKey(keyboardNames.NUM_FOUR), new LogiKey(keyboardNames.NUM_ONE),
            new LogiKey(keyboardNames.NUM_PERIOD), new LogiKey(keyboardNames.NUM_ZERO) };

        List<LogiKey> One = new List<LogiKey>() {
            new LogiKey(keyboardNames.NUM_ASTERISK), new LogiKey(keyboardNames.NUM_NINE), new LogiKey(keyboardNames.NUM_SIX),
             new LogiKey(keyboardNames.NUM_THREE),
            new LogiKey(keyboardNames.NUM_PERIOD)};

        public void lightCountDown(object sender, EventArgs e)
        {
            readySetGoTimer.Stop();
            foreach(var key in Three)
            {
                //key.Pulse(System.Drawing.Color.FromArgb(0, 255, 0, 0), System.Drawing.Color.FromArgb(0, 0, 0, 0), 2000, false);
                key.SetLighting(System.Drawing.Color.FromArgb(0, 255, 0, 0));
            }

            Thread.Sleep(2100);
            foreach (var key in Three)
            {
                //key.Pulse(System.Drawing.Color.FromArgb(0, 255, 0, 0), System.Drawing.Color.FromArgb(0, 0, 0, 0), 2000, false);
                key.SetLighting(System.Drawing.Color.FromArgb(0, 0, 0, 0));
            }

            foreach (var key in Two)
            {
                key.SetLighting(System.Drawing.Color.FromArgb(0, 255, 165, 0));
                //key.Pulse(System.Drawing.Color.FromArgb(0, 255, 165, 0), System.Drawing.Color.FromArgb(0, 0, 0, 0), 2000, false);
            }

            

            Thread.Sleep(2100);

            foreach (var key in Two)
            {
                key.SetLighting(System.Drawing.Color.FromArgb(0, 0, 0, 0));
                //key.Pulse(System.Drawing.Color.FromArgb(0, 255, 165, 0), System.Drawing.Color.FromArgb(0, 0, 0, 0), 2000, false);
            }

            foreach (var key in One)
            {
                key.SetLighting(System.Drawing.Color.FromArgb(0, 255, 215, 0));
               // key.Pulse(System.Drawing.Color.FromArgb(0, 255, 215, 0), System.Drawing.Color.FromArgb(0, 0, 0, 0), 2000, false);
            }

            Thread.Sleep(2100);

            LogitechGSDK.LogiLedSetLighting(0, 100, 0);
            DispatcherTimer t = new DispatcherTimer();
            t.Tick += (s, e1) => { LogitechGSDK.LogiLedSetLighting(0, 0, 0); t.Stop(); };
            t.Interval = new TimeSpan(0, 0, 3);
            t.Start(); 
        }
    }
}
