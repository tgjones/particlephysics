using System;

namespace Particles.Physics.Ode
{
	/// <summary>
	/// Used to calculate the derivative of the values in y
	/// with respect to x.
	/// </summary>
	/// <param name="y">Array of values to calculate the derivatives of.</param>
	/// <param name="x">Value to derive with respect to (usually time).</param>
	/// <returns>Array of the derivatives of the values of y.</returns>
	public delegate float[] CalculateDerivativesHandler(float[] y, float x);
}
