﻿/*
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

extern alias csharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using csharp::SonarAnalyzer.Rules.CSharp;
using SonarAnalyzer.UnitTest.TestFramework;

namespace SonarAnalyzer.UnitTest.Rules
{
    [TestClass]
    public class GenericReadonlyFieldPropertyAssignmentTest
    {
        [TestMethod]
        [TestCategory("Rule")]
        public void GenericReadonlyFieldPropertyAssignment() =>
            Verifier.VerifyAnalyzer(@"TestCases\GenericReadonlyFieldPropertyAssignment.cs",
                                    new GenericReadonlyFieldPropertyAssignment(),
                                    ParseOptionsHelper.FromCSharp8);

#if NET5_0
        [TestMethod]
        [TestCategory("Rule")]
        public void GenericReadonlyFieldPropertyAssignment_CSharp9() =>
            Verifier.VerifyAnalyzerFromCSharp9Library(@"TestCases\GenericReadonlyFieldPropertyAssignment.CSharp9.cs",
                                                      new GenericReadonlyFieldPropertyAssignment());
#endif

        [TestMethod]
        [TestCategory("CodeFix")]
        public void GenericReadonlyFieldPropertyAssignment_CodeFix_Remove_Statement() =>
            Verifier.VerifyCodeFix(@"TestCases\GenericReadonlyFieldPropertyAssignment.cs",
                                   @"TestCases\GenericReadonlyFieldPropertyAssignment.Remove.Fixed.cs",
                                   new GenericReadonlyFieldPropertyAssignment(),
                                   new GenericReadonlyFieldPropertyAssignmentCodeFixProvider(),
                                   GenericReadonlyFieldPropertyAssignmentCodeFixProvider.TitleRemove,
                                   ParseOptionsHelper.FromCSharp8);

        [TestMethod]
        [TestCategory("CodeFix")]
        public void GenericReadonlyFieldPropertyAssignment_CodeFix_Add_Generic_Type_Constraint() =>
            Verifier.VerifyCodeFix(@"TestCases\GenericReadonlyFieldPropertyAssignment.cs",
                                   @"TestCases\GenericReadonlyFieldPropertyAssignment.AddConstraint.Fixed.cs",
                                   new GenericReadonlyFieldPropertyAssignment(),
                                   new GenericReadonlyFieldPropertyAssignmentCodeFixProvider(),
                                   GenericReadonlyFieldPropertyAssignmentCodeFixProvider.TitleAddClassConstraint,
                                   ParseOptionsHelper.FromCSharp8);
    }
}
