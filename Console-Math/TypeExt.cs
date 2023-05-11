using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Console_Math
{
    public static class TypeExt
    {
        public static bool IsCallableWith(this MethodInfo info, Type[] argTypes)
        {
            var methodParams = info.GetParameters();
            var paramsMethod =
                methodParams[^1].GetCustomAttribute<ParamArrayAttribute>() is not null;
            var paramTypes = methodParams.Select(param => param.ParameterType).ToArray();
            for (var i = 0; i < argTypes.Length; i++)
            {
                if (i < paramTypes.Length)
                {
                    if (!argTypes[i].IsConvertible(paramTypes[i]))
                        return false;
                }
                else if(paramsMethod)
                {
                    if (!argTypes[i].IsConvertible(paramTypes[^1]))
                        return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsCallableWithGeneric(this MethodInfo info, Type[] genericArgs, Type[] argTypes)
        {
            if(!info.IsCallableWith(argTypes))
                return false;

            var genericTypes = info.GetGenericMethodDefinition().GetGenericArguments();
            if(genericTypes.Length != genericArgs.Length)
                return false;

            for (int i = 0; i < genericTypes.Length; i++)
            {
                if (!genericTypes[i].MeetsGenericConstraints(genericArgs[i]))
                    return false;
            }

            return true;
        }

        public static bool MeetsGenericConstraints(this Type genericConstraint, Type genericType)
        {
            var specialConstraints = genericConstraint.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;

            if ((specialConstraints & GenericParameterAttributes.ReferenceTypeConstraint) != 0)
            {
                if(!genericType.IsClass)
                    return false;
            }

            if ((specialConstraints & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
            {
                if (!( genericType.IsValueType && !genericType.IsNullableValueType()))
                {
                    return false;
                }
            }

            if ((specialConstraints & GenericParameterAttributes.ReferenceTypeConstraint) != 0)
            {
                if(!(genericType.IsValueType || (genericType.IsClass && genericType.HasDefaultConstructor())))
                {
                    return false;
                }
            }

            var typeConstraints = genericConstraint.GetGenericParameterConstraints();
            return typeConstraints.All(genericType.IsConvertible);
        }

        private static bool HasDefaultConstructor (this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }

        private static bool IsNullableValueType (this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static bool IsConvertible (this Type from, Type to)
        {
            if (from.IsArray && to.IsArray || from.IsPointer && to.IsPointer)
            {
                var fromEle = from.GetElementType();
                var toEle = to.GetElementType();
                if (fromEle is not null && toEle is not null)
                {
                    return fromEle.IsConvertible(fromEle);
                }
            }

            if (!from.IsGenericType || !to.IsGenericType) 
                return to.IsAssignableFrom(from) || from.IsCastableTo(to);

            var genFrom = from.GetGenericTypeDefinition();
            var genTo = to.GetGenericTypeDefinition();

            if(!genTo.IsAssignableFrom(genFrom) && !genFrom.IsCastableTo(genTo))
                return false;

            var genFromTypes = from.GetGenericArguments();
            var genToTypes = to.GetGenericArguments();

            return !genFromTypes.Where((t, i) => !t.IsConvertible(genToTypes[i])).Any();
        }

        public static bool IsCastableTo (this Type from, Type to)
        {
            return to.IsAssignableFrom(from)
                   || to.GetConvertOperators().Any(m => m.GetParameters()[0].ParameterType.IsAssignableFrom(from))
                   || from.GetConvertOperators(true).Any(m => to.IsAssignableFrom(m.ReturnType))
                   || from.IsPrimitiveCastableTo(to);
        }

        public static IEnumerable<MethodInfo> GetConvertOperators (this Type type, bool lookInBase = false)
        {
            var bindingFlags = BindingFlags.Public
                              | BindingFlags.Static
                              | ( lookInBase ? BindingFlags.FlattenHierarchy : BindingFlags.DeclaredOnly );
            return type.GetMethods(bindingFlags).Where(m => m.Name is "op_Implicit" or "op_Explicit");
        }

        private static readonly Dictionary<Type, Type[]> ImplicitPrimitiveTypeConversions = new()
        {
            {typeof(sbyte), new []{typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal), typeof(nint)}},
            {typeof(byte), new []{typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal), typeof(nint), typeof(nuint)}},
            {typeof(short), new []{typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal), typeof(nint)}},
            {typeof(ushort), new []{typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal), typeof(nint), typeof(nuint)}},
            {typeof(int), new []{typeof(long), typeof(float), typeof(double), typeof(decimal), typeof(nint)}},
            {typeof(uint), new []{typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal), typeof(nuint)}},
            {typeof(long), new []{typeof(float), typeof(double), typeof(decimal)}},
            {typeof(ulong), new []{typeof(float), typeof(double), typeof(decimal)}},
            {typeof(float), new []{typeof(double)}},
            {typeof(nint), new []{typeof(long), typeof(float), typeof(double), typeof(decimal)}},
            {typeof(nuint), new []{typeof(ulong), typeof(float), typeof(double), typeof(decimal)}}
        };

        private static readonly Dictionary<Type, Type[]> ExplicitPrimitiveTypeConversions = new()
        {
            { typeof(sbyte), new[] { typeof(byte), typeof(ushort), typeof(uint), typeof(ulong), typeof(nuint) } },
            { typeof(byte), new[] { typeof(sbyte) } },
            { typeof(short), new[] { typeof(sbyte), typeof(byte), typeof(ushort), typeof(uint), typeof(ulong), typeof(nuint) } },
            { typeof(ushort), new[] { typeof(sbyte), typeof(byte), typeof(short) } },
            { typeof(int), new[] { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(uint), typeof(ulong), typeof(nuint) } },
            { typeof(uint), new[] { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(nint) } },
            { typeof(long), new[] { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(ulong), typeof(nint), typeof(nuint) } },
            { typeof(ulong), new[] { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(nint), typeof(nuint) } },
            { typeof(float), new[] { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(decimal), typeof(nint), typeof(nuint) } },
            { typeof(double), new[] { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(decimal), typeof(nint), typeof(nuint) } },
            { typeof(decimal), new[] { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(nint), typeof(nuint) } },
            { typeof(nint), new[] { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(ulong), typeof(nuint) } },
            { typeof(nuint), new[] { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(nint) } }
        };

        public static bool IsPrimitiveCastableTo (this Type from, Type to, bool explicitConversions = false)
        {
            if (!from.IsPrimitive || !to.IsPrimitive)
                return false;

            if (ImplicitPrimitiveTypeConversions.TryGetValue(from, out var conversionTypes))
                if (conversionTypes.Any(conversionType => conversionType.IsEquivalentTo(to)))
                    return true;

            if (!explicitConversions)
                return false;

            if (ExplicitPrimitiveTypeConversions.TryGetValue(from, out conversionTypes))
                if (conversionTypes.Any(conversionType => conversionType.IsEquivalentTo(to)))
                    return true;

            return false;
        }
    }
}
