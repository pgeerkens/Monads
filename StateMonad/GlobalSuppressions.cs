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

[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "StateMonad.State`1.#GetCompose`1(System.Func`2<!0,StateMonad.State`2<!!0,!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "StateMonad.State`1.#GetBind`1(System.Func`2<!0,StateMonad.State`2<!!0,!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "StateMonad.StateExtensions.#Compose`3(StateMonad.State`2<!!0,!!1>,System.Func`2<!!1,StateMonad.State`2<!!2,!!1>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "StateMonad.StateExtensions.#SelectMany`4(StateMonad.State`2<!!0,!!1>,System.Func`2<!!0,StateMonad.State`2<!!2,!!1>>,System.Func`3<!!0,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "StateMonad.StateExtensions.#SelectMany`3(StateMonad.State`2<!!0,!!1>,System.Func`2<!!0,StateMonad.State`2<!!2,!!1>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "StateMonad.StateExtensions.#Select`3(StateMonad.State`2<!!0,!!1>,System.Func`2<!!0,StateMonad.State`2<!!2,!!1>+StateTuple>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "StateMonad.State`2.#Map`1(System.Func`2<!0,StateMonad.State`2<!!0,!1>+StateTuple>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "StateMonad.State`2.#Bind`1(System.Func`2<!0,StateMonad.State`2<!!0,!1>>)")]

[assembly: SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "StateMonad.State`2.#RunState")]
[assembly: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "StateMonad.State`2.#RunState", Justification="Making this a property unnecessarily complicates CodeContract specification.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope = "type", Target = "StateMonad.State`2+Transformer")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object,System.Object)", Scope = "member", Target = "StateMonad.GcdDemo.#Run(System.Int32,System.Collections.Generic.IList`1<StateMonad.GCDState>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "StateMonad.State`2.#ToState(!0)",Justification="Recommended named alternative to operator.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "StateMonad.State`2.#ToState(StateMonad.State`2<!0,!1>+Transformer)",Justification="Recommended named alternative to operator.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "StateMonad.State`1.#GetCompose`1(StateMonad.Func`2<!0,StateMonad.State`2<!!0,!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "StateMonad.State`1.#GetBind`1(StateMonad.Func`2<!0,StateMonad.State`2<!!0,!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "StateMonad.State.#Flatten`4(StateMonad.State`2<!!1,!!0>,StateMonad.Func`2<!!1,StateMonad.State`2<!!2,!!0>>,StateMonad.Func`3<!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "StateMonad.State.#Map`3(StateMonad.State`2<!!1,!!0>,StateMonad.Func`2<!!1,StateMonad.State`2<!!2,!!0>+StateTuple>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "StateMonad.State.#Bind`3(StateMonad.State`2<!!1,!!0>,StateMonad.Func`2<!!1,StateMonad.State`2<!!2,!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "StateMonad.StateMaybe2`2+Transformer.#Invoke(!1)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.IBindable`1.#Bind`1(System.Func`2<!0,PGSolutions.Utilities.Monads.IBindable`1<!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.State.#Bind`3(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<!!1,PGSolutions.Utilities.Monads.State`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.State.#Map`3(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<!!1,PGSolutions.Utilities.Monads.StatePayload`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.State.#Flatten`4(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<!!1,PGSolutions.Utilities.Monads.State`2<!!0,!!2>>,System.Func`3<!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope = "type", Target = "PGSolutions.Utilities.Monads.State`1+Selector")]
[assembly: SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope = "type", Target = "PGSolutions.Utilities.Monads.State`1+Transform")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.State`1.#GetCompose")]
[assembly: SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "PGSolutions.Utilities.Monads.State`1.#Get")]
[assembly: SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "PGSolutions.Utilities.Monads.State`1.#GetCompose")]
[assembly: SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "PGSolutions.Utilities.Monads.State`1.#Modify")]
[assembly: SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "PGSolutions.Utilities.Monads.State`1.#Put")]
[assembly: SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "PGSolutions.Utilities.Monads.State`1.#GetCompose")]
[assembly: SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "PGSolutions.Utilities.Monads.State`1.#Modify")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.State`1.#Modify")]
[assembly: SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "PGSolutions.Utilities.Monads.State`1.#Get")]
[assembly: SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "PGSolutions.Utilities.Monads.State`1.#Put")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.State`1.#Put")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.StateExtensions.#Select`3(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<!!1,PGSolutions.Utilities.Monads.StatePayload`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.StateExtensions.#SelectMany`3(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<!!1,PGSolutions.Utilities.Monads.State`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.StateExtensions.#SelectMany`4(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<!!1,PGSolutions.Utilities.Monads.State`2<!!0,!!2>>,System.Func`3<!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.StateExtensions.#Compose`3(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<!!0,PGSolutions.Utilities.Monads.State`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.StateMaybe`2.#Invoke(!0)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.StatePayload`2.#Bind`1(System.Func`2<PGSolutions.Utilities.Monads.StatePayload`2<!0,!1>,PGSolutions.Utilities.Monads.StatePayload`2<!0,!!0>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "PGSolutions.Utilities.Monads.StatePayload`2.#ToState(PGSolutions.Utilities.Monads.StatePayload`2<!0,!1>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.StateExtensions.#SelectMany`4(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Utilities.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Utilities.Monads.State`2<!!0,!!2>>,System.Func`3<!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.StateExtensions.#SelectMany`3(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Utilities.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Utilities.Monads.State`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.State.#Map`3(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Utilities.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Utilities.Monads.StatePayload`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.State.#Flatten`4(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Utilities.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Utilities.Monads.State`2<!!0,!!2>>,System.Func`3<!!1,!!2,!!3>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.State.#Bind`3(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,System.Func`2<PGSolutions.Utilities.Monads.StatePayload`2<!!0,!!1>,PGSolutions.Utilities.Monads.State`2<!!0,!!2>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGSolutions.Utilities.Monads.State.#Enumerate`2(PGSolutions.Utilities.Monads.State`2<!!0,!!1>,!!0)")]
