using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Strategy
{

    void Act();


}

public class StrategyHide : Strategy
{
    public void Act()
    { }
}

public class ConcreteStrategy2 : Strategy
{
    public void Act()
    { }
}