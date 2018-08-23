﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SonarAnalyzer.ShimLayer.CSharp
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal struct WhenClauseSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<CSharpSyntaxNode, SyntaxToken> WhenKeywordAccessor;
        private static readonly Func<CSharpSyntaxNode, ExpressionSyntax> ConditionAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithWhenKeywordAccessor;
        private static readonly Func<CSharpSyntaxNode, ExpressionSyntax, CSharpSyntaxNode> WithConditionAccessor;

        static WhenClauseSyntaxWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(WhenClauseSyntaxWrapper));
            WhenKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(WhenKeyword));
            ConditionAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, ExpressionSyntax>(WrappedType, nameof(Condition));
            WithWhenKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(WhenKeyword));
            WithConditionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, ExpressionSyntax>(WrappedType, nameof(Condition));
        }

        private WhenClauseSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.SyntaxNode = node;
        }

        public CSharpSyntaxNode SyntaxNode { get; }

        public SyntaxToken WhenKeyword
        {
            get
            {
                return WhenKeywordAccessor(this.SyntaxNode);
            }
        }

        public ExpressionSyntax Condition
        {
            get
            {
                return ConditionAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator WhenClauseSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default(WhenClauseSyntaxWrapper);
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new WhenClauseSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CSharpSyntaxNode(WhenClauseSyntaxWrapper wrapper)
        {
            return wrapper.SyntaxNode;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public WhenClauseSyntaxWrapper WithWhenKeyword(SyntaxToken whenKeyword)
        {
            return new WhenClauseSyntaxWrapper(WithWhenKeywordAccessor(this.SyntaxNode, whenKeyword));
        }

        public WhenClauseSyntaxWrapper WithCondition(ExpressionSyntax condition)
        {
            return new WhenClauseSyntaxWrapper(WithConditionAccessor(this.SyntaxNode, condition));
        }
    }
}
