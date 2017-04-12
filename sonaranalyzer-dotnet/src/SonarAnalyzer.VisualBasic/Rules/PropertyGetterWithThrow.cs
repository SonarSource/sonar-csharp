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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SonarAnalyzer.Common;
using SonarAnalyzer.Rules.Common;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using SonarAnalyzer.Helpers;

namespace SonarAnalyzer.Rules.VisualBasic
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    [Rule(DiagnosticId)]
    public sealed class PropertyGetterWithThrow : PropertyGetterWithThrowBase<SyntaxKind, AccessorBlockSyntax>
    {
        private static readonly DiagnosticDescriptor rule =
            DiagnosticDescriptorBuilder.GetDescriptor(DiagnosticId, MessageFormat, RspecStrings.ResourceManager);
        protected override DiagnosticDescriptor Rule => rule;

        protected override SyntaxKind ThrowSyntaxKind => SyntaxKind.ThrowStatement;

        protected override bool IsGetter(AccessorBlockSyntax propertyGetter) => propertyGetter.IsKind(SyntaxKind.GetAccessorBlock);

        protected override bool IsIndexer(AccessorBlockSyntax propertyGetter)
        {
            var propertyBlock = propertyGetter.Parent as PropertyBlockSyntax;
            if (propertyBlock == null)
            {
                return false;
            }
            return propertyBlock.PropertyStatement.ParameterList != null &&
                propertyBlock.PropertyStatement.ParameterList.Parameters.Any();
        }

        protected override SyntaxNode GetThrowExpression(SyntaxNode syntaxNode) => ((ThrowStatementSyntax)syntaxNode).Expression;

        protected sealed override GeneratedCodeRecognizer GeneratedCodeRecognizer => Helpers.VisualBasic.GeneratedCodeRecognizer.Instance;
    }
}