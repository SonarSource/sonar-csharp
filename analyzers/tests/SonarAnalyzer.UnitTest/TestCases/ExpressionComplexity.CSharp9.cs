﻿using System;

var d1 = true && false && true && false && true && true; // Noncompliant

object x = null;

if (x is true or true or true or true or true) { } // Noncompliant

if (x is true and true and true and true and true) { } // Noncompliant

if (x is 10 or 20 or 40 or 50 or 60 or 70) { } // Noncompliant

if (x is < 10 or < 20 or < 30 and (40 or 50 or 60)) { } // Noncompliant

switch (x)
{
    case true and true and true and true and true and true: // Noncompliant
        break;
    case false:
        break;
    default:
        break;
}

var y = x switch
{
    true and true and true and true and true and true => 1, // Noncompliant
    false => 2,
};

y = x switch
{
    int => 1,
    string => 1,
    decimal => 1,
    true => 1,
    false => 0
};

try
{
}
catch (Exception e) when (e is ArgumentNullException or ArgumentOutOfRangeException or DuplicateWaitObjectException or DivideByZeroException or NotFiniteNumberException or OverflowException) // Noncompliant
{
    throw;
}
