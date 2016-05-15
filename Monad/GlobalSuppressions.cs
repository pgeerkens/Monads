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

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Diagnostics.Contracts")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Scope = "type", Target = "PGSolutions.Utilities.Monads.IO")]

[assembly: SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Scope = "member", Target = "PGSolutions.Utilities.Monads.Maybe`1.#op_BitwiseOr(PGSolutions.Utilities.Monads.Maybe`1<!0>,!0)")]
[assembly: SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Scope = "member", Target = "PGSolutions.Utilities.Monads.MaybeX`1.#op_BitwiseOr(PGSolutions.Utilities.Monads.MaybeX`1<!0>,!0)")]

[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#Action(System.Action)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#Action`1(System.Action`1<!!0>)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#Action`2(System.Action`2<!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#Action`4(System.Action`4<!!0,!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "1#", Scope = "member", Target = "PGSolutions.Utilities.Monads.Functions.#Second`2(!!0,!!1)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Scope = "member", Target = "PGSolutions.Utilities.Monads.Functions.#First`2(!!0,!!1)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Scope = "member", Target = "PGSolutions.Utilities.Monads.ReaderLinq.#Unit`1(PGSolutions.Utilities.Monads.Unit)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#Action`3(System.Action`3<!!0,!!1,!!2>)")]

[assembly: SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "PGSolutions.Utilities.Monads.Maybe`1.#Nothing")]
[assembly: SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "PGSolutions.Utilities.Monads.StatePayload`2.#ToState(PGSolutions.Utilities.Monads.StatePayload`2<!0,!1>)")]

[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.StateMaybe`2.#Invoke(!0)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.Maybe`1.#Bind`1(System.Func`2<!0,PGSolutions.Utilities.Monads.Maybe`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.MaybeExtensions.#SelectMany`2(PGSolutions.Utilities.Monads.Maybe`1<!!0>,System.Func`2<!!0,PGSolutions.Utilities.Monads.Maybe`1<!!1>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.MaybeExtensions.#SelectMany`3(PGSolutions.Utilities.Monads.Maybe`1<!!0>,System.Func`2<!!0,PGSolutions.Utilities.Monads.Maybe`1<!!1>>,System.Func`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.Maybe`1.#op_Implicit(PGSolutions.Utilities.Monads.Maybe`1<PGSolutions.Utilities.Monads.Maybe`1<!0>>):PGSolutions.Utilities.Monads.Maybe`1<!0>")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.Maybe.#Bind`2(PGSolutions.Utilities.Monads.Maybe`1<!!0>,System.Func`2<!!0,PGSolutions.Utilities.Monads.Maybe`1<!!1>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.MonadsX.MaybeX`1.#Bind`1(System.Func`2<!0,PGSolutions.Utilities.MonadsX.MaybeX`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.MonadsX.MaybeXExtensions.#SelectMany`2(PGSolutions.Utilities.MonadsX.MaybeX`1<!!0>,System.Func`2<!!0,PGSolutions.Utilities.MonadsX.MaybeX`1<!!1>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.MonadsX.MaybeXExtensions.#SelectMany`3(PGSolutions.Utilities.MonadsX.MaybeX`1<!!0>,System.Func`2<!!0,PGSolutions.Utilities.MonadsX.MaybeX`1<!!1>>,System.Func`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.MaybeX`1.#Bind`1(System.Func`2<!0,PGSolutions.Utilities.Monads.MaybeX`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.MaybeXExtensions.#SelectMany`2(PGSolutions.Utilities.Monads.MaybeX`1<!!0>,System.Func`2<!!0,PGSolutions.Utilities.Monads.MaybeX`1<!!1>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.MaybeXExtensions.#SelectMany`3(PGSolutions.Utilities.Monads.MaybeX`1<!!0>,System.Func`2<!!0,PGSolutions.Utilities.Monads.MaybeX`1<!!1>>,System.Func`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.Reader.#Bind`3(PGSolutions.Utilities.Monads.Reader`2<!!0,!!1>,System.Func`2<!!1,PGSolutions.Utilities.Monads.Reader`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.Reader.#Flatten`4(PGSolutions.Utilities.Monads.Reader`2<!!0,!!1>,System.Func`2<!!1,PGSolutions.Utilities.Monads.Reader`2<!!0,!!2>>,System.Func`3<!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.ReaderLinq.#SelectMany`3(PGSolutions.Utilities.Monads.Reader`2<!!0,!!1>,System.Func`2<!!1,PGSolutions.Utilities.Monads.Reader`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.ReaderLinq.#SelectMany`4(PGSolutions.Utilities.Monads.Reader`2<!!0,!!1>,System.Func`2<!!1,PGSolutions.Utilities.Monads.Reader`2<!!0,!!2>>,System.Func`3<!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.State.#Enumerate`2(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,!!0)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.State.#Bind`3(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Utilities.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Utilities.Monads.State`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.State.#Flatten`4(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Utilities.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Utilities.Monads.State`2<!!0,!!2>>,System.Func`3<!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.State.#Map`3(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Utilities.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Utilities.Monads.StatePayload`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.StateExtensions.#SelectMany`3(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Utilities.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Utilities.Monads.State`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.StatePayload`2.#Bind`1(System.Func`2<PGSolutions.Utilities.Monads.StatePayload`2<!0,!1>,PGSolutions.Utilities.Monads.StatePayload`2<!0,!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.StateExtensions.#Compose`3(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<!!0,PGSolutions.Utilities.Monads.State`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.StateExtensions.#SelectMany`4(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Utilities.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Utilities.Monads.State`2<!!0,!!2>>,System.Func`3<!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IO.#FileExists")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IO.#FileWriteAllText")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOLinqExtensions.#SelectMany`3(PGSolutions.Utilities.Monads.IO`1<!!0>,System.Func`2<!!0,PGSolutions.Utilities.Monads.IO`1<!!1>>,System.Func`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOLinqExtensions.#SelectMany`2(PGSolutions.Utilities.Monads.IO`1<!!0>,System.Func`2<!!0,PGSolutions.Utilities.Monads.IO`1<!!1>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#AsIO`1(System.Action`1<!!0>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#AsIO`2(System.Action`2<!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#AsIO`3(System.Action`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#AsIO`4(System.Action`4<!!0,!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#AsIO`2(System.Func`2<!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#AsIO`3(System.Func`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#AsIO`4(System.Func`4<!!0,!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#AsIO`5(System.Func`5<!!0,!!1,!!2,!!3,!!4>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.TaskExtensions.#SelectMany`2(System.Threading.Tasks.Task`1<!!0>,System.Func`2<!!0,System.Threading.Tasks.Task`1<!!1>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.TaskExtensions.#SelectMany`3(System.Threading.Tasks.Task`1<!!0>,System.Func`2<!!0,System.Threading.Tasks.Task`1<!!1>>,System.Func`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.TaskExtensions.#SelectMany`1(System.Threading.Tasks.Task,System.Func`2<PGSolutions.Utilities.Monads.Unit,System.Threading.Tasks.Task`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.TaskExtensions.#SelectMany`2(System.Threading.Tasks.Task,System.Func`2<PGSolutions.Utilities.Monads.Unit,System.Threading.Tasks.Task`1<!!0>>,System.Func`3<PGSolutions.Utilities.Monads.Unit,!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.EnumerableExtensions.#FirstUnit(System.Collections.Generic.IEnumerable`1<PGSolutions.Utilities.Monads.IO`1<PGSolutions.Utilities.Monads.Unit>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.EnumerableExtensions.#LastUnit(System.Collections.Generic.IEnumerable`1<PGSolutions.Utilities.Monads.IO`1<PGSolutions.Utilities.Monads.Unit>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IO.#FileReadAllText")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#Action`1(System.Action`1<!!0>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#Action`2(System.Action`2<!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#Action`3(System.Action`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#Action`4(System.Action`4<!!0,!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#Func`2(System.Func`2<!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#Func`3(System.Func`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#Func`4(System.Func`4<!!0,!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOExtensions.#Func`5(System.Func`5<!!0,!!1,!!2,!!3,!!4>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.UnitExtensions.#SelectMany`1(System.Func`1<PGSolutions.Utilities.Monads.Unit>,System.Func`2<PGSolutions.Utilities.Monads.Unit,System.Func`1<!!0>>,System.Func`3<PGSolutions.Utilities.Monads.Unit,!!0,PGSolutions.Utilities.Monads.Unit>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.UnitExtensions.#SelectMany(System.Func`1<PGSolutions.Utilities.Monads.Unit>,System.Func`2<PGSolutions.Utilities.Monads.Unit,System.Func`1<PGSolutions.Utilities.Monads.Unit>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.Maybe`1.#Flatten`2(System.Func`2<!0,PGSolutions.Utilities.Monads.Maybe`1<!!0>>,System.Func`3<!0,!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.Maybe.#Extract`1(PGSolutions.Utilities.Monads.Maybe`1<System.Func`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.Maybe`1.#SelectMany`1(System.Func`2<!0,PGSolutions.Utilities.Monads.Maybe`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.Maybe`1.#SelectMany`2(System.Func`2<!0,PGSolutions.Utilities.Monads.Maybe`1<!!0>>,System.Func`3<!0,!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.MaybeX`1.#SelectMany`1(System.Func`2<!0,PGSolutions.Utilities.Monads.MaybeX`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.State.#SelectMany`3(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Utilities.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Utilities.Monads.State`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.State.#SelectMany`4(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Utilities.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Utilities.Monads.State`2<!!0,!!2>>,System.Func`3<!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.MaybeX`1.#SelectMany`2(System.Func`2<!0,PGSolutions.Utilities.Monads.MaybeX`1<!!0>>,System.Func`3<!0,!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IMonad`1.#SelectMany`1(System.Func`2<!0,PGSolutions.Utilities.Monads.IMonad`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IMonad`1.#SelectMany`2(System.Func`2<!0,PGSolutions.Utilities.Monads.IMonad`1<!!0>>,System.Func`3<!0,!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOMonad.#FileExists")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOMonad.#FileReadAllText")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IOMonad.#FileWriteAllText")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IO`1.#SelectMany`1(System.Func`2<!0,PGSolutions.Utilities.Monads.IO`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IO`1.#SelectMany`2(System.Func`2<!0,PGSolutions.Utilities.Monads.IO`1<!!0>>,System.Func`3<!0,!!0,!!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "PGSolutions.Utilities.Monads.MaybeX`1.#Nothing")]

