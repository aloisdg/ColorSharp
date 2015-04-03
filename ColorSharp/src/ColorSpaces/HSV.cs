/*
 * The MIT License (MIT)
 * Copyright (c) 2015 Alois de Gouvello
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

/*
 * Contributors:
 *  - Andrés Correa Casablanca <castarco@gmail.com , castarco@litipk.com>
 *  - Alois de Gouvello <aloisdegouvello@live.fr>
 */


using System;
using System.Linq.Expressions;
using Litipk.ColorSharp.Strategies;


namespace Litipk.ColorSharp
{
    namespace ColorSpaces
    {
        /**
         * <summary>HP's and Microsoft's 1996 sRGB Color Space.</summary>
         */
        public sealed class HSV : AConvertibleColor
        {
            #region private properties

            /**
			 * <value>Hue component</value>
			 */
            public readonly double H;

            /**
             * <value>Saturation component</value>
             */
            public readonly double S;

            /**
             * <value>Value component</value>
             */
            public readonly double V;

            #endregion


            #region constructors

            /**
			 * <summary>Creates a new color sample in the HSV color space</summary>
			 * <param name="H">Hue component (between 0 and 360)</param>
			 * <param name="S">Saturation component (between 0 and 1)</param>
			 * <param name="V">Value component (between 0 and 1)</param>
			 * <param name="dataSource">If you aren't working with ColorSharp internals, don't use this parameter</param>
			 */
            public HSV(double H, double S, double V, AConvertibleColor dataSource = null)
                : base(dataSource)
            {
                this.H = H;
                this.S = S;
                this.V = V;
            }

            #endregion


            #region AConvertibleColor methods

            /**
			 * <inheritdoc />
			 */
            public override bool IsInsideColorSpace(bool highPrecision = false)
            {
                return (
                    0.0 <= H && H <= 360.0 &&
                    0.0 <= S && S <= 1.0 &&
                    0.0 <= V && V <= 1.0
                );
            }

            /**
            * <inheritdoc />
            */
            public override CIEXYZ ToCIEXYZ()
            {
                return ToSRGB ().ToCIEXYZ ();
            }

            /**
             * <inheritdoc />
             */
            public override SRGB ToSRGB(ToSmallSpaceStrategy strategy = ToSmallSpaceStrategy.Default)
            {
                double h = H;
                double s = S;
                double v = V;

                // normalize the hue:
                while (h < 0)
                    h += 360;
                while (h > 360)
                    h -= 360;

                h /= 360;
                if (s > 0)
                {
                    if (h >= 1)
                        h = 0;
                    h *= 6;
                    int hueFloor = Convert.ToInt32 (Math.Floor (h));
                    byte a = ConvertToByte (v * (1.0 - s));
                    byte b = ConvertToByte (v * (1.0 - (s * (h - hueFloor))));
                    byte c = ConvertToByte (v * (1.0 - (s * (1.0 - (h - hueFloor)))));
                    byte d = ConvertToByte (v);

                    switch (hueFloor)
                    {
                        case 0: return new SRGB (d, c, a);
                        case 1: return new SRGB (b, d, a);
                        case 2: return new SRGB (a, d, c);
                        case 3: return new SRGB (a, b, d);
                        case 4: return new SRGB (c, a, d);
                        case 5: return new SRGB (d, a, b);
                        default: return new SRGB (0, 0, 0);
                    }
                }
                else
                {
                    byte d = (byte) (v * 255);
                    return new SRGB (d, d, d);
                }
            }

            /**
             * <inheritdoc />
             */
            public HSV ToHSV()
            {
                return this;
            }

            #endregion


            #region Object methods

            /**
			 * <inheritdoc />
			 */
            public override bool Equals(Object obj)
            {
                HSV srgbObj = obj as HSV;

                if (srgbObj == this)
                {
                    return true;
                }
                if (srgbObj == null || GetHashCode() != obj.GetHashCode())
                {
                    return false;
                }

                return (H == srgbObj.H && S == srgbObj.S && V == srgbObj.V);
            }

            /**
             * <inheritdoc />
             */
            public override int GetHashCode()
            {
                int hash = 32399 + H.GetHashCode(); // 32399 == 179 * 181

                hash = hash * 181 + S.GetHashCode();

                return hash * 181 + V.GetHashCode();
            }

            #endregion

            #region internal utilities
            
            static byte ConvertToByte(double num)
            {
                return (byte) Math.Round(255*num);
            }

            #endregion
        }
    }
}


