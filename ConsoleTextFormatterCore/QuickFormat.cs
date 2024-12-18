﻿using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System;

namespace NEG.CTF2.Core;
public static class QuickFormat
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Format(string _text, FormattingRules _rules) => 
		new TextFormatter(_text, _rules).GenerateFormat();
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Format(string _text) => 
		Format(_text, new FormattingRules());
}
