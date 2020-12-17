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

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using SonarAnalyzer.Helpers;

namespace SonarAnalyzer.Rules
{
    public abstract class UseArrayEmptyBase<TLanguageKindEnum, TCreation, TInitialization> : SonarDiagnosticAnalyzer
        where TLanguageKindEnum : struct
        where TCreation : SyntaxNode
        where TInitialization : SyntaxNode
    {
        protected const string DiagnosticId = "S5939";
        private const string MessageFormat = "Declare this empty array using Array.Empty{0}.";

        protected readonly INetFrameworkVersionProvider VersionProvider = new NetFrameworkVersionProvider();

        private readonly DiagnosticDescriptor rule;
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(rule);

        protected UseArrayEmptyBase(System.Resources.ResourceManager rspecResources) =>
            rule = DiagnosticDescriptorBuilder.GetDescriptor(DiagnosticId, string.Format(MessageFormat, ArrayEmptySuffix), rspecResources);

        protected abstract string ArrayEmptySuffix { get; }
        protected abstract GeneratedCodeRecognizer GeneratedCodeRecognizer { get; }
        protected abstract TLanguageKindEnum[] SyntaxKindsOfInterest { get; }

        protected override void Initialize(SonarAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionInNonGenerated(
               GeneratedCodeRecognizer,
               c =>
               {
                   if (VersionProvider.GetDotNetFrameworkVersion(c.Compilation) >= NetFrameworkVersion.After46)
                   {
                       var node = c.Node;
                       if (ShouldReport(node))
                       {
                           c.ReportDiagnosticWhenActive(Diagnostic.Create(rule, node.GetLocation()));
                       }
                   }
               },
               SyntaxKindsOfInterest);
        }

        protected virtual bool ShouldReport(SyntaxNode node)
            => (node is TInitialization initializationNode
            && IsEmptyInitialization(initializationNode))
            || (node is TCreation creationNode
            && IsEmptyCreation(creationNode));

        protected bool IsEmptyInitialization(TInitialization initializationNode)
            => !initializationNode.ChildNodes().Any();

        protected abstract bool IsEmptyCreation(TCreation creationNode);
    }
}

