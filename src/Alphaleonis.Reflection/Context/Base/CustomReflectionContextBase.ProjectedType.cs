﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Alphaleonis.Reflection.Context
{
   public partial class CustomReflectionContextBase
   {
      protected class ProjectedType<TContext> : DelegatingType, IProjector<TContext> where TContext : CustomReflectionContextBase
      {
         #region Constructor

         public ProjectedType(Type delegatingType, TContext context)
            : base(delegatingType)
         {
            ReflectionContext = context;
         }

         #endregion

         public TContext ReflectionContext { get; }

         CustomReflectionContextBase IProjector.ReflectionContext => ReflectionContext;

         public override Type BaseType => ReflectionContext.MapType(base.BaseType?.GetTypeInfo());

         public override Assembly Assembly => ReflectionContext.MapAssembly(base.Assembly);

         public override object[] GetCustomAttributes(bool inherit)
         {
            return GetCustomAttributes(typeof(Attribute), inherit);
         }

         public override object[] GetCustomAttributes(Type attributeType, bool inherit)
         {
            return ReflectionContext.AddContextIdentifierAttributeIfRequested(attributeType, base.GetCustomAttributes(attributeType, inherit));
         }

         public override bool IsDefined(Type attributeType, bool inherit)
         {
            if (attributeType.IsAssignableFrom(typeof(CustomReflectionContextIdAttribute)))
               return true;

            return base.IsDefined(attributeType, inherit);
         }

         protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
         {
            return ReflectionContext.MapMember(base.GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers));
         }

         public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) => ReflectionContext.MapMembers(base.GetProperties(bindingAttr));
        
         public override MethodInfo[] GetMethods(BindingFlags bindingAttr) => ReflectionContext.MapMembers(base.GetMethods(bindingAttr));

         public override Type[] FindInterfaces(TypeFilter filter, object filterCriteria) 
            => ReflectionContext.MapTypes(base.FindInterfaces(filter, filterCriteria));

         public override MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria) 
            => ReflectionContext.MapMembers(base.FindMembers(memberType, bindingAttr, filter, filterCriteria));

         protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
            => ReflectionContext.MapMember(base.GetConstructorImpl(bindingAttr, binder, callConvention, types, modifiers));

         public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) => ReflectionContext.MapMembers(base.GetConstructors(bindingAttr));

         public override MemberInfo[] GetDefaultMembers() => ReflectionContext.MapMembers(base.GetDefaultMembers());

         public override Type GetElementType() => ReflectionContext.MapType(base.GetElementType());

         public override Type GetEnumUnderlyingType() => ReflectionContext.MapType(base.GetEnumUnderlyingType());

         public override EventInfo GetEvent(string name, BindingFlags bindingAttr) => ReflectionContext.MapMember(base.GetEvent(name, bindingAttr));

         public override EventInfo[] GetEvents(BindingFlags bindingAttr) => ReflectionContext.MapMembers(base.GetEvents(bindingAttr));

         public override EventInfo[] GetEvents() => ReflectionContext.MapMembers(base.GetEvents());

         public override FieldInfo GetField(string name, BindingFlags bindingAttr) => ReflectionContext.MapMember(base.GetField(name, bindingAttr));
         
         public override FieldInfo[] GetFields(BindingFlags bindingAttr) => ReflectionContext.MapMembers(base.GetFields(bindingAttr));

         public override Type[] GetGenericArguments() => ReflectionContext.MapTypes(base.GetGenericArguments());

         public override Type[] GetGenericParameterConstraints() => ReflectionContext.MapTypes(base.GetGenericParameterConstraints());

         public override Type GetGenericTypeDefinition() => ReflectionContext.MapType(base.GetGenericTypeDefinition());

         public override Type GetInterface(string name, bool ignoreCase) => ReflectionContext.MapType(base.GetInterface(name, ignoreCase));

         public override InterfaceMapping GetInterfaceMap(Type interfaceType)
         {            
            var map = base.GetInterfaceMap(interfaceType);
            map.InterfaceMethods = ReflectionContext.MapMembers(map.InterfaceMethods);
            map.InterfaceType = ReflectionContext.MapType(map.InterfaceType);
            map.TargetMethods = ReflectionContext.MapMembers(map.TargetMethods);
            map.TargetType = ReflectionContext.MapType(map.TargetType);
            return map;
         }

         public override Type[] GetInterfaces() => ReflectionContext.MapTypes(base.GetInterfaces());

         public override MemberInfo[] GetMember(string name, BindingFlags bindingAttr) => ReflectionContext.MapMembers(base.GetMember(name, bindingAttr));

         public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr) => ReflectionContext.MapMembers(base.GetMember(name, type, bindingAttr));

         public override MemberInfo[] GetMembers(BindingFlags bindingAttr) => ReflectionContext.MapMembers(base.GetMembers(bindingAttr));

         protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => ReflectionContext.MapMember(base.GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers));

         public override Type GetNestedType(string name, BindingFlags bindingAttr) => ReflectionContext.MapType(base.GetNestedType(name, bindingAttr));

         public override Type[] GetNestedTypes(BindingFlags bindingAttr) => ReflectionContext.MapTypes(base.GetNestedTypes(bindingAttr));

         public override Type MakeArrayType() => ReflectionContext.MapType(base.MakeArrayType());

         public override Type MakeArrayType(int rank) => ReflectionContext.MapType(base.MakeArrayType(rank));

         public override Type MakeByRefType() => ReflectionContext.MapType(base.MakeByRefType());

         public override Type MakeGenericType(params Type[] typeArguments) => ReflectionContext.MapType(base.MakeGenericType(typeArguments));

         public override Type MakePointerType() => ReflectionContext.MapType(base.MakePointerType());

         public override MethodBase DeclaringMethod => ReflectionContext.MapMember(base.DeclaringMethod) as MethodBase;

         public override Type DeclaringType => ReflectionContext.MapType(base.DeclaringType);

         public override Type[] GenericTypeArguments => ReflectionContext.MapTypes(base.GenericTypeArguments);

         public override Type ReflectedType => ReflectionContext.MapType(base.ReflectedType);
      }
   }

   
}
