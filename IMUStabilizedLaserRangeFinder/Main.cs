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
				SpatialInitialization();
				
				Console.WriteLine("Press any key to end");
				
				GoStable();
				Console.Read();
				SpatialClose();
				ServoStop();
				
				
				
			}
			catch (PhidgetException ex)
			{
				Console.WriteLine(ex.Description);
			}
		}
		static Spatial spatial;
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

        //Position CHange event handler...display which motor d(o.o)b changed position and 
        //its new position value to the console
        static void servo_PositionChange(object sender, PositionChangeEventArgs e)
        {
            Console.WriteLine("Servo {0} Position {1}", e.Index, e.Position);
        }
		#endregion
#region Spatial
		static void SpatialInitialization()
		{
			spatial = new Spatial();
				
			spatial.Attach += new AttachEventHandler(accel_Attach);
			spatial.Detach += new DetachEventHandler(accel_Detach);
			spatial.Error += new ErrorEventHandler(accel_Error);
			spatial.SpatialData += new SpatialDataEventHandler(spatial_SpatialData);
			
			spatial.open();
			
			Console.WriteLine("Waiting for spatial to be attached....");
			spatial.waitForAttachment();
			
//			spatial.DataRate = 496; //multiple of 80
			spatial.DataRate = 240; //multiple of 80
		}
		static void SpatialClose()
		{
			spatial.close();
			spatial = null;
			Console.WriteLine("ok");
		}
		static void spatial_SpatialData(object sender, SpatialDataEventArgs e)
		{
			if (e.spatialData[0].Acceleration.Length > 0)
			{
				accx = e.spatialData[0].Acceleration[2]; 
				accy = e.spatialData[0].Acceleration[1]; 
				accz = e.spatialData[0].Acceleration[0]; 
			}
			double th = 0.05;
			
			if(accy > (-1)*th && accy < th)
			{
				Console.WriteLine(accy);
//				Console.WriteLine("Stable");
			}
			else
			{
				if(accy < (th * -1))
					servo.servos[0].Position -= 5;
				
				if(accy > th)
					servo.servos[0].Position += 5;
				
			}
			//			Console.WriteLine("{0} {1} {2}",accx,accy,accz);
//			Console.WriteLine("{0} {1} {2}",accy,accy,accy);
		}
		
		static void accel_Attach(object sender, AttachEventArgs e)
		{
            Console.WriteLine("Spatial {0} attached!", 
			                  e.Device.SerialNumber.ToString());
		}
		static void accel_Detach(object sender, DetachEventArgs e)
		{
			Console.WriteLine("Spatial {0} detached!", 
			                  e.Device.SerialNumber.ToString());
		}
		static void accel_Error(object sender, ErrorEventArgs e)
		{
			Console.WriteLine(e.Description);
		}
	}
#endregion
}

