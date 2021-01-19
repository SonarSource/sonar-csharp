﻿/*
 * SonarAnalyzer for .NET
 * Copyright (C) 2015-2021 SonarSource SA
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

#if NET
using System.Linq;
#endif
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SonarAnalyzer.UnitTest.MetadataReferences;
using SonarAnalyzer.UnitTest.TestFramework;
using CS = SonarAnalyzer.Rules.CSharp;

namespace SonarAnalyzer.UnitTest.Rules
{
    [TestClass]
    public class TestClassShouldHaveTestMethodTest
    {
        [DataTestMethod]
        [DataRow("2.5.7.10213")]
        [DataRow(Constants.NuGetLatestVersion)]
        [TestCategory("Rule")]
        public void TestClassShouldHaveTestMethod_NUnit(string testFwkVersion) =>
            Verifier.VerifyAnalyzer(@"TestCases\TestClassShouldHaveTestMethod.NUnit.cs",
                                    new CS.TestClassShouldHaveTestMethod(),
                                    additionalReferences: NuGetMetadataReference.NUnit(testFwkVersion));

        [DataTestMethod]
        [DataRow("1.1.11")]
        [DataRow(Constants.NuGetLatestVersion)]
        [TestCategory("Rule")]
        public void TestClassShouldHaveTestMethod_MSTest(string testFwkVersion) =>
            Verifier.VerifyAnalyzer(@"TestCases\TestClassShouldHaveTestMethod.MsTest.cs",
                                    new CS.TestClassShouldHaveTestMethod(),
                                    additionalReferences: NuGetMetadataReference.MSTestTestFramework(testFwkVersion));

#if NET
        [DataTestMethod]
        [TestCategory("Rule")]
        public void TestClassShouldHaveTestMethod_CSharp9() =>
            Verifier.VerifyAnalyzerFromCSharp9Library(@"TestCases\TestClassShouldHaveTestMethod.CSharp9.cs",
                            new CS.TestClassShouldHaveTestMethod(),
                            NuGetMetadataReference.MSTestTestFramework(Constants.NuGetLatestVersion)
                                .Concat(NuGetMetadataReference.NUnit(Constants.NuGetLatestVersion)));
#endif
    }
}
