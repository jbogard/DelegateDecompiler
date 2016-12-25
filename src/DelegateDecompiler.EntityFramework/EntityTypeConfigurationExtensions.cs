﻿using System;
using System.Data.Entity.ModelConfiguration;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiler.EntityFramework
{
    public static class EntityTypeConfigurationExtensions
    {
        public static EntityTypeConfiguration<T> Computed<T, TResult>(this EntityTypeConfiguration<T> configuration, Expression<Func<T, TResult>> expression) where T : class
        {
            var memberInfo = ExtractMemberInfo(expression.Body);

            Configuration.Instance.RegisterDecompileableMember(memberInfo);

            return configuration;
        }

        static MemberInfo ExtractMemberInfo(Expression body)
        {
            if (body.NodeType == ExpressionType.MemberAccess)
            {
                var member = ((MemberExpression) body).Member;
                if (!(member is PropertyInfo))
                {
                    throw new InvalidOperationException("MemberExpression expected to have a Member of PropertyInfo type, but got " + member.GetType().Name);
                }
                return member;
            }
            if (body.NodeType == ExpressionType.Call)
            {
                return ((MethodCallExpression) body).Method;
            }
            throw new ArgumentException("Expression expected to be of MemberAccess or Call type, but got " + body.NodeType);
        }
    }
}
