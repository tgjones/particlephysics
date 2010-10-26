using System;

namespace Particles.Physics.Ode
{
	/// <summary>
	/// Used to determine which numerical integrator
	/// is used by the Solver class
	/// </summary>
	public enum Integrator
	{
		/// <summary>
		/// Euler integration
		/// </summary>
		Euler,

		/// <summary>
		/// 2 step Runge-Kutta
		/// </summary>
		RK2,

		/// <summary>
		/// 4 step Runge-Kutta
		/// </summary>
		RK4
	}
}
