using System;
using System.Reflection;
using System.Reflection.Emit;
using JetBrains.Annotations;

namespace ExtendedStorage
{
    // Token: 0x0200000C RID: 12
    public static class Access
    {
        // Token: 0x06000041 RID: 65 RVA: 0x00002F18 File Offset: 0x00001118
        public static Func<T, P> GetPropertyGetter<T, P>([NotNull] string propertyName)
        {
            var getMethod = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetGetMethod(true);
            var dynamicMethod = new DynamicMethod(string.Empty, typeof(P), new[]
            {
                typeof(T)
            }, typeof(T));
            var ilgenerator = dynamicMethod.GetILGenerator();
            ilgenerator.Emit(OpCodes.Ldarg_0);
            ilgenerator.Emit(OpCodes.Callvirt, getMethod!);
            ilgenerator.Emit(OpCodes.Ret);
            return (Func<T, P>)dynamicMethod.CreateDelegate(typeof(Func<T, P>));
        }

        // Token: 0x06000042 RID: 66 RVA: 0x00002FAC File Offset: 0x000011AC
        public static Func<P> GetPropertyGetter<P>([NotNull] string propertyName, [NotNull] Type ownerType)
        {
            var getMethod = ownerType.GetProperty(propertyName, BindingFlags.Static | BindingFlags.NonPublic)
                ?.GetGetMethod(true);
            var dynamicMethod = new DynamicMethod(string.Empty, typeof(P), Type.EmptyTypes, ownerType);
            var ilgenerator = dynamicMethod.GetILGenerator();
            ilgenerator.Emit(OpCodes.Call, getMethod!);
            ilgenerator.Emit(OpCodes.Ret);
            return (Func<P>)dynamicMethod.CreateDelegate(typeof(Func<P>));
        }

        // Token: 0x06000043 RID: 67 RVA: 0x00003014 File Offset: 0x00001214
        public static Func<T, F> GetFieldGetter<T, F>([NotNull] string fieldName)
        {
            var field = typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            var dynamicMethod = new DynamicMethod(string.Empty, typeof(F), new[]
            {
                typeof(T)
            }, typeof(T));
            var ilgenerator = dynamicMethod.GetILGenerator();
            ilgenerator.Emit(OpCodes.Ldarg_0);
            ilgenerator.Emit(OpCodes.Ldfld, field!);
            ilgenerator.Emit(OpCodes.Ret);
            return (Func<T, F>)dynamicMethod.CreateDelegate(typeof(Func<T, F>));
        }

        // Token: 0x06000044 RID: 68 RVA: 0x000030A0 File Offset: 0x000012A0
        public static Func<F> GetFieldGetter<F>([NotNull] string fieldName, [NotNull] Type ownerType)
        {
            var field = ownerType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
            var dynamicMethod = new DynamicMethod(string.Empty, typeof(F), Type.EmptyTypes, ownerType);
            var ilgenerator = dynamicMethod.GetILGenerator();
            ilgenerator.Emit(OpCodes.Ldsfld, field!);
            ilgenerator.Emit(OpCodes.Ret);
            return (Func<F>)dynamicMethod.CreateDelegate(typeof(Func<F>));
        }

        // Token: 0x06000045 RID: 69 RVA: 0x00003104 File Offset: 0x00001304
        public static Action<T, F> GetFieldSetter<T, F>([NotNull] string fieldName)
        {
            var field = typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            var dynamicMethod = new DynamicMethod(string.Empty, null, new[]
            {
                typeof(T),
                typeof(F)
            }, typeof(T));
            var ilgenerator = dynamicMethod.GetILGenerator();
            ilgenerator.Emit(OpCodes.Ldarg_0);
            ilgenerator.Emit(OpCodes.Ldarg_1);
            ilgenerator.Emit(OpCodes.Stfld, field!);
            ilgenerator.Emit(OpCodes.Ret);
            return (Action<T, F>)dynamicMethod.CreateDelegate(typeof(Action<T, F>));
        }

        // Token: 0x06000046 RID: 70 RVA: 0x000031A0 File Offset: 0x000013A0
        public static Action<F> GetFieldSetter<F>([NotNull] string fieldName, [NotNull] Type ownerType)
        {
            var field = ownerType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
            var dynamicMethod = new DynamicMethod(string.Empty, null, new[]
            {
                typeof(F)
            }, ownerType);
            var ilgenerator = dynamicMethod.GetILGenerator();
            ilgenerator.Emit(OpCodes.Ldarg_0);
            ilgenerator.Emit(OpCodes.Stsfld, field!);
            ilgenerator.Emit(OpCodes.Ret);
            return (Action<F>)dynamicMethod.CreateDelegate(typeof(Action<F>));
        }
    }
}