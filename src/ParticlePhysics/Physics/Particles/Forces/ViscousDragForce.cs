using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Particles.Physics.Particles.Forces
{
	public class ViscousDragForce : UnaryForce
	{
		public float DragCoefficient;

		public ViscousDragForce(float dragCoefficient)
		{
			DragCoefficient = dragCoefficient;
		}

		public override void ApplyForce()
		{
			Particle.ForceAccumulator -= DragCoefficient * Particle.Velocity;
		}
	}
}
