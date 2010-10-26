using System;
using Microsoft.Xna.Framework;

namespace Particles.Physics.Particles
{
	public class Particle
	{
		public float MassInverse;
		public Vector3 Position;
		public Vector3 Velocity;
		public Vector3 ForceAccumulator;

		public float Mass
		{
			get { return (MassInverse == 0) ? 0 : 1.0f / MassInverse; }
		}
	}
}
