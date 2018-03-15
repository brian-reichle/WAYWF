// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using WAYWF.Agent.CorDebugApi;
using WAYWF.Agent.IL;
using WAYWF.Agent.MetaCache;

namespace WAYWF.Agent.PendingTasks
{
	sealed class StateMachineDescriptorFactory
	{
		public StateMachineDescriptorFactory(MetaDataCache mdCache)
		{
			_mdCache = mdCache;
		}

		public StateMachineDescriptor GetDescriptor(ICorDebugClass cl)
		{
			var smType = _mdCache.GetType(cl);
			var module = cl.GetModule();
			var methodToken = GetMethodToken(module, smType);
			var ilEnum = module.GetMethodIL(methodToken);

			if (ilEnum == null)
			{
				return null;
			}

			var method = _mdCache.GetMethod(module.GetFunctionFromToken(methodToken));
			return CreateDescriptor(module, smType, method, ilEnum);
		}

		StateMachineDescriptor CreateDescriptor(ICorDebugModule module, MetaResolvedType smType, MetaMethod method, IEnumerable<Instruction> il)
		{
			var paramFields = new SMField[method.Signature.Parameters.Count];
			SMField stateField = null;
			SMField thisField = null;

			Instruction prevInstruction = null;

			foreach (var instruction in il)
			{
				if (IsSettingField(instruction, out var fieldToken) &&
					IsSetStateMachineField(module, smType.Token, ref fieldToken, out var name) &&
					prevInstruction != null)
				{
					if (TryGetParameterIndex(prevInstruction, out var value))
					{
						if (!method.Signature.CallingConventions.HasImplicitThis())
						{
							paramFields[value] = new SMField(fieldToken, name);
						}
						else if (value == 0)
						{
							thisField = new SMField(fieldToken, name);
						}
						else
						{
							paramFields[value - 1] = new SMField(fieldToken, name);
						}
					}
					else if (LoadsConstantInt(prevInstruction))
					{
						stateField = new SMField(fieldToken, name);
					}
				}

				prevInstruction = instruction;
			}

			var seen = new HashSet<MetaDataToken>(paramFields.Concat(new[] { thisField, stateField }).Where(x => x != null).Select(x => x.FieldToken));
			var localFields = _mdCache.GetFields(module, smType.Token);
			localFields = localFields.Where(x => !seen.Contains(x.Token)).ToArray();

			var moveNextMethod = module.GetMethodBody(smType.Token, "mscorlib", "System.Runtime.CompilerServices.IAsyncStateMachine", "MoveNext");
			return new StateMachineDescriptor(method, moveNextMethod, smType, stateField, thisField, paramFields, localFields);
		}

		static bool IsSettingField(Instruction instruction, out MetaDataToken fieldToken)
		{
			if (instruction.OpCode == OpCodes.Stfld)
			{
				fieldToken = ((Instruction<MetaDataToken>)instruction).Value;
				return true;
			}

			fieldToken = MetaDataToken.Nil;
			return false;
		}

		bool IsSetStateMachineField(ICorDebugModule module, MetaDataToken classToken, ref MetaDataToken fieldToken, out string name)
		{
			var import = module.GetMetaDataImport();

			if (fieldToken.TokenType == TokenType.Field)
			{
				import.GetFieldProps(fieldToken, out var scopeToken, out name);

				if (scopeToken == classToken)
				{
					return true;
				}
			}
			else if (fieldToken.TokenType == TokenType.MemberRef)
			{
				import.GetMemberRefProps(fieldToken, out var scopeToken, out name);

				switch (scopeToken.TokenType)
				{
					case TokenType.TypeDef:
						if (scopeToken == classToken)
						{
							return true;
						}
						break;

					case TokenType.TypeSpec:
						var declaringType = _mdCache.GetType(module, scopeToken);

						if (TryGetUnderlyingToken(declaringType, out var declaringTypeToken) && declaringTypeToken == classToken)
						{
							fieldToken = import.FindField(classToken, name, IntPtr.Zero);
							return true;
						}
						break;
				}
			}

			name = null;
			return false;
		}

		static bool TryGetUnderlyingToken(MetaTypeBase typeBase, out MetaDataToken token)
		{
			if (typeBase is MetaType type)
			{
				token = type.Token;
				return true;
			}
			else if (typeBase is MetaGenType genType)
			{
				token = genType.BaseType.Token;
				return true;
			}
			else
			{
				token = MetaDataToken.Nil;
				return false;
			}
		}

		static bool TryGetParameterIndex(Instruction instruction, out int parameterIndex)
		{
			if (instruction.OpCode == OpCodes.Ldarg || instruction.OpCode == OpCodes.Ldarg_S)
			{
				parameterIndex = ((Instruction<short>)instruction).Value;
			}
			else if (instruction.OpCode == OpCodes.Ldarg_0)
			{
				parameterIndex = 0;
			}
			else if (instruction.OpCode == OpCodes.Ldarg_1)
			{
				parameterIndex = 1;
			}
			else if (instruction.OpCode == OpCodes.Ldarg_2)
			{
				parameterIndex = 2;
			}
			else if (instruction.OpCode == OpCodes.Ldarg_3)
			{
				parameterIndex = 3;
			}
			else
			{
				parameterIndex = -1;
				return false;
			}

			return true;
		}

		static bool LoadsConstantInt(Instruction instruction)
		{
			return instruction.OpCode == OpCodes.Ldc_I4
				|| instruction.OpCode == OpCodes.Ldc_I4_0
				|| instruction.OpCode == OpCodes.Ldc_I4_1
				|| instruction.OpCode == OpCodes.Ldc_I4_2
				|| instruction.OpCode == OpCodes.Ldc_I4_3
				|| instruction.OpCode == OpCodes.Ldc_I4_4
				|| instruction.OpCode == OpCodes.Ldc_I4_5
				|| instruction.OpCode == OpCodes.Ldc_I4_6
				|| instruction.OpCode == OpCodes.Ldc_I4_7
				|| instruction.OpCode == OpCodes.Ldc_I4_8
				|| instruction.OpCode == OpCodes.Ldc_I4_M1
				|| instruction.OpCode == OpCodes.Ldc_I4_S;
		}

		static MetaDataToken GetMethodToken(ICorDebugModule module, MetaResolvedType smType)
		{
			var enclosingClassToken = smType.DeclaringType.Token;
			var import = module.GetMetaDataImport();
			var canonicalName = CalculateCanonicalName(smType);
			var e = IntPtr.Zero;
			var result = MetaDataToken.Nil;

			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				while (import.EnumMethods(ref e, enclosingClassToken, out var methodToken))
				{
					if (module.IsStateMachineMatching(methodToken, canonicalName))
					{
						result = methodToken;
						break;
					}
				}
			}
			finally
			{
				if (e != IntPtr.Zero)
				{
					import.CloseEnum(e);
				}
			}

			return result;
		}

		static string CalculateCanonicalName(MetaResolvedType type)
		{
			var builder = new StringBuilder();
			BuildCanonicalName(builder, type);
			return builder.ToString();
		}

		static void BuildCanonicalName(StringBuilder builder, MetaResolvedType type)
		{
			if (type.DeclaringType != null)
			{
				BuildCanonicalName(builder, type.DeclaringType);
				builder.Append('+');
			}

			builder.Append(type.Name);
		}

		readonly MetaDataCache _mdCache;
	}
}
