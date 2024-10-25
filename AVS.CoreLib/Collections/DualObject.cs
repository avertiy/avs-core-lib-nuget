using System;
using System.Diagnostics;

namespace AVS.CoreLib.Collections
{
    /// <summary>
    /// Represents a class that can hold values of two types
    /// </summary>
    /// <typeparam name="TValue1">The first type</typeparam>
    /// <typeparam name="TValue2">The second type</typeparam>
    [DebuggerDisplay("DualObject [Value1: {Value1}; Value2: {Value2}]")]
    [Serializable]
    public class DualObject<TValue1, TValue2>
        : IEquatable<DualObject<TValue1, TValue2>>
    {
        /// <summary>
        /// Initializes a new instance of the DualObject class
        /// </summary>
        /// <param name="value1">First value</param>
        /// <param name="value2">Second value</param>
        public DualObject(TValue1 value1, TValue2 value2)
        {
            this.Value1 = value1;
            this.Value2 = value2;
        }

        /// <summary>
        /// Initializes a new instance of the DualObject class
        /// </summary>
        /// <param name="value2">Second value</param>
        /// <param name="value1">First value</param>
        public DualObject(TValue2 value2, TValue1 value1)
            : this(value1, value2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DualObject class
        /// </summary>
        /// <param name="value">First value</param>
        public DualObject(TValue1 value)
        {
            this.Value1 = value;
        }

        /// <summary>
        /// Initializes a new instance of the DualObject class
        /// </summary>
        /// <param name="value">Second value</param>
        public DualObject(TValue2 value)
        {
            this.Value2 = value;
        }

        private bool _isFirstValueSet;

        private TValue1? _value1;

        public TValue1? Value1
        {
            get
            {
                return this._value1;
            }

            set
            {
                this._value1 = value;
                this._isFirstValueSet = true;
            }
        }

        private bool _isSecondValueSet;

        private TValue2? _value2;

        public TValue2? Value2
        {
            get
            {
                return this._value2;
            }

            set
            {
                this._value2 = value;
                this._isSecondValueSet = true;
            }
        }

        /// <summary>
        /// Implicit conversion operator to convert to T1
        /// </summary>
        /// <param name="dualObject">The DualObject to convert from</param>
        /// <returns>The converted object</returns>
        public static implicit operator TValue1?(
            DualObject<TValue1, TValue2> dualObject)
        {
            return dualObject.Value1;
        }

        /// <summary>
        /// Implicit conversion operator to convert to T2
        /// </summary>
        /// <param name="dualObject">The DualObject to convert from</param>
        /// <returns>The converted object</returns>
        public static implicit operator TValue2?(
            DualObject<TValue1, TValue2> dualObject)
        {
            return dualObject.Value2;
        }

        /// <summary>
        /// Implicit conversion operator to convert from T1
        /// </summary>
        /// <param name="value">The object to convert from</param>
        /// <returns>The converted DualObject</returns>
        public static implicit operator DualObject<TValue1, TValue2>(
            TValue1 value)
        {
            return new DualObject<TValue1, TValue2>(value);
        }

        /// <summary>
        /// Implicit conversion operator to convert from T2
        /// </summary>
        /// <param name="value">The object to convert from</param>
        /// <returns>The converted DualObject</returns>
        public static implicit operator DualObject<TValue1, TValue2>(
            TValue2 value)
        {
            return new DualObject<TValue1, TValue2>(value);
        }

        /// <summary>
        /// Sets the value for the first type
        /// </summary>
        /// <param name="value">The value to set</param>
        public void Set(TValue1 value)
        {
            this.Value1 = value;
        }

        /// <summary>
        ///  Sets the value for the second type
        /// </summary>
        /// <param name="value">The value to set</param>
        public void Set(TValue2 value)
        {
            this.Value2 = value;
        }

        /// <summary>
        /// Sets the values from another DualObject
        /// </summary>
        /// <param name="dualObject">The DualObject 
        /// to copy the values from</param>
        public void Set(DualObject<TValue1, TValue2> dualObject)
        {
            if (dualObject._isFirstValueSet)
            {
                this.Value1 = dualObject.Value1;
            }

            if (dualObject._isSecondValueSet)
            {
                this.Value2 = dualObject.Value2;
            }
        }

        #region IEquatable<DualObject<T1,T2>> Members

        /// <summary>
        /// Indicates whether the current DualObject is 
        /// equal to another DualObject
        /// Note : This will return true if either one of the 
        /// values are equal.
        /// </summary>
        /// <param name="other">The DualObject to compare with</param>
        /// <returns>True if the objects are equal</returns>
        public bool Equals(DualObject<TValue1, TValue2>? other)
        {
            var firstEqual = this.Value1 == null ?
                other.Value1 == null :
                this.Value1.Equals(other.Value1);

            var secondEqual = this.Value2 == null ?
                other.Value2 == null :
                this.Value2.Equals(other.Value2);

            return firstEqual || secondEqual;
        }

        #endregion
    }
}