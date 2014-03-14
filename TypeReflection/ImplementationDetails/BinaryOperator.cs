﻿using System.Reflection;

namespace TddEbook.TypeReflection.ImplementationDetails
{

  public class BinaryOperator<T, TResult> : IBinaryOperator<T,TResult>
  {
    private readonly IBinaryOperator _method;

    public BinaryOperator(MethodInfo method)
    {
      _method = new BinaryOperator(method);
    }

    private BinaryOperator(IBinaryOperator binaryOperator)
    {
      _method = binaryOperator;
    }

    public TResult Evaluate(T instance1, T instance2)
    {
      return (TResult)_method.Evaluate(instance1, instance2);
    }

    public static BinaryOperator<T, bool> Wrap(Maybe<MethodInfo> maybeOperator, string op)
    {
      if (maybeOperator.HasValue)
      {
        return new BinaryOperator<T, bool>(maybeOperator.Value);
      }
      else
      {
        throw new NoSuchOperatorInTypeException("No method " + op + " on type " + typeof(T));
      }
    }

    public static IBinaryOperator<T, bool> Wrap(IBinaryOperator binaryOperator)
    {
      return new BinaryOperator<T, bool>(binaryOperator);
    }
  }

  public class BinaryOperator : IBinaryOperator
  {
    private readonly MethodInfo _method;

    public BinaryOperator(MethodInfo method)
    {
      _method = method;
    }

    public object Evaluate(object instance1, object instance2)
    {
      return _method.Invoke(null, new[] { instance1, instance2 });
    }

    public static BinaryOperator Wrap(Maybe<MethodInfo> maybeOperator, string op)
    {
      if (maybeOperator.HasValue)
      {
        return new BinaryOperator(maybeOperator.Value);
      }
      else
      {
        throw new NoSuchOperatorInTypeException("No method " + op);
      }
    }
  }
}