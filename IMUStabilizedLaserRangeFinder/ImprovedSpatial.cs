
using System;
using Phidgets; 
using Phidgets.Events;
using System.Collections.Generic;
using MBF.Math;
using MBF.Core;
using MBF.Sensors;
//using ISE;



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

	
	public class ImprovedSpatial:Spatial
	{
		public event SpatialAccChangeEventHandler SpatialAccChanged;
		
		public ImprovedSpatial ()
		{
			
		}
		public void Initialization()
		{
			this.open();
		}
	}
}

	