#region The MIT License - Copyright (C) 2012-2016 Pieter Geerkens
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
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.

using System.Diagnostics.CodeAnalysis;

//[assembly: SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Scope = "type", Target = "PGSolutions.Monads.IO")]

[assembly: SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "PGSolutions.Monads.Maybe`1.#Nothing")]

[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#Action(System.Action)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#Action`1(System.Action`1<!!0>)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#Action`2(System.Action`2<!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#Action`4(System.Action`4<!!0,!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "1#", Scope = "member", Target = "PGSolutions.Monads.Functions.#Second`2(!!0,!!1)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Scope = "member", Target = "PGSolutions.Monads.Functions.#First`2(!!0,!!1)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Scope = "member", Target = "PGSolutions.Monads.ReaderLinq.#Unit`1(PGSolutions.Monads.Unit)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#Action`3(System.Action`3<!!0,!!1,!!2>)")]

[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.StateMaybe`2.#Invoke(!0)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.Maybe`1.#Bind`1(System.Func`2<!0,PGSolutions.Monads.Maybe`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.MaybeExtensions.#SelectMany`2(PGSolutions.Monads.Maybe`1<!!0>,System.Func`2<!!0,PGSolutions.Monads.Maybe`1<!!1>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.MaybeExtensions.#SelectMany`3(PGSolutions.Monads.Maybe`1<!!0>,System.Func`2<!!0,PGSolutions.Monads.Maybe`1<!!1>>,System.Func`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.Maybe`1.#op_Implicit(PGSolutions.Monads.Maybe`1<PGSolutions.Monads.Maybe`1<!0>>):PGSolutions.Monads.Maybe`1<!0>")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.Maybe.#Bind`2(PGSolutions.Monads.Maybe`1<!!0>,System.Func`2<!!0,PGSolutions.Monads.Maybe`1<!!1>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.Reader.#Bind`3(PGSolutions.Monads.Reader`2<!!0,!!1>,System.Func`2<!!1,PGSolutions.Monads.Reader`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.Reader.#Flatten`4(PGSolutions.Monads.Reader`2<!!0,!!1>,System.Func`2<!!1,PGSolutions.Monads.Reader`2<!!0,!!2>>,System.Func`3<!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.ReaderLinq.#SelectMany`3(PGSolutions.Monads.Reader`2<!!0,!!1>,System.Func`2<!!1,PGSolutions.Monads.Reader`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.ReaderLinq.#SelectMany`4(PGSolutions.Monads.Reader`2<!!0,!!1>,System.Func`2<!!1,PGSolutions.Monads.Reader`2<!!0,!!2>>,System.Func`3<!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.State.#Enumerate`2(PGSolutions.Monads.State`2<!!0,!!1>,!!0)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.State.#Bind`3(PGSolutions.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Monads.State`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.State.#Flatten`4(PGSolutions.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Monads.State`2<!!0,!!2>>,System.Func`3<!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.State.#Map`3(PGSolutions.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Monads.StatePayload`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.StateExtensions.#SelectMany`3(PGSolutions.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Monads.State`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.StatePayload`2.#Bind`1(System.Func`2<PGSolutions.Monads.StatePayload`2<!0,!1>,PGSolutions.Monads.StatePayload`2<!0,!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.StateExtensions.#Compose`3(PGSolutions.Monads.State`2<!!0,!!1>,System.Func`2<!!0,PGSolutions.Monads.State`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.StateExtensions.#SelectMany`4(PGSolutions.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Monads.State`2<!!0,!!2>>,System.Func`3<!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IO.#FileExists")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IO.#FileWriteAllText")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOLinqExtensions.#SelectMany`3(PGSolutions.Monads.IO`1<!!0>,System.Func`2<!!0,PGSolutions.Monads.IO`1<!!1>>,System.Func`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOLinqExtensions.#SelectMany`2(PGSolutions.Monads.IO`1<!!0>,System.Func`2<!!0,PGSolutions.Monads.IO`1<!!1>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#AsIO`1(System.Action`1<!!0>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#AsIO`2(System.Action`2<!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#AsIO`3(System.Action`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#AsIO`4(System.Action`4<!!0,!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#AsIO`2(System.Func`2<!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#AsIO`3(System.Func`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#AsIO`4(System.Func`4<!!0,!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#AsIO`5(System.Func`5<!!0,!!1,!!2,!!3,!!4>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.TaskExtensions.#SelectMany`2(System.Threading.Tasks.Task`1<!!0>,System.Func`2<!!0,System.Threading.Tasks.Task`1<!!1>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.TaskExtensions.#SelectMany`3(System.Threading.Tasks.Task`1<!!0>,System.Func`2<!!0,System.Threading.Tasks.Task`1<!!1>>,System.Func`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.TaskExtensions.#SelectMany`1(System.Threading.Tasks.Task,System.Func`2<PGSolutions.Monads.Unit,System.Threading.Tasks.Task`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.TaskExtensions.#SelectMany`2(System.Threading.Tasks.Task,System.Func`2<PGSolutions.Monads.Unit,System.Threading.Tasks.Task`1<!!0>>,System.Func`3<PGSolutions.Monads.Unit,!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.EnumerableExtensions.#FirstUnit(System.Collections.Generic.IEnumerable`1<PGSolutions.Monads.IO`1<PGSolutions.Monads.Unit>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.EnumerableExtensions.#LastUnit(System.Collections.Generic.IEnumerable`1<PGSolutions.Monads.IO`1<PGSolutions.Monads.Unit>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IO.#FileReadAllText")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#Action`1(System.Action`1<!!0>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#Action`2(System.Action`2<!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#Action`3(System.Action`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#Action`4(System.Action`4<!!0,!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#Func`2(System.Func`2<!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#Func`3(System.Func`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#Func`4(System.Func`4<!!0,!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOExtensions.#Func`5(System.Func`5<!!0,!!1,!!2,!!3,!!4>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.UnitExtensions.#SelectMany`1(System.Func`1<PGSolutions.Monads.Unit>,System.Func`2<PGSolutions.Monads.Unit,System.Func`1<!!0>>,System.Func`3<PGSolutions.Monads.Unit,!!0,PGSolutions.Monads.Unit>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.UnitExtensions.#SelectMany(System.Func`1<PGSolutions.Monads.Unit>,System.Func`2<PGSolutions.Monads.Unit,System.Func`1<PGSolutions.Monads.Unit>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.Maybe`1.#Flatten`2(System.Func`2<!0,PGSolutions.Monads.Maybe`1<!!0>>,System.Func`3<!0,!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.Maybe.#Extract`1(PGSolutions.Monads.Maybe`1<System.Func`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.Maybe`1.#SelectMany`1(System.Func`2<!0,PGSolutions.Monads.Maybe`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.Maybe`1.#SelectMany`2(System.Func`2<!0,PGSolutions.Monads.Maybe`1<!!0>>,System.Func`3<!0,!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.State.#SelectMany`3(PGSolutions.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Monads.State`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.State.#SelectMany`4(PGSolutions.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Monads.State`2<!!0,!!2>>,System.Func`3<!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IMonad`1.#SelectMany`1(System.Func`2<!0,PGSolutions.Monads.IMonad`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IMonad`1.#SelectMany`2(System.Func`2<!0,PGSolutions.Monads.IMonad`1<!!0>>,System.Func`3<!0,!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOMonad.#FileExists")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOMonad.#FileReadAllText")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOMonad.#FileWriteAllText")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IO`1.#SelectMany`1(System.Func`2<!0,PGSolutions.Monads.IO`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IO`1.#SelectMany`2(System.Func`2<!0,PGSolutions.Monads.IO`1<!!0>>,System.Func`3<!0,!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOMonads.#FileExists")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOMonads.#FileReadAllText")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.IOMonads.#FileWriteAllText")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.State.#SelectMany`3(PGSolutions.Monads.State`2<!!0,!!1>,System.Func`2<!!1,PGSolutions.Monads.State`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.StatePayload`2.#.ctor(System.Func`1<PGSolutions.Monads.StructTuple`2<!0,!1>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.State.#SelectMany`4(PGSolutions.Monads.State`2<!!0,!!1>,System.Func`2<!!1,PGSolutions.Monads.State`2<!!0,!!2>>,System.Func`3<!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.TaskExtensionsAsync.#SelectMany`2(System.Threading.Tasks.Task`1<!!0>,System.Func`2<!!0,System.Threading.Tasks.Task`1<!!1>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.TaskExtensionsAsync.#SelectMany`3(System.Threading.Tasks.Task`1<!!0>,System.Func`2<!!0,System.Threading.Tasks.Task`1<!!1>>,System.Func`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.TaskExtensionsAsync.#FlatMap`1(System.Threading.Tasks.Task`1<System.Threading.Tasks.Task`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.FsmTask`1+FsmReader.#Invoke(!0)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.MaybeX`1.#SelectMany`1(System.Func`2<!0,PGSolutions.Monads.MaybeX`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.MaybeX`1.#SelectMany`2(System.Func`2<!0,PGSolutions.Monads.MaybeX`1<!!0>>,System.Func`3<!0,!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Monads.MaybeTaskExtensions+MaybeTask`1.#Invoke()")]

