// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
using WAYWF.Agent.Data;

namespace WAYWF.Agent.Formatting.Test;

[TestFixture]
public class StateFormatterTests
{
	[Test]
	public void Format_MinimalProcess_GeneratesValidXml()
	{
		var options = new CaptureOptions(walkHeap: false, waitSeconds: 0);
		var nativeUser = new RuntimeUser("testuser", "testdomain");
		var native = new RuntimeNative(1234, @"Q:\app\test.exe", nativeUser, []);
		var clrVersion = new Version(4, 0, 30319);

		var process = new RuntimeProcess(
			options,
			native,
			clrVersion,
			[],
			[],
			[],
			[],
			[]);

		var xml = FormatProcessXml(process);

		Assert.That(xml, Does.StartWith("<?xml version=\"1.0\" encoding=\"utf-16\"?><?xml-stylesheet type=\"text/xsl\" href=\"waywf.xslt\"?>"));

		var doc = XDocument.Parse(xml);
		var root = doc.Root;
		Assert.That(root.Name, Is.EqualTo(E.WAYWF));

		var os = root.Element(E.Os);
		Assert.That(os, Is.Not.Null);

		var login = root.Element(E.Login);
		Assert.That(login, Is.Not.Null);

		var procElem = root.Element(E.Process);
		Assert.That(procElem, Is.Not.Null);
		Assert.That(procElem.Attribute(A.Pid)?.Value, Is.EqualTo("1234"));
		Assert.That(procElem.Attribute(A.ClrVersion)?.Value, Is.EqualTo("4.0.30319"));
		Assert.That(procElem.Attribute(A.ImagePath)?.Value, Is.EqualTo(@"Q:\app\test.exe"));
	}

	[Test]
	public void Format_WithOptions_IncludesOptionsAttributes()
	{
		var options = new CaptureOptions(walkHeap: true, waitSeconds: 15);
		var native = new RuntimeNative(5678, null, new RuntimeUser("user", "domain"), []);

		var process = new RuntimeProcess(
			options,
			native,
			clrVersion: null,
			[],
			[],
			[],
			[],
			[]);

		var xml = FormatProcessXml(process);
		var doc = XDocument.Parse(xml);
		var root = doc.Root;

		Assert.That(root.Attribute(A.WalkHeap)?.Value, Is.EqualTo("true"));
		Assert.That(root.Attribute(A.Wait)?.Value, Is.EqualTo("15"));
	}

	[Test]
	public void Format_AppDomainsAssembliesAndModules()
	{
		var assembly = new MetaAssembly(
			@"Q:\app\MyAssembly.dll",
			"MyAssembly",
			new Version(1, 2, 3, 4),
			0x1234567890ABCDEF,
			"en-US");

		var module1 = new MetaModule(
			assembly,
			Identity.NewSource().New(),
			@"Q:\app\Module1.dll",
			"Module1.dll",
			isInMemory: true,
			isDynamic: false,
			Guid.NewGuid());

		var appDomain = new RuntimeAppDomain(
			appDomainId: 10,
			name: "TestAppDomain",
			modules: [module1]);

		var options = new CaptureOptions(false, 0);
		var native = new RuntimeNative(100, null, new RuntimeUser("user", "domain"), []);
		var process = new RuntimeProcess(
			options,
			native,
			clrVersion: null,
			appDomains: [appDomain],
			threads: [],
			documents: [],
			referenceValues: [],
			pendingTasks: []);

		var xml = FormatProcessXml(process);
		var doc = XDocument.Parse(xml);

		var appDomainElem = doc.Root.Element(E.Process).Element(E.AppDomain);
		Assert.That(appDomainElem, Is.Not.Null);
		Assert.That(appDomainElem.Attribute(A.Id)?.Value, Is.EqualTo("10"));
		Assert.That(appDomainElem.Attribute(A.Name)?.Value, Is.EqualTo("TestAppDomain"));

		var assemblyElem = appDomainElem.Element(E.Assembly);
		Assert.That(assemblyElem, Is.Not.Null);
		Assert.That(assemblyElem.Attribute(A.Name)?.Value, Is.EqualTo("MyAssembly"));
		Assert.That(assemblyElem.Attribute(A.Version)?.Value, Is.EqualTo("1.2.3.4"));

		var moduleElem = assemblyElem.Element(E.Module);
		Assert.That(moduleElem, Is.Not.Null);
		Assert.That(moduleElem.Attribute(A.Name)?.Value, Is.EqualTo("Module1.dll"));
		Assert.That(moduleElem.Attribute(A.IsInMemory)?.Value, Is.EqualTo("true"));
	}

	[Test]
	public void Format_SourceDocuments_TrimsCommonPrefix()
	{
		var idSource = Identity.NewSource();
		var doc1 = new SourceDocument(idSource.New(), @"Q:\solution\src\File1.cs", SourceLanguage.CSharp, SourceDocumentType.Text);
		var doc2 = new SourceDocument(idSource.New(), @"Q:\solution\src\Folder\File2.cs", SourceLanguage.CSharp, SourceDocumentType.Text);

		var options = new CaptureOptions(false, 0);
		var native = new RuntimeNative(100, null, new RuntimeUser("user", "domain"), []);
		var process = new RuntimeProcess(
			options,
			native,
			clrVersion: null,
			appDomains: [],
			threads: [],
			documents: [doc1, doc2],
			referenceValues: [],
			pendingTasks: []);

		var xml = FormatProcessXml(process);
		var xDoc = XDocument.Parse(xml);

		var sourceElem = xDoc.Root.Element(E.Process).Element(E.Source);
		Assert.That(sourceElem, Is.Not.Null);
		Assert.That(sourceElem.Attribute(A.Path)?.Value, Is.EqualTo(@"Q:\solution\src\"));

		var docElems = sourceElem.Elements(E.Document);
		var paths = docElems.Select(e => e.Value).ToList();
		Assert.That(paths, Contains.Item("File1.cs"));
		Assert.That(paths, Contains.Item(@"Folder\File2.cs"));
	}

	[Test]
	public void Format_Windows()
	{
		var window = new RuntimeWindow(
			handle: new IntPtr(0x1000),
			threadID: 42,
			owner: new IntPtr(0x2000),
			className: "TestWindowClass",
			isVisible: true,
			isEnabled: true,
			caption: "Window Title");

		var options = new CaptureOptions(false, 0);
		var native = new RuntimeNative(100, null, new RuntimeUser("user", "domain"), [window]);
		var process = new RuntimeProcess(
			options,
			native,
			clrVersion: null,
			appDomains: [],
			threads: [],
			documents: [],
			referenceValues: [],
			pendingTasks: []);

		var xml = FormatProcessXml(process);
		var doc = XDocument.Parse(xml);

		var winElem = doc.Root.Element(E.Process).Element(E.Window);
		Assert.That(winElem, Is.Not.Null);
		Assert.That(winElem.Attribute(A.HWND)?.Value, Is.EqualTo("4096"));
		Assert.That(winElem.Attribute(A.OwnerThread)?.Value, Is.EqualTo("42"));
		Assert.That(winElem.Attribute(A.ClassName)?.Value, Is.EqualTo("TestWindowClass"));
		Assert.That(winElem.Attribute(A.Visible)?.Value, Is.EqualTo("true"));
		Assert.That(winElem.Value, Is.EqualTo("Window Title"));
	}

	[Test]
	public void Format_ThreadsAndFrames()
	{
		var declaringType = new MetaSimpleResolvedType(DummyModule, new MetaDataToken(0x02000001), null, "MyClass", 0);
		var signature = new MetaMethodSignature(CallingConventions.Standard, 0, new MetaVariable(MetaKnownType.Void, null, false, false), []);
		var method = new MetaMethod(new MetaDataToken(0x06000001), DummyModule, declaringType, "MyMethod", signature, []);

		var ilFrame = new RuntimeILFrame(
			method: method,
			ilOffset: 12,
			ilMapping: RuntimeILMapping.Epilog,
			source: null,
			@this: null,
			typeArgs: [],
			arguments: [],
			locals: [],
			localNames: [])
		{
			Duration = 1.23456,
		};

		var internalFrame = new RuntimeInternalFrame(RuntimeInternalFrameKind.ManagedToUnmanaged);

		var chain = new RuntimeFrameChain(RuntimeFrameChainReason.ClassConstructor, [ilFrame, internalFrame]);
		var thread = new RuntimeThread(101, RuntimeThreadStates.Suspended, [chain], []);

		var options = new CaptureOptions(false, 0);
		var native = new RuntimeNative(100, null, new RuntimeUser("user", "domain"), []);
		var process = new RuntimeProcess(
			options,
			native,
			clrVersion: null,
			appDomains: [],
			threads: [thread],
			documents: [],
			referenceValues: [],
			pendingTasks: []);

		var xml = FormatProcessXml(process);
		var doc = XDocument.Parse(xml);

		var threadElem = doc.Root.Element(E.Process).Element(E.Thread);
		Assert.That(threadElem, Is.Not.Null);
		Assert.That(threadElem.Attribute("tid")?.Value, Is.EqualTo("101"));

		var chainElem = threadElem.Element(E.Chain);
		Assert.That(chainElem, Is.Not.Null);

		var frameElem = chainElem.Element(E.Frame);
		Assert.That(frameElem, Is.Not.Null);
		Assert.That(frameElem.Attribute(A.MethodDisplayText)?.Value, Is.EqualTo("MyMethod"));
		Assert.That(frameElem.Attribute(A.ILOffset)?.Value, Is.EqualTo("12"));
		Assert.That(frameElem.Attribute(A.ILMapping)?.Value, Is.EqualTo("Epilog"));
		Assert.That(frameElem.Attribute(A.Duration)?.Value, Is.EqualTo("1.2346"));

		var internalFrameElem = chainElem.Element(E.InternalFrame);
		Assert.That(internalFrameElem, Is.Not.Null);
		Assert.That(internalFrameElem.Value, Is.EqualTo("ManagedToUnmanaged"));
	}

	[Test]
	public void Format_PendingTasks()
	{
		var declaringType = new MetaSimpleResolvedType(DummyModule, new MetaDataToken(0x02000001), null, "MyDeclaringType", 0);
		var asyncMethodSignature = new MetaMethodSignature(CallingConventions.Standard, 0, new MetaVariable(MetaKnownType.Void, null, false, false), []);
		var asyncMethod = new MetaMethod(new MetaDataToken(0x06000001), DummyModule, declaringType, "AsyncDoWork", asyncMethodSignature, []);

		var descriptor = new StateMachineDescriptor(
			asyncMethod,
			new MetaDataToken(0x06000002),
			declaringType,
			stateField: null,
			thisField: null,
			taskFieldSequence: default,
			paramFields: [],
			localFields: []);

		var stateValue = new RuntimeSimpleValue(Identity.NewSource().New(), MetaKnownType.Int32, 2);

		var pendingTask = new PendingStateMachineTask(
			descriptor,
			typeArgs: [],
			stateValue: stateValue,
			thisValue: null,
			taskValue: null,
			parameterValues: [],
			localValues: [],
			state: new SourceAsyncState(42, null));

		var options = new CaptureOptions(false, 0);
		var native = new RuntimeNative(100, null, new RuntimeUser("user", "domain"), []);
		var process = new RuntimeProcess(
			options,
			native,
			clrVersion: null,
			appDomains: [],
			threads: [],
			documents: [],
			referenceValues: [],
			pendingTasks: [pendingTask]);

		var xml = FormatProcessXml(process);
		var doc = XDocument.Parse(xml);

		var pendingTasksElem = doc.Root.Element(E.Process).Element(E.PendingTasks);
		Assert.That(pendingTasksElem, Is.Not.Null);

		var smTaskElem = pendingTasksElem.Element(E.PendingSMTask);
		Assert.That(smTaskElem, Is.Not.Null);
		Assert.That(smTaskElem.Attribute(A.MethodDisplayText)?.Value, Is.EqualTo("AsyncDoWork"));
		Assert.That(smTaskElem.Attribute(A.State)?.Value, Is.EqualTo("2"));
		Assert.That(smTaskElem.Attribute(A.ILOffset)?.Value, Is.EqualTo("42"));
	}

	[Test]
	public void Format_GlobalValues_SimpleValue_ReferenceCount()
	{
		var idSource = Identity.NewSource();
		var id1 = idSource.New();
		var id2 = idSource.New();

		var globalValue = new RuntimeSimpleValue(id1, MetaKnownType.String, "GlobalText")
		{
			ReferenceCount = 2,
		};
		var localOnlyValue = new RuntimeSimpleValue(id2, MetaKnownType.String, "LocalOnlyText")
		{
			ReferenceCount = 1,
		};
		var nullIdValue = new RuntimeSimpleValue(null, MetaKnownType.String, "NullIdText")
		{
			ReferenceCount = 2,
		};

		var process = CreateProcess(referenceValues: [globalValue, localOnlyValue, nullIdValue]);
		var xml = FormatProcessXml(process);
		var doc = XDocument.Parse(xml);

		var procElem = doc.Root.Element(E.Process);
		var valueElems = procElem.Elements(E.Value).ToList();

		Assert.That(valueElems, Has.Count.EqualTo(1));
		Assert.That(valueElems[0].Attribute(A.ObjectID)?.Value, Is.EqualTo(id1.ToString()));
		Assert.That(valueElems[0].Attribute(A.Type)?.Value, Is.EqualTo("System.String"));
		Assert.That(valueElems[0].Value, Is.EqualTo("GlobalText"));
	}

	[Test]
	public void Format_GlobalValues_SimpleValue_TextFormatting()
	{
		var idSource = Identity.NewSource();
		var id1 = idSource.New();
		var id2 = idSource.New();
		var id3 = idSource.New();
		var id4 = idSource.New();

		var normalValue = new RuntimeSimpleValue(id1, MetaKnownType.String, "NormalText") { ReferenceCount = 2 };
		var cdataValue = new RuntimeSimpleValue(id2, MetaKnownType.String, "Line 1\nLine 2\tTabbed") { ReferenceCount = 2 };
		var suppressedValue = new RuntimeSimpleValue(id3, MetaKnownType.String, "BadChar\0Value") { ReferenceCount = 2 };
		var nullInnerValue = new RuntimeSimpleValue(id4, MetaKnownType.Object, null) { ReferenceCount = 2 };

		var process = CreateProcess(referenceValues: [normalValue, cdataValue, suppressedValue, nullInnerValue]);
		var xml = FormatProcessXml(process);
		var doc = XDocument.Parse(xml);

		var procElem = doc.Root.Element(E.Process);
		var valueElems = procElem.Elements(E.Value).ToList();

		Assert.That(valueElems, Has.Count.EqualTo(4));

		// Normal text
		Assert.That(valueElems[0].Attribute(A.ObjectID)?.Value, Is.EqualTo(id1.ToString()));
		Assert.That(valueElems[0].Value, Is.EqualTo("NormalText"));
		Assert.That(valueElems[0].Attribute(A.Suppressed), Is.Null);

		// CDATA text
		Assert.That(valueElems[1].Attribute(A.ObjectID)?.Value, Is.EqualTo(id2.ToString()));
		Assert.That(valueElems[1].Value, Is.EqualTo("Line 1\nLine 2\tTabbed"));
		var cdataNode = valueElems[1].Nodes().OfType<XCData>().FirstOrDefault();
		Assert.That(cdataNode, Is.Not.Null);
		Assert.That(cdataNode.Value, Is.EqualTo("Line 1\nLine 2\tTabbed"));

		// Suppressed unsafe XML character
		Assert.That(valueElems[2].Attribute(A.ObjectID)?.Value, Is.EqualTo(id3.ToString()));
		Assert.That(valueElems[2].Attribute(A.Suppressed)?.Value, Is.EqualTo("true"));
		Assert.That(valueElems[2].Value, Is.Empty);

		// Null inner value
		Assert.That(valueElems[3].Attribute(A.ObjectID)?.Value, Is.EqualTo(id4.ToString()));
		Assert.That(valueElems[3].Attribute(A.Type)?.Value, Is.EqualTo("System.Object"));
		Assert.That(valueElems[3].Value, Is.Empty);
		Assert.That(valueElems[3].Attribute(A.Suppressed), Is.Null);
	}

	[Test]
	public void Format_GlobalValues_RcwValue()
	{
		var idSource = Identity.NewSource();
		var id1 = idSource.New();
		var id2 = idSource.New();

		var globalRcw = new RuntimeRcwValue(
			id: id1,
			type: MetaKnownType.Object,
			interfaceTypes: [MetaKnownType.Int32, MetaKnownType.String],
			interfacePointers: [
				new RuntimeNativeInterface(new RuntimeVirtualAddress(new MemoryAddress(0x1000)), new RuntimeVirtualAddress(new MemoryAddress(0x2000))),
				new RuntimeNativeInterface(new RuntimeVirtualAddress(new MemoryAddress(0x3000)), new RuntimeVirtualAddress(new MemoryAddress(0x4000))),
			])
		{
			ReferenceCount = 2,
		};

		var localOnlyRcw = new RuntimeRcwValue(
			id: id2,
			type: MetaKnownType.Object,
			interfaceTypes: [MetaKnownType.Int32],
			interfacePointers: [])
		{
			ReferenceCount = 1,
		};

		var process = CreateProcess(referenceValues: [globalRcw, localOnlyRcw]);
		var xml = FormatProcessXml(process);
		var doc = XDocument.Parse(xml);

		var procElem = doc.Root.Element(E.Process);
		var rcwElems = procElem.Elements(E.RCWValue).ToList();

		Assert.That(rcwElems, Has.Count.EqualTo(1));
		Assert.That(rcwElems[0].Attribute(A.ObjectID)?.Value, Is.EqualTo(id1.ToString()));
		Assert.That(rcwElems[0].Attribute(A.Type)?.Value, Is.EqualTo("System.Object"));

		var managedElems = rcwElems[0].Elements(E.Managed).ToList();
		Assert.That(managedElems, Has.Count.EqualTo(2));
		Assert.That(managedElems[0].Attribute("type")?.Value, Is.EqualTo("System.Int32"));
		Assert.That(managedElems[1].Attribute("type")?.Value, Is.EqualTo("System.String"));

		var nativeElems = rcwElems[0].Elements(E.Native).ToList();
		Assert.That(nativeElems, Has.Count.EqualTo(2));
		Assert.That(nativeElems[0].Attribute("ptr")?.Value, Is.EqualTo("0000000000001000"));
		Assert.That(nativeElems[0].Attribute("vtbl")?.Value, Is.EqualTo("0000000000002000"));
		Assert.That(nativeElems[1].Attribute("ptr")?.Value, Is.EqualTo("0000000000003000"));
		Assert.That(nativeElems[1].Attribute("vtbl")?.Value, Is.EqualTo("0000000000004000"));
	}

	[Test]
	public void Format_GlobalValues_PointerAndNull_NotEmitted()
	{
		var pointerValue = new RuntimePointerValue(MetaKnownType.Int32, new MemoryAddress(0x1234), null)
		{
			ReferenceCount = 5,
		};
		var nullValue = RuntimeNullValue.Instance;
		nullValue.ReferenceCount = 5;

		var process = CreateProcess(referenceValues: [pointerValue, nullValue]);
		var xml = FormatProcessXml(process);
		var doc = XDocument.Parse(xml);

		var procElem = doc.Root.Element(E.Process);
		Assert.That(procElem.Element(E.PointerValue), Is.Null);
		Assert.That(procElem.Element(E.Null), Is.Null);
	}

	[Test]
	public void Format_LocalValues_NullValue()
	{
		var declaringType = new MetaSimpleResolvedType(DummyModule, new MetaDataToken(0x02000001), null, "MyClass", 0);
		var signature = new MetaMethodSignature(
			CallingConventions.HasThis,
			0,
			new MetaVariable(MetaKnownType.Void, null, false, false),
			[new MetaVariable(MetaKnownType.Object, "param1", false, false)]);

		var method = new MetaMethod(
			new MetaDataToken(0x06000001),
			DummyModule,
			declaringType,
			"MyMethod",
			signature,
			[new MetaVariable(MetaKnownType.Object, "local1", false, false)]);

		var ilFrame = new RuntimeILFrame(
			method: method,
			ilOffset: 0,
			ilMapping: RuntimeILMapping.Exact,
			source: null,
			@this: RuntimeNullValue.Instance,
			typeArgs: [],
			arguments: [RuntimeNullValue.Instance],
			locals: [RuntimeNullValue.Instance],
			localNames: ["local1"]);

		var blockingObject = new RuntimeBlockingObject(
			value: RuntimeNullValue.Instance,
			ownerId: 0,
			timeout: 0,
			blockingReason: RuntimeBlockingReason.Wait);

		var chain = new RuntimeFrameChain(RuntimeFrameChainReason.Unknown, [ilFrame]);
		var thread = new RuntimeThread(1, RuntimeThreadStates.None, [chain], [blockingObject]);

		var process = CreateProcess(threads: [thread]);
		var xml = FormatProcessXml(process);
		var doc = XDocument.Parse(xml);

		var procElem = doc.Root.Element(E.Process);
		var threadElem = procElem.Element(E.Thread);

		var bobjElem = threadElem.Element(E.BlockingObject);
		Assert.That(bobjElem.Element(E.Null), Is.Not.Null);

		var frameElem = threadElem.Element(E.Chain).Element(E.Frame);
		Assert.That(frameElem.Element(E.This)?.Element(E.Null), Is.Not.Null);
		Assert.That(frameElem.Element(E.Param)?.Element(E.Null), Is.Not.Null);
		Assert.That(frameElem.Element(E.Local)?.Element(E.Null), Is.Not.Null);
	}

	[Test]
	public void Format_LocalValues_SimpleValue_InlineVsRef()
	{
		var idSource = Identity.NewSource();
		var id1 = idSource.New();
		var id2 = idSource.New();

		var inlineValue = new RuntimeSimpleValue(id1, MetaKnownType.String, "InlineString")
		{
			ReferenceCount = 1,
		};
		var refValue = new RuntimeSimpleValue(id2, MetaKnownType.Int32, 123)
		{
			ReferenceCount = 2,
		};

		var declaringType = new MetaSimpleResolvedType(DummyModule, new MetaDataToken(0x02000001), null, "MyClass", 0);
		var signature = new MetaMethodSignature(
			CallingConventions.Standard,
			0,
			new MetaVariable(MetaKnownType.Void, null, false, false),
			[
				new MetaVariable(MetaKnownType.String, "pInline", false, false),
				new MetaVariable(MetaKnownType.Int32, "pRef", false, false),
			]);

		var method = new MetaMethod(new MetaDataToken(0x06000001), DummyModule, declaringType, "MyMethod", signature, []);

		var ilFrame = new RuntimeILFrame(
			method: method,
			ilOffset: 0,
			ilMapping: RuntimeILMapping.Exact,
			source: null,
			@this: null,
			typeArgs: [],
			arguments: [inlineValue, refValue],
			locals: [],
			localNames: []);

		var chain = new RuntimeFrameChain(RuntimeFrameChainReason.Unknown, [ilFrame]);
		var thread = new RuntimeThread(1, RuntimeThreadStates.None, [chain], []);

		var process = CreateProcess(referenceValues: [refValue], threads: [thread]);
		var xml = FormatProcessXml(process);
		var doc = XDocument.Parse(xml);

		var frameElem = doc.Root.Element(E.Process)
			.Element(E.Thread)
			.Element(E.Chain)
			.Element(E.Frame);

		var paramElems = frameElem.Elements(E.Param).ToList();
		Assert.That(paramElems, Has.Count.EqualTo(2));

		// First parameter: ReferenceCount == 1 -> formatted inline <value>, no objectId attribute
		var inlineElem = paramElems[0].Element(E.Value);
		Assert.That(inlineElem, Is.Not.Null);
		Assert.That(inlineElem.Attribute(A.ObjectID), Is.Null);
		Assert.That(inlineElem.Attribute(A.Type)?.Value, Is.EqualTo("System.String"));
		Assert.That(inlineElem.Value, Is.EqualTo("InlineString"));

		// Second parameter: ReferenceCount == 2 -> formatted as <valueRef objectId="..." />
		var refElem = paramElems[1].Element(E.ValueRef);
		Assert.That(refElem, Is.Not.Null);
		Assert.That(refElem.Attribute(A.ObjectID)?.Value, Is.EqualTo(id2.ToString()));
	}

	[Test]
	public void Format_LocalValues_RcwValue_InlineVsRef()
	{
		var idSource = Identity.NewSource();
		var id1 = idSource.New();
		var id2 = idSource.New();

		var inlineRcw = new RuntimeRcwValue(
			id: id1,
			type: MetaKnownType.Object,
			interfaceTypes: [MetaKnownType.Int32],
			interfacePointers: [new RuntimeNativeInterface(new RuntimeVirtualAddress(new MemoryAddress(0x1000)), new RuntimeVirtualAddress(new MemoryAddress(0x2000)))])
		{
			ReferenceCount = 1,
		};

		var refRcw = new RuntimeRcwValue(
			id: id2,
			type: MetaKnownType.Object,
			interfaceTypes: [MetaKnownType.Int32],
			interfacePointers: [])
		{
			ReferenceCount = 2,
		};

		var declaringType = new MetaSimpleResolvedType(DummyModule, new MetaDataToken(0x02000001), null, "MyClass", 0);
		var signature = new MetaMethodSignature(
			CallingConventions.Standard,
			0,
			new MetaVariable(MetaKnownType.Void, null, false, false),
			[
				new MetaVariable(MetaKnownType.Object, "rcwInline", false, false),
				new MetaVariable(MetaKnownType.Object, "rcwRef", false, false),
			]);

		var method = new MetaMethod(new MetaDataToken(0x06000001), DummyModule, declaringType, "MyMethod", signature, []);

		var ilFrame = new RuntimeILFrame(
			method: method,
			ilOffset: 0,
			ilMapping: RuntimeILMapping.Exact,
			source: null,
			@this: null,
			typeArgs: [],
			arguments: [inlineRcw, refRcw],
			locals: [],
			localNames: []);

		var chain = new RuntimeFrameChain(RuntimeFrameChainReason.Unknown, [ilFrame]);
		var thread = new RuntimeThread(1, RuntimeThreadStates.None, [chain], []);

		var process = CreateProcess(referenceValues: [refRcw], threads: [thread]);
		var xml = FormatProcessXml(process);
		var doc = XDocument.Parse(xml);

		var frameElem = doc.Root.Element(E.Process)
			.Element(E.Thread)
			.Element(E.Chain)
			.Element(E.Frame);

		var paramElems = frameElem.Elements(E.Param).ToList();
		Assert.That(paramElems, Has.Count.EqualTo(2));

		// First parameter: ReferenceCount == 1 -> formatted inline <rcwValue>, no objectId attribute
		var rcwElem = paramElems[0].Element(E.RCWValue);
		Assert.That(rcwElem, Is.Not.Null);
		Assert.That(rcwElem.Attribute(A.ObjectID), Is.Null);
		Assert.That(rcwElem.Attribute(A.Type)?.Value, Is.EqualTo("System.Object"));
		Assert.That(rcwElem.Element(E.Managed)?.Attribute(A.Type)?.Value, Is.EqualTo("System.Int32"));
		Assert.That(rcwElem.Element(E.Native)?.Attribute(A.Ptr)?.Value, Is.EqualTo("0000000000001000"));

		// Second parameter: ReferenceCount == 2 -> formatted as <valueRef objectId="..." />
		var refElem = paramElems[1].Element(E.ValueRef);
		Assert.That(refElem, Is.Not.Null);
		Assert.That(refElem.Attribute(A.ObjectID)?.Value, Is.EqualTo(id2.ToString()));
	}

	[Test]
	public void Format_LocalValues_PointerValue_NestedAndVariations()
	{
		var idSource = Identity.NewSource();
		var id1 = idSource.New();

		var sharedValue = new RuntimeSimpleValue(id1, MetaKnownType.Int32, 999) { ReferenceCount = 2 };

		// 1. Pointer with null inner value
		var ptrNullInner = new RuntimePointerValue(MetaKnownType.IntPtr, new MemoryAddress(0x1000), null);

		// 2. Pointer pointing to RuntimeNullValue
		var ptrNullVal = new RuntimePointerValue(MetaKnownType.IntPtr, new MemoryAddress(0x2000), RuntimeNullValue.Instance);

		// 3. Pointer pointing to inline SimpleValue (RefCount = 1)
		var inlineVal = new RuntimeSimpleValue(idSource.New(), MetaKnownType.Int32, 42) { ReferenceCount = 1 };
		var ptrInline = new RuntimePointerValue(MetaKnownType.Int32, new MemoryAddress(0x3000), inlineVal);

		// 4. Pointer pointing to shared SimpleValue (RefCount = 2)
		var ptrRef = new RuntimePointerValue(MetaKnownType.Int32, new MemoryAddress(0x4000), sharedValue);

		// 5. Pointer to pointer (nested)
		var ptrToPtr = new RuntimePointerValue(MetaKnownType.IntPtr, new MemoryAddress(0x5000), ptrInline);

		var declaringType = new MetaSimpleResolvedType(DummyModule, new MetaDataToken(0x02000001), null, "MyClass", 0);
		var signature = new MetaMethodSignature(
			CallingConventions.Standard,
			0,
			new MetaVariable(MetaKnownType.Void, null, false, false),
			[
				new MetaVariable(MetaKnownType.IntPtr, "p1", false, false),
				new MetaVariable(MetaKnownType.IntPtr, "p2", false, false),
				new MetaVariable(MetaKnownType.IntPtr, "p3", false, false),
				new MetaVariable(MetaKnownType.IntPtr, "p4", false, false),
				new MetaVariable(MetaKnownType.IntPtr, "p5", false, false),
			]);

		var method = new MetaMethod(new MetaDataToken(0x06000001), DummyModule, declaringType, "MyMethod", signature, []);

		var ilFrame = new RuntimeILFrame(
			method: method,
			ilOffset: 0,
			ilMapping: RuntimeILMapping.Exact,
			source: null,
			@this: null,
			typeArgs: [],
			arguments: [ptrNullInner, ptrNullVal, ptrInline, ptrRef, ptrToPtr],
			locals: [],
			localNames: []);

		var chain = new RuntimeFrameChain(RuntimeFrameChainReason.Unknown, [ilFrame]);
		var thread = new RuntimeThread(1, RuntimeThreadStates.None, [chain], []);

		var process = CreateProcess(referenceValues: [sharedValue], threads: [thread]);
		var xml = FormatProcessXml(process);
		var doc = XDocument.Parse(xml);

		var frameElem = doc.Root.Element(E.Process)
			.Element(E.Thread)
			.Element(E.Chain)
			.Element(E.Frame);

		var paramElems = frameElem.Elements(E.Param).ToList();
		Assert.That(paramElems, Has.Count.EqualTo(5));

		// 1. Pointer with null inner value
		var p1 = paramElems[0].Element(E.PointerValue);
		Assert.That(p1, Is.Not.Null);
		Assert.That(p1.Attribute(A.Type)?.Value, Is.EqualTo("System.IntPtr"));
		Assert.That(p1.Attribute(A.Address)?.Value, Is.EqualTo("0000000000001000"));
		Assert.That(p1.HasElements, Is.False);

		// 2. Pointer pointing to RuntimeNullValue
		var p2 = paramElems[1].Element(E.PointerValue);
		Assert.That(p2, Is.Not.Null);
		Assert.That(p2.Attribute(A.Address)?.Value, Is.EqualTo("0000000000002000"));
		Assert.That(p2.Element(E.Null), Is.Not.Null);

		// 3. Pointer pointing to inline SimpleValue
		var p3 = paramElems[2].Element(E.PointerValue);
		Assert.That(p3, Is.Not.Null);
		Assert.That(p3.Attribute(A.Address)?.Value, Is.EqualTo("0000000000003000"));
		var p3Val = p3.Element(E.Value);
		Assert.That(p3Val, Is.Not.Null);
		Assert.That(p3Val.Value, Is.EqualTo("42"));

		// 4. Pointer pointing to shared SimpleValue (valueRef)
		var p4 = paramElems[3].Element(E.PointerValue);
		Assert.That(p4, Is.Not.Null);
		Assert.That(p4.Attribute(A.Address)?.Value, Is.EqualTo("0000000000004000"));
		var p4Ref = p4.Element(E.ValueRef);
		Assert.That(p4Ref, Is.Not.Null);
		Assert.That(p4Ref.Attribute(A.ObjectID)?.Value, Is.EqualTo(id1.ToString()));

		// 5. Pointer to pointer
		var p5 = paramElems[4].Element(E.PointerValue);
		Assert.That(p5, Is.Not.Null);
		Assert.That(p5.Attribute(A.Address)?.Value, Is.EqualTo("0000000000005000"));
		var p5Nested = p5.Element(E.PointerValue);
		Assert.That(p5Nested, Is.Not.Null);
		Assert.That(p5Nested.Attribute(A.Address)?.Value, Is.EqualTo("0000000000003000"));
		Assert.That(p5Nested.Element(E.Value)?.Value, Is.EqualTo("42"));
	}

	[Test]
	public void Format_LocalValues_PendingStateMachineTask()
	{
		var idSource = Identity.NewSource();
		var id1 = idSource.New();
		var id2 = idSource.New();

		var taskVal = new RuntimeSimpleValue(id1, MetaKnownType.Object, null) { ReferenceCount = 2 };
		var paramVal = new RuntimeSimpleValue(idSource.New(), MetaKnownType.String, "ParamValue") { ReferenceCount = 1 };
		var ptrVal = new RuntimePointerValue(MetaKnownType.Int32, new MemoryAddress(0x1000), new RuntimeSimpleValue(id2, MetaKnownType.Int32, 100) { ReferenceCount = 2 });

		var declaringType = new MetaSimpleResolvedType(DummyModule, new MetaDataToken(0x02000001), null, "MyDeclaringType", 0);
		var asyncMethodSignature = new MetaMethodSignature(
			CallingConventions.HasThis,
			0,
			new MetaVariable(MetaKnownType.Void, null, false, false),
			[new MetaVariable(MetaKnownType.String, "strParam", false, false)]);
		var asyncMethod = new MetaMethod(new MetaDataToken(0x06000001), DummyModule, declaringType, "AsyncDoWork", asyncMethodSignature, []);

		var descriptor = new StateMachineDescriptor(
			asyncMethod,
			new MetaDataToken(0x06000002),
			declaringType,
			stateField: null,
			thisField: null,
			taskFieldSequence: [new MetaDataToken(0x04000001)],
			paramFields: [new SMField(new MetaDataToken(0x04000002), "p1")],
			localFields: [new MetaField(new MetaDataToken(0x04000003), MetaKnownType.Int32, "loc1")]);

		var pendingTask = new PendingStateMachineTask(
			descriptor,
			typeArgs: [],
			stateValue: null,
			thisValue: RuntimeNullValue.Instance,
			taskValue: taskVal,
			parameterValues: [paramVal],
			localValues: [ptrVal],
			state: null);

		var process = CreateProcess(referenceValues: [taskVal, ptrVal.Value], pendingTasks: [pendingTask]);
		var xml = FormatProcessXml(process);
		var doc = XDocument.Parse(xml);

		var smTaskElem = doc.Root.Element(E.Process)
			.Element(E.PendingTasks)
			.Element(E.PendingSMTask);

		Assert.That(smTaskElem, Is.Not.Null);

		// this -> null
		var thisElem = smTaskElem.Element(E.This);
		Assert.That(thisElem?.Element(E.Null), Is.Not.Null);

		// task -> valueRef
		var taskElem = smTaskElem.Element(E.Task);
		Assert.That(taskElem?.Element(E.ValueRef)?.Attribute(A.ObjectID)?.Value, Is.EqualTo(id1.ToString()));

		// param -> inline value
		var paramElem = smTaskElem.Element(E.Param);
		Assert.That(paramElem?.Element(E.Value)?.Value, Is.EqualTo("ParamValue"));

		// local -> pointerValue containing valueRef
		var localElem = smTaskElem.Element(E.Local);
		var ptrElem = localElem?.Element(E.PointerValue);
		Assert.That(ptrElem, Is.Not.Null);
		Assert.That(ptrElem.Element(E.ValueRef)?.Attribute(A.ObjectID)?.Value, Is.EqualTo(id2.ToString()));
	}

	static RuntimeProcess CreateProcess(
		System.Collections.Immutable.ImmutableArray<RuntimeValue> referenceValues = default,
		System.Collections.Immutable.ImmutableArray<RuntimeThread> threads = default,
		System.Collections.Immutable.ImmutableArray<PendingStateMachineTask> pendingTasks = default)
	{
		var options = new CaptureOptions(false, 0);
		var native = new RuntimeNative(100, null, new RuntimeUser("user", "domain"), []);
		return new RuntimeProcess(
			options,
			native,
			clrVersion: null,
			appDomains: [],
			threads: threads.IsDefault ? [] : threads,
			documents: [],
			referenceValues: referenceValues.IsDefault ? [] : referenceValues,
			pendingTasks: pendingTasks.IsDefault ? [] : pendingTasks);
	}

	static string FormatProcessXml(RuntimeProcess process)
	{
		var sb = new StringBuilder();
		using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = false, Encoding = Encoding.UTF8 }))
		{
			StateFormatter.Format(writer, process);
		}

		return sb.ToString();
	}

	static readonly MetaAssembly DummyAssembly = new MetaAssembly(
		@"Q:\app\SomeAssembly.dll",
		"SomeAssembly",
		new Version(1, 0),
		null,
		null);

	static readonly MetaModule DummyModule = new MetaModule(
		DummyAssembly,
		Identity.NewSource().New(),
		@"Q:\app\SomeModule.dll",
		"SomeModule",
		false,
		false,
		Guid.NewGuid());

	static readonly XNamespace WaywfNamespace = "waywf-capture";

	static class A
	{
		public static readonly XName Address = "address";
		public static readonly XName ClassName = "className";
		public static readonly XName ClrVersion = "clrVersion";
		public static readonly XName Duration = "duration";
		public static readonly XName HWND = "hwnd";
		public static readonly XName Id = "id";
		public static readonly XName ILMapping = "ilMapping";
		public static readonly XName ILOffset = "ilOffset";
		public static readonly XName ImagePath = "imagePath";
		public static readonly XName IsInMemory = "isInMemory";
		public static readonly XName MethodDisplayText = "methodDisplayText";
		public static readonly XName Name = "name";
		public static readonly XName ObjectID = "objectId";
		public static readonly XName OwnerThread = "ownerThread";
		public static readonly XName Path = "path";
		public static readonly XName Pid = "pid";
		public static readonly XName Ptr = "ptr";
		public static readonly XName State = "state";
		public static readonly XName Suppressed = "suppressed";
		public static readonly XName Type = "type";
		public static readonly XName Version = "version";
		public static readonly XName Visible = "visible";
		public static readonly XName Wait = "wait";
		public static readonly XName WalkHeap = "walkheap";
	}

	static class E
	{
		public static readonly XName AppDomain = WaywfNamespace + "appDomain";
		public static readonly XName Assembly = WaywfNamespace + "assembly";
		public static readonly XName BlockingObject = WaywfNamespace + "blockingObject";
		public static readonly XName Chain = WaywfNamespace + "chain";
		public static readonly XName Document = WaywfNamespace + "document";
		public static readonly XName Frame = WaywfNamespace + "frame";
		public static readonly XName InternalFrame = WaywfNamespace + "internalFrame";
		public static readonly XName Local = WaywfNamespace + "local";
		public static readonly XName Login = WaywfNamespace + "login";
		public static readonly XName Managed = WaywfNamespace + "managed";
		public static readonly XName Module = WaywfNamespace + "module";
		public static readonly XName Native = WaywfNamespace + "native";
		public static readonly XName Null = WaywfNamespace + "null";
		public static readonly XName Os = WaywfNamespace + "os";
		public static readonly XName Param = WaywfNamespace + "param";
		public static readonly XName PendingSMTask = WaywfNamespace + "pendingSMTask";
		public static readonly XName PendingTasks = WaywfNamespace + "pendingTasks";
		public static readonly XName PointerValue = WaywfNamespace + "pointerValue";
		public static readonly XName Process = WaywfNamespace + "process";
		public static readonly XName RCWValue = WaywfNamespace + "rcwValue";
		public static readonly XName Source = WaywfNamespace + "source";
		public static readonly XName Task = WaywfNamespace + "task";
		public static readonly XName This = WaywfNamespace + "this";
		public static readonly XName Thread = WaywfNamespace + "thread";
		public static readonly XName Value = WaywfNamespace + "value";
		public static readonly XName ValueRef = WaywfNamespace + "valueRef";
		public static readonly XName WAYWF = WaywfNamespace + "waywf";
		public static readonly XName Window = WaywfNamespace + "window";
	}
}
