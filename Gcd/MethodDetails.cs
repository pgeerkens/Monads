﻿#region The MIT License - Copyright (C) 2012-2016 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Monad Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2016 Pieter Geerkens (email: pgeerkens@hotmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:
//     The above copyright notice and this permission notice shall be 
//     included in all copies or substantial portions of the Software.
// 
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//     EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//     OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//     NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//     HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//     FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//     OTHER DEALINGS IN THE SOFTWARE.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using PGSolutions.Monads;

namespace PGSolutions.Monads.Demos {
    /// <summary>TODO</summary>
    public interface IMethodDetails<TDetails> {
        /// <summary>TODO</summary>
        string   Name        { get; }
        /// <summary>TODO</summary>
        string   Description { get; }
        /// <summary>TODO</summary>
        TDetails Details     { get; }
    }

    /// <summary>TODO</summary>
    public static class MethodDetails
    {
        /// <summary>TODO</summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
        public static X<IList<IMethodDetails<TDetails>>> GetMethodDescriptions<TDetails>(this Type type, 
            Predicate<string>        predicate,
            BindingFlags             bindingFlags,
            Func<PropertyInfo, TDetails> transform
        ) where TDetails : class =>
            from pred in predicate.AsX()
            select  ( from field in type?.GetProperties(bindingFlags)
                      from atts   in field?.CustomAttributes
                      where type != null
                      where pred.Invoke(field?.Name ?? "")
                         && atts?.AttributeType.Name == "DescriptionAttribute"
                      select new Inner<TDetails> (
                            _name(type, field),
                            _attributes(atts),
                            transform?.Invoke(field)
                      ) as IMethodDetails<TDetails>
                    ).ToList().AsReadOnly() as IList<IMethodDetails<TDetails>>;

        private static readonly Func<CustomAttributeData,string> _attributes = atts =>
            atts?.ConstructorArguments[0].Value as string ?? "";
        private static readonly Func<Type,PropertyInfo,string>   _name       = (@class,field) =>
            ( @class?.Name + "." + field?.Name ) ?? "";

        private class Inner<TDetails> : IMethodDetails<TDetails> {
            public Inner(string name, string description, TDetails details) {
                Name        = name;
                Description = description;
                Details     = details;
            }
            public string   Name        { get; }
            public string   Description { get; }
            public TDetails Details   { get; }
        }
    }
}
