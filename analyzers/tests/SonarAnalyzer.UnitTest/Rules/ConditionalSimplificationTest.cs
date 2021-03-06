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
using Microsoft.CodeAnalysis;
#endif
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SonarAnalyzer.Rules.CSharp;
using SonarAnalyzer.UnitTest.TestFramework;

namespace SonarAnalyzer.UnitTest.Rules
{
    [TestClass]
    public class ConditionalSimplificationTest
    {
        [TestMethod]
        [TestCategory("Rule")]
        public void ConditionalSimplification_BeforeCSharp8() =>
            Verifier.VerifyAnalyzer(@"TestCases\ConditionalSimplification.BeforeCSharp8.cs",
                                    new ConditionalSimplification(),
                                    ParseOptionsHelper.BeforeCSharp8);

        [TestMethod]
        [TestCategory("Rule")]
        public void ConditionalSimplification_CSharp8() =>
            Verifier.VerifyAnalyzer(@"TestCases\ConditionalSimplification.CSharp8.cs",
                                    new ConditionalSimplification(),
                                    new[] { new CSharpParseOptions(LanguageVersion.CSharp8) });

        [TestMethod]
        [TestCategory("CodeFix")]
        public void ConditionalSimplification_CSharp8_CodeFix() =>
            Verifier.VerifyCodeFix(@"TestCases\ConditionalSimplification.CSharp8.cs",
                                   @"TestCases\ConditionalSimplification.CSharp8.Fixed.cs",
                                   new ConditionalSimplification(),
                                   new ConditionalSimplificationCodeFixProvider(),
                                   new[] { new CSharpParseOptions(LanguageVersion.CSharp8) });

        [TestMethod]
        [TestCategory("Rule")]
        public void ConditionalSimplification_FromCSharp8() =>
            Verifier.VerifyAnalyzer(@"TestCases\ConditionalSimplification.FromCSharp8.cs",
                                    new ConditionalSimplification(),
                                    ParseOptionsHelper.FromCSharp8);
#if NET
        [TestMethod]
        [TestCategory("Rule")]
        public void ConditionalSimplification_FromCSharp9() =>
            Verifier.VerifyAnalyzerFromCSharp9Console(@"TestCases\ConditionalSimplification.FromCSharp9.cs",
                                                      new ConditionalSimplification());
#endif

        [TestMethod]
        [TestCategory("CodeFix")]
        public void ConditionalSimplification_BeforeCSharp8_CodeFix() =>
            Verifier.VerifyCodeFix(@"TestCases\ConditionalSimplification.BeforeCSharp8.cs",
                                   @"TestCases\ConditionalSimplification.BeforeCSharp8.Fixed.cs",
                                   new ConditionalSimplification(),
                                   new ConditionalSimplificationCodeFixProvider(),
                                   ParseOptionsHelper.BeforeCSharp8);

        [TestMethod]
        [TestCategory("CodeFix")]
        public void ConditionalSimplification_FromCSharp8_CodeFix() =>
            Verifier.VerifyCodeFix(@"TestCases\ConditionalSimplification.FromCSharp8.cs",
                                   @"TestCases\ConditionalSimplification.FromCSharp8.Fixed.cs",
                                   new ConditionalSimplification(),
                                   new ConditionalSimplificationCodeFixProvider(),
                                   ParseOptionsHelper.FromCSharp8);

#if NET
        [TestMethod]
        [TestCategory("CodeFix")]
        public void ConditionalSimplification_FromCSharp9_CodeFix() =>
            Verifier.VerifyCodeFix(@"TestCases\ConditionalSimplification.FromCSharp9.cs",
                                   @"TestCases\ConditionalSimplification.FromCSharp9.Fixed.cs",
                                   new ConditionalSimplification(),
                                   new ConditionalSimplificationCodeFixProvider(),
                                   ParseOptionsHelper.FromCSharp9,
                                   OutputKind.ConsoleApplication);
#endif
    }
}
