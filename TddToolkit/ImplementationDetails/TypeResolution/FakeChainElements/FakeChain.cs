using System.Collections;
using Castle.DynamicProxy;
using TddEbook.TddToolkit.ImplementationDetails.TypeResolution.Interceptors;
using TddEbook.TypeReflection;

namespace TddEbook.TddToolkit.ImplementationDetails.TypeResolution.FakeChainElements
{
  interface IFakeChain<out T>
  {
    T Resolve();
  }

  internal class FakeChain<T> : IFakeChain<T>
  {
    private readonly IChainElement<T> _chainHead;

    public static IFakeChain<T> NewInstance(
      CachedReturnValueGeneration cachedGeneration, 
      NestingLimit nestingLimit,
      ProxyGenerator proxyGenerator)
    {
      
      return new LimitedFakeChain<T>(
        nestingLimit,
        new FakeChain<T>(
          new ChainElement<T>(new FakeSpecialCase<T>(),
          new ChainElement<T>(SpecialCasesOfResolutions<T>.CreateResolutionOfArray(),
          new ChainElement<T>(SpecialCasesOfResolutions<T>.CreateResolutionOfSimpleIEnumerableAndList(),
          new ChainElement<T>(SpecialCasesOfResolutions<T>.CreateResolutionOfSimpleSet(),
          new ChainElement<T>(SpecialCasesOfResolutions<T>.CreateResolutionOfSimpleDictionary(),
          new ChainElement<T>(SpecialCasesOfResolutions<T>.CreateResolutionOfSortedList(),
          new ChainElement<T>(SpecialCasesOfResolutions<T>.CreateResolutionOfSortedSet(),
          new ChainElement<T>(SpecialCasesOfResolutions<T>.CreateResolutionOfSortedDictionary(),
          new ChainElement<T>(SpecialCasesOfResolutions<T>.CreateResolutionOfKeyValuePair(),
          new ChainElement<T>(SpecialCasesOfResolutions<T>.CreateResolutionOfGenericEnumerator(),
          new ChainElement<T>(new FakeEnumerator<T>(),
          new ChainElement<T>(new FakeUnknownCollection<T>(),
          new ChainElement<T>(new FakeOrdinaryInterface<T>(cachedGeneration, proxyGenerator),
          new ChainElement<T>(new FakeAbstractClass<T>(cachedGeneration, proxyGenerator),
          new ChainElement<T>(new FakeConcreteClassWithNonConcreteConstructor<T>(),
          new ChainElement<T>(new FakeConcreteClass<T>(),
          new InvalidChainElement<T>()))))))))))))))))));
    }


    public FakeChain(IChainElement<T> chainHead)
    {
      _chainHead = chainHead;
    }

    public T Resolve()
    {
      return _chainHead.Resolve();
    }
  }

  public class FakeEnumerator<T> : IResolution<T>
  {
    public bool Applies()
    {
      return TypeOf<T>.Is<IEnumerator>();
    }

    public T Apply()
    {
      return (T)(Any.Array<object>().GetEnumerator());
    }
  }
}