using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TddEbook.TypeReflection.ImplementationDetails;

namespace TddEbook.TypeReflection
{
  public static class TypeOf<T>
  {
    private static readonly TypeWrapper _typeWrapper;

    static TypeOf()
    {
      _typeWrapper = new TypeWrapper(typeof (T));
    }

    public static bool HasParameterlessConstructor()
    {
      return _typeWrapper.HasParameterlessConstructor();
    }

    public static bool IsImplementationOfOpenGeneric(Type openGenericType)
    {
      return _typeWrapper.IsImplementationOfOpenGeneric(openGenericType);
    }

    public static bool IsConcrete()
    {
      return _typeWrapper.IsConcrete();
    }

    public static IEnumerable<IFieldWrapper> GetAllInstanceFields()
    {
      return _typeWrapper.GetAllInstanceFields();
    }

    public static IEnumerable<IPropertyWrapper> GetAllInstanceProperties()
    {
      return _typeWrapper.GetAllInstanceProperties();
    }

    public static IConstructorWrapper PickConstructorWithLeastNonPointersParameters()
    {
      return _typeWrapper.PickConstructorWithLeastNonPointersParameters();
    }

    public static IBinaryOperator<T, bool> Equality()
    {
      return BinaryOperator<T, bool>.Wrap(_typeWrapper.Equality());
    }

    public static IBinaryOperator<T, bool> Inequality()
    {
      return BinaryOperator<T, bool>.Wrap(_typeWrapper.Inequality());
    }

    public static bool IsInterface()
    {
      return _typeWrapper.IsInterface();
    }

    public static bool Is<T1>()
    {
      return typeof (T) == typeof (T1);
    }
  }

  public class TypeWrapper
  {
    private readonly Type _type;

    public TypeWrapper(Type type)
    {
      _type = type;
    }

    public bool HasParameterlessConstructor()
    {
      var constructors = ConstructorWrapper.ExtractAllFrom(_type);
      return constructors.Any(c => c.IsParameterless());
    }

    public bool IsOpenGeneric(Type openGenericType)
    {
      return _type.IsGenericType && _type.GetGenericTypeDefinition() == openGenericType;
    }


    public bool IsImplementationOfOpenGeneric(Type openGenericType)
    {
      return _type.GetInterfaces().Any(
        ifaceType => ifaceType.IsGenericType && ifaceType.GetGenericTypeDefinition() == openGenericType);
    }

    public bool IsConcrete()
    {
      return !_type.IsAbstract && !_type.IsInterface;
    }

    public IEnumerable<IFieldWrapper> GetAllInstanceFields()
    {
      var fields = _type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      return fields.Select(f => new FieldWrapper(f));
    }

    public IEnumerable<IPropertyWrapper> GetAllInstanceProperties()
    {
      var properties = _type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
      return properties.Select(p => new PropertyWrapper(p));
    }

    public IConstructorWrapper PickConstructorWithLeastNonPointersParameters()
    {
      var constructors = ConstructorWrapper.ExtractAllFrom(_type);
      IConstructorWrapper leastParamsConstructor = null;
      var numberOfParams = int.MaxValue;

      foreach (var typeConstructor in constructors)
      {
        if (
          typeConstructor.HasNonPointerArgumentsOnly()
          && typeConstructor.HasLessParametersThan(numberOfParams))
        {
          leastParamsConstructor = typeConstructor;
          numberOfParams = typeConstructor.GetParametersCount();
        }
      }

      return leastParamsConstructor;
    }

    private const string OpEquality = "op_Equality";
    private const string OpInequality = "op_Inequality";

    private Maybe<MethodInfo> EqualityMethod()
    {
      var equality = _type.GetMethod(OpEquality);

      return equality == null ? Maybe<MethodInfo>.Nothing : new Maybe<MethodInfo>(equality);
    }

    private Maybe<MethodInfo> InequalityMethod()
    {
      var inequality = _type.GetMethod(OpInequality);

      return inequality == null ? Maybe<MethodInfo>.Nothing : new Maybe<MethodInfo>(inequality);
    }

    private Maybe<MethodInfo> ValueTypeEqualityMethod()
    {
      return _type.IsValueType ?
        Maybe.Wrap(this.GetType().GetMethod("ValuesEqual"))
        : Maybe<MethodInfo>.Nothing;

    }

    private Maybe<MethodInfo> ValueTypeInequalityMethod()
    {
      return _type.IsValueType ?
        Maybe.Wrap(this.GetType().GetMethod("ValuesNotEqual")) 
        : Maybe<MethodInfo>.Nothing;
    }

    public IBinaryOperator Equality()
    {
      return BinaryOperator.Wrap(_type, EqualityMethod(), ValueTypeEqualityMethod(), "operator ==");
    }

    public IBinaryOperator Inequality()
    {
      return BinaryOperator.Wrap(_type, InequalityMethod(), ValueTypeInequalityMethod(), "operator !=");
    }

    public static TypeWrapper For(Type type)
    {
      return new TypeWrapper(type);
    }

    public static bool ValuesEqual(object instance1, object instance2)
    {
      return ValueType.Equals(instance1, instance2);
    }

    public static bool ValuesNotEqual(object instance1, object instance2)
    {
      return !ValueType.Equals(instance1, instance2);
    }


    public bool IsInterface()
    {
      return _type.IsInterface;
    }
  }

}