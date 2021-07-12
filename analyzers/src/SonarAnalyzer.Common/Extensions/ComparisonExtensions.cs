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

using System;
using Microsoft.CodeAnalysis;
using Comparison = SonarAnalyzer.Helpers.ComparisonKind;
namespace SonarAnalyzer.Helpers
{
    public static class ComparisonKindExtensions
    {
        public static Comparison ComparisonKind(this IMethodSymbol method) =>
            method?.Name switch
            {
                "op_Equality" => Comparison.Equals,
                "op_Inequality" => Comparison.NotEquals,
                "op_LessThan" => Comparison.LessThan,
                "op_LessThanOrEqual" => Comparison.LessThanOrEqual,
                "op_GreaterThan" => Comparison.GreaterThan,
                "op_GreaterThanOrEqual" => Comparison.GreaterThanOrEqual,
                _ => Comparison.None,
            };

        public static Comparison Mirror(this Comparison comparison) =>
           comparison switch
           {
               Comparison.GreaterThan => Comparison.LessThan,
               Comparison.GreaterThanOrEqual => Comparison.LessThanOrEqual,
               Comparison.LessThan => Comparison.GreaterThan,
               Comparison.LessThanOrEqual => Comparison.GreaterThanOrEqual,
               _ => comparison,
           };

        public static string CSharp(this Comparison kind) =>
            kind switch
            {
                Comparison.Equals => "==",
                Comparison.NotEquals => "!=",
                Comparison.LessThan => "<",
                Comparison.LessThanOrEqual => "<=",
                Comparison.GreaterThan => ">",
                Comparison.GreaterThanOrEqual => ">=",
                _ => throw new InvalidOperationException(),
            };
    }
}
