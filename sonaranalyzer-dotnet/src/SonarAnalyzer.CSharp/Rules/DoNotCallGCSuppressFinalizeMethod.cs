/*
 * SonarAnalyzer for .NET
 * Copyright (C) 2015-2017 SonarSource SA
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

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SonarAnalyzer;
using SonarAnalyzer.Helpers;

namespace SonarAnalyzer.Rules.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [Rule(DiagnosticId)]
    public sealed class DoNotCallGCSuppressFinalizeMethod : DoNotCallMethodsBase
    {
        internal const string DiagnosticId = "S3971";
        private const string MessageFormat = "Do not call 'GC.SuppressFinalize'.";

        private static readonly DiagnosticDescriptor rule =
            DiagnosticDescriptorBuilder.GetDescriptor(DiagnosticId, MessageFormat, RspecStrings.ResourceManager);
        protected override DiagnosticDescriptor Rule => rule;

        private static readonly IEnumerable<MethodSignature> checkedMethods = new List<MethodSignature>
        {
            new MethodSignature(KnownType.System_GC, "SuppressFinalize")
        };
        internal sealed override IEnumerable<MethodSignature> CheckedMethods => checkedMethods;

        protected override bool ShouldReportOnMethodCall(InvocationExpressionSyntax invocation,
            SemanticModel semanticModel)
        {
            var methodDeclaration = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            if (methodDeclaration == null)
            {
                // We want to report on all calls not made from a method
                return true;
            }

            var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);
            if (!methodSymbol.IsIDisposableDispose())
            {
                return true;
            }

            return false;
        }
    }
}