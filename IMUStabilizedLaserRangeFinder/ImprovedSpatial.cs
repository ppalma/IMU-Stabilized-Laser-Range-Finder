
using System;
using Phidgets; 
using Phidgets.Events;
namespace IMUStabilizedLaserRangeFinder
{


	public class ImprovedSpatial
	{

		public ImprovedSpatial ()
		{
		}
		Spatial spatial ;
		double accx,accy,accz;
			void SpatialInitialization()
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
		void SpatialClose()
		{
			spatial.close();
			spatial = null;
			Console.WriteLine("ok");
		}
		void spatial_SpatialData(object sender, SpatialDataEventArgs e)
		{
			if (e.spatialData[0].Acceleration.Length > 0)
			{
				accx = e.spatialData[0].Acceleration[2]; 
				accy = e.spatialData[0].Acceleration[1]; 
				accz = e.spatialData[0].Acceleration[0]; 
			}
//			double th = 0.05;
//			
//			if(accy > (-1)*th && accy < th)
//			{
//				Console.WriteLine(accy);
//				Console.WriteLine("Stable");
//			}
//			else
//			{
//				if(accy < (th * -1))
//					servo.servos[0].Position -= 5;
//				
//				if(accy > th)
//					servo.servos[0].Position += 5;
//				
//			}
			//			Console.WriteLine("{0} {1} {2}",accx,accy,accz);
//			Console.WriteLine("{0} {1} {2}",accy,accy,accy);
		}
		
		void accel_Attach(object sender, AttachEventArgs e)
		{
            Console.WriteLine("Spatial {0} attached!", 
			                  e.Device.SerialNumber.ToString());
		}
		void accel_Detach(object sender, DetachEventArgs e)
		{
			Console.WriteLine("Spatial {0} detached!", 
			                  e.Device.SerialNumber.ToString());
		}
		void accel_Error(object sender, ErrorEventArgs e)
		{
			Console.WriteLine(e.Description);
		}
	}
}

