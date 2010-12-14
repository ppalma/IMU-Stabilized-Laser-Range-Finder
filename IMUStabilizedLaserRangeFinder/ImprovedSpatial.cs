
using System;
using Phidgets; 
using Phidgets.Events;
using System.Collections.Generic;
namespace IMUStabilizedLaserRangeFinder
{
	public class SpatialAccEventArgs: EventArgs
	{
		public SpatialAccEventArgs(double pitch, double roll)
		{
			this.pitch = pitch;
			this.roll = roll;
			
		}
		double roll,pitch;
		
		public double Roll {
			get {
				return roll;
			}
			set {
				roll = value;
			}
		}
		
		
		public double Pitch {
			get {
				return pitch;
			}
			set {
				pitch = value;
			}
		}
		
		
		
	}
	public delegate void SpatialAccChangeEventHandler(object sender, SpatialAccEventArgs e );

	public class ImprovedSpatial
	{
	public event SpatialAccChangeEventHandler SpatialAccChanged;

		public ImprovedSpatial ()
		{
		}
		Spatial spatial ;
		public void Initialization()
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
		public void Close()
		{
			spatial.close();
			spatial = null;
			Console.WriteLine("ok");
		}
		double accx,accy,accz;
		double gyrox,gyroy,gyroz;
		double compassx,compassy,compassz;
		 double[] lastMsCount = { 0, 0, 0 };
        bool[] lastMsCountGood = { false, false, false };
        double[] gyroHeading = { 0, 0, 0 }; //degrees
//
        List<double[]> compassBearingFilter = new List<double[]>();
        int compassBearingFilterSize = 10;
        double compassBearing = 0;
		double pitch,roll;
//
//        const double ambientMagneticField = 0.57142; //Calgary
//        const double ambientGravity = 1; //in G's
		
		void spatial_SpatialData(object sender, SpatialDataEventArgs e)
		{
			if (spatial.accelerometerAxes.Count > 0)
			{
				accx = e.spatialData[0].Acceleration[0];
				accy = e.spatialData[0].Acceleration[1];
				accz = e.spatialData[0].Acceleration[2];
			}
			
			if(spatial.gyroAxes.Count > 0)
			{
				calculateGyroHeading(e.spatialData, 0); //x axis
				calculateGyroHeading(e.spatialData, 1); //y axis
				calculateGyroHeading(e.spatialData, 2); //z axis
				
				gyrox = gyroHeading[0];
				gyroy = gyroHeading[1];
				gyroz = gyroHeading[2];
				
			}
			
			//Even when there is a compass chip, sometimes there won't be valid data in the event.
			if (spatial.compassAxes.Count > 0 && e.spatialData[0].MagneticField.Length > 0)
			{
				compassx = spatial.compassAxes[0].MagneticField;
				compassy = spatial.compassAxes[1].MagneticField;
				compassz = spatial.compassAxes[2].MagneticField;
				
				try
				{
					calculateCompassBearing();
				}
				catch
				{
				}
			}
//			Console.WriteLine("{0} {1}", Roll.ToString("F3"), Pitch.ToString("F3"));
//			Console.WriteLine(Pitch.ToString("F3"));
//			SpatialAccEventHandler(this,new SpatialAccEventArgs(Pitch,Roll));
			SpatialAccChanged(this,new SpatialAccEventArgs(pitch,roll));
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
		   void calculateGyroHeading(SpatialEventData[] data, int index)
        {
            double gyro = 0;
            for (int i = 0; i < data.Length; i++)
            {
                gyro = data[i].AngularRate[index];

                if (lastMsCountGood[index])
                {
                    //calculate heading
                    double timechange = data[i].Timestamp.TotalMilliseconds - lastMsCount[index]; // in ms
                    double timeChangeSeconds = (double)timechange / 1000.0;

                    gyroHeading[index] += timeChangeSeconds * gyro;
                }

                lastMsCount[index] = data[i].Timestamp.TotalMilliseconds;
                lastMsCountGood[index] = true;
            }
        }
		//This finds a magnetic north bearing, correcting for board tilt and roll as measured by the accelerometer
		//This doesn't account for dynamic acceleration - ie accelerations other then gravity will throw off the calculation
		double lastBearing=0;
		public double Roll {
			get {
				return roll;
			}
			set {
				roll = value;
			}
		}
		
		
		public double Pitch {
			get {
				return pitch;
			}
			set {
				pitch = value;
			}
		}
		
		
		void calculateCompassBearing()
		{
			double Xh = 0;
			double Yh = 0;
			
			//find the tilt of the board wrt gravity
			Vector3 gravity = Vector3.Normalize(
                new Vector3(
			                                                spatial.accelerometerAxes[0].Acceleration,
			                                                spatial.accelerometerAxes[2].Acceleration,
			                                                spatial.accelerometerAxes[1].Acceleration)
			                                    );
			
			double pitchAngle = Math.Asin(gravity.X);
			double rollAngle = Math.Asin(gravity.Z);
			
            //The board is up-side down
			if (gravity.Y < 0)
            {
				pitchAngle = -pitchAngle;
				rollAngle = -rollAngle;
			}
			
			//Construct a rotation matrix for rotating vectors measured in the body frame, into the earth frame
			//this is done by using the angles between the board and the gravity vector.
			Matrix3x3 xRotMatrix = new Matrix3x3();
			xRotMatrix.matrix[0, 0] = Math.Cos(pitchAngle); xRotMatrix.matrix[1, 0] = -Math.Sin(pitchAngle); xRotMatrix.matrix[2, 0] = 0;
			xRotMatrix.matrix[0, 1] = Math.Sin(pitchAngle); xRotMatrix.matrix[1, 1] = Math.Cos(pitchAngle); xRotMatrix.matrix[2, 1] = 0;
			xRotMatrix.matrix[0, 2] = 0; xRotMatrix.matrix[1, 2] = 0; xRotMatrix.matrix[2, 2] = 1;
			
			Matrix3x3 zRotMatrix = new Matrix3x3();
			zRotMatrix.matrix[0, 0] = 1; zRotMatrix.matrix[1, 0] = 0; zRotMatrix.matrix[2, 0] = 0;
			zRotMatrix.matrix[0, 1] = 0; zRotMatrix.matrix[1, 1] = Math.Cos(rollAngle); zRotMatrix.matrix[2, 1] = -Math.Sin(rollAngle);
			zRotMatrix.matrix[0, 2] = 0; zRotMatrix.matrix[1, 2] = Math.Sin(rollAngle); zRotMatrix.matrix[2, 2] = Math.Cos(rollAngle);
			
			Matrix3x3 rotMatrix = Matrix3x3.Multiply(xRotMatrix, zRotMatrix);
			
			Vector3 data = new Vector3(spatial.compassAxes[0].MagneticField, spatial.compassAxes[2].MagneticField, -spatial.compassAxes[1].MagneticField);
			Vector3 correctedData = Matrix3x3.Multiply(data, rotMatrix);
			
			//These represent the x and y components of the magnetic field vector in the earth frame
			Xh = -correctedData.Z;
			Yh = -correctedData.X;
			
            //we use the computed X-Y to find a magnetic North bearing in the earth frame
			try
			{
				double bearing = 0;
				double _360inRads = (360 * Math.PI / 180.0);
				if (Xh < 0)
					bearing = Math.PI - Math.Atan(Yh / Xh);
                else {
					
				} if (Xh > 0 && Yh < 0)
					bearing = -Math.Atan(Yh / Xh);
				else {
					
				} if (Xh > 0 && Yh > 0)
					bearing = Math.PI * 2 - Math.Atan(Yh / Xh);
				else if (Xh == 0 && Yh < 0)
					bearing = Math.PI / 2.0;
				else if (Xh == 0 && Yh > 0)
					bearing = Math.PI * 1.5;
				
                //The board is up-side down
				if (gravity.Y < 0)
				{
					bearing = Math.Abs(bearing - _360inRads);
				}
				
                //passing the 0 <-> 360 point, need to make sure the filter never contains both values near 0 and values near 360 at the same time.
				if (Math.Abs(bearing - lastBearing) > 2) //2 radians == ~115 degrees
				{
					if(bearing > lastBearing)
						foreach (double[] stuff in compassBearingFilter)
							stuff[0] += _360inRads;
					else
						foreach (double[] stuff in compassBearingFilter)
							stuff[0] -= _360inRads;
				}
				
				compassBearingFilter.Add(new double[] { bearing, pitchAngle, rollAngle });
				if (compassBearingFilter.Count > compassBearingFilterSize)
					compassBearingFilter.RemoveAt(0);
				
				bearing = pitchAngle = rollAngle = 0;
				foreach (double[] stuff in compassBearingFilter)
				{
					bearing += stuff[0];
					pitchAngle += stuff[1];
					rollAngle += stuff[2];
				}
				bearing /= compassBearingFilter.Count;
				pitchAngle /= compassBearingFilter.Count;
				rollAngle /= compassBearingFilter.Count;
				
				compassBearing = bearing * (180.0 / Math.PI);
				lastBearing = bearing;
				
//				bearingTxt.Text = (bearing * (180.0 / Math.PI)).ToString("F1") + "°";
				roll = (pitchAngle * (180.0 / Math.PI));
				pitch = (rollAngle * (180.0 / Math.PI));
			}
			catch { }
		}
		
	}
}

