using System;
using System.Threading;
using Microsoft.SPOT;
using System.Text;
using Microsoft.SPOT.Hardware;
//using Microsoft.SPOT.Hardware.PWM;


namespace Hero_PWM1
{
    public class Program
    {
        static StringBuilder stringBuilder = new StringBuilder();
        /* create a gamepad */
        static CTRE.Gamepad _gamepad = new CTRE.Gamepad(CTRE.UsbHostDevice.GetInstance());
 
        public static void Main()
        {
            uint period = 50000; //period between pulses
            uint duration = 1500; //duration of pulse
            int delta, cross;
            CTRE.GamepadValues val = new CTRE.GamepadValues();

            PWM servo = new PWM(CTRE.HERO.IO.Port3.PWM_Pin7, period, duration,
            PWM.ScaleFactor.Microseconds, false);
            servo.Start(); //starts the signal
 
            /* loop forever */
            while (true)
            {
                /* get gamepad info */
                _gamepad.GetAllValues(ref val);
                //use the cross controller to change the servo position
                cross = val.pov;
                switch (cross)
                {
                    case 0:// N
                        delta = 10;
                        break;
                    case 1: // NE
                        delta = 100;
                        break;
                    case 7:// NW
                        delta = 1;
                        break;
                    case 4:// S
                        delta = -10;
                        break;
                    case 3: // SE
                        delta = -100;
                        break;
                    case 5:// SW
                        delta = -1;
                        break;
                    default:
                        delta = 0;
                        break;
                }
                delta += (int)servo.Duration;
                // delta has a new duration.  Limit it and put it back into duration
                Limit(750, ref delta, 2250); // the range on the servo I'm using is a bit larger than 1000 - 2000
                servo.Duration = (uint)(delta);

                /* feed watchdog to keep Talon's enabled if Gamepad is inserted. */
                // also display data
                if (_gamepad.GetConnectionStatus() == CTRE.UsbDeviceConnection.Connected)
                {
                    CTRE.Watchdog.Feed();
                    stringBuilder.Append("\t");
                    stringBuilder.Append(servo.Duration);
                    Debug.Print(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
                else
                {
                    servo.Duration = 1500;
                }
                /* run this task every 20ms */
                Thread.Sleep(20);
            }
        }
        static void Limit(int low, ref int x, int high)
        {
            if (x < low) x = low;
            if (x > high) x = high;
        }

    }

}
