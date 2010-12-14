using System;
using Phidgets; 
using Phidgets.Events;

namespace IMUStabilizedLaserRangeFinder
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			try
			{
				 

				ServoInitialization();
				
				Console.WriteLine("Press any key to end");
				
				GoStable();
				Console.Read();
				ServoStop();
				
				
				
			}
			catch (PhidgetException ex)
			{
				Console.WriteLine(ex.Description);
			}
		}
		static Servo servo;
		static double accx,accy,accz;
		
		
		static void GoStable()
		{
			Console.WriteLine("tring!!!");
			
		}
		
		#region Servo
		static void ServoInitialization()
		{
			servo = new Servo();

			servo.Attach += new AttachEventHandler(servo_Attach);
			servo.Detach += new DetachEventHandler(servo_Detach);
			servo.Error += new ErrorEventHandler(servo_Error);
			
			servo.PositionChange += new PositionChangeEventHandler(
			                                                       servo_PositionChange);
			
			servo.open();
			
			Console.WriteLine("Waiting for Servo to be attached...");
			servo.waitForAttachment();
			servo.servos[0].Position = 110.00;
				
		}
		static void ServoStop()
		{
			servo.close();
			servo = null;
			Console.WriteLine("ok");
		}
		  static void servo_Attach(object sender, AttachEventArgs e)
        {
            Console.WriteLine("Servo {0} attached!", e.Device.SerialNumber.ToString());
        }

        //Detach event handler...Display the serial number of the detached servo device
        static void servo_Detach(object sender, DetachEventArgs e)
        {
            Console.WriteLine("Servo {0} detached!", e.Device.SerialNumber.ToString());
        }

        //Error event handler....Display the error description to the console
        static void servo_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.Description);
        }

        //Position CHange event handler...display which motor changed position and 
        //its new position value to the console
        static void servo_PositionChange(object sender, PositionChangeEventArgs e)
        {
            Console.WriteLine("Servo {0} Position {1}", e.Index, e.Position);
        }
		#endregion
		
	}
}

