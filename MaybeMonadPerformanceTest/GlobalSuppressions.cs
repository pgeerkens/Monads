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
using System.Diagnostics.CodeAnalysis;

// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body", Scope = "member", Target = "PGSolutions.Utilities.Monads.Program.#GcdTest()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Diagnostics.Contracts.ContractExtensions.#AssumeInvariant`1(!!0)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Napoleonics", Scope = "namespace", Target = "PGNapoleonics.HexUtilities.Common")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "rhs", Scope = "member", Target = "PGNapoleonics.HexUtilities.Common.Maybe`1.#op_Inequality(PGNapoleonics.HexUtilities.Common.Maybe`1<!0>,PGNapoleonics.HexUtilities.Common.Maybe`1<!0>)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "rhs", Scope = "member", Target = "PGNapoleonics.HexUtilities.Common.Maybe`1.#op_Equality(PGNapoleonics.HexUtilities.Common.Maybe`1<!0>,PGNapoleonics.HexUtilities.Common.Maybe`1<!0>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames")]
[assembly: SuppressMessage("Microsoft.Design", "CA1014:MarkAssembliesWithClsCompliant")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGNapoleonics.HexUtilities.Common.MaybeExtensions.#SelectMany`2(PGNapoleonics.HexUtilities.Common.Maybe`1<!!0>,System.Func`2<!!0,PGNapoleonics.HexUtilities.Common.Maybe`1<!!1>>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGNapoleonics.HexUtilities.Common.MaybeExtensions.#SelectMany`3(PGNapoleonics.HexUtilities.Common.Maybe`1<!!0>,System.Func`2<!!0,PGNapoleonics.HexUtilities.Common.Maybe`1<!!1>>,System.Func`3<!!0,!!1,!!2>)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "PGNapoleonics.HexUtilities.Common.Maybe`1.#Bind`1(System.Func`2<!0,PGNapoleonics.HexUtilities.Common.Maybe`1<!!0>>)")]
//[assembly: SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Scope = "member", Target = "PGSolutions.Utilities.Monads.Gcd_S4.#.cctor()")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)", Scope = "member", Target = "PGSolutions.Utilities.Monads.Program.#BasicTest()")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)", Scope = "member", Target = "PGSolutions.Utilities.Monads.Program.#WesDyerTest()")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)", Scope = "member", Target = "PGSolutions.Utilities.Monads.Program.#MikeHadlowTest()")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)", Scope = "member", Target = "PGSolutions.Utilities.Monads.Program.#ExternalStateTest()")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object,System.Object)", Scope = "member", Target = "PGSolutions.Utilities.Monads.Program.#GcdTest1(System.Collections.Generic.IList`1<System.Int32>)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object,System.Object,System.Object)", Scope = "member", Target = "PGSolutions.Utilities.Monads.Program.#GcdTest()")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)", Scope = "member", Target = "PGSolutions.Utilities.Monads.Program.#GcdTest()")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object)", Scope = "member", Target = "PGSolutions.Utilities.Monads.GcdDemo.#WriteResults(PGSolutions.Utilities.Monads.GCDState,System.String,System.Boolean,System.String)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object,System.Object)", Scope = "member", Target = "PGSolutions.Utilities.Monads.GcdDemo.#WriteResults(PGSolutions.Utilities.Monads.GCDState,System.String,System.Boolean,System.String)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)", Scope = "member", Target = "PGSolutions.Utilities.Monads.GcdDemo.#WriteTiming(System.TimeSpan)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object,System.Object)", Scope = "member", Target = "PGSolutions.Utilities.Monads.GcdDemo.#WriteResults(System.String,System.String,System.Boolean,System.String)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object)", Scope = "member", Target = "PGSolutions.Utilities.Monads.GcdDemo.#WriteResults(System.String,System.String,System.Boolean,System.String)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])", Scope = "member", Target = "PGSolutions.Utilities.Monads.Program.#GcdTest()")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString", Scope = "member", Target = "PGSolutions.Utilities.Monads.Program.#ExternalStateTest()")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString", Scope = "member", Target = "PGSolutions.Utilities.Monads.Program.#MikeHadlowTest()")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)", Scope = "member", Target = "PGSolutions.Utilities.Monads.GcdDemo.#Run(System.Collections.Generic.IList`1<PGSolutions.Utilities.Monads.GCDState>)")]
