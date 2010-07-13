using System;

namespace Particles.Physics.Ode
{
	/// <summary>
	/// This class implements functions that solve first-order
	/// ordinary differential equations (ODEs).
	/// </summary>
	public sealed class OdeSolver
	{
		/// <summary>
		/// This method solves first-order ODEs. It is passed an array
		/// of dependent variables in "array" at x, and returns an
		/// array of new values at x + h.
		/// </summary>
		/// <param name="initial">Array of initial values</param>
		/// <param name="x">Initial value of x (usually time)</param>
		/// <param name="h">Increment value for x (usually delta time)</param>
		/// <param name="system">Class containing a method to calculate the
		/// derivatives of the initial values</param>
		/// <param name="integrator">Type of numeric
		/// integration to use</param>
		/// <returns></returns>
		public static float[] Solve(float[] initial, float x, float h,
			CalculateDerivativesHandler calculateDerivatives,
			Integrator integrator)
		{
			switch (integrator)
			{
				case Integrator.Euler :
					return SolveEuler(initial, x, h, calculateDerivatives);
				case Integrator.RK2 :
					return SolveRK2(initial, x, h, calculateDerivatives);
				case Integrator.RK4 :
					goto default;
				default :
					return SolveRK4(initial, x, h, calculateDerivatives);
			}
		}

		/// <summary>
		/// Processes a simple Euler step
		/// </summary>
		/// <param name="initial"></param>
		/// <param name="h"></param>
		/// <returns></returns>
		private static float[] DoEulerStep(float[] initial, float[] derivs, float h)
		{
			int len = initial.Length;
			float[] ret = new float[len];
			for (int i = 0; i < len; i++)
			{
				ret[i] = initial[i] + derivs[i] * h;
			}
			return ret;
		}

		/// <summary>
		/// Solves an ODE using Euler integration
		/// </summary>
		/// <param name="initial"></param>
		/// <param name="x"></param>
		/// <param name="h"></param>
		/// <param name="system"></param>
		/// <returns></returns>
		private static float[] SolveEuler(float[] initial, float x, float h, CalculateDerivativesHandler calculateDerivatives)
		{
			// final = initial + derived * h
			float[] derivs = calculateDerivatives(initial, x);
			return DoEulerStep(initial, derivs, h);
		}

		/// <summary>
		/// Solves an ODE using RK2
		/// </summary>
		/// <param name="initial"></param>
		/// <param name="x"></param>
		/// <param name="h"></param>
		/// <param name="system"></param>
		/// <returns></returns>
		private static float[] SolveRK2(float[] initial, float x, float h, CalculateDerivativesHandler calculateDerivatives)
		{
			// do Euler step with half the step value
			float[] k1 = calculateDerivatives(initial, x);
			float[] temp = DoEulerStep(initial, k1, h / 2.0f);

			// calculate again at midpoint
			float[] k2 = calculateDerivatives(temp, x + (h / 2.0f));
			
			// use derivatives for complete timestep
			return DoEulerStep(initial, k2, h);
		}

		/// <summary>
		/// Solves an ODE using RK4
		/// </summary>
		/// <param name="initial"></param>
		/// <param name="x"></param>
		/// <param name="h"></param>
		/// <param name="system"></param>
		/// <returns></returns>
		private static float[] SolveRK4(float[] initial, float x, float h, CalculateDerivativesHandler calculateDerivatives)
		{
			float[] k1 = calculateDerivatives(initial, x);
			float[] temp = DoEulerStep(initial, k1, h / 2.0f);

			float[] k2 = calculateDerivatives(temp, x + (h / 2.0f));
			temp = DoEulerStep(initial, k2, h / 2.0f);

			float[] k3 = calculateDerivatives(temp, x + (h / 2.0f));
			temp = DoEulerStep(initial, k3, h);

			float[] k4 = calculateDerivatives(temp, x + h);

			float[] ret = DoEulerStep(initial, k1, h / 6.0f);
			ret = DoEulerStep(ret, k2, h / 3.0f);
			ret = DoEulerStep(ret, k3, h / 3.0f);
			ret = DoEulerStep(ret, k4, h / 6.0f);
			return ret;
		}
	}
}
