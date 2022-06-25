using System;
using System.Globalization;
using AVS.CoreLib.Math.Extensions;

namespace AVS.CoreLib.Math.MathUtils.Fractions
{
    public struct Fraction : IComparable<Fraction>
    {
        private bool _isPositive;
        public int Sign
        {
            get => _isPositive ? 1 : -1;
            set => _isPositive = value == 1;
        }

        /// <summary>
        /// number
        /// </summary>
        public readonly ulong Numerator;

        private ulong? _d;

        /// <summary>
        /// denominator
        /// </summary>
        public ulong Denominator
        {
            get
            {
                _d ??= 1;
                return _d.Value;
            }
        }

        public double Value => (double)Numerator / Denominator * Sign;

        public ulong Rest => Numerator % Denominator;

        public bool IsPrime => Denominator == 1L && Numerator.IsPrime();

        public bool IsFractional => Rest > 0;
        public bool IsZero => Numerator == 0;
        public bool IsOne => Numerator == 1 && Denominator == 1 || Numerator == Denominator;

        public bool IsHalf => Numerator == 1 && Denominator == 2;

        public bool IsCorrect => Numerator < Denominator;

        public Fraction Inverse => Numerator == 0 ? this : new Fraction(Denominator, Numerator, Sign);

        public bool IsRepeating => Value.ToString(CultureInfo.InvariantCulture).Length >= 18;

        public Fraction(ulong numerator, ulong d = 1, int sign = 1)
        {
            Numerator = numerator;
            _d = d;
            _isPositive = sign == 1;
        }

        public Fraction(long n, long d = 1)
        {
            Numerator = (ulong)System.Math.Abs(n);
            _d = (ulong)System.Math.Abs(d);
            _isPositive = true;
            if (n < 0)
            {
                _isPositive = !_isPositive;
            }
            if (d < 0)
            {
                _isPositive = !_isPositive;
            }
        }

        public static explicit operator long(Fraction fraction)
        {
            if (fraction.IsFractional)
            {
                throw new InvalidCastException("fraction can't be converted to int without loss");
            }

            var d = fraction.Numerator / fraction.Denominator;
            if (d > long.MaxValue)
                throw new InvalidCastException("fraction value greater than long.MaxValue");
            var l = (long)d * fraction.Sign;
            return l;
        }

        public static implicit operator Fraction(int n)
        {
            return new Fraction((ulong)n);
        }

        public ulong ToUInt64() => Numerator / Denominator;
        public long ToInt64() => (long)this;

        public double ToDouble()
        {
            return ((double)Numerator / Denominator);
        }

        public override string ToString()
        {
            if (Numerator == 0)
                return "0";
            if (Denominator == 1)
                return Numerator.ToString();
            if (Rest == 0)
                return (Numerator / Denominator).ToString();

            return $"{Numerator}/{Denominator}";
        }

        public string ToString(string format)
        {
            if (Numerator == 0)
                return "0";
            if (Denominator == 1)
                return Numerator.ToString();
            if (Rest == 0)
                return (Numerator / Denominator).ToString();

            if (format == "d")
            {
                return Value.ToString();
            }

            return ToString();
        }

        #region opeartors
        public static Fraction operator +(Fraction a) => a;
        public static Fraction operator -(Fraction a) => new Fraction(a.Numerator, a.Denominator, -a.Sign);

        public static Fraction operator +(Fraction a, Fraction b)
            => new Fraction(a.Numerator * b.Denominator + b.Numerator * a.Denominator, a.Denominator * b.Denominator);

        public static Fraction operator -(Fraction a, Fraction b)
            => a + (-b);

        public static Fraction operator *(Fraction a, Fraction b)
            => new Fraction(a.Numerator * b.Numerator, a.Denominator * b.Denominator);

        public static Fraction operator /(Fraction a, Fraction b)
        {
            if (b.Numerator == 0)
            {
                throw new DivideByZeroException();
            }
            return new Fraction(a.Numerator * b.Denominator, a.Denominator * b.Numerator);
        }

        /// <summary>
        /// overloaed user defined conversions: from fractions to double and string
        /// </summary>
        public static explicit operator double(Fraction frac)
        { return frac.ToDouble(); }

        public static implicit operator string(Fraction frac)
        { return frac.ToString(); }

        /// <summary>
        /// overloaed user defined conversions: from numeric data types to Fractions
        /// </summary>
        public static implicit operator Fraction(long value)
        {
            if (value >= 0)
                return new Fraction((ulong)value);
            var l = (long)-value;
            return new Fraction();
        }
        public static implicit operator Fraction(double value)
        { return ToFraction(value); }
        public static implicit operator Fraction(string strNo)
        { return ToFraction(strNo); }

        public static bool operator ==(Fraction frac1, Fraction frac2)
        { return frac1.Equals(frac2); }

        public static bool operator !=(Fraction frac1, Fraction frac2)
        { return (!frac1.Equals(frac2)); }

        public static bool operator ==(Fraction frac1, int iNo)
        { return frac1.Equals(new Fraction(iNo)); }

        public static bool operator !=(Fraction frac1, int iNo)
        { return (!frac1.Equals(new Fraction(iNo))); }

        public static bool operator ==(Fraction frac1, double dbl)
        { return frac1.Equals(ToFraction(dbl)); }

        public static bool operator !=(Fraction frac1, double dbl)
        { return (!frac1.Equals(ToFraction(dbl))); }

        public static bool operator <(Fraction frac1, Fraction frac2)
        { return frac1.Value < frac2.Value; }

        public static bool operator >(Fraction frac1, Fraction frac2)
        { return frac1.Value > frac2.Value; }

        public static bool operator <=(Fraction frac1, Fraction frac2)
        { return frac1.Value <= frac2.Value; }

        public static bool operator >=(Fraction frac1, Fraction frac2)
        { return frac1.Value >= frac2.Value; }

        public static bool operator ==(Fraction frac1, string fraction)
        {
            if (TryParse(fraction, out Fraction frac2))
            {
                return frac1 == frac2;
            }

            return false;
        }

        public static bool operator !=(Fraction frac1, string fraction)
        {
            return !(frac1 == fraction);
        }

        public static bool operator >=(Fraction frac1, string fraction)
        {
            if (TryParse(fraction, out Fraction frac2))
            {
                return frac1 >= frac2;
            }

            throw new FractionException($"{fraction} is not a valid Fraction");
        }

        public static bool operator <=(Fraction frac1, string fraction)
        {
            if (TryParse(fraction, out Fraction frac2))
            {
                return frac1 <= frac2;
            }

            throw new FractionException($"{fraction} is not a valid Fraction");
        }

        #endregion

        /// <summary>
        /// checks whether two fractions are equal
        /// </summary>
        public override bool Equals(object obj)
        {
            var frac = (Fraction)obj;
            if (Numerator == 0 && frac.Numerator == 0)
                return true;
            return (Numerator == frac.Numerator && Denominator == frac.Denominator && Sign == frac.Sign);
        }

        /// <summary>
        /// returns a hash code for this fraction
        /// </summary>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// The function takes an string as an argument and returns its corresponding reduced fraction
        /// the string can be an in the form of and integer, double or fraction.
        /// e.g it can be like "123" or "123.321" or "123/456"
        /// </summary>
        public static Fraction ToFraction(string strValue)
        {
            int i;
            for (i = 0; i < strValue.Length; i++)
                if (strValue[i] == '/')
                    break;
            // if string is not in the form of a fraction
            // then it is double or integer
            if (i == strValue.Length)
                return (Convert.ToDouble(strValue));

            // else string is in the form of Numerator/Denominator
            var iNumerator = Convert.ToInt64(strValue.Substring(0, i));
            var iDenominator = Convert.ToInt64(strValue.Substring(i + 1));
            return new Fraction(iNumerator, iDenominator);
        }

        /// <summary>
        /// The function takes a floating point number as an argument 
        /// and returns its corresponding reduced fraction
        /// </summary>
        public static Fraction ToFraction(double d)
        {
            try
            {

                var sign = 1;
                var dValue = d;
                if (dValue < 0)
                {
                    sign = -1;
                    dValue = d * -1.00;
                }
                checked
                {
                    Fraction frac;
                    if (dValue % 1 == 0)    // if whole number
                    {
                        frac = new Fraction((ulong)dValue, 1UL, sign);
                    }
                    else
                    {
                        double dTemp = dValue;
                        ulong iMultiple = 1;
                        string strTemp = dValue.ToString();
                        while (strTemp.IndexOf("E") > 0)    // if in the form like 12E-9
                        {
                            dTemp *= 10;
                            iMultiple *= 10;
                            strTemp = dTemp.ToString();
                        }
                        int i = 0;
                        while (strTemp[i] != '.')
                            i++;
                        int iDigitsAfterDecimal = strTemp.Length - i - 1;
                        while (iDigitsAfterDecimal > 0)
                        {
                            dTemp *= 10;
                            iMultiple *= 10;
                            iDigitsAfterDecimal--;
                        }
                        frac = new Fraction((ulong)System.Math.Round(dTemp), iMultiple, sign);
                    }
                    return frac;
                }
            }
            catch (OverflowException)
            {
                throw new FractionException("Conversion not possible due to overflow");
            }
            catch (Exception)
            {
                throw new FractionException("Conversion not possible");
            }
        }

        public static bool TryParse(string input, out Fraction fraction)
        {
            fraction = default;
            var parts = input.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                var sign = 1;
                var numeratorStr = parts[0];
                if (parts[0].StartsWith("-"))
                {
                    sign = sign * -1;
                    numeratorStr = parts[0].Substring(1);
                }

                var dStr = parts[1];
                if (parts[1].StartsWith("-"))
                {
                    sign = sign * -1;
                    dStr = parts[1].Substring(1);
                }
                if (!ulong.TryParse(numeratorStr, out ulong n) || !ulong.TryParse(dStr, out ulong d) || d <= 0)
                {
                    return false;
                }

                fraction = new Fraction(n, d, sign);
                return true;
            }

            if (parts.Length == 1)
            {
                var sign = 1;
                var numeratorStr = parts[0];
                if (parts[0].StartsWith("-"))
                {
                    sign = sign * -1;
                    numeratorStr = parts[0].Substring(1);
                }
                if (ulong.TryParse(numeratorStr, out ulong n))
                {
                    fraction = new Fraction(n, 1UL, sign);
                    return true;
                }
            }

            return false;
        }

        public int CompareTo(Fraction other)
        {
            return Value.CompareTo(other.Value);
        }
    }
}