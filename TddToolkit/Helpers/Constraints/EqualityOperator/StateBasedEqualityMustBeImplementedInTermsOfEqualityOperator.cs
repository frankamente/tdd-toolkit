﻿using System;
using System.Collections.Generic;
using TddEbook.TddToolkit.ImplementationDetails.ConstraintAssertions;

namespace TddEbook.TddToolkit.Helpers.Constraints.EqualityOperator
{
  public class StateBasedEqualityMustBeImplementedInTermsOfEqualityOperator<T> : IConstraint where T : class
  {
    private readonly ValueObjectActivator<T> _activator;

    public StateBasedEqualityMustBeImplementedInTermsOfEqualityOperator(ValueObjectActivator<T> activator)
    {
      _activator = activator;
    }

    public void CheckAndRecord(ConstraintsViolations violations)
    {
      var instance1 = _activator.CreateInstanceAsValueObjectWithFreshParameters();
      var instance2 = _activator.CreateInstanceAsValueObjectWithPreviousParameters();

      RecordedAssertions.True(Are.EqualInTermsOfEqualityOperator(instance1, instance2), 
        "a == b should return true if both are created with the same arguments", violations);
      RecordedAssertions.True(Are.EqualInTermsOfEqualityOperator(instance2, instance1), 
        "b == a should return true if both are created with the same arguments", violations);
    }
  }
}