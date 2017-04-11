﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LithpBlunt.Serialization
{
	public class LithpParser
	{
		[Flags]
		public enum Classification
		{
			NONE = 0,
			/// <summary>
			/// Literal (1, 2, "test")
			/// </summary>
			LITERAL = 1 << 0,
			/// <summary>
			/// Opening OpChain '('
			/// </summary>
			OPCHAIN = 1 << 1,
			/// <summary>
			/// Opening FunctionCall '('
			/// </summary>
			FUNCTIONCALL = 1 << 2,
			/// <summary>
			/// Collect number (whole or float: [0-9.]+f?$)
			/// </summary>
			NUMBER = 1 << 3,
			/// <summary>
			/// Collect atom
			/// </summary>
			ATOM = 1 << 4,
			/// <summary>
			/// Variables
			/// </summary>
			VARIABLE = 1 << 5,
			/// <summary>
			/// Collect character
			/// </summary>
			STRING_CHARACTER = 1 << 6,
			/// <summary>
			/// Expecting a single quote to end
			/// </summary>
			STRING_SINGLE = 1 << 7,
			/// <summary>
			/// Expecting a double quote to end "
			/// </summary>
			STRING_DOUBLE = 1 << 8,
			/// <summary>
			/// Expecting a space
			/// </summary>
			PARAM_SEPARATOR = 1 << 9,
			/// <summary>
			/// Expected a ), end of call
			/// </summary>
			CALL_END = 1 << 10,
			/// <summary>
			/// Expect a ), end of opchain
			/// </summary>
			OPCHAIN_END = 1 << 11,
			/// <summary>
			/// Comments
			/// </summary>
			COMMENT = 1 << 12,
			/// <summary>
			/// Already compiled
			/// </summary>
			COMPILED = 1 << 13,
			/// <summary>
			/// #     next: Arg1,Arg2 :: (...)
			/// </summary>
			FUNCTION_MARKER = 1 << 14,
			/// <summary>
			/// #     this: Arg1
			/// </summary>
			FUNCTION_PARAM = 1 << 15,
			/// <summary>
			/// #Arg1 this: ,
			/// </summary>
			FUNCTION_PARAM_SEP = 1 << 16,
			/// <summary>
			/// #Arg1,Arg2  this: ::
			/// </summary>
			FUNCTION_BODY = 1 << 17
		}

		public Classification Classify(string phrase)
		{
			Classification result = Classification.NONE;
			switch(phrase[0])
			{
				// Convert these to param separaters
				case '\t':
				case ' ':
				case '\r':
				case '\n': result = Classification.PARAM_SEPARATOR; break;
				case '(': result = Classification.OPCHAIN | Classification.FUNCTIONCALL; break;
				case ')': result = Classification.CALL_END | Classification.OPCHAIN_END; break;
				case '\'': result = Classification.STRING_SINGLE; break;
				case '"': result = Classification.STRING_DOUBLE; break;
				case '%': result = Classification.COMMENT; break;
				case ',':result = Classification.FUNCTION_BODY; break;
				default:
					if (Regex.IsMatch(phrase, "^[a-z][a-zA-Z0-9_]*$"))
						result = Classification.ATOM;
					else if (Regex.IsMatch(phrase, "^[A-Z][A-Za-z0-9_]*$"))
						result = Classification.VARIABLE;
					else if (Regex.IsMatch(phrase, "^-?[0-9e][0-9e.]*$"))
						result = Classification.NUMBER | Classification.ATOM;
					else if (phrase.Length > 1 && Regex.IsMatch(phrase, "^\".*\"$"))
						result = Classification.STRING_DOUBLE;
					else if (phrase.Length > 1 && Regex.IsMatch(phrase, "^'.*'$"))
						result = Classification.STRING_SINGLE;
					else
						result = Classification.ATOM;
					break;
			}
			result |= Classification.STRING_CHARACTER;
			return result;
		}
	}
}
