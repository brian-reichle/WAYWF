// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using WAYWF.Agent.CorDebugApi;
using WAYWF.Agent.Data;
using WAYWF.Agent.MetaCache;
using WAYWF.Agent.PendingTasks;
using WAYWF.Agent.Source;
using WAYWF.Options;

namespace WAYWF.Agent
{
	sealed class StateFormatter : IRuntimeFrameVisitor
	{
		const string TrueString = "true";
		const string FalseString = "false";

		StateFormatter(XmlWriter writer)
		{
			_writer = writer;
			_formatter = new MetaFormatter();
			_localValueFormatter = new LocalValueWriter(this);
			_globalValueFormatter = new GlobalValueWriter(this);
		}

		public static void Format(XmlWriter writer, RuntimeProcess process, CmdLineOptions options)
		{
			var formatter = new StateFormatter(writer);
			formatter._writer.WriteProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"waywf.xslt\"");
			formatter.WriteCapture(process, options);
		}

		void WriteCapture(RuntimeProcess process, CmdLineOptions options)
		{
			_writer.WriteStartElement("waywf", "waywf-capture");
			_writer.WriteAttributeString("version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
			_writer.WriteAttributeString("timestamp", process.DateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
			_writer.WriteAttributeString("timezone", process.DateTime.ToString("zzz", CultureInfo.InvariantCulture));

			if (options.WaitSeconds > 0)
			{
				_writer.WriteAttributeString("wait", options.WaitSeconds.ToString(CultureInfo.InvariantCulture));
			}

			if (options.WalkHeap)
			{
				_writer.WriteAttributeString("walkheap", TrueString);
			}

			WriteEnvironment();
			WriteProcess(process);
			_writer.WriteEndElement();
		}

		void WriteEnvironment()
		{
			_writer.WriteStartElement("os");
			_writer.WriteAttributeString("platform", Environment.OSVersion.Platform.ToString());
			_writer.WriteAttributeString("version", Environment.OSVersion.Version.ToString());
			_writer.WriteAttributeString("servicePack", Environment.OSVersion.ServicePack);
			_writer.WriteAttributeString("is64bit", BoolString(Environment.Is64BitOperatingSystem));
			_writer.WriteEndElement();

			_writer.WriteStartElement("login");
			_writer.WriteAttributeString("user", Environment.UserName);
			_writer.WriteAttributeString("domain", Environment.UserDomainName);
			_writer.WriteAttributeString("machine", Environment.MachineName);
			_writer.WriteEndElement();
		}

		void WriteProcess(RuntimeProcess process)
		{
			_writer.WriteStartElement("process");
			_writer.WriteAttributeString("pid", process.Native.ProcessID.ToString(CultureInfo.InvariantCulture));
			_writer.WriteAttributeString("is64bit", BoolString(Environment.Is64BitProcess)); // bitness of debuggee must match bitness of debugger.

			if (process.ClrVersion != null)
			{
				_writer.WriteAttributeString("clrVersion", process.ClrVersion.ToString());
			}

			_writer.WriteAttributeString("imagePath", process.Native.ImageName);

			WriteUser(process.Native.User);

			foreach (var appDomain in process.AppDomains)
			{
				WriteAppDomain(appDomain);
			}

			WriteDocumentList(process.Documents);

			foreach (var value in process.ReferenceValues)
			{
				value.Apply(_globalValueFormatter);
			}

			foreach (var thread in process.Threads)
			{
				WriteThread(thread);
			}

			WritePendingTasks(process, process.PendingTasks);

			foreach (var window in process.Native.Windows)
			{
				WriteWindow(window);
			}

			_writer.WriteEndElement();
		}

		void WriteUser(RuntimeUser user)
		{
			_writer.WriteStartElement("user");
			_writer.WriteAttributeString("user", user.User);
			_writer.WriteAttributeString("domain", user.Domain);
			_writer.WriteEndElement();
		}

		void WriteAppDomain(RuntimeAppDomain domain)
		{
			_writer.WriteStartElement("appDomain");
			_writer.WriteAttributeString("id", domain.AppDomainID.ToString(CultureInfo.InvariantCulture));
			_writer.WriteAttributeString("name", domain.Name);

			foreach (var assembly in domain.Assembly.OrderBy(x => x.Name))
			{
				WriteAssembly(assembly);
			}

			_writer.WriteEndElement();
		}

		void WriteAssembly(MetaAssembly assembly)
		{
			_writer.WriteStartElement("assembly");
			_writer.WriteAttributeString("name", assembly.Name);

			if (assembly.Version != null)
			{
				_writer.WriteAttributeString("version", assembly.Version.ToString());
			}

			if (!string.IsNullOrEmpty(assembly.Locale))
			{
				_writer.WriteAttributeString("locale", assembly.Locale);
			}

			if (assembly.PublicKeyToken.HasValue)
			{
				_writer.WriteAttributeString("publicKeyToken", assembly.PublicKeyToken.Value.ToString("X16", CultureInfo.InvariantCulture));
			}

			foreach (var module in assembly.Modules)
			{
				WriteModule(module);
			}

			_writer.WriteEndElement();
		}

		void WriteModule(MetaModule module)
		{
			_writer.WriteStartElement("module");
			_writer.WriteAttributeString("moduleId", module.ModuleID.ToString());
			_writer.WriteAttributeString("name", module.Name);
			_writer.WriteAttributeString("path", module.Path);
			_writer.WriteAttributeString("mvid", module.MVID.ToString());

			if (module.IsInMemory)
			{
				_writer.WriteAttributeString("isInMemory", TrueString);
			}

			if (module.IsDynamic)
			{
				_writer.WriteAttributeString("isDynamic", TrueString);
			}

			_writer.WriteEndElement();
		}

		void WriteThread(RuntimeThread thread)
		{
			_writer.WriteStartElement("thread");
			_writer.WriteAttributeString("tid", thread.ThreadID.ToString(CultureInfo.InvariantCulture));
			_writer.WriteAttributeString("state", FormatFlags(thread.UserState));

			foreach (var bobj in thread.BlockingObject)
			{
				WriteBlockingObject(bobj);
			}

			foreach (var chain in thread.Chains)
			{
				WriteChain(chain);
			}

			_writer.WriteEndElement();
		}

		void WriteChain(RuntimeFrameChain chain)
		{
			_writer.WriteStartElement("chain");
			_writer.WriteAttributeString("reason", FormatFlags(chain.Reason));

			foreach (var frame in chain.Frames)
			{
				frame.Apply(this);
			}

			_writer.WriteEndElement();
		}

		void WriteBlockingObject(RuntimeBlockingObject obj)
		{
			_writer.WriteStartElement("blockingObject");
			_writer.WriteAttributeString("reason", obj.BlockingReason.ToString());

			if (obj.OwnerId > 0)
			{
				_writer.WriteAttributeString("ownerThread", obj.OwnerId.ToString(CultureInfo.InvariantCulture));
			}

			if (obj.Timeout > 0)
			{
				_writer.WriteAttributeString("timeout", obj.Timeout.ToString(CultureInfo.InvariantCulture));
			}

			obj.Value.Apply(_localValueFormatter);
			_writer.WriteEndElement();
		}

		void WriteVariable(string elementName, IMetaGenericContext context, MetaVariable variable, RuntimeValue value, string nameOverride)
		{
			WriteVariable(elementName, context, variable.Type, variable.IsByRef, variable.Pinned, value, nameOverride ?? variable.Name);
		}

		void WriteVariable(string elementName, IMetaGenericContext context, MetaTypeBase type, RuntimeValue value, string name)
		{
			WriteVariable(elementName, context, type, false, false, value, name);
		}

		void WriteVariable(string elementName, IMetaGenericContext context, MetaTypeBase type, bool isByRef, bool pinned, RuntimeValue value, string name)
		{
			_writer.WriteStartElement(elementName);
			_writer.WriteAttributeString("type", Format(type, context));

			if (isByRef)
			{
				_writer.WriteAttributeString("byRef", TrueString);
			}

			if (pinned)
			{
				_writer.WriteAttributeString("pinned", TrueString);
			}

			if (!string.IsNullOrEmpty(name))
			{
				_writer.WriteAttributeString("name", name);
			}

			if (value != null)
			{
				value.Apply(_localValueFormatter);
			}

			_writer.WriteEndElement();
		}

		void WriteDocumentList(IList<SourceDocument> documents)
		{
			if (documents.Count == 0)
			{
				return;
			}

			var path = documents[0].Path;
			var matchLength = path.Length;

			for (var i = 1; i < documents.Count; i++)
			{
				var nextPath = documents[i].Path;

				if (matchLength > nextPath.Length)
				{
					matchLength = nextPath.Length;
				}

				matchLength = GetMatchLength(path, nextPath, matchLength);
			}

			while (matchLength > 0)
			{
				var c = path[matchLength - 1];

				if (c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)
				{
					break;
				}

				matchLength--;
			}

			path = path.Substring(0, matchLength);

			_writer.WriteStartElement("source");
			_writer.WriteAttributeString("path", path);

			foreach (var document in documents.OrderBy(x => x.Path))
			{
				WriteDocument(document, matchLength);
			}

			_writer.WriteEndElement();
		}

		void WriteDocument(SourceDocument document, int commonPrefixLength)
		{
			_writer.WriteStartElement("document");
			_writer.WriteAttributeString("documentId", document.ID.ToString());
			_writer.WriteAttributeString("language", document.Language.ToString());

			if (document.DocumentType != SourceDocumentType.Text)
			{
				_writer.WriteAttributeString("documentType", document.DocumentType.ToString());
			}

			_writer.WriteString(document.Path.Substring(commonPrefixLength));
			_writer.WriteEndElement();
		}

		void WriteSourceRef(SourceRef source)
		{
			_writer.WriteStartElement("source");
			_writer.WriteAttributeString("documentId", source.Document.ID.ToString());
			_writer.WriteAttributeString("line", source.FromLine.ToString(CultureInfo.InvariantCulture));
			_writer.WriteEndElement();
		}

		void WritePendingTasks(RuntimeProcess process, IEnumerable<PendingStateMachineTask> pendingTasks)
		{
			using (var enumerator = pendingTasks.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					_writer.WriteStartElement("pendingTasks");

					do
					{
						WritePendingTask(process, enumerator.Current);
					}
					while (enumerator.MoveNext());

					_writer.WriteEndElement();
				}
			}
		}

		void WritePendingTask(RuntimeProcess process, PendingStateMachineTask pendingTask)
		{
			var function = pendingTask.Descriptor.AsyncMethod;
			_writer.WriteStartElement("pendingSMTask");
			_writer.WriteAttributeString("moduleId", GetModuleIdString(process, function.Module));
			_writer.WriteAttributeString("token", function.Token.ToString());

			if (function.DeclaringType != null)
			{
				_writer.WriteAttributeString("typeDisplayText", Format(function.DeclaringType, pendingTask));
			}

			_writer.WriteAttributeString("methodDisplayText", Format(function, pendingTask));

			if (pendingTask.StateValue != null && pendingTask.StateValue.Value != null)
			{
				_writer.WriteAttributeString("state", pendingTask.StateValue.Value.ToString());
			}

			var state = pendingTask.State;

			if (state != null)
			{
				_writer.WriteAttributeString("ilOffset", state.YieldOffset.ToString(CultureInfo.InvariantCulture));

				if (state.YieldSource != null)
				{
					WriteSourceRef(state.YieldSource);
				}
			}

			var signature = function.Signature;
			WriteVariable("result", pendingTask, signature.ResultParam, null, null);

			if (signature.CallingConventions.HasImplicitThis())
			{
				_writer.WriteStartElement("this");

				if (pendingTask.ThisValue != null)
				{
					pendingTask.ThisValue.Apply(_localValueFormatter);
				}

				_writer.WriteEndElement();
			}

			if (pendingTask.Descriptor.TaskFieldSequence != null)
			{
				_writer.WriteStartElement("task");

				if (pendingTask.TaskValue != null)
				{
					pendingTask.TaskValue.Apply(_localValueFormatter);
				}

				_writer.WriteEndElement();
			}

			var parameters = signature.Parameters;

			for (var i = 0; i < parameters.Count; i++)
			{
				WriteVariable("param", pendingTask, parameters[i], pendingTask.ParameterValues[i], null);
			}

			var locals = pendingTask.Descriptor.LocalFields;
			var context = new SMContext(pendingTask);

			for (var i = 0; i < locals.Count; i++)
			{
				WriteVariable("local", context, locals[i].Type, pendingTask.LocalValues[i], locals[i].Name);
			}

			_writer.WriteEndElement();
		}

		static string GetModuleIdString(RuntimeProcess process, MetaModule referenceModule)
		{
			var matchingModules =
				from domain in process.AppDomains
				from assembly in domain.Assembly
				from module in assembly.Modules
				where module.MVID == referenceModule.MVID
					&& module.Path == referenceModule.Path
				select module.ModuleID.ID.ToString(CultureInfo.InvariantCulture);

			return string.Join(" ", matchingModules);
		}

		void WriteILAttributes(CorDebugMappingResult ilMapping, int ilOffset)
		{
			string mappingText;

			switch (ilMapping)
			{
				case CorDebugMappingResult.MAPPING_EPILOG:
					mappingText = "Epilog";
					break;

				case CorDebugMappingResult.MAPPING_PROLOG:
					mappingText = "Prolog";
					break;

				case CorDebugMappingResult.MAPPING_APPROXIMATE:
					mappingText = "Approx";
					break;

				case CorDebugMappingResult.MAPPING_EXACT:
					mappingText = null;
					break;

				default:
					return;
			}

			_writer.WriteAttributeString("ilOffset", ilOffset.ToString(CultureInfo.InvariantCulture));

			if (mappingText != null)
			{
				_writer.WriteAttributeString("ilMapping", mappingText);
			}
		}

		void WriteWindow(RuntimeWindow window)
		{
			_writer.WriteStartElement("window");
			_writer.WriteAttributeString("hwnd", window.Handle.ToString());
			_writer.WriteAttributeString("ownerThread", window.ThreadID.ToString(CultureInfo.InvariantCulture));

			if (window.Owner != IntPtr.Zero)
			{
				_writer.WriteAttributeString("ownerHwnd", window.Owner.ToString());
			}

			if (!string.IsNullOrEmpty(window.ClassName))
			{
				_writer.WriteAttributeString("className", window.ClassName);
			}

			if (window.IsVisible)
			{
				_writer.WriteAttributeString("visible", TrueString);
			}

			if (!window.IsEnabled)
			{
				_writer.WriteAttributeString("enabled", FalseString);
			}

			if (!string.IsNullOrEmpty(window.Caption))
			{
				_writer.WriteString(window.Caption);
			}

			_writer.WriteEndElement();
		}

		#region IRuntimeFrameVisitor Members

		void IRuntimeFrameVisitor.Visit(RuntimeILFrame frame)
		{
			var function = frame.Method;

			_writer.WriteStartElement("frame");
			_writer.WriteAttributeString("moduleId", function.Module.ModuleID.ToString());
			_writer.WriteAttributeString("token", function.Token.ToString());

			if (function.DeclaringType != null)
			{
				_writer.WriteAttributeString("typeDisplayText", Format(function.DeclaringType, frame));
			}

			_writer.WriteAttributeString("methodDisplayText", Format(function, frame));

			if (frame.Duration.HasValue)
			{
				_writer.WriteAttributeString("duration", Math.Round(frame.Duration.Value, 4).ToString(CultureInfo.InvariantCulture));
			}

			WriteILAttributes(frame.ILMapping, frame.ILOffset);

			if (frame.Source != null)
			{
				WriteSourceRef(frame.Source);
			}

			var signature = function.Signature;
			WriteVariable("result", frame, signature.ResultParam, null, null);

			if (signature.CallingConventions.HasImplicitThis())
			{
				_writer.WriteStartElement("this");
				frame.This?.Apply(_localValueFormatter);

				_writer.WriteEndElement();
			}

			var parameters = signature.Parameters;
			var arguments = frame.Arguments;

			for (var i = 0; i < parameters.Count; i++)
			{
				WriteVariable("param", frame, parameters[i], arguments[i], null);
			}

			var locals = frame.Locals;

			if (locals.Count > 0)
			{
				var localDefs = frame.Method.Locals;
				var names = frame.LocalNames;

				for (var i = 0; i < frame.Locals.Count; i++)
				{
					WriteVariable("local", frame, localDefs[i], locals[i], names.Count > i ? names[i] : null);
				}
			}

			_writer.WriteEndElement();
		}

		void IRuntimeFrameVisitor.Visit(RuntimeInternalFrame frame)
		{
			_writer.WriteElementString("internalFrame", frame.InternalFrameType.ToString());
		}

		#endregion

		string Format(MetaMethod method, IMetaGenericContext context)
		{
			_formatter.Clear();
			SetupFormatter(context);
			_formatter.Write(method);
			return _formatter.ToString();
		}

		string Format(MetaTypeBase type, IMetaGenericContext context)
		{
			_formatter.Clear();
			SetupFormatter(context);
			_formatter.Write(type);
			return _formatter.ToString();
		}

		string Format(MetaResolvedType type, IMetaGenericContext context)
		{
			_formatter.Clear();
			SetupFormatter(null);
			_formatter.Write(type, context.TypeArgs);
			return _formatter.ToString();
		}

		static int GetMatchLength(string str1, string str2, int count)
		{
			for (var i = 0; i < count; i++)
			{
				if (str1[i] != str2[i])
				{
					return i;
				}
			}

			return count;
		}

		void SetupFormatter(IMetaGenericContext context)
		{
			if (context != null)
			{
				_formatter.TypeArgs = context.TypeArgs;
				_formatter.MethodArgsStart = context.StartOfMethodArgs;
			}
			else
			{
				_formatter.TypeArgs = null;
				_formatter.MethodArgsStart = 0;
			}
		}

		static string FormatFlags<T>(T value)
			where T : IConvertible
		{
			var tmp = value.ToInt32(null);
			var flagsType = typeof(T);
			var builder = new StringBuilder();
			var remaining = 0;

			while (tmp != 0)
			{
				var next = tmp & ~(tmp - 1);
				tmp -= next;

				if (Enum.IsDefined(flagsType, next))
				{
					if (builder.Length > 0)
					{
						builder.Append(' ');
					}

					builder.Append(Enum.GetName(flagsType, next));
				}
				else
				{
					remaining |= next;
				}
			}

			if (remaining > 0)
			{
				if (builder.Length > 0)
				{
					builder.Append(' ');
				}

				builder.Append(remaining);
			}

			return builder.ToString();
		}

		static string BoolString(bool value)
		{
			return value ? TrueString : FalseString;
		}

		static bool RequiresCDATA(string value)
		{
			for (var i = 0; i < value.Length; i++)
			{
				switch (value[i])
				{
					case '\t':
					case '\n':
					case '\r':
						return true;
				}
			}

			return false;
		}

		static bool IsStringXmlSafe(string value)
		{
			for (var i = 0; i < value.Length; i++)
			{
				var c = value[i];

				switch (c)
				{
					case '\0':
					case '\uFFFE':
					case '\uFFFF':
						return false;
				}
			}

			return true;
		}

		readonly XmlWriter _writer;
		readonly MetaFormatter _formatter;
		readonly IRuntimeValueVisitor _localValueFormatter;
		readonly IRuntimeValueVisitor _globalValueFormatter;

		sealed class SMContext : IMetaGenericContext
		{
			/*
			 * The state machine for a generic async method combines both sets of type arguments into a single set,
			 * this means we need to re-map method type arguments into type type arguments when formatting types
			 * that were taken directly from the state machine.
			 */
			public SMContext(PendingStateMachineTask task)
			{
				_task = task;
			}

			// Pretend that all type arguments are type type arguments.
			public int StartOfMethodArgs => _task.TypeArgs.Count;
			public ReadOnlyCollection<MetaTypeBase> TypeArgs => _task.TypeArgs;

			readonly PendingStateMachineTask _task;
		}

		sealed class LocalValueWriter : ValueWriter
		{
			public LocalValueWriter(StateFormatter formatter)
				: base(formatter)
			{
			}

			public override void Visit(RuntimeNullValue value)
			{
				_writer.WriteStartElement("null");
				_writer.WriteEndElement();
			}

			public override void Visit(RuntimeSimpleValue value)
			{
				if (MakeGlobal(value))
				{
					WriteRef(value.ID);
				}
				else
				{
					base.Visit(value);
				}
			}

			public override void Visit(RuntimeRcwValue value)
			{
				if (MakeGlobal(value))
				{
					WriteRef(value.ID);
				}
				else
				{
					base.Visit(value);
				}
			}

			public override void Visit(RuntimePointerValue value)
			{
				_writer.WriteStartElement("pointerValue");
				_writer.WriteAttributeString("type", _formatter.Format(value.Type, null));
				_writer.WriteAttributeString("address", value.Address.ToString());
				value.Value?.Apply(this);
				_writer.WriteEndElement();
			}

			void WriteRef(Identity id)
			{
				_writer.WriteStartElement("valueRef");
				_writer.WriteAttributeString("objectId", id.ToString());
				_writer.WriteEndElement();
			}
		}

		sealed class GlobalValueWriter : ValueWriter
		{
			public GlobalValueWriter(StateFormatter formatter)
				: base(formatter)
			{
			}

			public override void Visit(RuntimeSimpleValue value)
			{
				if (MakeGlobal(value))
				{
					base.Visit(value);
				}
			}

			public override void Visit(RuntimeRcwValue value)
			{
				if (MakeGlobal(value))
				{
					base.Visit(value);
				}
			}
		}

		abstract class ValueWriter : IRuntimeValueVisitor
		{
			public ValueWriter(StateFormatter formatter)
			{
				_formatter = formatter;
				_writer = formatter._writer;
			}

			public virtual void Visit(RuntimeNullValue value)
			{
			}

			public virtual void Visit(RuntimeSimpleValue value)
			{
				_writer.WriteStartElement("value");

				WriteStandardAttributes(value);

				if (value.Value != null)
				{
					var text = value.Value.ToString();

					if (!IsStringXmlSafe(text))
					{
						_writer.WriteAttributeString("suppressed", TrueString);
					}
					else if (RequiresCDATA(text))
					{
						_writer.WriteCData(text);
					}
					else
					{
						_writer.WriteString(text);
					}
				}

				_writer.WriteEndElement();
			}

			public virtual void Visit(RuntimeRcwValue value)
			{
				_writer.WriteStartElement("rcwValue");

				WriteStandardAttributes(value);

				foreach (var type in value.InterfaceTypes)
				{
					_writer.WriteStartElement("managed");
					_writer.WriteAttributeString("type", _formatter.Format(type, null));
					_writer.WriteEndElement();
				}

				foreach (var ptr in value.InterfacePointers)
				{
					_writer.WriteStartElement("native");
					_writer.WriteAttributeString("ptr", ptr.InterfaceAddress.ToString());
					_writer.WriteAttributeString("vtbl", ptr.VtblAddress.ToString());
					_writer.WriteEndElement();
				}

				_writer.WriteEndElement();
			}

			public virtual void Visit(RuntimePointerValue value)
			{
			}

			protected static bool MakeGlobal(RuntimeIdentifiedValue value)
			{
				return value.ID != null && value.ReferenceCount > 1;
			}

			void WriteStandardAttributes(RuntimeIdentifiedValue value)
			{
				if (MakeGlobal(value))
				{
					_writer.WriteAttributeString("objectId", value.ID.ToString());
				}

				_writer.WriteAttributeString("type", _formatter.Format(value.Type, null));
			}

			protected readonly StateFormatter _formatter;
			protected readonly XmlWriter _writer;
		}
	}
}
