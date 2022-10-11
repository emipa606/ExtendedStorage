using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ExtendedStorage;

public static class Access
{
    public static Func<T, P> GetPropertyGetter<T, P>(string propertyName)
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

    public static Func<P> GetPropertyGetter<P>(string propertyName, Type ownerType)
    {
        var getMethod = ownerType.GetProperty(propertyName, BindingFlags.Static | BindingFlags.NonPublic)
            ?.GetGetMethod(true);
        var dynamicMethod = new DynamicMethod(string.Empty, typeof(P), Type.EmptyTypes, ownerType);
        var ilgenerator = dynamicMethod.GetILGenerator();
        ilgenerator.Emit(OpCodes.Call, getMethod!);
        ilgenerator.Emit(OpCodes.Ret);
        return (Func<P>)dynamicMethod.CreateDelegate(typeof(Func<P>));
    }

    public static Func<T, F> GetFieldGetter<T, F>(string fieldName)
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

    public static Func<F> GetFieldGetter<F>(string fieldName, Type ownerType)
    {
        var field = ownerType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        var dynamicMethod = new DynamicMethod(string.Empty, typeof(F), Type.EmptyTypes, ownerType);
        var ilgenerator = dynamicMethod.GetILGenerator();
        ilgenerator.Emit(OpCodes.Ldsfld, field!);
        ilgenerator.Emit(OpCodes.Ret);
        return (Func<F>)dynamicMethod.CreateDelegate(typeof(Func<F>));
    }

    public static Action<T, F> GetFieldSetter<T, F>(string fieldName)
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

    public static Action<F> GetFieldSetter<F>(string fieldName, Type ownerType)
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