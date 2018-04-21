﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace Alphaleonis.Reflection
{
   public partial class CustomAttributeTableBuilder
   {
      private class CustomAttributeTable : ICustomAttributeTable
      {
         private ImmutableDictionary<Type, TypeMetadata> m_metadata;

         public CustomAttributeTable(ImmutableDictionary<Type, TypeMetadata> metadata)
         {
            m_metadata = metadata;
         }

         public IEnumerable<Attribute> GetCustomAttributes(Type type)
         {
            return GetTypeMetadata(type).TypeAttributes;
         }

         public IEnumerable<Attribute> GetCustomAttributes(MemberInfo member)
         {
            if (member == null)
               throw new ArgumentNullException(nameof(member));

            switch (member.MemberType)
            {
               case MemberTypes.Event:
               case MemberTypes.Field:
               case MemberTypes.Property:
                  var result = GetMemberAttributes(member);

                  return result;

               case MemberTypes.Method:
               case MemberTypes.Constructor:
                  ImmutableMethodMetadata methodMetadata;
                  if (GetTypeMetadata(member.DeclaringType).MethodAttributes.TryGetValue(new MethodKey(member as MethodBase), out methodMetadata))
                  {
                     return methodMetadata.MethodAttributes;
                  }
                  else
                  {
                     return ImmutableList<Attribute>.Empty;
                  }

               case MemberTypes.TypeInfo:
               case MemberTypes.NestedType:
                  return GetCustomAttributes((Type)member);

               case MemberTypes.Custom:
               default:
                  return ImmutableList<Attribute>.Empty;
            }
         }

         public IEnumerable<Attribute> GetCustomAttributes(ParameterInfo parameterInfo)
         {
            if (parameterInfo == null)
               throw new ArgumentNullException(nameof(parameterInfo));

            var typeMetadata = GetTypeMetadata(parameterInfo.Member.DeclaringType);
            ImmutableMethodMetadata methodMetadata;
            if (typeMetadata.MethodAttributes.TryGetValue(new MethodKey(parameterInfo.Member as MethodBase), out methodMetadata))
            {
               if (parameterInfo.Position == -1)
                  return methodMetadata.ReturnParameterAttributes;
               else
                  return methodMetadata.ParameterAttributes[parameterInfo.Position];
            }
            else
            {
               return ImmutableList<Attribute>.Empty;
            }
         }

         private TypeMetadata GetTypeMetadata(Type type)
         {
            TypeMetadata metadata;
            if (m_metadata.TryGetValue(type, out metadata))
            {
               return metadata;
            }

            return TypeMetadata.Empty;
         }

         private IImmutableList<Attribute> GetMemberAttributes(MemberInfo member)
         {
            IImmutableList<Attribute> attributes;
            if (GetTypeMetadata(member.DeclaringType).MemberAttributes.TryGetValue(new MemberKey(member), out attributes))
            {
               return attributes;
            }
            else
            {
               return ImmutableList<Attribute>.Empty;
            }
         }
      }      
   }


}
