using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Particles.Physics.Particles.Forces;
using Particles.Physics.Ode;

namespace Particles.Physics.Particles
{
	public class ParticleSystem
	{
		public List<Particle> Particles;
		public float Time;

		public List<Force> Forces;

		private int Dimensions
		{
			get { return 6 * Particles.Count; }
		}

		public ParticleSystem()
		{
			Particles = new List<Particle>();
			Forces = new List<Force>();
		}

		/// <summary>
		/// Copies the current state of the particle system into
		/// a generic float array. This float array is then
		/// passed to the ODE solver.
		/// </summary>
		/// <returns>Float array containing current state.</returns>
		private float[] CopyStateToArray()
		{
			float[] result = new float[Dimensions];
			int counter = 0;
			foreach (Particle particle in Particles)
			{
				result[counter++] = particle.Position.X;
				result[counter++] = particle.Position.Y;
				result[counter++] = particle.Position.Z;
				result[counter++] = particle.Velocity.X;
				result[counter++] = particle.Velocity.Y;
				result[counter++] = particle.Velocity.Z;
			}
			return result;
		}

		/// <summary>
		/// Copies the array passed in to the current state.
		/// </summary>
		/// <param name="array">Array that is returned
		/// from the ODE solver.</param>
		private void CopyArrayToState(float[] state)
		{
			int counter = 0;
			for (int i = 0; i < Particles.Count; i++)
			{
				if (!(Particles[i] is ControlledParticle))
				{
					Particles[i].Position.X = state[counter++];
					Particles[i].Position.Y = state[counter++];
					Particles[i].Position.Z = state[counter++];
					Particles[i].Velocity.X = state[counter++];
					Particles[i].Velocity.Y = state[counter++];
					Particles[i].Velocity.Z = state[counter++];
				}
				else
				{
					counter += 6;
				}
			}
		}

		/// <summary>
		/// Calculates the derivative of the state
		/// </summary>
		/// <param name="y">Initial state</param>
		/// <param name="x">Value to differentiate y with respect to</param>
		/// <returns>Derivative of state</returns>
		private float[] CalculateDerivatives(float[] y, float x)
		{
			CopyArrayToState(y);

			float[] result = new float[Dimensions];
			int counter = 0;
			for (int i = 0, length = Particles.Count; i < length; i++)
			{
				// xdot = V
				result[counter++] = Particles[i].Velocity.X;
				result[counter++] = Particles[i].Velocity.Y;
				result[counter++] = Particles[i].Velocity.Z;

				// vdot = f / m
				result[counter++] = Particles[i].ForceAccumulator.X * Particles[i].MassInverse;
				result[counter++] = Particles[i].ForceAccumulator.Y * Particles[i].MassInverse;
				result[counter++] = Particles[i].ForceAccumulator.Z * Particles[i].MassInverse;
			}

			return result;
		}

		private void CalculateForces()
		{
			// Hardcode gravity for all particles.
			foreach (Particle particle in Particles)
				particle.ForceAccumulator += Vector3.Down * 9.81f * particle.Mass;

			// TODO: Check performance of foreach loop.
			foreach (Force force in Forces)
				force.ApplyForce();
		}

		public void Update(float deltaTime)
		{
			CalculateForces();

			// Integrate.
			float[] initial = CopyStateToArray();
			float[] final = OdeSolver.Solve(initial, Time, deltaTime,
				new CalculateDerivativesHandler(CalculateDerivatives),
				Integrator.RK4);
			CopyArrayToState(final);

			// Update time. BODGE.
			Time += deltaTime;

			// Check for collisions. TODO: Loop through solver again.
			DoCollisionDetection();

			// Zero the force accumulators.
			Particles.ForEach(p => p.ForceAccumulator = Vector3.Zero);
		}

		private void DoCollisionDetection()
		{
			// Detect collisions. Currently hardcoded to check
			// for collisions with the ground plane.
			foreach (Particle particle in Particles.Where(p => p.Position.Y < 0))
			{
				Vector3 vn = Vector3.Dot(Vector3.Up, particle.Velocity) * Vector3.Up;
				Vector3 vt = particle.Velocity - vn;

				// Apply coefficient of restitution.
				const float e = 0.6f;
				particle.Velocity = (vn * -e) + vt;

				particle.Position.Y = 0;
			}
		}
	}
}
