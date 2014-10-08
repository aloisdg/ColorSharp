﻿/**
 * The MIT License (MIT)
 * Copyright (c) 2014 Andrés Correa Casablanca
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

/**
 * Contributors:
 *  - Andrés Correa Casablanca <castarco@gmail.com , castarco@litipk.com>
 */


using System;
using System.Collections.Generic;


namespace Litipk.ColorSharp
{
	public class LightSpectrum : AConvertibleColor
	{
		#region private properties

		// Needed values to interpret the data points
		double nmPerStep;
		double minWaveLength;
		double maxWaveLength;

		// Data points
		double[] amplitudes;
		#endregion

		#region constructors

		/**
		 * This constructor "installs" the conversor methods into the instance
		 */
		protected LightSpectrum(AConvertibleColor dataSource=null) : base(dataSource) {
			// TODO: Add conversors
		}

		// Constructor
		public LightSpectrum (double minWaveLength, double maxWaveLength, double[] amplitudes, AConvertibleColor dataSource=null) : this(dataSource)
		{
			this.minWaveLength = minWaveLength;
			this.maxWaveLength = maxWaveLength;
			nmPerStep = (maxWaveLength - minWaveLength) / (amplitudes.Length - 1);
			this.amplitudes = amplitudes;
		}

		// Constructor
		public LightSpectrum (double minWaveLength, double[] amplitudes, double nmPerStep, AConvertibleColor dataSource=null) : this(dataSource)
		{
			this.nmPerStep = nmPerStep;
			this.minWaveLength = minWaveLength;
			maxWaveLength = minWaveLength + (nmPerStep) * (amplitudes.Length - 1);
			this.amplitudes = amplitudes;
		}

		#endregion

		#region conversors

		/**
		 * 
		 */
		public CIEXYZ ToCIEXYZ (Dictionary<KeyValuePair<Type, Type>, object> strategies=null)
		{
			var strategyKey = new KeyValuePair<Type, Type> (
				typeof(LightSpectrum), typeof(CIEXYZ)
			);

			if (strategies == null || !strategies.ContainsKey(strategyKey)) {
				throw new ArgumentException (
					"Unable top find the proper strategy"
				);
			}

			var MFs = (IMatchingFunction[])strategies [strategyKey];

			if (MFs == null || MFs.Length != 3) {
				throw new ArgumentException (
					"Unable top find the matching functions"
				);
			}

			return new CIEXYZ (
				MFs[0].DoConvolution (this), MFs[1].DoConvolution (this), MFs[2].DoConvolution (this), this
			);
		}

		#endregion
	}
}