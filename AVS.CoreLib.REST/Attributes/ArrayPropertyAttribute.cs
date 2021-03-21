using System;

namespace AVS.CoreLib.REST.Attributes
{
    /// <summary>
    /// Instructs <see cref="T:AVS.CoreLib.Json.ArrayConverter" /> how to serialize/deserialize json array
    /// </summary>
    public class ArrayPropertyAttribute : Attribute
    {
        public int Index { get; set; }

        /// <summary>
        /// exclusive means either the property will be written
        /// (the rest of content props with higher Index value is ignored)
        /// or the property is not written but the rest content is written
        /// </summary>
        public bool Exclusive { get; set; }

        public ArrayPropertyAttribute(int index, bool exclusive = false)
        {
            Index = index;
            //Optional = optional;
            Exclusive = exclusive;
        }
    }
}