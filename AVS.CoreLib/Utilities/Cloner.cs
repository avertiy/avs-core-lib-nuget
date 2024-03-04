using System;
using System.Reflection;
using System.Reflection.Emit;

namespace AVS.CoreLib.Utilities
{
    public static class Cloner
    {
        public static T Clone<T>(T obj) where T : new()
        {
            if (obj is ICloneable cloneable)
                return (T)cloneable.Clone();

            return Cloner<T>.Clone(obj);
        }

        public static TInterface Clone<TInterface, TImplementation>(TInterface obj)
            where TImplementation : TInterface, new()
        {
            if (obj is ICloneable cloneable)
                return (TInterface)cloneable.Clone();

            return Cloner<TImplementation>.Clone(((TImplementation)obj!));
        }
    }

    public static class Cloner<T> where T : new()
    {
        private static Func<T, T> cloner = CreateCloner();

        private static Func<T, T> CreateCloner()
        {
            var cloneMethod = new DynamicMethod("CloneImplementation", typeof(T), new Type[] { typeof(T) }, true);
            var defaultCtor = typeof(T).GetConstructor(new Type[] { });

            var generator = cloneMethod.GetILGenerator();

            var loc1 = generator.DeclareLocal(typeof(T));

            generator.Emit(OpCodes.Newobj, defaultCtor);
            generator.Emit(OpCodes.Stloc, loc1);

            foreach (var field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                generator.Emit(OpCodes.Ldloc, loc1);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, field);
                generator.Emit(OpCodes.Stfld, field);
            }

            generator.Emit(OpCodes.Ldloc, loc1);
            generator.Emit(OpCodes.Ret);

            return ((Func<T, T>)cloneMethod.CreateDelegate(typeof(Func<T, T>)));
        }

        public static T Clone(T myObject)
        {
            return cloner(myObject);
        }
    }
}