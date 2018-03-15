// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Reflection.Emit;

namespace WAYWF.Agent.IL
{
	partial class ILReader
	{
		Instruction Read()
		{
			switch (_buffer[_offset])
			{
				case 0x00: return ReadInlineNoneInstruction(OpCodes.Nop);
				case 0x01: return ReadInlineNoneInstruction(OpCodes.Break);
				case 0x02: return ReadInlineNoneInstruction(OpCodes.Ldarg_0);
				case 0x03: return ReadInlineNoneInstruction(OpCodes.Ldarg_1);
				case 0x04: return ReadInlineNoneInstruction(OpCodes.Ldarg_2);
				case 0x05: return ReadInlineNoneInstruction(OpCodes.Ldarg_3);
				case 0x06: return ReadInlineNoneInstruction(OpCodes.Ldloc_0);
				case 0x07: return ReadInlineNoneInstruction(OpCodes.Ldloc_1);
				case 0x08: return ReadInlineNoneInstruction(OpCodes.Ldloc_2);
				case 0x09: return ReadInlineNoneInstruction(OpCodes.Ldloc_3);
				case 0x0A: return ReadInlineNoneInstruction(OpCodes.Stloc_0);
				case 0x0B: return ReadInlineNoneInstruction(OpCodes.Stloc_1);
				case 0x0C: return ReadInlineNoneInstruction(OpCodes.Stloc_2);
				case 0x0D: return ReadInlineNoneInstruction(OpCodes.Stloc_3);
				case 0x0E: return ReadShortInlineVarInstruction(OpCodes.Ldarg_S);
				case 0x0F: return ReadShortInlineVarInstruction(OpCodes.Ldarga_S);
				case 0x10: return ReadShortInlineVarInstruction(OpCodes.Starg_S);
				case 0x11: return ReadShortInlineVarInstruction(OpCodes.Ldloc_S);
				case 0x12: return ReadShortInlineVarInstruction(OpCodes.Ldloca_S);
				case 0x13: return ReadShortInlineVarInstruction(OpCodes.Stloc_S);
				case 0x14: return ReadInlineNoneInstruction(OpCodes.Ldnull);
				case 0x15: return ReadInlineNoneInstruction(OpCodes.Ldc_I4_M1);
				case 0x16: return ReadInlineNoneInstruction(OpCodes.Ldc_I4_0);
				case 0x17: return ReadInlineNoneInstruction(OpCodes.Ldc_I4_1);
				case 0x18: return ReadInlineNoneInstruction(OpCodes.Ldc_I4_2);
				case 0x19: return ReadInlineNoneInstruction(OpCodes.Ldc_I4_3);
				case 0x1A: return ReadInlineNoneInstruction(OpCodes.Ldc_I4_4);
				case 0x1B: return ReadInlineNoneInstruction(OpCodes.Ldc_I4_5);
				case 0x1C: return ReadInlineNoneInstruction(OpCodes.Ldc_I4_6);
				case 0x1D: return ReadInlineNoneInstruction(OpCodes.Ldc_I4_7);
				case 0x1E: return ReadInlineNoneInstruction(OpCodes.Ldc_I4_8);
				case 0x1F: return ReadShortInlineIInstruction(OpCodes.Ldc_I4_S);
				case 0x20: return ReadInlineIInstruction(OpCodes.Ldc_I4);
				case 0x21: return ReadInlineI8Instruction(OpCodes.Ldc_I8);
				case 0x22: return ReadShortInlineRInstruction(OpCodes.Ldc_R4);
				case 0x23: return ReadInlineRInstruction(OpCodes.Ldc_R8);
				case 0x25: return ReadInlineNoneInstruction(OpCodes.Dup);
				case 0x26: return ReadInlineNoneInstruction(OpCodes.Pop);
				case 0x27: return ReadInlineTokInstruction(OpCodes.Jmp);
				case 0x28: return ReadInlineTokInstruction(OpCodes.Call);
				case 0x29: return ReadInlineTokInstruction(OpCodes.Calli);
				case 0x2A: return ReadInlineNoneInstruction(OpCodes.Ret);
				case 0x2B: return ReadShortInlineBrTargetInstruction(OpCodes.Br_S);
				case 0x2C: return ReadShortInlineBrTargetInstruction(OpCodes.Brfalse_S);
				case 0x2D: return ReadShortInlineBrTargetInstruction(OpCodes.Brtrue_S);
				case 0x2E: return ReadShortInlineBrTargetInstruction(OpCodes.Beq_S);
				case 0x2F: return ReadShortInlineBrTargetInstruction(OpCodes.Bge_S);
				case 0x30: return ReadShortInlineBrTargetInstruction(OpCodes.Bgt_S);
				case 0x31: return ReadShortInlineBrTargetInstruction(OpCodes.Ble_S);
				case 0x32: return ReadShortInlineBrTargetInstruction(OpCodes.Blt_S);
				case 0x33: return ReadShortInlineBrTargetInstruction(OpCodes.Bne_Un_S);
				case 0x34: return ReadShortInlineBrTargetInstruction(OpCodes.Bge_Un_S);
				case 0x35: return ReadShortInlineBrTargetInstruction(OpCodes.Bgt_Un_S);
				case 0x36: return ReadShortInlineBrTargetInstruction(OpCodes.Ble_Un_S);
				case 0x37: return ReadShortInlineBrTargetInstruction(OpCodes.Blt_Un_S);
				case 0x38: return ReadInlineBrTargetInstruction(OpCodes.Br);
				case 0x39: return ReadInlineBrTargetInstruction(OpCodes.Brfalse);
				case 0x3A: return ReadInlineBrTargetInstruction(OpCodes.Brtrue);
				case 0x3B: return ReadInlineBrTargetInstruction(OpCodes.Beq);
				case 0x3C: return ReadInlineBrTargetInstruction(OpCodes.Bge);
				case 0x3D: return ReadInlineBrTargetInstruction(OpCodes.Bgt);
				case 0x3E: return ReadInlineBrTargetInstruction(OpCodes.Ble);
				case 0x3F: return ReadInlineBrTargetInstruction(OpCodes.Blt);
				case 0x40: return ReadInlineBrTargetInstruction(OpCodes.Bne_Un);
				case 0x41: return ReadInlineBrTargetInstruction(OpCodes.Bge_Un);
				case 0x42: return ReadInlineBrTargetInstruction(OpCodes.Bgt_Un);
				case 0x43: return ReadInlineBrTargetInstruction(OpCodes.Ble_Un);
				case 0x44: return ReadInlineBrTargetInstruction(OpCodes.Blt_Un);
				case 0x45: return ReadInlineSwitchInstruction(OpCodes.Switch);
				case 0x46: return ReadInlineNoneInstruction(OpCodes.Ldind_I1);
				case 0x47: return ReadInlineNoneInstruction(OpCodes.Ldind_U1);
				case 0x48: return ReadInlineNoneInstruction(OpCodes.Ldind_I2);
				case 0x49: return ReadInlineNoneInstruction(OpCodes.Ldind_U2);
				case 0x4A: return ReadInlineNoneInstruction(OpCodes.Ldind_I4);
				case 0x4B: return ReadInlineNoneInstruction(OpCodes.Ldind_U4);
				case 0x4C: return ReadInlineNoneInstruction(OpCodes.Ldind_I8);
				case 0x4D: return ReadInlineNoneInstruction(OpCodes.Ldind_I);
				case 0x4E: return ReadInlineNoneInstruction(OpCodes.Ldind_R4);
				case 0x4F: return ReadInlineNoneInstruction(OpCodes.Ldind_R8);
				case 0x50: return ReadInlineNoneInstruction(OpCodes.Ldind_Ref);
				case 0x51: return ReadInlineNoneInstruction(OpCodes.Stind_Ref);
				case 0x52: return ReadInlineNoneInstruction(OpCodes.Stind_I1);
				case 0x53: return ReadInlineNoneInstruction(OpCodes.Stind_I2);
				case 0x54: return ReadInlineNoneInstruction(OpCodes.Stind_I4);
				case 0x55: return ReadInlineNoneInstruction(OpCodes.Stind_I8);
				case 0x56: return ReadInlineNoneInstruction(OpCodes.Stind_R4);
				case 0x57: return ReadInlineNoneInstruction(OpCodes.Stind_R8);
				case 0x58: return ReadInlineNoneInstruction(OpCodes.Add);
				case 0x59: return ReadInlineNoneInstruction(OpCodes.Sub);
				case 0x5A: return ReadInlineNoneInstruction(OpCodes.Mul);
				case 0x5B: return ReadInlineNoneInstruction(OpCodes.Div);
				case 0x5C: return ReadInlineNoneInstruction(OpCodes.Div_Un);
				case 0x5D: return ReadInlineNoneInstruction(OpCodes.Rem);
				case 0x5E: return ReadInlineNoneInstruction(OpCodes.Rem_Un);
				case 0x5F: return ReadInlineNoneInstruction(OpCodes.And);
				case 0x60: return ReadInlineNoneInstruction(OpCodes.Or);
				case 0x61: return ReadInlineNoneInstruction(OpCodes.Xor);
				case 0x62: return ReadInlineNoneInstruction(OpCodes.Shl);
				case 0x63: return ReadInlineNoneInstruction(OpCodes.Shr);
				case 0x64: return ReadInlineNoneInstruction(OpCodes.Shr_Un);
				case 0x65: return ReadInlineNoneInstruction(OpCodes.Neg);
				case 0x66: return ReadInlineNoneInstruction(OpCodes.Not);
				case 0x67: return ReadInlineNoneInstruction(OpCodes.Conv_I1);
				case 0x68: return ReadInlineNoneInstruction(OpCodes.Conv_I2);
				case 0x69: return ReadInlineNoneInstruction(OpCodes.Conv_I4);
				case 0x6A: return ReadInlineNoneInstruction(OpCodes.Conv_I8);
				case 0x6B: return ReadInlineNoneInstruction(OpCodes.Conv_R4);
				case 0x6C: return ReadInlineNoneInstruction(OpCodes.Conv_R8);
				case 0x6D: return ReadInlineNoneInstruction(OpCodes.Conv_U4);
				case 0x6E: return ReadInlineNoneInstruction(OpCodes.Conv_U8);
				case 0x6F: return ReadInlineTokInstruction(OpCodes.Callvirt);
				case 0x70: return ReadInlineTokInstruction(OpCodes.Cpobj);
				case 0x71: return ReadInlineTokInstruction(OpCodes.Ldobj);
				case 0x72: return ReadInlineTokInstruction(OpCodes.Ldstr);
				case 0x73: return ReadInlineTokInstruction(OpCodes.Newobj);
				case 0x74: return ReadInlineTokInstruction(OpCodes.Castclass);
				case 0x75: return ReadInlineTokInstruction(OpCodes.Isinst);
				case 0x76: return ReadInlineNoneInstruction(OpCodes.Conv_R_Un);
				case 0x79: return ReadInlineTokInstruction(OpCodes.Unbox);
				case 0x7A: return ReadInlineNoneInstruction(OpCodes.Throw);
				case 0x7B: return ReadInlineTokInstruction(OpCodes.Ldfld);
				case 0x7C: return ReadInlineTokInstruction(OpCodes.Ldflda);
				case 0x7D: return ReadInlineTokInstruction(OpCodes.Stfld);
				case 0x7E: return ReadInlineTokInstruction(OpCodes.Ldsfld);
				case 0x7F: return ReadInlineTokInstruction(OpCodes.Ldsflda);
				case 0x80: return ReadInlineTokInstruction(OpCodes.Stsfld);
				case 0x81: return ReadInlineTokInstruction(OpCodes.Stobj);
				case 0x82: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_I1_Un);
				case 0x83: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_I2_Un);
				case 0x84: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_I4_Un);
				case 0x85: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_I8_Un);
				case 0x86: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_U1_Un);
				case 0x87: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_U2_Un);
				case 0x88: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_U4_Un);
				case 0x89: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_U8_Un);
				case 0x8A: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_I_Un);
				case 0x8B: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_U_Un);
				case 0x8C: return ReadInlineTokInstruction(OpCodes.Box);
				case 0x8D: return ReadInlineTokInstruction(OpCodes.Newarr);
				case 0x8E: return ReadInlineNoneInstruction(OpCodes.Ldlen);
				case 0x8F: return ReadInlineTokInstruction(OpCodes.Ldelema);
				case 0x90: return ReadInlineNoneInstruction(OpCodes.Ldelem_I1);
				case 0x91: return ReadInlineNoneInstruction(OpCodes.Ldelem_U1);
				case 0x92: return ReadInlineNoneInstruction(OpCodes.Ldelem_I2);
				case 0x93: return ReadInlineNoneInstruction(OpCodes.Ldelem_U2);
				case 0x94: return ReadInlineNoneInstruction(OpCodes.Ldelem_I4);
				case 0x95: return ReadInlineNoneInstruction(OpCodes.Ldelem_U4);
				case 0x96: return ReadInlineNoneInstruction(OpCodes.Ldelem_I8);
				case 0x97: return ReadInlineNoneInstruction(OpCodes.Ldelem_I);
				case 0x98: return ReadInlineNoneInstruction(OpCodes.Ldelem_R4);
				case 0x99: return ReadInlineNoneInstruction(OpCodes.Ldelem_R8);
				case 0x9A: return ReadInlineNoneInstruction(OpCodes.Ldelem_Ref);
				case 0x9B: return ReadInlineNoneInstruction(OpCodes.Stelem_I);
				case 0x9C: return ReadInlineNoneInstruction(OpCodes.Stelem_I1);
				case 0x9D: return ReadInlineNoneInstruction(OpCodes.Stelem_I2);
				case 0x9E: return ReadInlineNoneInstruction(OpCodes.Stelem_I4);
				case 0x9F: return ReadInlineNoneInstruction(OpCodes.Stelem_I8);
				case 0xA0: return ReadInlineNoneInstruction(OpCodes.Stelem_R4);
				case 0xA1: return ReadInlineNoneInstruction(OpCodes.Stelem_R8);
				case 0xA2: return ReadInlineNoneInstruction(OpCodes.Stelem_Ref);
				case 0xA3: return ReadInlineTokInstruction(OpCodes.Ldelem);
				case 0xA4: return ReadInlineTokInstruction(OpCodes.Stelem);
				case 0xA5: return ReadInlineTokInstruction(OpCodes.Unbox_Any);
				case 0xB3: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_I1);
				case 0xB4: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_U1);
				case 0xB5: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_I2);
				case 0xB6: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_U2);
				case 0xB7: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_I4);
				case 0xB8: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_U4);
				case 0xB9: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_I8);
				case 0xBA: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_U8);
				case 0xC2: return ReadInlineTokInstruction(OpCodes.Refanyval);
				case 0xC3: return ReadInlineNoneInstruction(OpCodes.Ckfinite);
				case 0xC6: return ReadInlineTokInstruction(OpCodes.Mkrefany);
				case 0xD0: return ReadInlineTokInstruction(OpCodes.Ldtoken);
				case 0xD1: return ReadInlineNoneInstruction(OpCodes.Conv_U2);
				case 0xD2: return ReadInlineNoneInstruction(OpCodes.Conv_U1);
				case 0xD3: return ReadInlineNoneInstruction(OpCodes.Conv_I);
				case 0xD4: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_I);
				case 0xD5: return ReadInlineNoneInstruction(OpCodes.Conv_Ovf_U);
				case 0xD6: return ReadInlineNoneInstruction(OpCodes.Add_Ovf);
				case 0xD7: return ReadInlineNoneInstruction(OpCodes.Add_Ovf_Un);
				case 0xD8: return ReadInlineNoneInstruction(OpCodes.Mul_Ovf);
				case 0xD9: return ReadInlineNoneInstruction(OpCodes.Mul_Ovf_Un);
				case 0xDA: return ReadInlineNoneInstruction(OpCodes.Sub_Ovf);
				case 0xDB: return ReadInlineNoneInstruction(OpCodes.Sub_Ovf_Un);
				case 0xDC: return ReadInlineNoneInstruction(OpCodes.Endfinally);
				case 0xDD: return ReadInlineBrTargetInstruction(OpCodes.Leave);
				case 0xDE: return ReadShortInlineBrTargetInstruction(OpCodes.Leave_S);
				case 0xDF: return ReadInlineNoneInstruction(OpCodes.Stind_I);
				case 0xE0: return ReadInlineNoneInstruction(OpCodes.Conv_U);
				case 0xFE:
					switch (_buffer[_offset + 1])
					{
						case 0x00: return ReadInlineNoneInstruction(OpCodes.Arglist);
						case 0x01: return ReadInlineNoneInstruction(OpCodes.Ceq);
						case 0x02: return ReadInlineNoneInstruction(OpCodes.Cgt);
						case 0x03: return ReadInlineNoneInstruction(OpCodes.Cgt_Un);
						case 0x04: return ReadInlineNoneInstruction(OpCodes.Clt);
						case 0x05: return ReadInlineNoneInstruction(OpCodes.Clt_Un);
						case 0x06: return ReadInlineTokInstruction(OpCodes.Ldftn);
						case 0x07: return ReadInlineTokInstruction(OpCodes.Ldvirtftn);
						case 0x09: return ReadInlineVarInstruction(OpCodes.Ldarg);
						case 0x0A: return ReadInlineVarInstruction(OpCodes.Ldarga);
						case 0x0B: return ReadInlineVarInstruction(OpCodes.Starg);
						case 0x0C: return ReadInlineVarInstruction(OpCodes.Ldloc);
						case 0x0D: return ReadInlineVarInstruction(OpCodes.Ldloca);
						case 0x0E: return ReadInlineVarInstruction(OpCodes.Stloc);
						case 0x0F: return ReadInlineNoneInstruction(OpCodes.Localloc);
						case 0x11: return ReadInlineNoneInstruction(OpCodes.Endfilter);
						case 0x12: return ReadShortInlineIInstruction(OpCodes.Unaligned);
						case 0x13: return ReadInlineNoneInstruction(OpCodes.Volatile);
						case 0x14: return ReadInlineNoneInstruction(OpCodes.Tailcall);
						case 0x15: return ReadInlineTokInstruction(OpCodes.Initobj);
						case 0x16: return ReadInlineTokInstruction(OpCodes.Constrained);
						case 0x17: return ReadInlineNoneInstruction(OpCodes.Cpblk);
						case 0x18: return ReadInlineNoneInstruction(OpCodes.Initblk);
						case 0x1A: return ReadInlineNoneInstruction(OpCodes.Rethrow);
						case 0x1C: return ReadInlineTokInstruction(OpCodes.Sizeof);
						case 0x1D: return ReadInlineNoneInstruction(OpCodes.Refanytype);
						case 0x1E: return ReadInlineNoneInstruction(OpCodes.Readonly);
					}
					break;
			}

			throw new ILException();
		}
	}
}
