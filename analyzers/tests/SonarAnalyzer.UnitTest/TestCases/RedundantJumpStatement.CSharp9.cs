﻿using System;

goto A; // Compliant - FN
A:
return; // Compliant - FN

Action<int> b = static i =>
{
    return;  // Noncompliant
};

static void LocalMethod()
{
    return; // Noncompliant
};

record Record
{
    int Prop
    {
        init
        {
            goto A; // Noncompliant
A:
            return; // Noncompliant
        }
    }
}
