﻿namespace Alphaleonis.Reflection.Context
{
   public abstract partial class CustomReflectionContextBase
   {
      protected interface IProjector
      {
         CustomReflectionContextBase ReflectionContext { get; }
      }

      protected interface IProjector<out TContext> : IProjector where TContext : CustomReflectionContextBase
      {
         new TContext ReflectionContext { get; }
      }

   }
}
