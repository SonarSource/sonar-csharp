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

using Microsoft.CodeAnalysis.VisualBasic;

namespace SonarAnalyzer.Helpers.Facade
{
    internal sealed class VisualBasicSyntaxKindFacade : ISyntaxKindFacade<SyntaxKind>
    {
        public SyntaxKind InvocationExpression => SyntaxKind.InvocationExpression;
        public SyntaxKind[] ObjectCreationExpressions => new[] {SyntaxKind.ObjectCreationExpression};
        public SyntaxKind[] ClassAndRecordDeclaration => new[] {SyntaxKind.ClassBlock};
        public SyntaxKind[] TypeDeclaration { get; } = {SyntaxKind.ClassBlock, SyntaxKind.StructureBlock, SyntaxKind.InterfaceBlock, SyntaxKind.EnumBlock};
        public SyntaxKind EnumDeclaration => SyntaxKind.EnumStatement;
        public SyntaxKind SimpleMemberAccessExpression => SyntaxKind.SimpleMemberAccessExpression;
        public SyntaxKind Attribute => SyntaxKind.Attribute;
        public SyntaxKind IdentifierName => SyntaxKind.IdentifierName;
        public SyntaxKind StringLiteralExpression => SyntaxKind.StringLiteralExpression;
        public SyntaxKind InterpolatedStringExpression => SyntaxKind.InterpolatedStringExpression;
        public SyntaxKind ReturnStatement => SyntaxKind.ReturnStatement;
    }
}
