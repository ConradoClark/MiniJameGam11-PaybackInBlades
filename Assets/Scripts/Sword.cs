using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class Sword : BaseGameObject
{
    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(Float());
    }

    private IEnumerable<IEnumerable<Action>> Float()
    {
        while (isActiveAndEnabled)
        {
            yield return transform.GetAccessor()
                .LocalPosition
                .Y
                .Increase(0.25f)
                .Over(1f)
                .WithStep(0.05f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(GameTimer)
                .Build();

            yield return transform.GetAccessor()
                .LocalPosition
                .Y
                .Decrease(0.25f)
                .Over(1f)
                .WithStep(0.05f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(GameTimer)
                .Build();
        }

    }
}
