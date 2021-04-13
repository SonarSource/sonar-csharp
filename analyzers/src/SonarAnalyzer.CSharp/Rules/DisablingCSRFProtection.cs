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

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SonarAnalyzer.Common;
using SonarAnalyzer.Extensions;
using SonarAnalyzer.Helpers;

namespace SonarAnalyzer.Rules.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [Rule(DiagnosticId)]
    public sealed class DisablingCSRFProtection : HotspotDiagnosticAnalyzer
    {
        private const string DiagnosticId = "S4502";
        private const string MessageFormat = "Make sure disabling CSRF protection is safe here.";
        private const string IgnoreAntiforgeryTokenAttributeName = "IgnoreAntiforgeryTokenAttribute";
        private const SyntaxKind ImplicitObjectCreationExpression = (SyntaxKind)8659;

        private static readonly DiagnosticDescriptor Rule = DiagnosticDescriptorBuilder.GetDescriptor(DiagnosticId, MessageFormat, RspecStrings.ResourceManager);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public DisablingCSRFProtection() : base(AnalyzerConfiguration.Hotspot) { }

        public DisablingCSRFProtection(IAnalyzerConfiguration configuration) : base(configuration) { }

        protected override void Initialize(SonarAnalysisContext context) =>
            context.RegisterSyntaxNodeActionInNonGenerated(c =>
               {
                   var shouldReport = c.Node switch
                   {
                       AttributeSyntax attributeSyntax => attributeSyntax.IsKnownType(KnownType.Microsoft_AspNetCore_Mvc_IgnoreAntiforgeryTokenAttribute, c.SemanticModel),
                       ObjectCreationExpressionSyntax objectCreation => IsIgnoreAntiforgeryTokenAttribute(objectCreation, c.SemanticModel),
                       _ => c.Node.IsKnownType(KnownType.Microsoft_AspNetCore_Mvc_IgnoreAntiforgeryTokenAttribute, c.SemanticModel)
                   };

                   if (shouldReport)
                   {
                       ReportDiagnostic(c);
                   }
               },
               SyntaxKind.Attribute,
               SyntaxKind.ObjectCreationExpression,
               ImplicitObjectCreationExpression);

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context) =>
            context.ReportDiagnosticWhenActive(Diagnostic.Create(Rule, context.Node.GetLocation()));

        private static bool IsIgnoreAntiforgeryTokenAttribute(ObjectCreationExpressionSyntax objectCreation, SemanticModel semanticModel) =>
            (objectCreation.Type.NameIs(IgnoreAntiforgeryTokenAttributeName) || objectCreation.Type.NameIs(string.Empty))
            && objectCreation.IsKnownType(KnownType.Microsoft_AspNetCore_Mvc_IgnoreAntiforgeryTokenAttribute, semanticModel);
    }
}