﻿extern alias csharp;
/*
* SonarAnalyzer for .NET
* Copyright (C) 2015-2020 SonarSource SA
* mailto: contact AT sonarsource DOT com
*
* This program is free software; you can redistribute it and/or
* modify it under the terms of the GNU Lesser General Public
* License as published by the Free Software Foundation; either
* version 3 of the License, or (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
* Lesser General Public License for more details.
*
* You should have received a copy of the GNU Lesser General Public License
* along with this program; if not, write to the Free Software Foundation,
* Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/


using csharp::SonarAnalyzer.Rules.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SonarAnalyzer.UnitTest.MetadataReferences;
using SonarAnalyzer.UnitTest.TestFramework;

namespace SonarAnalyzer.UnitTest.Rules
{
    [TestClass]
    public class DoNotCallExitMethodsTest
    {

// ToDo: add support for windows forms in .Net Core
// https://github.com/SonarSource/sonar-dotnet/issues/3426
#if NETFRAMEWORK
        [TestMethod]
        [TestCategory("Rule")]
        public void DoNotCallExitMethods() =>
            Verifier.VerifyAnalyzer(@"TestCases\DoNotCallExitMethods.cs",
                new DoNotCallExitMethods(),
                additionalReferences:MetadataReferenceFacade.GetSystemWindowsForms());
#endif

#if NET5_0
        [TestMethod]
        [TestCategory("Rule")]
        public void DoNotCallExitMethods_CSharp9() =>
            Verifier.VerifyAnalyzerFromCSharp9Console(@"TestCases\DoNotCallExitMethods.CSharp9.cs", new DoNotCallExitMethods());
#endif
    }
}
