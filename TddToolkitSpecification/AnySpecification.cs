using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using TddEbook.TddToolkit;

namespace TddToolkitSpecification
{
  public class AnySpecification
  {
    [Test]
    public void ShouldGenerateDifferentIntegerEachTime()
    {
      //GIVEN
      var int1 = Any.Integer();
      var int2 = Any.Integer();

      //THEN
      XAssert.NotEqual(int1, int2);
    }

    [Test]
    public void ShouldGenerateDifferentTypeEachTime()
    {
      //GIVEN
      var type1 = Any.InstanceOf<Type>();
      var type2 = Any.InstanceOf<Type>();
      var type3 = Any.InstanceOf<Type>();

      //THEN
      XAssert.NotNull(type1);
      XAssert.NotNull(type2);
      XAssert.NotNull(type3);

      XAssert.NotEqual(type1, type2);
      XAssert.NotEqual(type2, type3);
      XAssert.NotEqual(type3, type1);
    }

    [Test]
    public void ShouldBeAbleToProxyConcreteReturnTypesOfMethods()
    {
      var obj = Any.InstanceOf<ISimple>();

      XAssert.NotEqual(default(int), obj.GetInt());
      XAssert.NotEqual(string.Empty, obj.GetString());
      XAssert.NotEqual(string.Empty, obj.GetStringProperty);
      XAssert.NotNull(obj.GetString());
      XAssert.NotNull(obj.GetStringProperty);
    }

    [Test]
    public void ShouldBeAbleToProxyMethodsThatReturnInterfaces()
    {
      //GIVEN
      var obj = Any.InstanceOf<ISimple>();
      
      //WHEN
      obj = obj.GetInterface();

      //THEN
      XAssert.NotNull(obj);
      XAssert.NotEqual(default(int), obj.GetInt());
      XAssert.NotEqual(string.Empty, obj.GetString());
      XAssert.NotEqual(string.Empty, obj.GetStringProperty);
      XAssert.NotNull(obj.GetString());
      XAssert.NotNull(obj.GetStringProperty);
    }

    [Test]
    public void ShouldAlwaysReturnTheSameValueFromProxiedMethodOnTheSameObject()
    {
      //GIVEN
      var obj = Any.InstanceOf<ISimple>();

      //WHEN
      var valueFirstTime = obj.GetString();
      var valueSecondTime = obj.GetString();

      //THEN
      XAssert.Equal(valueFirstTime, valueSecondTime);
    }

    [Test]
    public void ShouldAlwaysReturnTheDifferentValueFromProxiedTheSameMethodOnDifferentObject()
    {
      //GIVEN
      var obj = Any.InstanceOf<ISimple>();
      var obj2 = Any.InstanceOf<ISimple>();

      //WHEN
      var valueFromFirstInstance = obj.GetString();
      var valueFromSecondInstance = obj2.GetString();

      //THEN
      XAssert.NotEqual(valueFromFirstInstance, valueFromSecondInstance);
    }

    [Test]
    public void ShouldGenerateFiniteEnumerables()
    {
      //GIVEN
      var o = Any.InstanceOf<ISimple>();

      //WHEN
      var enumerable = o.Simples;

      //THEN
      foreach (var simple in enumerable)
      {
        XAssert.NotNull(simple);
      }
    }

    [Test]
    public void ShouldGenerateMembersReturningTypeOfType()
    {
      //GIVEN
      var obj1 = Any.InstanceOf<ISimple>();
      var obj2 = Any.InstanceOf<ISimple>();

      //THEN
      XAssert.NotNull(obj1.GetTypeProperty);
      XAssert.NotNull(obj2.GetTypeProperty);
      XAssert.NotEqual(obj1.GetTypeProperty, obj2.GetTypeProperty);
      XAssert.Equal(obj1.GetTypeProperty, obj1.GetTypeProperty);
      XAssert.Equal(obj2.GetTypeProperty, obj2.GetTypeProperty);
    }

    [Test]
    public void ShouldBeAbleToGenerateInstancesOfConcreteClassesWithInterfacesAsTheirConstructorArguments()
    {
      //GIVEN
      var createdProxy = Any.InstanceOf<ObjectWithInterfaceInConstructor>();

      //THEN
      XAssert.NotNull(createdProxy.ConstructorArgument);
      XAssert.NotNull(createdProxy.ConstructorNestedArgument);
    }

    [Test]
    public void ShouldBeAbleToGenerateInstancesOfAbstractClasses()
    {
      //GIVEN
      var createdProxy = Any.InstanceOf<AbstractObjectWithInterfaceInConstructor>();

      //THEN
      XAssert.NotNull(createdProxy.ConstructorArgument);
      XAssert.NotNull(createdProxy.ConstructorNestedArgument);
      XAssert.NotEqual(default(int), createdProxy.AbstractInt);
      XAssert.NotEqual(default(int), createdProxy.SettableInt);
    }

    [Test]
    public void ShouldOverrideVirtualMethodsThatReturnDefaultTypeValuesOnAbstractClassProxy()
    {
      //GIVEN
      var obj = Any.InstanceOf<AbstractObjectWithVirtualMethods>();

      //THEN
      XAssert.NotEqual(default(string), obj.GetSomething());
      XAssert.NotEqual("Something", obj.GetSomething2());
    }

    [Test]
    public void ShouldOverrideVirtualMethodsThatThrowExceptionsOnAbstractClassProxy()
    {
      //GIVEN
      var obj = Any.InstanceOf<AbstractObjectWithVirtualMethods>();

      //THEN
      XAssert.NotEqual(default(string), obj.GetSomethingButThrowExceptionWhileGettingIt());
    }

    [Test]
    public void ShouldPassValueTypeAssertionForProperValueType()
    {
      XAssert.IsValueType<ProperValueType>();
    }

    [Test]
    public void ShouldNotCreateTheSameMethodInfoTwiceInARow()
    {
      //GIVEN
      var x = Any.InstanceOf<MethodInfo>();
      var y = Any.InstanceOf<MethodInfo>();
      
      //THEN
      XAssert.NotAlike(x,y);

    }

    [Test]
    public void ShouldCreateDifferentExceptionEachTime()
    {
      //GIVEN
      var exception1 = Any.InstanceOf<Exception>();
      var exception2 = Any.InstanceOf<Exception>();
      
      //THEN
      XAssert.NotAlike(exception2, exception1);
    }

    [Test]
    public void ShouldGenerateDifferentValueEachTimeAndNotAmongPassedOnesWhenAskedToCreateAnyValueBesidesGiven()
    {
      //WHEN
      var int1 = Any.ValueOtherThan(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
      var int2 = Any.ValueOtherThan(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);

      //THEN
      XAssert.NotEqual(int1, int2);
      Assert.That(int1, Is.Not.InRange(1,10));
      Assert.That(int2, Is.Not.InRange(1, 10));
    }

    [Test]
    public void ShouldGenerateDifferentValueEachTimeAndFromAmongPassedOnesWhenAskedToCreateAnyValueFromGiven()
    {
      //bug current implementation does not adhere to this specification!
      //WHEN
      var int1 = Any.From(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
      var int2 = Any.From(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);

      //THEN
      XAssert.NotEqual(int1, int2);
      Assert.That(int1, Is.InRange(1, 10));
      Assert.That(int2, Is.InRange(1, 10));
    }

    [Test]
    public void ShouldGenerateStringAccordingtoRegex()
    {
      //GIVEN
      var exampleRegex = @"content/([A-Za-z0-9\-]+)\.aspx$";
      
      //WHEN
      var result = Any.StringMatching(exampleRegex);
      
      //THEN
      Assert.True(Regex.IsMatch(result, exampleRegex));
    }

    [Test]
    public void ShouldGenerateStringOfGivenLength()
    {
      //GIVEN
      var stringLength = Any.Integer();

      //WHEN
      var str = Any.StringOfLength(stringLength);

      //THEN
      XAssert.Equal(stringLength, str.Length);
    }

    [Test]
    public void ShouldCreateSortedSetWithThreeDistinctValues()
    {
      //WHEN
      var set = Any.SortedSet<int>();

      //THEN
      CollectionAssert.IsOrdered(set);
      CollectionAssert.AllItemsAreUnique(set);
      XAssert.Equal(3, set.Count);
    }

    [Test]
    public void ShouldBeAbleToGenerateDistinctLettersEachTime()
    {
      //WHEN
      var char1 = Any.AlphaChar();
      var char2 = Any.AlphaChar();
      var char3 = Any.AlphaChar();

      //THEN
      XAssert.NotEqual(char1, char2);
      XAssert.NotEqual(char2, char3);
      Assert.True(Char.IsLetter(char1));
      Assert.True(Char.IsLetter(char2));
      Assert.True(Char.IsLetter(char3));
    }

    [Test]
    public void ShouldBeAbleToGenerateDistinctDigitsEachTime()
    {
      //WHEN
      var char1 = Any.DigitChar();
      var char2 = Any.DigitChar();
      var char3 = Any.DigitChar();

      //THEN
      XAssert.NotEqual(char1, char2);
      XAssert.NotEqual(char2, char3);
      Assert.True(Char.IsDigit(char1));
      Assert.True(Char.IsDigit(char2));
      Assert.True(Char.IsDigit(char3));
    }

    [Test, Timeout(1000)]
    public void ShouldHandleEmptyExcludedStringsWhenGeneratingAnyStringNotContainingGiven()
    {
      Assert.DoesNotThrow(() => Any.StringNotContaining(string.Empty));
    }

    public interface ISimple
    {
      int GetInt();
      string GetString();
      ISimple GetInterface();

      string GetStringProperty { get; }
      Type GetTypeProperty { get; }
      IEnumerable<ISimple> Simples { get; }
    }
    
    public class ObjectWithInterfaceInConstructor
    {
      private readonly int _a;
      public readonly ISimple ConstructorArgument;
      private readonly string _b;
      public readonly ObjectWithInterfaceInConstructor ConstructorNestedArgument;

      public ObjectWithInterfaceInConstructor(
        int a, 
        ISimple constructorArgument, 
        string b, 
        ObjectWithInterfaceInConstructor constructorNestedArgument)
      {
        _a = a;
        ConstructorArgument = constructorArgument;
        _b = b;
        ConstructorNestedArgument = constructorNestedArgument;
      }
    }

    public abstract class AbstractObjectWithInterfaceInConstructor
    {
      private readonly int _a;
      public readonly ISimple ConstructorArgument;
      private readonly string _b;
      public readonly ObjectWithInterfaceInConstructor ConstructorNestedArgument;

      public AbstractObjectWithInterfaceInConstructor(
        int a,
        ISimple constructorArgument,
        string b,
        ObjectWithInterfaceInConstructor constructorNestedArgument)
      {
        _a = a;
        ConstructorArgument = constructorArgument;
        _b = b;
        ConstructorNestedArgument = constructorNestedArgument;
      }

      public abstract int AbstractInt { get; }

      public int SettableInt { get; set; }
    }

    public abstract class AbstractObjectWithVirtualMethods
    {
      public virtual string GetSomething()
      {
        return default(string);
      }

      public virtual string GetSomething2()
      {
        return "something";
      }

      public virtual string GetSomethingButThrowExceptionWhileGettingIt()
      {
        throw new Exception("Let's suppose dummy data cause this method to throw exception");
      }
    }

  }

  public class ProperValueType : IEquatable<ProperValueType>
  {
    public bool Equals(ProperValueType other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return _a == other._a && Equals(_anArray, other._anArray);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((ProperValueType) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (_a*397) ^ (_anArray != null ? _anArray.GetHashCode() : 0);
      }
    }

    public static bool operator ==(ProperValueType left, ProperValueType right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(ProperValueType left, ProperValueType right)
    {
      return !Equals(left, right);
    }

    private readonly int _a;
    private readonly IEnumerable<int> _anArray;

    public ProperValueType(int a, IEnumerable<int> anArray)
    {
      this._a = a;
      _anArray = anArray;
    }

  }
}

