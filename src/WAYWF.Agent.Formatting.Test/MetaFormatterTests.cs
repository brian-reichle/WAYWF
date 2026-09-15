// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Reflection;
using NUnit.Framework;
using WAYWF.Agent.Data;

namespace WAYWF.Agent.Formatting.Test;

[TestFixture]
public class MetaFormatterTests
{
	[Test]
	public void Format_KnownType()
	{
		var formatter = new MetaFormatter();
		formatter.Write(MetaKnownType.Int32);
		Assert.That(formatter.ToString(), Is.EqualTo("System.Int32"));
	}

	[Test]
	public void Format_SimpleResolvedType_NonNested()
	{
		var type = new MetaSimpleResolvedType(DummyModule, new MetaDataToken(0x02000001), null, "MyNamespace.MyClass", 0);
		var formatter = new MetaFormatter();
		formatter.Write(type);
		Assert.That(formatter.ToString(), Is.EqualTo("MyNamespace.MyClass"));
	}

	[Test]
	public void Format_SimpleResolvedType_Nested()
	{
		var outer = new MetaSimpleResolvedType(DummyModule, new MetaDataToken(0x02000001), null, "OuterClass", 0);
		var inner = new MetaSimpleResolvedType(DummyModule, new MetaDataToken(0x02000002), outer, "InnerClass", 0);
		var formatter = new MetaFormatter();
		formatter.Write(inner);
		Assert.That(formatter.ToString(), Is.EqualTo("OuterClass.InnerClass"));
	}

	[Test]
	public void Format_SimpleResolvedType_Generic()
	{
		var type = new MetaSimpleResolvedType(DummyModule, new MetaDataToken(0x02000001), null, "MyGeneric`1", 1);
		var formatter = new MetaFormatter();
		formatter.Write(type, [MetaKnownType.Int32]);
		Assert.That(formatter.ToString(), Is.EqualTo("MyGeneric<System.Int32>"));
	}

	[Test]
	public void Format_UnresolvedType()
	{
		var outer = new MetaUnresolvedType(new MetaDataToken(0x02000001), null, "OuterUnresolved");
		var inner = new MetaUnresolvedType(new MetaDataToken(0x02000002), outer, "InnerUnresolved");
		var formatter = new MetaFormatter();
		formatter.Write(inner);
		Assert.That(formatter.ToString(), Is.EqualTo("OuterUnresolved.InnerUnresolved"));
	}

	[Test]
	public void Format_UnresolvedType_WithOverrideTypeArgs()
	{
		var type = new MetaUnresolvedType(new MetaDataToken(0x02000001), null, "MyUnresolved");
		var formatter = new MetaFormatter();
		formatter.Write(type, [MetaKnownType.String]);
		Assert.That(formatter.ToString(), Is.EqualTo("MyUnresolved<System.String>"));
	}

	[Test]
	public void Format_GenType_ResolvedBase()
	{
		var baseType = new MetaSimpleResolvedType(DummyModule, new MetaDataToken(0x02000001), null, "Dictionary`2", 2);
		var genType = new MetaGenType(baseType, [MetaKnownType.String, MetaKnownType.Int32]);
		var formatter = new MetaFormatter();
		formatter.Write(genType);
		Assert.That(formatter.ToString(), Is.EqualTo("Dictionary<System.String, System.Int32>"));
	}

	[Test]
	public void Format_GenType_UnresolvedBase()
	{
		var baseType = new MetaUnresolvedType(new MetaDataToken(0x02000001), null, "CustomGen");
		var genType = new MetaGenType(baseType, [MetaKnownType.Boolean]);
		var formatter = new MetaFormatter();
		formatter.Write(genType);
		Assert.That(formatter.ToString(), Is.EqualTo("CustomGen<System.Boolean>"));
	}

	[Test]
	public void Format_ArrayType_1D()
	{
		var arrayType = new MetaArrayType(MetaKnownType.Int32, 1);
		var formatter = new MetaFormatter();
		formatter.Write(arrayType);
		Assert.That(formatter.ToString(), Is.EqualTo("System.Int32[]"));
	}

	[Test]
	public void Format_ArrayType_2D()
	{
		var arrayType = new MetaArrayType(MetaKnownType.Int32, 2);
		var formatter = new MetaFormatter();
		formatter.Write(arrayType);
		Assert.That(formatter.ToString(), Is.EqualTo("System.Int32[,]"));
	}

	[Test]
	public void Format_ArrayType_Jagged()
	{
		var innerArray = new MetaArrayType(MetaKnownType.Int32, 1);
		var outerArray = new MetaArrayType(innerArray, 1);
		var formatter = new MetaFormatter();
		formatter.Write(outerArray);
		Assert.That(formatter.ToString(), Is.EqualTo("System.Int32[][]"));
	}

	[Test]
	public void Format_ArrayType_WithDimensions()
	{
		var arrayType = new MetaArrayType(MetaKnownType.Int32, 2);
		var formatter = new MetaFormatter();
		formatter.Write(arrayType, [10, 20]);
		Assert.That(formatter.ToString(), Is.EqualTo("System.Int32[10, 20]"));
	}

	[Test]
	public void Format_ArrayType_JaggedWithDimensions()
	{
		var innerArray = new MetaArrayType(MetaKnownType.Int32, 2);
		var outerArray = new MetaArrayType(innerArray, 1);
		var formatter = new MetaFormatter();
		formatter.Write(outerArray, [5]);
		Assert.That(formatter.ToString(), Is.EqualTo("System.Int32[5][,]"));
	}

	[Test]
	public void Format_PointerType()
	{
		var pointerType = new MetaPointerType(MetaKnownType.Int32);
		var formatter = new MetaFormatter();
		formatter.Write(pointerType);
		Assert.That(formatter.ToString(), Is.EqualTo("System.Int32*"));
	}

	[Test]
	public void Format_NullableType()
	{
		var nullableType = new MetaNullableType(
			DummyModule,
			new MetaDataToken(0x02000001),
			new MetaDataToken(0x04000001),
			new MetaDataToken(0x04000002));
		var formatter = new MetaFormatter();
		formatter.Write(nullableType);
		Assert.That(formatter.ToString(), Is.EqualTo("System.Nullable`1"));
	}

	[Test]
	public void Format_EnumType()
	{
		var enumType = new MetaEnumType(
			DummyModule,
			new MetaDataToken(0x02000001),
			null,
			"MyEnum",
			MetaKnownType.Int32,
			false,
			[],
			[]);
		var formatter = new MetaFormatter();
		formatter.Write(enumType);
		Assert.That(formatter.ToString(), Is.EqualTo("MyEnum"));
	}

	[Test]
	public void Format_GCHandleType()
	{
		var gcHandleType = new MetaGCHandleType(
			DummyModule,
			new MetaDataToken(0x02000001),
			new MetaDataToken(0x04000001));
		var formatter = new MetaFormatter();
		formatter.Write(gcHandleType);
		Assert.That(formatter.ToString(), Is.EqualTo("System.Runtime.InteropServices.GCHandle"));
	}

	[Test]
	public void Format_VarType_ClassTypeArg()
	{
		var varType = new MetaVarType(false, 0);
		var formatter = new MetaFormatter
		{
			TypeArgs = [MetaKnownType.String, MetaKnownType.Int32],
			MethodArgsStart = 2,
		};
		formatter.Write(varType);
		Assert.That(formatter.ToString(), Is.EqualTo("System.String"));
	}

	[Test]
	public void Format_VarType_MethodTypeArg()
	{
		var varType = new MetaVarType(true, 0);
		var formatter = new MetaFormatter
		{
			TypeArgs = [MetaKnownType.String, MetaKnownType.Int32],
			MethodArgsStart = 1,
		};
		formatter.Write(varType);
		Assert.That(formatter.ToString(), Is.EqualTo("System.Int32"));
	}

	[Test]
	public void Format_VarType_OutOfBounds_ThrowsShitFanContactException()
	{
		var varType = new MetaVarType(false, 5);
		var formatter = new MetaFormatter
		{
			TypeArgs = [MetaKnownType.String],
			MethodArgsStart = 1,
		};
		Assert.That(() => formatter.Write(varType), Throws.TypeOf<ArgumentException>());
	}

	[Test]
	public void Format_Method_NonGeneric()
	{
		var declaringType = new MetaSimpleResolvedType(DummyModule, new MetaDataToken(0x02000001), null, "MyClass", 0);
		var signature = new MetaMethodSignature(CallingConventions.Standard, 0, new MetaVariable(MetaKnownType.Void, null, false, false), []);
		var method = new MetaMethod(new MetaDataToken(0x06000001), DummyModule, declaringType, "DoSomething", signature, []);

		var formatter = new MetaFormatter();
		formatter.Write(method);
		Assert.That(formatter.ToString(), Is.EqualTo("DoSomething"));
	}

	[Test]
	public void Format_Method_Generic()
	{
		var declaringType = new MetaSimpleResolvedType(DummyModule, new MetaDataToken(0x02000001), null, "MyClass", 0);
		var signature = new MetaMethodSignature(CallingConventions.Standard, 1, new MetaVariable(MetaKnownType.Void, null, false, false), []);
		var method = new MetaMethod(new MetaDataToken(0x06000001), DummyModule, declaringType, "DoGeneric", signature, []);

		var formatter = new MetaFormatter
		{
			TypeArgs = [MetaKnownType.Int32],
			MethodArgsStart = 0,
		};
		formatter.Write(method);
		Assert.That(formatter.ToString(), Is.EqualTo("DoGeneric<System.Int32>"));
	}

	[Test]
	public void Clear_ResetsFormatter()
	{
		var formatter = new MetaFormatter();
		formatter.Write(MetaKnownType.Int32);
		Assert.That(formatter.ToString(), Is.EqualTo("System.Int32"));

		formatter.Clear();
		Assert.That(formatter.ToString(), Is.Empty);

		formatter.Write(MetaKnownType.String);
		Assert.That(formatter.ToString(), Is.EqualTo("System.String"));
	}

	static MetaAssembly DummyAssembly { get; } = new MetaAssembly(@"C:\app\SomeAssembly.dll", "SomeAssembly", new Version(1, 0), null, null);
	static MetaModule DummyModule { get; } = new MetaModule(DummyAssembly, Identity.NewSource().New(), @"C:\app\SomeModule.dll", "SomeModule", false, false, Guid.NewGuid());
}
