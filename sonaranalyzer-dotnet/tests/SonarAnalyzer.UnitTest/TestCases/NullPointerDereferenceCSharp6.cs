﻿using System;
using System.Collections.Generic;

namespace Tests.Diagnostics
{
    public class A { }

    class NullPointerDereferenceWithFieldsCSharp6 : A
    {
        private object _foo1;

        void ConditionalThisFieldAccess()
        {
            object o = null;
            this._foo1 = o;
            this?._foo1.ToString(); // Noncompliant
        }

        string TryCatch3()
        {
            object o = null;
            try
            {
                o = new object();
            }
            catch (Exception e) when (e.Message != null)
            {
                o = new object();
            }
            return o.ToString(); // Noncompliant, when e.Message is null o will be null
        }

        // https://github.com/SonarSource/sonar-csharp/issues/1324
        public void FlasePositive(object o)
        {
            try
            {
                var a = o?.ToString();
            }
            catch (InvalidOperationException) when (o != null)
            {
                var b = o.ToString(); // Compliant, o is checked for null in this branch
            }
            catch (ApplicationException) when (o == null)
            {
                var b = o.ToString(); // Noncompliant
            }
        }

        public void TryCatch4(object o)
        {
            o = null;
            try
            {
                var a = o?.ToString();
            }
            catch (Exception e) when (e.Message != null)
            {
                var b = o.ToString(); // Noncompliant, o could be null here
            }
        }

    public void Compliant(List<int> list)
    {
      var row = list?.Count;
      if (row != null)
      {
        var type = list.ToArray();
      }
    }

    public class A
    {
      public bool booleanVal { get; set; }
    }

    public void Compliant1(List<int> list, A a)
    {
      var row = list?.Count;
      if (a.booleanVal = (row != null))
      {
        var type = list.ToArray();
      }
    }

    public void NonCompliant(List<int> list)
    {
      var row = list?.Count;
      if (row == null)
      {
        var type = list.ToArray(); // Noncompliant
      }
    }

    public void NonCompliant1(List<int> list, A a)
    {
      var row = list?.Count;
      if (a.booleanVal = (row == null))
      {
        var type = list.ToArray(); // Noncompliant
      }
    }

    void Compliant2(object o)
    {
      switch (o?.GetHashCode())
      {
        case 1:
          o.ToString();
          break;
        default:
          break;
      }
    }

    void NonCompliant2()
    {
      object o = null;
      switch (o?.GetHashCode())
      {
        case null:
          o.ToString(); // Noncompliant
          break;
        default:
          break;
      }
    }
  }

  public class S2259
  {
    public ConsoleColor Color { get; set; }

    public void Method1(S2259 obj)
    {
      switch (obj?.Color)
      {
        case null:
          Console.ForegroundColor = obj.Color; // FN @2338
          break;
        case ConsoleColor.Red:
          Console.ForegroundColor = obj.Color; //compliant
          break;
        default:
          Console.WriteLine($"Color {obj.Color} is not supported."); //compliant
          break;
      }
    }

    public void Method2(S2259 obj)
    {
      switch (obj?.Color)
      {
        case ConsoleColor.Red:
          Console.ForegroundColor = obj.Color; //compliant
          break;
        default:
          Console.WriteLine($"Color {obj.Color} is not supported."); // FN @2338
          break;
      }
    }
  }

}
