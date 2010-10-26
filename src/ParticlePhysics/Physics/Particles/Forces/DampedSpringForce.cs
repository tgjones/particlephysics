using System;
using Microsoft.Xna.Framework;

namespace Particles.Physics.Particles.Forces
{
	public class DampedSpringForce : BinaryForce
	{
		public float RestLength;

		/// <summary>
		/// Spring constant.
		/// </summary>
		public float Ks;

		/// <summary>
		/// Damping constant.
		/// </summary>
		public float Kd;

		public DampedSpringForce(float restLength, float springConstant, float dampingConstant)
		{
			RestLength = restLength;
			Ks = springConstant;
			Kd = dampingConstant;
		}

		public override void ApplyForce()
		{
			Vector3 l = ParticleA.Position - ParticleB.Position;
			Vector3 lDot = ParticleA.Velocity - ParticleB.Velocity;
			float lLength = l.Length();

			Vector3 force = -(Ks * (lLength - RestLength) + (Kd * (Vector3.Dot(lDot, l) / lLength)))
				* (l / lLength);

			ParticleA.ForceAccumulator += force;
			ParticleB.ForceAccumulator -= force;
		}
	}
}
