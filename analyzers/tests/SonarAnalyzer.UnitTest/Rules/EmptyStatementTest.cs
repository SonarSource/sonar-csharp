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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SonarAnalyzer.Rules.CSharp;
using SonarAnalyzer.UnitTest.TestFramework;

namespace SonarAnalyzer.UnitTest.Rules
{
    [TestClass]
    public class EmptyStatementTest
    {
        [TestMethod]
        [TestCategory("Rule")]
        public void EmptyStatement() =>
            Verifier.VerifyAnalyzer(@"TestCases\EmptyStatement.cs", new EmptyStatement());

#if NET
        [TestMethod]
        [TestCategory("Rule")]
        public void EmptyStatement_CSharp9() =>
            Verifier.VerifyAnalyzerFromCSharp9Console(@"TestCases\EmptyStatement.CSharp9.cs", new EmptyStatement());
#endif

        [TestMethod]
        [TestCategory("CodeFix")]
        public void EmptyStatement_CodeFix() =>
            Verifier.VerifyCodeFix(
                @"TestCases\EmptyStatement.cs",
                @"TestCases\EmptyStatement.Fixed.cs",
                new EmptyStatement(),
                new EmptyStatementCodeFixProvider());

#if NET
        [TestMethod]
        [TestCategory("CodeFix")]
        public void EmptyStatement_CodeFix_CSharp9() =>
            Verifier.VerifyCodeFix(@"TestCases\EmptyStatement.CSharp9.cs",
                                   @"TestCases\EmptyStatement.CSharp9.Fixed.cs",
                                   new EmptyStatement(),
                                   new EmptyStatementCodeFixProvider(),
                                   ParseOptionsHelper.FromCSharp9,
                                   OutputKind.ConsoleApplication);
#endif
    }
}
