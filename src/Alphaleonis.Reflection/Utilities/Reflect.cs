﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Alphaleonis.Reflection
{
   /// <summary>Defines methods to get reflection information based on lambda expressions.</summary>
   public static class Reflect
   {
      #region GetProperty

      /// <summary>
      /// Gets the <see cref="PropertyInfo" /> representing the property accessed in the
      /// <see cref="expression"/>.
      /// </summary>
      /// <typeparam name="T">The type of the property.</typeparam>
      /// <typeparam name="U">The declaring type of the property.</typeparam>
      /// <param name="expression">The expression. This must be a lambda expression referring to a
      /// property.</param>
      /// <returns>The <see cref="PropertyInfo"/> representing the property.</returns>
      /// <exception cref="ArgumentException">The expression does not reference a property</exception>      
      public static PropertyInfo GetProperty<T, U>(Expression<Func<U, T>> expression)
      {
         return GetProperty((LambdaExpression)expression);
      }

      /// <summary>
      /// Gets the <see cref="PropertyInfo" /> representing the property accessed in the
      /// <see cref="expression"/>.
      /// </summary>
      /// <exception cref="ArgumentException">The expression does not reference a property.</exception>
      /// <typeparam name="T">The type of the property.</typeparam>
      /// <param name="expression">The expression. This must be a lambda expression referring to a
      /// property.</param>
      /// <returns>The <see cref="PropertyInfo"/> representing the property.</returns>
      public static PropertyInfo GetProperty<T>(Expression<Func<T>> expression)
      {
         return GetProperty((LambdaExpression)expression);
      }

      /// <summary>
      /// Gets the <see cref="PropertyInfo" /> representing the property accessed in the
      /// <see cref="expression"/>.
      /// </summary>
      /// <exception cref="ArgumentException">The expression does not reference a property.</exception>
      /// <typeparam name="T">The type of the property.</typeparam>
      /// <param name="expression">The expression. This must be a lambda expression referring to a
      /// property.</param>
      /// <returns>The <see cref="PropertyInfo"/> representing the property.</returns>
      public static PropertyInfo GetProperty<T>(Expression<Action<T>> expression)
      {
         return GetProperty((LambdaExpression)expression);
      }

      internal static PropertyInfo GetProperty(LambdaExpression expression)
      {
         var memberInfo = GetMemberInternal(expression.Body, false) as PropertyInfo;
         if (memberInfo == null)
            throw new ArgumentException($"The expression {expression} does not reference a property.");

         return memberInfo;
      }

      #endregion

      #region GetField

      public static FieldInfo GetField<T, U>(Expression<Func<U, T>> expression)
      {
         return GetField((LambdaExpression)expression);
      }

      public static FieldInfo GetField<T>(Expression<Func<T>> expression)
      {
         return GetField((LambdaExpression)expression);
      }

      public static FieldInfo GetField<T>(Expression<Action<T>> expression)
      {
         return GetField((LambdaExpression)expression);
      }

      internal static FieldInfo GetField(LambdaExpression expression)
      {
         var memberInfo = GetMemberInternal(expression.Body, false) as FieldInfo;
         if (memberInfo == null)
            throw new ArgumentException($"The expression {expression} does not reference a Field.");

         return memberInfo;
      }

      #endregion     

      #region GetMember

      public static MemberInfo GetMember<T>(Expression<Action<T>> expression)
      {
         return GetMember((LambdaExpression)expression);
      }

      public static MemberInfo GetMember<T>(Expression<Func<T, object>> expression)
      {
         return GetMember((LambdaExpression)expression);
      }

      public static MemberInfo GetMember<T>(Expression<Action> expression)
      {
         return GetMember((LambdaExpression)expression);
      }

      public static MemberInfo GetMember<T>(Expression<Func<T>> expression)
      {
         return GetMember((LambdaExpression)expression);
      }

      internal static MemberInfo GetMember(LambdaExpression expression)
      {
         return GetMemberInternal(expression.Body, false);
      }

      internal static MemberInfo GetMemberInternal(Expression expression, bool declaredOnly)
      {
         MemberInfo member = null;
         Type ownerType;

         switch (expression)
         {
            case MethodCallExpression methodCallExpr:
               member = methodCallExpr.Method;
               ownerType = methodCallExpr.Object == null ? methodCallExpr.Method.DeclaringType : methodCallExpr.Object.Type;
               break;

            case MemberExpression memberExpression:
               member = memberExpression.Member;
               ownerType = memberExpression.Expression?.Type ?? member.DeclaringType;
               break;

            case UnaryExpression unaryExpression:
               return GetMemberInternal(unaryExpression.Operand, declaredOnly);

            default:
               throw new ArgumentException($"Expression '{expression}' does not refer to a property, field or event.");
         }

         if (member is PropertyInfo)
         {
            if (!member.DeclaringType.Equals(ownerType))
            {
               // We are accessing the property in one type, but the PropertyInfo we got is from a base type. 
               // So we need to check if this property exists on the derived type as well.
               var declaredMember = ownerType.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | (declaredOnly ? BindingFlags.DeclaredOnly : 0))
                  .FirstOrDefault(p => p.GetBaseDefinition()?.Equals(member) == true);

               if (declaredMember == null && declaredOnly)
               {
                  throw new ArgumentException($"The property {member.Name} is not declared on type {ownerType}, it's base declaration is on {member.DeclaringType}.");
               }
               else if (declaredMember != null)
               {
                  member = declaredMember;
               }
            }
         }
         else if (member is MethodInfo)
         {
            if (!member.DeclaringType.Equals(ownerType))
            {
               // We are accessing the property in one type, but the PropertyInfo we got is from a base type. 
               // So we need to check if this property exists on the derived type as well.
               var declaredMember = ownerType.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | (declaredOnly ? BindingFlags.DeclaredOnly : 0))
                  .FirstOrDefault(p => p.GetBaseDefinition()?.Equals(member.GetBaseDefinition()) == true);

               if (declaredMember == null && declaredOnly)
               {
                  throw new ArgumentException($"The method {member} is not declared on type {ownerType}, it's base declaration is on {member.DeclaringType}.");
               }
               else if (declaredMember != null)
               {
                  member = declaredMember;
               }
            }
         }

         return member;
      }

      #endregion

      #region GetMethod

      public static MethodInfo GetMethod(Expression<Action> expression)
      {
         return GetMethod((LambdaExpression)expression);
      }

      public static MethodInfo GetMethod<T>(Expression<Action<T>> expression)
      {
         return GetMethod((LambdaExpression)expression);
      }

      public static MethodInfo GetMethod(LambdaExpression expression)
      {
         MethodCallExpression methodCallExpression = expression.Body as MethodCallExpression ?? throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");

         // When using expressions the member returned always seems to point to the base definition of the method. Here we need to have
         // the one referring to the derived type, to mimic what you would get if you did typeof(T).GetMethod(...). So in case the type
         // of the object used is different from the declaring type of the member, we need to find the method in the object type.
         if (methodCallExpression.Object != null && (!methodCallExpression.Method.DeclaringType.Equals(methodCallExpression.Object.Type) 
            || methodCallExpression.Method.IsGenericMethod && !methodCallExpression.Method.IsGenericMethodDefinition))
         {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var result = methodCallExpression.Object.Type.GetMethods(bindingFlags).FirstOrDefault(m => m.GetBaseDefinition().Equals(methodCallExpression.Method.GetBaseDefinition()));

            return result;
         }

         return methodCallExpression.Method;
      }

      #endregion
   }

   public static class Reflect<TSource>
   {
      public static FieldInfo GetField<T>(Expression<Func<TSource, T>> expression)
      {
         return Reflect.GetField((LambdaExpression)expression);
      }
     
      public static PropertyInfo GetProperty<T>(Expression<Func<TSource, T>> expression)
      {
         return Reflect.GetProperty((LambdaExpression)expression);
      }

      public static MethodInfo GetMethod(Expression<Action> expression)
      {
         return Reflect.GetMethod((LambdaExpression)expression);
      }

      public static MethodInfo GetMethod(Expression<Action<TSource>> expression)
      {
         return Reflect.GetMethod((LambdaExpression)expression);
      }

      public static MethodInfo GetMethod<TResult>(Expression<Func<TResult>> expression)
      {
         return Reflect.GetMethod((LambdaExpression)expression);
      }

      public static MethodInfo GetMethod<TResult>(Expression<Func<TSource, TResult>> expression)
      {
         return Reflect.GetMethod((LambdaExpression)expression);
      }

   }
}