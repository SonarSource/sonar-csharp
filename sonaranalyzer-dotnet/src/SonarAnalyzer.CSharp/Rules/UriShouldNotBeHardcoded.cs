﻿/*
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

using SonarAnalyzer;
using SonarAnalyzer.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SonarAnalyzer.Rules.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [Rule(DiagnosticId)]
    public sealed class UriShouldNotBeHardcoded
        : UriShouldNotBeHardcodedBase<ExpressionSyntax, LiteralExpressionSyntax,
            SyntaxKind, BinaryExpressionSyntax, ArgumentSyntax, VariableDeclaratorSyntax>
    {
        private static readonly DiagnosticDescriptor rule =
            DiagnosticDescriptorBuilder.GetDescriptor(DiagnosticId, MessageFormat, RspecStrings.ResourceManager);
        protected override DiagnosticDescriptor Rule => rule;
        protected override SyntaxKind StringLiteralSyntaxKind { get; } = SyntaxKind.StringLiteralExpression;
        protected override SyntaxKind[] StringConcatenateExpressions { get; } = new[] { SyntaxKind.AddExpression };
        protected sealed override GeneratedCodeRecognizer GeneratedCodeRecognizer => Helpers.CSharp.GeneratedCodeRecognizer.Instance;
        protected override string GetLiteralText(LiteralExpressionSyntax literalExpression) => literalExpression?.Token.ValueText;
        protected override ExpressionSyntax GetLeftNode(BinaryExpressionSyntax binaryExpression) => binaryExpression.Left;
        protected override ExpressionSyntax GetRightNode(BinaryExpressionSyntax binaryExpression) => binaryExpression.Right;
        protected override bool IsInvocationOrObjectCreation(SyntaxNode node) =>
            node.IsKind(SyntaxKind.InvocationExpression) ||
            node.IsKind(SyntaxKind.ObjectCreationExpression);

        protected override int? GetArgumentIndex(ArgumentSyntax argument) =>
            (argument?.Parent as ArgumentListSyntax)?.Arguments.IndexOf(argument);

        protected override string GetDeclaratorIdentifierName(VariableDeclaratorSyntax declarator) =>
            declarator?.Identifier.ValueText;
    }
}