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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SonarAnalyzer.Common;
using SonarAnalyzer.Helpers;

namespace SonarAnalyzer.Rules.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [Rule(DiagnosticId)]
    public sealed class SecurityPInvokeMethodShouldNotBeCalled : SecurityPInvokeMethodShouldNotBeCalledBase<SyntaxKind, InvocationExpressionSyntax>
    {
        protected override SyntaxKind SyntaxKind => SyntaxKind.InvocationExpression;
        protected override ILanguageFacade Language => CSharpFacade.Instance;

        public SecurityPInvokeMethodShouldNotBeCalled() : base(RspecStrings.ResourceManager) { }

        protected override SyntaxNode Expression(InvocationExpressionSyntax invocationExpression) =>
            invocationExpression.Expression;

        protected override SyntaxToken Identifier(SyntaxNode syntaxNode) =>
            ((IdentifierNameSyntax)syntaxNode).Identifier;

        protected override IMethodSymbol MethodSymbolForInvalidInvocation(SyntaxNode syntaxNode, SemanticModel semanticModel) =>
            syntaxNode is IdentifierNameSyntax identifierName
            && InvalidMethods.Contains(identifierName.Identifier.ValueText)
            && semanticModel.GetSymbolInfo(syntaxNode).Symbol is IMethodSymbol methodSymbol
                ? methodSymbol
                : null;
    }
}
