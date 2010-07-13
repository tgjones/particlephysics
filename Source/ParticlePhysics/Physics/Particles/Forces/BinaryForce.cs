using System;

namespace Particles.Physics.Particles.Forces
{
	public abstract class BinaryForce : Force
	{
		public Particle ParticleA;
		public Particle ParticleB;
	}
}
