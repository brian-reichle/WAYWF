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
		Assert.That(root.Name.LocalName, Is.EqualTo("waywf"));
		Assert.That(root.Name.NamespaceName, Is.EqualTo("waywf-capture"));

		var os = root.Element(XName.Get("os", "waywf-capture"));
		Assert.That(os, Is.Not.Null);

		var login = root.Element(XName.Get("login", "waywf-capture"));
		Assert.That(login, Is.Not.Null);

		var procElem = root.Element(XName.Get("process", "waywf-capture"));
		Assert.That(procElem, Is.Not.Null);
		Assert.That(procElem.Attribute("pid")?.Value, Is.EqualTo("1234"));
		Assert.That(procElem.Attribute("clrVersion")?.Value, Is.EqualTo("4.0.30319"));
		Assert.That(procElem.Attribute("imagePath")?.Value, Is.EqualTo(@"Q:\app\test.exe"));
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

		Assert.That(root.Attribute("walkheap")?.Value, Is.EqualTo("true"));
		Assert.That(root.Attribute("wait")?.Value, Is.EqualTo("15"));
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
		var ns = "waywf-capture";

		var appDomainElem = doc.Root.Element(XName.Get("process", ns)).Element(XName.Get("appDomain", ns));
		Assert.That(appDomainElem, Is.Not.Null);
		Assert.That(appDomainElem.Attribute("id")?.Value, Is.EqualTo("10"));
		Assert.That(appDomainElem.Attribute("name")?.Value, Is.EqualTo("TestAppDomain"));

		var assemblyElem = appDomainElem.Element(XName.Get("assembly", ns));
		Assert.That(assemblyElem, Is.Not.Null);
		Assert.That(assemblyElem.Attribute("name")?.Value, Is.EqualTo("MyAssembly"));
		Assert.That(assemblyElem.Attribute("version")?.Value, Is.EqualTo("1.2.3.4"));

		var moduleElem = assemblyElem.Element(XName.Get("module", ns));
		Assert.That(moduleElem, Is.Not.Null);
		Assert.That(moduleElem.Attribute("name")?.Value, Is.EqualTo("Module1.dll"));
		Assert.That(moduleElem.Attribute("isInMemory")?.Value, Is.EqualTo("true"));
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
		var ns = "waywf-capture";

		var sourceElem = xDoc.Root.Element(XName.Get("process", ns)).Element(XName.Get("source", ns));
		Assert.That(sourceElem, Is.Not.Null);
		Assert.That(sourceElem.Attribute("path")?.Value, Is.EqualTo(@"Q:\solution\src\"));

		var docElems = sourceElem.Elements(XName.Get("document", ns));
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
		var ns = "waywf-capture";

		var winElem = doc.Root.Element(XName.Get("process", ns)).Element(XName.Get("window", ns));
		Assert.That(winElem, Is.Not.Null);
		Assert.That(winElem.Attribute("hwnd")?.Value, Is.EqualTo("4096"));
		Assert.That(winElem.Attribute("ownerThread")?.Value, Is.EqualTo("42"));
		Assert.That(winElem.Attribute("className")?.Value, Is.EqualTo("TestWindowClass"));
		Assert.That(winElem.Attribute("visible")?.Value, Is.EqualTo("true"));
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
		var ns = "waywf-capture";

		var threadElem = doc.Root.Element(XName.Get("process", ns)).Element(XName.Get("thread", ns));
		Assert.That(threadElem, Is.Not.Null);
		Assert.That(threadElem.Attribute("tid")?.Value, Is.EqualTo("101"));

		var chainElem = threadElem.Element(XName.Get("chain", ns));
		Assert.That(chainElem, Is.Not.Null);

		var frameElem = chainElem.Element(XName.Get("frame", ns));
		Assert.That(frameElem, Is.Not.Null);
		Assert.That(frameElem.Attribute("methodDisplayText")?.Value, Is.EqualTo("MyMethod"));
		Assert.That(frameElem.Attribute("ilOffset")?.Value, Is.EqualTo("12"));
		Assert.That(frameElem.Attribute("ilMapping")?.Value, Is.EqualTo("Epilog"));
		Assert.That(frameElem.Attribute("duration")?.Value, Is.EqualTo("1.2346"));

		var internalFrameElem = chainElem.Element(XName.Get("internalFrame", ns));
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
		var ns = "waywf-capture";

		var pendingTasksElem = doc.Root.Element(XName.Get("process", ns)).Element(XName.Get("pendingTasks", ns));
		Assert.That(pendingTasksElem, Is.Not.Null);

		var smTaskElem = pendingTasksElem.Element(XName.Get("pendingSMTask", ns));
		Assert.That(smTaskElem, Is.Not.Null);
		Assert.That(smTaskElem.Attribute("methodDisplayText")?.Value, Is.EqualTo("AsyncDoWork"));
		Assert.That(smTaskElem.Attribute("state")?.Value, Is.EqualTo("2"));
		Assert.That(smTaskElem.Attribute("ilOffset")?.Value, Is.EqualTo("42"));
	}

	[Test]
	public void Format_Values_Simple_Rcw_Pointer_Null()
	{
		var idSource = Identity.NewSource();
		var id1 = idSource.New();
		var simpleValue = new RuntimeSimpleValue(id1, MetaKnownType.String, "Line1\nLine2")
		{
			ReferenceCount = 2,
		};

		var id2 = idSource.New();
		var rcwValue = new RuntimeRcwValue(
			id: id2,
			type: MetaKnownType.Object,
			interfaceTypes: [MetaKnownType.Int32],
			interfacePointers: [new RuntimeNativeInterface(new RuntimeVirtualAddress(new MemoryAddress(0x1000)), new RuntimeVirtualAddress(new MemoryAddress(0x2000)))])
		{
			ReferenceCount = 2,
		};

		var options = new CaptureOptions(false, 0);
		var native = new RuntimeNative(100, null, new RuntimeUser("user", "domain"), []);
		var process = new RuntimeProcess(
			options,
			native,
			clrVersion: null,
			appDomains: [],
			threads: [],
			documents: [],
			referenceValues: [simpleValue, rcwValue],
			pendingTasks: []);

		var xml = FormatProcessXml(process);
		var doc = XDocument.Parse(xml);
		var ns = "waywf-capture";

		var procElem = doc.Root.Element(XName.Get("process", ns));
		var valElem = procElem.Element(XName.Get("value", ns));
		Assert.That(valElem, Is.Not.Null);
		Assert.That(valElem.Attribute("objectId")?.Value, Is.EqualTo(id1.ToString()));
		Assert.That(valElem.Attribute("type")?.Value, Is.EqualTo("System.String"));

		var rcwElem = procElem.Element(XName.Get("rcwValue", ns));
		Assert.That(rcwElem, Is.Not.Null);
		Assert.That(rcwElem.Element(XName.Get("managed", ns))?.Attribute("type")?.Value, Is.EqualTo("System.Int32"));
		Assert.That(rcwElem.Element(XName.Get("native", ns))?.Attribute("ptr")?.Value, Is.EqualTo("0000000000001000"));
	}

	[Test]
	public void Format_Values_SuppressedUnsafeXmlChar()
	{
		var id = Identity.NewSource().New();
		var unsafeValue = new RuntimeSimpleValue(id, MetaKnownType.String, "BadChar\0Test")
		{
			ReferenceCount = 2,
		};

		var options = new CaptureOptions(false, 0);
		var native = new RuntimeNative(100, null, new RuntimeUser("user", "domain"), []);
		var process = new RuntimeProcess(
			options,
			native,
			clrVersion: null,
			appDomains: [],
			threads: [],
			documents: [],
			referenceValues: [unsafeValue],
			pendingTasks: []);

		var xml = FormatProcessXml(process);
		var doc = XDocument.Parse(xml);
		var ns = "waywf-capture";

		var valElem = doc.Root.Element(XName.Get("process", ns)).Element(XName.Get("value", ns));
		Assert.That(valElem, Is.Not.Null);
		Assert.That(valElem.Attribute("suppressed")?.Value, Is.EqualTo("true"));
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

	static MetaAssembly DummyAssembly { get; } = new MetaAssembly(@"Q:\app\SomeAssembly.dll", "SomeAssembly", new Version(1, 0), null, null);
	static MetaModule DummyModule { get; } = new MetaModule(DummyAssembly, Identity.NewSource().New(), @"Q:\app\SomeModule.dll", "SomeModule", false, false, Guid.NewGuid());
}
