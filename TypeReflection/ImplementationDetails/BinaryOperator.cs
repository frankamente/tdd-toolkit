﻿using System.Reflection;

namespace TddEbook.TypeReflection.ImplementationDetails
{

  public class BinaryOperator<T, TResult> : IBinaryOperator<T,TResult>
  {
    private readonly MethodInfo _method;

    public BinaryOperator(MethodInfo method)
    {
      _method = method;
    }

    public TResult Evaluate(T instance1, T instance2)
    {
      return (TResult)_method.Invoke(null, new[] { (object)instance1, (object)instance2 });
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
  }
}