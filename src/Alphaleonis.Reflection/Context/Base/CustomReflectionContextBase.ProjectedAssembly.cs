﻿using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;

namespace Alphaleonis.Reflection.Context
{
   public partial class CustomReflectionContextBase
   {
      protected class ProjectedAssembly<TContext> : DelegatingAssembly, IProjector<TContext> where TContext : CustomReflectionContextBase
      {
         public ProjectedAssembly(Assembly assembly, TContext context) : base(assembly)
         {
            ReflectionContext = context;
         }

         public TContext ReflectionContext { get; }

         CustomReflectionContextBase IProjector.ReflectionContext => ReflectionContext;

         public override object[] GetCustomAttributes(bool inherit)
         {
            return ReflectionContext.AddContextIdentifierAttribute(base.GetCustomAttributes(inherit));
         }

         public override object[] GetCustomAttributes(Type attributeType, bool inherit)
         {
            return ReflectionContext.AddContextIdentifierAttributeIfRequested(attributeType, base.GetCustomAttributes(attributeType, inherit));
         }

         public override Type GetType(string name, bool throwOnError, bool ignoreCase) => ReflectionContext.MapType(base.GetType(name, throwOnError, ignoreCase));

         public override Type[] GetTypes() => ReflectionContext.MapTypes(base.GetTypes());         

         public override Type[] GetExportedTypes() => ReflectionContext.MapTypes(base.GetExportedTypes());

         public override IEnumerable<TypeInfo> DefinedTypes => base.DefinedTypes.Select(type => ReflectionContext.MapType(type));

         public override MethodInfo EntryPoint => ReflectionContext.MapMember(base.EntryPoint);

         public override IEnumerable<Type> ExportedTypes => base.ExportedTypes.Select(type => ReflectionContext.MapType(type));

         public override Assembly GetSatelliteAssembly(CultureInfo culture) => ReflectionContext.MapAssembly(base.GetSatelliteAssembly(culture));

         public override Assembly GetSatelliteAssembly(CultureInfo culture, Version version) => ReflectionContext.MapAssembly(base.GetSatelliteAssembly(culture, version));

         public override Type GetType(string name) => ReflectionContext.MapType(base.GetType(name));

         public override Type GetType(string name, bool throwOnError) => ReflectionContext.MapType(base.GetType(name, throwOnError));
      }
   }

}
