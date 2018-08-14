﻿/*
 * SonarAnalyzer for .NET
 * Copyright (C) 2015-2018 SonarSource SA
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

using System.Collections.Immutable;
using System.Linq;
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
    public sealed class OptionalParameterWithDefaultValue : SonarDiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S3451";
        private const string MessageFormat = "Use '[DefaultParameterValue]' instead.";

        private static readonly DiagnosticDescriptor rule =
            DiagnosticDescriptorBuilder.GetDescriptor(DiagnosticId, MessageFormat, RspecStrings.ResourceManager);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(rule);

        protected override void Initialize(SonarAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionInNonGenerated(
                c =>
                {
                    var parameter = (ParameterSyntax)c.Node;
                    if (!parameter.AttributeLists.Any())
                    {
                        return;
                    }

                    var attributes = AttributeSyntaxSymbolMapping.GetAttributesForParameter(parameter, c.SemanticModel).ToList();

                    var hasNoOptional = attributes.All(attr => !attr.Symbol.IsInType(KnownType.System_Runtime_InteropServices_OptionalAttribute));
                    if (hasNoOptional)
                    {
                        return;
                    }

                    var hasDefaultParameterValue = attributes.Any(attr =>
                        attr.Symbol.IsInType(KnownType.System_Runtime_InteropServices_DefaultParameterValueAttribute));
                    if (hasDefaultParameterValue)
                    {
                        return;
                    }

                    var defaultValueAttribute = attributes
                        .FirstOrDefault(a => a.Symbol.IsInType(KnownType.System_ComponentModel_DefaultValueAttribute));

                    if (defaultValueAttribute != null)
                    {
                        c.ReportDiagnosticWhenActive(Diagnostic.Create(rule, defaultValueAttribute.SyntaxNode.GetLocation()));
                    }
                },
                SyntaxKind.Parameter);
        }
    }
}
