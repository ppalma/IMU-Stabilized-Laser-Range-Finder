using System;
using Phidgets; 
using Phidgets.Events;

namespace IMUStabilizedLaserRangeFinder
{
	class MainClass
	{
		static ImprovedSpatial spatial = new ImprovedSpatial();
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			try
			{
				 

				ServoInitialization();
				
				Console.WriteLine("Press any key to end");
				spatial.Initialization();
				spatial.SpatialAccChanged += new SpatialAccChangeEventHandler(GoStable);
//				GoStable();	
//					servo.servos[0].Position = 60;
//					System.Threading.Thread.Sleep(1000);
//				
//				for(double pos =  60 ; pos <130  ;pos++)
//				{
//					System.Threading.Thread.Sleep(1000);
//					servo.servos[0].Position = pos;
//					Console.WriteLine("{0} {1}", pos, pitch);
//				}
				char ch; 
			
				do { 
					ch = (char) System.Console.Read(); 
					switch(ch){
					case 'l':
						//					Console.WriteLine("asdfafsa");
						break;
						
					default:
						
						break;
					}
				} while(ch != 'q'); 
				//				Console.Read();
			
				spatial.Close();
				ServoStop();
				
				
				
			}
			catch (PhidgetException ex)
			{
				Console.WriteLine(ex.Description);
			}
		}
		static Servo servo;
//		static double accx,accy,accz;
		
		static double pitch =0;
		static double currentPosition =0;
		static double th = 1;
		
		static void GoStable(object  sender, SpatialAccEventArgs e)
		{
//			Console.WriteLine(e.Pitch);
			try{
				pitch = e.Pitch;
			
				if(pitch > (-1)*th && pitch < th)
				{
					Console.WriteLine("Stable");
				}
				else
				{
					if( pitch > (-1)*th) // move up
					{
						servo.servos[0].Position += Math.Abs(pitch);
						Console.WriteLine("Off set down {0} ", Math.Abs(pitch));
						System.Threading.Thread.Sleep(400);
					}
					
					if( pitch < th){
						Console.WriteLine("Off set up {0} ", Math.Abs(pitch));
						System.Threading.Thread.Sleep(400);
					
						servo.servos[0].Position -= Math.Abs(pitch);		
					}
				}
			}
			catch(Exception ex) {Console.WriteLine(ex.Message);}
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
//            Console.WriteLine("Servo {0} Position {1}", e.Index, e.Position);
			currentPosition = e.Position;
        }
		#endregion
		
	}
}

