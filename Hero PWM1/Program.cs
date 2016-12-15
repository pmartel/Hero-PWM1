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
            // I noticed that the normal 1000 to 2000 ms delay does not allow full range on the servo I'm using
            // so I'm setting up high/low and center values to give a fuller range
            // eventually create a Servo class with these values and a PWM
            uint servoLow = 600;
            uint servoHi = 2400;
            uint servoCenter = (uint)((servoLow + servoHi) / 2);
            uint duration = servoCenter;
            float y;
            // CTRE.GamepadValues val = new CTRE.GamepadValues();

            PWM servo = new PWM(CTRE.HERO.IO.Port3.PWM_Pin7, period, duration,
            PWM.ScaleFactor.Microseconds, false);
            servo.Start(); //starts the signal
 
            /* loop forever */
            while (true)
            {
                /* use gamepad axis to set servo */
                y = _gamepad.GetAxis(1);

                duration = (uint)(servoLow + (y + 1) / 2 * (servoHi - servoLow));
                // delta has a new duration.  Limit it and put it back into duration
                servo.Duration = duration;

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
                {   // put the servo to the center before the watchdog starves.
                    servo.Duration = servoCenter;
                }
                /* run this task every 20ms */
                Thread.Sleep(20);
            }
        }
    }
}
