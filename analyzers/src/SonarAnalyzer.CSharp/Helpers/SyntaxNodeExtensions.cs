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

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SonarAnalyzer.ShimLayer.CSharp;

namespace SonarAnalyzer.Helpers
{
    internal static class SyntaxNodeExtensions
    {
        public static bool ContainsConditionalConstructs(this SyntaxNode node) =>
            node != null &&
            node.DescendantNodes()
                .Any(descendant => descendant.IsAnyKind(SyntaxKind.IfStatement,
                    SyntaxKind.ConditionalExpression,
                    SyntaxKind.CoalesceExpression,
                    SyntaxKind.SwitchStatement,
                    SyntaxKindEx.SwitchExpression,
                    SyntaxKindEx.CoalesceAssignmentExpression));

        public static bool ToStringContains(this SyntaxNode node, string s) =>
            node.ToString().Contains(s);

        public static bool ToStringContainsEitherOr(this SyntaxNode node, string a, string b)
        {
            var toString = node.ToString();
            return toString.Contains(a) || toString.Contains(b);
        }

        public static object FindConstantValue(this SyntaxNode node, SemanticModel semanticModel) =>
            new CSharpConstantValueFinder(semanticModel).FindConstant(node);

        public static string FindStringConstant(this SyntaxNode node, SemanticModel semanticModel) =>
            FindConstantValue(node, semanticModel) as string;
    }
}
